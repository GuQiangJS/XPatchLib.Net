// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if (NET_35_UP || NETSTANDARD)
using System.Xml.Linq;
#endif
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
    public class TestSerializeArray:TestBase
    {
#if XUNIT
        public TestSerializeArray()
        {
            TestInitialize();
        }
#endif
#if NUNIT
        [SetUp]
#endif
        public void TestInitialize()
        {
            TypeExtendContainer.ClearAll();
        }

        [Test]
        [Description("测试合并集合类型但是传入的类型不是集合类型时，是否抛出ArgumentOutOfRangeException错误")]
        public void TestCombineNonCollectionType()
        {
            try
            {
                new CombineIEnumerable(new TypeExtend(typeof(AuthorClass), null));
                Assert.Fail("未能抛出ArgumentOutOfRangeException异常");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual(ex.ParamName, typeof(AuthorClass).FullName);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出ArgumentOutOfRangeException异常");
            }
        }

        [Test]
        [Description("测试合并类型没有定义主键的对象是否会抛出异常")]
        public void TestCombineNullPrimaryKeyDefingeClass()
        {
            try
            {
                new CombineIEnumerable(new TypeExtend(typeof(List<AuthorClass>), null));
                Assert.Fail("未能抛出AttributeMissException异常");
            }
            catch (AttributeMissException ex)
            {
                Assert.AreEqual(typeof(PrimaryKeyAttribute).Name, ex.AttributeName);
                Assert.AreEqual(typeof(AuthorClass), ex.ErrorType);
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture, ResourceHelper.GetResourceString(LocalizationRes.Exp_String_AttributeMiss), ex.ErrorType.FullName,
                        ex.AttributeName), ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
                
        }

        [Test]
        [Description("测试合并类型没有定义主键的对象是否会抛出异常")]
        public void TestCombineNullPrimaryKeyDefingeClassWithXmlSerializer()
        {
            try
            {
                var a = new List<AuthorClass>();
                a.Add(new AuthorClass {Name = "Author A"});
                a.Add(new AuthorClass {Name = "Author B"});
                a.Add(new AuthorClass {Name = "Author C"});

                DoAssert(typeof(List<AuthorClass>), String.Empty, null, a, true);
                Assert.Fail("未能抛出AttributeMissException异常");
            }
            catch (AttributeMissException ex)
            {
                Assert.AreEqual(typeof(PrimaryKeyAttribute).Name, ex.AttributeName);
                Assert.AreEqual(typeof(AuthorClass), ex.ErrorType);
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture, ResourceHelper.GetResourceString(LocalizationRes.Exp_String_AttributeMiss), ex.ErrorType.FullName,
                        ex.AttributeName), ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
        }

        [Test]
        public void TestCombineNullPrimaryKeyDefingeClassWithXmlSerializerRegisteredType()
        {
            var a = new List<AuthorClass>();
            a.Add(new AuthorClass {Name = "Author A"});
            a.Add(new AuthorClass {Name = "Author B"});
            a.Add(new AuthorClass {Name = "Author C"});

            const string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<List_AuthorClass>
  <AuthorClass Action=""Add"">
    <Name>Author A</Name>
  </AuthorClass>
  <AuthorClass Action=""Add"">
    <Name>Author B</Name>
  </AuthorClass>
  <AuthorClass Action=""Add"">
    <Name>Author C</Name>
  </AuthorClass>
</List_AuthorClass>";

            var serializer = new Serializer(typeof(List<AuthorClass>));

            var types = new Dictionary<Type, string[]>();
            types.Add(typeof(AuthorClass), new[] {"Name"});

            var context = string.Empty;

            serializer.RegisterTypes(types);
            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    serializer.Divide(writer, null, a);
                    context = UnitTest.TestHelper.StreamToString(stream);
                }
            }

            Assert.AreEqual(result, context);
        }

        [Test]
        [Description("测试合并一个只有空白的根节点的内容")]
        public void TestCombineSameBookClassCollection()
        {
            var serializer = new Serializer(typeof(BookClassCollection));

            const string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<BookClassCollection />";

            var c = new BookClassCollection();
            c.Add(new BookClass {Name = "A"});
            c.Add(new BookClass {Name = "B"});
            c.Add(new BookClass {Name = "C"});
            c.Add(new BookClass {Name = "D"});

            using (XmlReader reader = XmlReader.Create(new StringReader(result)))
            {
                using (XmlTextReader xmlReader = new XmlTextReader(reader))
                {
                    var b = serializer.Combine(xmlReader, c) as BookClassCollection;

                    Assert.AreEqual(b, c);
                }
            }
        }

        [Test]
        public void TestDivideAndSerializeBookClassCollection()
        {
            var b = new BookClassCollection();

            const string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<BookClassCollection>
  <BookClass Action=""Add"">
    <Name>A</Name>
  </BookClass>
  <BookClass Action=""Add"">
    <Name>B</Name>
  </BookClass>
  <BookClass Action=""Add"">
    <Name>C</Name>
  </BookClass>
  <BookClass Action=""Add"">
    <Name>D</Name>
  </BookClass>
</BookClassCollection>";

            b.Add(new BookClass {Name = "A"});
            b.Add(new BookClass {Name = "B"});
            b.Add(new BookClass {Name = "C"});
            b.Add(new BookClass {Name = "D"});

            DoAssert(typeof(BookClassCollection), result, null, b, true);
            DoAssert(typeof(BookClassCollection), result, new BookClassCollection(), b, true);
            DoAssert(typeof(BookClassCollection), result, new BookClassCollection(), b, false);
        }

        [Test]
        public void TestDivideAndSerializeEmptyBookClassCollection()
        {
            var b = new BookClassCollection();

            const string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<BookClassCollection />";

            DoAssert(typeof(BookClassCollection), result, null, b, true);
            //与空白集合产生增量内容，为空白内容。
            DoAssert(typeof(BookClassCollection), TestHelper.XmlHeaderContext, new BookClassCollection(), b, true);
            DoAssert(typeof(BookClassCollection), TestHelper.XmlHeaderContext, new BookClassCollection(), b, false);
        }

        [Test]
        [Description("测试拆分两个相同内容的集合，应该产生空白内容")]
        public void TestDivideAndSerializeSameBookClassCollection()
        {
            var b = new BookClassCollection();
            b.Add(new BookClass {Name = "A"});
            b.Add(new BookClass {Name = "B"});
            b.Add(new BookClass {Name = "C"});
            b.Add(new BookClass {Name = "D"});

            var c = new BookClassCollection();
            c.Add(new BookClass { Name = "A" });
            c.Add(new BookClass { Name = "B" });
            c.Add(new BookClass { Name = "C" });
            c.Add(new BookClass { Name = "D" });

            DoAssert(typeof(BookClassCollection), TestHelper.XmlHeaderContext, c, b, true);
            DoAssert(typeof(BookClassCollection), TestHelper.XmlHeaderContext, c, b, false);
        }

        [Test]
        [Description("测试拆分集合类型但是传入的类型不是集合类型时，是否抛出ArgumentOutOfRangeException错误")]
        public void TestDivideNonCollectionType()
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    using (ITextWriter writer = TestHelper.CreateWriter(stream))
                    {
                        new DivideIEnumerable(writer, new TypeExtend(typeof(AuthorClass), writer.IgnoreAttributeType));
                    }
                }
                Assert.Fail("未能抛出ArgumentOutOfRangeException异常");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual(ex.ParamName, typeof(AuthorClass).FullName);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出ArgumentOutOfRangeException异常");
            }
        }

        [Test]
        [Description("测试拆分类型没有定义主键的对象是否会抛出异常")]
        public void TestDivideNullPrimaryKeyDefingeClass()
        {
            try
            {
                DoAssert(typeof(List<AuthorClass>), string.Empty, null, new List<AuthorClass>(), true);
                Assert.Fail("未能抛出AttributeMissException异常");
            }
            catch (AttributeMissException ex)
            {
                Assert.AreEqual("PrimaryKeyAttribute", ex.AttributeName);
                Assert.AreEqual(typeof(AuthorClass), ex.ErrorType);
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture, ResourceHelper.GetResourceString(LocalizationRes.Exp_String_AttributeMiss), ex.ErrorType.FullName,
                        ex.AttributeName), ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出AttributeMissException异常");
            }
        }

        [Test]
        public void TestSerializeBookClassCollection()
        {
            var b1 = new BookClassCollection() {new BookClass {Name = "A"}, new BookClass {Name = "B"}};
            var b2 = new BookClassCollection() {new BookClass {Name = "B"}, new BookClass {Name = "C"}};

            const string result =
                @"<BookClassCollection>
  <BookClass Action=""Remove"" Name=""A"" />
  <BookClass Action=""Add"">
    <Name>C</Name>
  </BookClass>
</BookClassCollection>";

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    var ser = new DivideIEnumerable(writer,
                        new TypeExtend(typeof(BookClassCollection), writer.IgnoreAttributeType));

                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(BookClassCollection)), b1, b2));
                }

                AssertHelper.AreEqual(result, stream, string.Empty);
                var com = new CombineIEnumerable(new TypeExtend(typeof(BookClassCollection), null));
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (ITextReader reader = new XmlTextReader(xmlReader))
                    {
                        var b3 =
                            com.Combine(reader, b1, ReflectionUtils.GetTypeFriendlyName(typeof(BookClassCollection))) as
                                BookClassCollection;

                        Assert.AreEqual(b2, b3);
                    }
                }
            }
            
            b1 = new BookClassCollection() { new BookClass { Name = "A" }, new BookClass { Name = "B" } };
            b2 = new BookClassCollection() { new BookClass { Name = "B" }, new BookClass { Name = "C" } };

            DoAssert(typeof(BookClassCollection),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), b1,
                b2, true);
            DoAssert(typeof(BookClassCollection),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), b1,
                b2, false);

        }

        [Test]
        public void TestSerializeBookClassCollectionSerializeDefaultValue()
        {
            var b1 = new BookClassCollection() { new BookClass { Name = "A" }, new BookClass { Name = "B" } };
            var b2 = new BookClassCollection() { new BookClass { Name = "B" }, new BookClass { Name = "C" } };

            const string result =
                @"<BookClassCollection>
  <BookClass Action=""Remove"" Name=""A"" />
  <BookClass Action=""Add"">
    <Author>
      <Comments Action=""SetNull"" />
      <Name Action=""SetNull"" />
    </Author>
    <Comments Action=""SetNull"" />
    <Name>C</Name>
    <Price>0</Price>
    <PublishYear>0</PublishYear>
  </BookClass>
</BookClassCollection>";
            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    writer.Setting.SerializeDefalutValue = true;
                    var ser = new DivideIEnumerable(writer,
                        new TypeExtend(typeof(BookClassCollection), writer.IgnoreAttributeType));

                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(BookClassCollection)), b1, b2));
                }

                AssertHelper.AreEqual(result, stream, string.Empty);

                var com = new CombineIEnumerable(new TypeExtend(typeof(BookClassCollection), null));
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (ITextReader reader = new XmlTextReader(xmlReader))
                    {
                        var b3 =
                            com.Combine(reader, b1, ReflectionUtils.GetTypeFriendlyName(typeof(BookClassCollection))) as
                                BookClassCollection;

                        Assert.AreEqual(b2, b3);
                    }
                }
            }

            b1 = new BookClassCollection() { new BookClass { Name = "A" }, new BookClass { Name = "B" } };
            b2 = new BookClassCollection() { new BookClass { Name = "B" }, new BookClass { Name = "C" } };

            DoAssert(typeof(BookClassCollection),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), b1,
                b2, true, new XmlSerializeSetting() {SerializeDefalutValue = true});
            DoAssert(typeof(BookClassCollection),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), b1,
                b2, false, new XmlSerializeSetting() { SerializeDefalutValue = true });
        }

        [Test]
        public void TestByteArray()
        {
            Byte[] bytes1 = {1, 2, 3, 4, 5};
            Byte[] bytes2 = {1, 3, 5, 7, 9};

            Type t = typeof(Byte[]);

            const string result =
                @"<Array1OfByte>
  <Byte Action=""Remove"">2</Byte>
  <Byte Action=""Remove"">4</Byte>
  <Byte Action=""Add"">7</Byte>
  <Byte Action=""Add"">9</Byte>
</Array1OfByte>";

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    var ser = new DivideIEnumerable(writer,
                        new TypeExtend(t, writer.IgnoreAttributeType));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(t), bytes1, bytes2));
                }

                AssertHelper.AreEqual(result, stream, string.Empty);

                var com = new CombineIEnumerable(new TypeExtend(t, null));
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (ITextReader reader = new XmlTextReader(xmlReader))
                    {
                        var bytes3 = com.Combine(reader, bytes1, ReflectionUtils.GetTypeFriendlyName(t)) as Byte[];
                        Assert.AreEqual(bytes2.Length, bytes3.Length);

                        Assert.IsTrue(bytes3.Contains(bytes2[3]));
                        Assert.IsTrue(bytes3.Contains(bytes2[4]));
                    }
                }
            }

            bytes1 = new Byte[] { 1, 2, 3, 4, 5 };
            bytes2 = new Byte[] { 1, 3, 5, 7, 9 };

            DoAssert(typeof(Byte[]),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), bytes1,
                bytes2, true);
            DoAssert(typeof(Byte[]),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), bytes1,
                bytes2, false);
        }

        [Test]
        public void TestSerializeSimpleArray()
        {
            string[] s1 = {"ABC", "DEF"};
            string[] s2 = {"DEF", "HGI"};

            const string result =
                @"<Array1OfString>
  <String Action=""Remove"">ABC</String>
  <String Action=""Add"">HGI</String>
</Array1OfString>";

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    var ser = new DivideIEnumerable(writer,
                        new TypeExtend(typeof(string[]), writer.IgnoreAttributeType));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), s1, s2));
                }

                AssertHelper.AreEqual(result, stream, string.Empty);

                var com = new CombineIEnumerable(new TypeExtend(typeof(string[]), null));
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (ITextReader reader = new XmlTextReader(xmlReader))
                    {
                        var s3 = com.Combine(reader, s1, ReflectionUtils.GetTypeFriendlyName(s1.GetType())) as string[];
                        Assert.AreEqual(s2.Length, s3.Length);

                        Assert.IsTrue(s3.Contains(s2[0]));
                        Assert.IsTrue(s3.Contains(s2[1]));
                    }
                }
            }

            s1 = new string[] {"ABC", "DEF"};
            s2 = new string[] {"DEF", "HGI"};

            DoAssert(typeof(string[]),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), s1,
                s2, true);
            DoAssert(typeof(string[]),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), s1,
                s2, false);
        }

        [Test]
        public void TestSerializeSimpleArrayAdd()
        {
            string[] s1 = {"ABC", "DEF"};

            const string result =
                @"<Array1OfString>
  <String Action=""Add"">ABC</String>
  <String Action=""Add"">DEF</String>
</Array1OfString>";

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    var ser = new DivideIEnumerable(writer,
                        new TypeExtend(typeof(string[]), writer.IgnoreAttributeType));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), null, s1));
                }

                AssertHelper.AreEqual(result, stream, string.Empty);

                var com = new CombineIEnumerable(new TypeExtend(typeof(string[]), null));
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (ITextReader reader = new XmlTextReader(xmlReader))
                    {
                        var s2 =
                            com.Combine(reader, new string[] { }, ReflectionUtils.GetTypeFriendlyName(s1.GetType())) as
                                string[];
                        Assert.AreEqual(2, s2.Length);

                        Assert.IsTrue(s2.Contains("ABC"));
                        Assert.IsTrue(s2.Contains("DEF"));
                    }
                }
            }

            s1 = new string[] { "ABC", "DEF" };

            DoAssert(typeof(string[]),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), null,
                s1, true);
            DoAssert(typeof(string[]),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), null,
                s1, false);
        }

        [Test]
        public void TestSerializeSimpleArrayRemove()
        {
            string[] s1 = {"ABC", "DEF"};
            string[] s2 = {"DEF"};

            const string result =
                @"<Array1OfString>
  <String Action=""Remove"">ABC</String>
</Array1OfString>";
            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    var ser = new DivideIEnumerable(writer,
                        new TypeExtend(typeof(string[]), writer.IgnoreAttributeType));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), s1, s2));
                }

                AssertHelper.AreEqual(result, stream, string.Empty);
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (ITextReader reader = new XmlTextReader(xmlReader))
                    {
                        var com = new CombineIEnumerable(new TypeExtend(typeof(string[]), null));
                        var s3 = com.Combine(reader, s1, ReflectionUtils.GetTypeFriendlyName(s1.GetType())) as string[];

                        Assert.AreEqual(s2.Length, s3.Length);

                        Assert.AreEqual(s2[0], s3[0]);
                    }
                }
            }

            s1 = new string[] { "ABC", "DEF" };
            s2 = new string[] { "DEF" };

            DoAssert(typeof(string[]),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), s1,
                s2, true);
            DoAssert(typeof(string[]),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), s1,
                s2, false);
        }

        [Test]
        public void TestSerializeSimpleArraySetNull()
        {
            string[] s1 = {"ABC", "DEF"};

            const string result =
                @"<Array1OfString Action=""SetNull"" />";

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    var ser = new DivideIEnumerable(writer,
                        new TypeExtend(typeof(string[]), writer.IgnoreAttributeType));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), s1, null));
                }

                AssertHelper.AreEqual(result, stream, string.Empty);
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (ITextReader reader = new XmlTextReader(xmlReader))
                    {
                        var com = new CombineIEnumerable(new TypeExtend(typeof(BookClassCollection), null));
                        var b3 =
                            com.Combine(reader, s1, ReflectionUtils.GetTypeFriendlyName(s1.GetType())) as
                                BookClassCollection;

                        Assert.AreEqual(null, b3);
                    }
                }
            }

            s1 = new string[] { "ABC", "DEF" };

            DoAssert(typeof(string[]),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), s1,
                null, true);
            DoAssert(typeof(string[]),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), s1,
                null, false);
        }

        [Test]
        public void TestSerializeSimpleArraySetNullByChangedAction()
        {
            string[] s1 = {"ABC", "DEF"};

            const string newActionName = "Action123";

            const string result =
                "<Array1OfString " + newActionName + "=\"SetNull\" />";

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    writer.Setting.ActionName = newActionName;
                    var ser = new DivideIEnumerable(writer,
                        new TypeExtend(typeof(string[]), writer.IgnoreAttributeType));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), s1, null));
                }

                AssertHelper.AreEqual(result, stream, string.Empty);
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (ITextReader reader = new XmlTextReader(xmlReader))
                    {
                        reader.Setting.ActionName = newActionName;
                        var com = new CombineIEnumerable(new TypeExtend(typeof(BookClassCollection), null));
                        var b3 =
                            com.Combine(reader, s1, ReflectionUtils.GetTypeFriendlyName(s1.GetType())) as
                                BookClassCollection;

                        Assert.AreEqual(null, b3);
                    }
                }
            }

            s1 = new string[] { "ABC", "DEF" };

            DoAssert(typeof(string[]),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), s1,
                null, true,new XmlSerializeSetting(){ActionName = newActionName });
            DoAssert(typeof(string[]),
                string.Format("{0}{1}{2}", ForXml.TestHelper.XmlHeaderContext, System.Environment.NewLine, result), s1,
                null, false, new XmlSerializeSetting() { ActionName = newActionName });
        }
    }
}