// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using XPatchLib.UnitTest.TestClass;
#if NUNIT
using NUnit.Framework;
#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest.ForXml
{
    [TestFixture]
    public class TestXmlSerializer:TestBase
    {
        public class Account
        {
            public Account()
            {
                Roles = new List<string>();
            }

            public string Email { get; set; }
            public bool Active { get; set; }
            public DateTime CreatedDate { get; set; }
            public IList<string> Roles { get; set; }

            public override bool Equals(object obj)
            {
                Account account=obj as Account;
                if (account == null) return false;
                bool result= string.Equals(Email, account.Email)
                       && bool.Equals(Active, account.Active)
                       && DateTime.Equals(CreatedDate, account.CreatedDate)
                       && int.Equals(Roles.Count, account.Roles.Count);
                if (result)
                {
                    for (int i = 0; i < Roles.Count; i++)
                    {
                        if (!string.Equals(Roles[i], account.Roles[i]))
                            return false;
                    }
                }
                return result;
            }
        }

        private const string ChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<BookClass>
  <Author>
    <Name>" + newAuthorName + @"</Name>
  </Author>
</BookClass>";

        private const string newAuthorName = "Barack Obama";

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

        [Test]
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
            
            DoAssert(typeof(Account), context, account1, account2, true);
            DoAssert(typeof(Account), context, account1, account2, false);

        }

        [Test]
        [Description("")]
        public void TestCallDivideAndCombineNotMergeDataWithDefaultValue()
        {
            AuthorClass emptyAuthor = new AuthorClass();
            AuthorClass author = AuthorClass.GetSampleInstance();

            DoAssert(typeof(AuthorClass), string.Empty, emptyAuthor, author, true);

            DoAssert(typeof(AuthorClass), string.Empty, emptyAuthor, author, false);
        }

        [Test]
        [Description("测试使用同一Serializer实例，对没有标记PrimaryKey的对象集合，先做增量序列化，后做增量反序列化（反序列化时不覆盖原有对象实例）")]
        public void TestCallDivideAndCombineNotMergeDataWithoutPrimaryKeyAttribute()
        {
            List<AuthorClass> authors1 = new List<AuthorClass>();
            authors1.Add(new AuthorClass {Name = "A1"});
            authors1.Add(new AuthorClass {Name = "A2"});
            authors1.Add(new AuthorClass {Name = "A3"});

            IDictionary<Type, string[]> keys = new Dictionary<Type, string[]>();
            keys.Add(typeof(AuthorClass), new[] { "Name" });

            DoAssert(typeof(List<AuthorClass>), string.Empty, new List<AuthorClass>(), authors1, true, null, keys);
            
            DoAssert(typeof(List<AuthorClass>), string.Empty, new List<AuthorClass>(), authors1, false, null, keys);
            
        }

        [Test]
        [Description("测试Serializer中参数类型为 XmlTextReader 和 XmlTextWriter 的Divide和Combine方法")]
        public void TestXmlSerializerStreamDivideAndCombine()
        {
            DoAssert(typeof(BookClass), ChangedContext, OriObject, RevObject, true);


            DoAssert(typeof(BookClass), ChangedContext, OriObject, RevObject, false);
            
        }

        [Test]
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

        [Test]
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
                var serializer = new Serializer(typeof(XmlIgnoreClass));
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    serializer.Divide(writer, c1, c2);
                    var context = UnitTest.TestHelper.StreamToString(stream);
                    Assert.AreEqual(changedContext, context);
                }
                stream.Position = 0;
                using (var reader = new XmlTextReader(XmlReader.Create(stream)))
                {
                    var changedObj = serializer.Combine(reader, c1) as XmlIgnoreClass;
                    Assert.AreEqual(c3, changedObj);
                }
            }
        }
    }
}