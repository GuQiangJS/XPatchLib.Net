// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
        public class Account
        {
            public string Email { get; set; }
            public bool Active { get; set; }
            public DateTime CreatedDate { get; set; }
            public IList<string> Roles { get; set; }
        }

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
                var revObj = BookClass.GetSampleInstance();
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
        [Description("测试Serializer中参数类型为 XmlTextReader 和 XmlTextWriter 的Divide和Combine方法。测试对象包含 XmlIgnoreAttribute")]
        public void TestXmlSerializerStreamDivideAndCombineForIgnoreAttribute()
        {
            var c1 = new XmlIgnoreClass {A = "A", B = "B"};
            var c2 = new XmlIgnoreClass {A = "C", B = "D"};
            //因为属性A不参与序列化，所以应该还是原值
            var c3 = new XmlIgnoreClass {A = "A", B = "D"};

            var changedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<XmlIgnoreClass>
  <B>D</B>
</XmlIgnoreClass>";

            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    var serializer = new Serializer(typeof(XmlIgnoreClass));
                    serializer.Divide(writer, c1, c2);
                    var context = UnitTest.TestHelper.StreamToString(stream);
                    Assert.AreEqual(changedContext, context);
                }
            }
            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    var serializer = new Serializer(typeof(XmlIgnoreClass));
                    serializer.Divide(writer, c1, c2);
                    stream.Position = 0;
                    using (var reader = new XmlTextReader(XmlReader.Create(stream)))
                    {
                        var changedObj = serializer.Combine(reader, c1) as XmlIgnoreClass;
                        Assert.AreEqual(c3, changedObj);
                    }
                }
            }
        }

        [TestMethod]
        [Description("测试Serializer中参数类型为 XmlTextReader 和 XmlTextWriter 的Divide和Combine方法")]
        public void TestXmlSerializerStreamDivideAndCombine()
        {
            var serializer = new Serializer(typeof(BookClass));
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
                    using (var reader = new XmlTextReader(XmlReader.Create(stream)))
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

            var serializer = new Serializer(typeof(BookClass));
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
                    using (var reader = new XmlTextReader(XmlReader.Create(stream)))
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

        [TestMethod]
        public void SimpleTestXmlSerializer()
        {
            const string context = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Account>
  <Active>true</Active>
  <CreatedDate>2013-01-20T00:00:00Z</CreatedDate>
  <Email>xpatchlib@example.com</Email>
  <Roles>
    <String Action=""Add"">User</String>
    <String Action=""Add"">Admin</String>
  </Roles>
</Account>";

            var account1 = new Account();

            var account2 = new Account
            {
                Email = "xpatchlib@example.com",
                Active = true,
                CreatedDate = new DateTime(2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                Roles = new List<string>
                {
                    "User",
                    "Admin"
                }
            };

            var serializer = new Serializer(typeof(Account));
            using (var stream = new MemoryStream())
            {
                var settings = new XmlWriterSettings
                {
                    ConformanceLevel = ConformanceLevel.Document,
                    Indent = true,
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = false
                };
                using (var writer = TestHelper.CreateWriter(stream,settings))
                {
                    serializer.Divide(writer, account1, account2);
                    var result = UnitTest.TestHelper.StreamToString(stream);
                    Debug.WriteLine(context);
                    Assert.AreEqual(context,result);
                }
            }
        }

        [TestMethod]
        [Description("测试使用同一Serializer实例，对没有标记PrimaryKey的对象集合，先做增量序列化，后做增量反序列化（反序列化时不覆盖原有对象实例）")]
        public void TestCallDivideAndCombineNotMergeDataWithoutPrimaryKeyAttribute() {
            List<AuthorClass> authors1 = new List<AuthorClass>();
            authors1.Add(new AuthorClass() { Name = "A1" });
            authors1.Add(new AuthorClass() { Name = "A2" });
            authors1.Add(new AuthorClass() { Name = "A3" });

            Serializer serializer=new Serializer(typeof(List<AuthorClass>));

            IDictionary<Type,string[]> keys=new Dictionary<Type, string[]>();
            keys.Add(typeof(AuthorClass), new string[] {"Name"});
            serializer.RegisterTypes(keys);

            string divideString = string.Empty;
            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    serializer.Divide(writer, new List<AuthorClass>(), authors1);
                    //divideString = UnitTest.TestHelper.StreamToString(stream);
                    stream.Position = 0;

                    using (var reader = new XmlTextReader(XmlReader.Create(stream)))
                    {
                        var changedObj = serializer.Combine(reader, new List<AuthorClass>()) as List<AuthorClass>;
                        Assert.IsNotNull(changedObj);
                        Assert.AreEqual(authors1.Count, changedObj.Count);
                        for (int i = 0; i < authors1.Count; i++) {
                            Assert.AreEqual(authors1[0], changedObj[0]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        [Description("")]
        public void TestCallDivideAndCombineNotMergeDataWithDefaultValue() {

            AuthorClass emptyAuthor = new AuthorClass();
            AuthorClass author = AuthorClass.GetSampleInstance();

            Serializer serializer = new Serializer(typeof(AuthorClass));

            string divideString = string.Empty;
            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    serializer.Divide(writer, emptyAuthor, author);
                    //divideString = UnitTest.TestHelper.StreamToString(stream);
                    stream.Position = 0;

                    using (var reader = new XmlTextReader(XmlReader.Create(stream)))
                    {
                        var changedObj = serializer.Combine(reader, new AuthorClass()) as AuthorClass;
                        Assert.IsNotNull(changedObj);
                        Assert.AreEqual(author, changedObj);
                    }
                }
            }
        }

        #endregion Public Methods
    }
}