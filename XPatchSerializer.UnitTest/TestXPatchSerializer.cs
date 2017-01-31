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

        private const string ChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<BookClass>
  <Author>
    <Name>" + newAuthorName + @"</Name>
  </Author>
</BookClass>";

        private const string newAuthorName = "Barack Obama";

        #endregion Private Fields

        #region Private Properties

        private BookClass OriObject
        {
            get
            {
                return BookClass.GetSampleInstance();
            }
        }

        private BookClass RevObject
        {
            get
            {
                BookClass revObj = BookClass.GetSampleInstance();
                    revObj.Author.Name = newAuthorName;
                return revObj;
            }
        }

        #endregion Private Properties

        #region Public Methods

        [TestMethod]
        [Description("测试XPatchSerializer中参数类型为Stream的Divide和Combine方法")]
        public void TestXPatchSeraialzerStreamDivideAndCombine()
        {
            XPatchSerializer serializer = new XPatchSerializer(typeof(BookClass));
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, OriObject, RevObject);
                var context = TestHelper.StreamToString(stream);
                Assert.AreEqual(ChangedContext, context);
            }
            serializer = new XPatchSerializer(typeof(BookClass));
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, OriObject, RevObject);
                stream.Position = 0;
                var changedObj = serializer.Combine(stream, OriObject) as BookClass;
                Assert.AreEqual(RevObject, changedObj);
            }
        }

        [TestMethod]
        [Description("测试XPatchSerializer中参数类型为TextWriter的Divide方法和TextReader的Combine方法")]
        public void TestXPatchSeraialzerTextWriterDivideAndTextReaderCombine()
        {
            XPatchSerializer serializer = new XPatchSerializer(typeof(BookClass));
            var sb = new StringBuilder();
            using (var swr = new StringWriter(sb))
            {
                serializer.Divide(swr, OriObject, RevObject);
                sb.Replace("utf-16", "utf-8");
                Assert.AreEqual(ChangedContext, sb.ToString());
            }
            serializer = new XPatchSerializer(typeof(BookClass));
            using (var reader = new StringReader(ChangedContext))
            {
                var changedObj = serializer.Combine(reader, OriObject) as BookClass;
                Assert.AreEqual(RevObject, changedObj);
            }
        }

        [TestMethod]
        [Description("测试XPatchSerializer中参数类型为XmlWriter的Divide方法和XmlReader的Combine方法")]
        public void TestXPatchSeraialzerXmlWriterDivideAndXmlReaderCombine()
        {
            XPatchSerializer serializer = new XPatchSerializer(typeof(BookClass));
            using (var stream = new MemoryStream())
            {
                var setting = new XmlWriterSettings();
                setting.Encoding = new UTF8Encoding(false);
                setting.Indent = true;
                using (var xwr = XmlWriter.Create(stream, setting))
                {
                    serializer.Divide(xwr, OriObject, RevObject);
                    Assert.AreEqual(ChangedContext, Encoding.UTF8.GetString(stream.ToArray()));
                }
            }
            serializer = new XPatchSerializer(typeof(BookClass));
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, OriObject, RevObject);
                stream.Position = 0;
                using (var reader = XmlReader.Create(stream))
                {
                    var changedObjByReader = serializer.Combine(reader, OriObject) as BookClass;
                    Assert.AreEqual(RevObject, changedObjByReader);
                }
            }
        }

        #endregion Public Methods
    }
}