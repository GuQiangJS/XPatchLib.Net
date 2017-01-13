using System.IO;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestXPatchSerializer
    {
        #region Private Fields

        private const string changedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<BookClass>
  <Author>
    <Name>" + newAuthorName + @"</Name>
  </Author>
</BookClass>";

        private const string newAuthorName = "Barack Obama";
        private BookClass revObj, oriObj;
        private XPatchSerializer serializer = new XPatchSerializer(typeof(BookClass));

        #endregion Private Fields

        #region Private Properties

        private BookClass OriObject
        {
            get
            {
                if (oriObj == null)
                {
                    oriObj = BookClass.GetSampleInstance();
                }
                return oriObj;
            }
        }

        private BookClass RevObject
        {
            get
            {
                if (revObj == null)
                {
                    revObj = BookClass.GetSampleInstance();
                    revObj.Author.Name = newAuthorName;
                }
                return revObj;
            }
        }

        #endregion Private Properties

        #region Public Methods

        [TestMethod]
        [Description("测试XPatchSerializer中参数类型为Stream的Divide和Combine方法")]
        public void TestXPatchSeraialzerStreamDivideAndCombine()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, OriObject, RevObject);
                string context = TestHelper.StreamToString(stream);
                Assert.AreEqual(changedContext, context);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, OriObject, RevObject);
                stream.Position = 0;
                BookClass changedObj = serializer.Combine(stream, OriObject) as BookClass;
                Assert.AreEqual(RevObject, changedObj);
            }
        }

        [TestMethod]
        [Description("测试XPatchSerializer中参数类型为TextWriter的Divide方法和TextReader的Combine方法")]
        public void TestXPatchSeraialzerTextWriterDivideAndTextReaderCombine()
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter swr = new StringWriter(sb))
            {
                serializer.Divide(swr, OriObject, RevObject);
                sb.Replace("utf-16", "utf-8");
                Assert.AreEqual(changedContext, sb.ToString());
            }

            using (StringReader reader = new StringReader(changedContext))
            {
                BookClass changedObj = serializer.Combine(reader, OriObject) as BookClass;
                Assert.AreEqual(RevObject, changedObj);
            }
        }

        [TestMethod]
        [Description("测试XPatchSerializer中参数类型为XmlWriter的Divide方法和XmlReader的Combine方法")]
        public void TestXPatchSeraialzerXmlWriterDivideAndXmlReaderCombine()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Encoding = new UTF8Encoding(false);
                setting.Indent = true;
                using (XmlWriter xwr = XmlWriter.Create(stream, setting))
                {
                    serializer.Divide(xwr, OriObject, RevObject);
                    Assert.AreEqual(changedContext, Encoding.UTF8.GetString(stream.ToArray()));
                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, OriObject, RevObject);
                stream.Position = 0;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    BookClass changedObjByReader = serializer.Combine(reader, OriObject) as BookClass;
                    Assert.AreEqual(RevObject, changedObjByReader);
                }
            }
        }

        #endregion Public Methods
    }
}