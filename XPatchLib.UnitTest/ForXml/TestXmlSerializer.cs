// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest.ForXml
{
    [TestClass]
    public class TestXmlSerializer
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
            get { return BookClass.GetSampleInstance(); }
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

        //[TestMethod]
        //[Description("测试Serializer中参数类型为Stream的Divide和Combine方法")]
        //public void TestXmlSerializerStreamDivideAndCombine()
        //{
        //    Serializer serializer = new Serializer(typeof(BookClass));
        //    using (var stream = new MemoryStream())
        //    {
        //        serializer.Divide(stream, OriObject, RevObject);
        //        var context = UnitTest.TestHelper.StreamToString(stream);
        //        Assert.AreEqual(ChangedContext, context);
        //    }
        //    serializer = new Serializer(typeof(BookClass));
        //    using (var stream = new MemoryStream())
        //    {
        //        serializer.Divide(stream, OriObject, RevObject);
        //        stream.Position = 0;
        //        var changedObj = serializer.Combine(stream, OriObject) as BookClass;
        //        Assert.AreEqual(RevObject, changedObj);
        //    }
        //}

        [TestMethod]
        [Description("测试Serializer中参数类型为 XmlTextReader 和 XmlTextWriter 的Divide和Combine方法")]
        public void TestXmlSerializerStreamDivideAndCombine()
        {
            Serializer serializer = new Serializer(typeof(BookClass));
            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    serializer.Divide(writer, OriObject, RevObject);
                    var context = UnitTest.TestHelper.StreamToString(stream);
                    Assert.AreEqual(ChangedContext, context);
                }
            }
            serializer = new Serializer(typeof(BookClass));
            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    serializer.Divide(writer, OriObject, RevObject);
                    stream.Position = 0;
                    using (XmlTextReader reader = new XmlTextReader(XmlReader.Create(stream)))
                    {
                        var changedObj = serializer.Combine(reader, OriObject) as BookClass;
                        Assert.AreEqual(RevObject, changedObj);
                    }
                }
            }
        }

        [TestMethod]
        [Description("测试序列化时指定的XmlTextWriter，更改了Encoding")]
        public void TestXmlSerializerStreamDivideAndCombineChangedEncoding()
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = false;
            ;
            settings.Encoding = Encoding.ASCII;

            Serializer serializer = new Serializer(typeof(BookClass));
            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, settings))
                {
                    serializer.Divide(writer, OriObject, RevObject);
                    var context = UnitTest.TestHelper.StreamToString(stream);
                    Debug.WriteLine(context);
                    Assert.AreEqual(ChangedContext.Replace("utf-8", "us-ascii"), context);
                }
            }
            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, settings))
                {
                    serializer.Divide(writer, OriObject, RevObject);
                    stream.Position = 0;
                    using (XmlTextReader reader = new XmlTextReader(XmlReader.Create(stream)))
                    {
                        var changedObj = serializer.Combine(reader, OriObject) as BookClass;
                        Assert.AreEqual(RevObject, changedObj);
                    }
                }
            }

            settings.Indent = false;
            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, settings))
                {
                    serializer.Divide(writer, OriObject, RevObject);
                    var context = UnitTest.TestHelper.StreamToString(stream);
                    Debug.WriteLine(context);
                    Assert.AreEqual(
                        ChangedContext.Replace("utf-8", "us-ascii")
                            .Replace(Environment.NewLine, "")
                            .Replace(" ", "")
                            .Replace("BarackObama", "Barack Obama")
                            .Replace(@"xmlversion=""1.0""encoding=""us-ascii""",
                                @"xml version=""1.0"" encoding=""us-ascii"""), context);
                }
            }
        }

        //[TestMethod]
        //[Description("测试Serializer中参数类型为TextWriter的Divide方法和TextReader的Combine方法")]
        //public void TestXmlSerializerTextWriterDivideAndTextReaderCombine()
        //{
        //    Serializer serializer = new Serializer(typeof(BookClass));
        //    var sb = new StringBuilder();
        //    using (var swr = new StringWriter(sb))
        //    {
        //        serializer.Divide(swr, OriObject, RevObject);
        //        sb.Replace("utf-16", "utf-8");
        //        Assert.AreEqual(ChangedContext, sb.ToString());
        //    }
        //    serializer = new Serializer(typeof(BookClass));
        //    using (var reader = new StringReader(ChangedContext))
        //    {
        //        var changedObj = serializer.Combine(reader, OriObject) as BookClass;
        //        Assert.AreEqual(RevObject, changedObj);
        //    }
        //}

        //[TestMethod]
        //[Description("测试Serializer中参数类型为XmlWriter的Divide方法和XmlReader的Combine方法")]
        //public void TestXmlSerializerXmlWriterDivideAndXmlReaderCombine()
        //{
        //    Serializer serializer = new Serializer(typeof(BookClass));
        //    using (var stream = new MemoryStream())
        //    {
        //        var setting = new XmlWriterSettings();
        //        setting.Encoding = new UTF8Encoding(false);
        //        setting.Indent = true;
        //        using (var xwr = XmlWriter.Create(stream, setting))
        //        {
        //            serializer.Divide(xwr, OriObject, RevObject);
        //            Assert.AreEqual(ChangedContext, Encoding.UTF8.GetString(stream.ToArray()));
        //        }
        //    }
        //    serializer = new Serializer(typeof(BookClass));
        //    using (var stream = new MemoryStream())
        //    {
        //        serializer.Divide(stream, OriObject, RevObject);
        //        stream.Position = 0;
        //        using (var reader = XmlReader.Create(stream))
        //        {
        //            var changedObjByReader = serializer.Combine(reader, OriObject) as BookClass;
        //            Assert.AreEqual(RevObject, changedObjByReader);
        //        }
        //    }
        //}

        #endregion Public Methods
    }
}