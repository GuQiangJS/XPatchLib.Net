using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestSerializeArray
    {
        [TestInitialize]
        public void TestInitialize()
        {
            TypeExtendContainer.Clear();
        }

        #region Public Methods

        [TestMethod]
        [Description("测试合并集合类型但是传入的类型不是集合类型时，是否抛出ArgumentOutOfRangeException错误")]
        public void TestCombineNonCollectionType()
        {
            var exceptionCatched = false;
            try
            {
                new CombineIEnumerable(new TypeExtend(typeof(AuthorClass)));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual(ex.ParamName, typeof(AuthorClass).FullName);
                exceptionCatched = true;
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出ArgumentOutOfRangeException异常");
            }
            if (!exceptionCatched)
                Assert.Fail("未能抛出PrimaryKeyException异常");
        }

        [TestMethod]
        [Description("测试合并类型没有定义主键的对象是否会抛出异常")]
        public void TestCombineNullPrimaryKeyDefingeClass()
        {
            var exceptionCatched = false;
            try
            {
                new CombineIEnumerable(new TypeExtend(typeof(List<AuthorClass>)));
            }
            catch (AttributeMissException ex)
            {
                Assert.AreEqual("PrimaryKeyAttribute", ex.AttributeName);
                Assert.AreEqual(typeof(AuthorClass), ex.ErrorType);
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture, "类型 {0} 上没有定义 '{1}' Attribute .", ex.ErrorType.FullName,
                        ex.AttributeName), ex.Message);
                exceptionCatched = true;
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
            if (!exceptionCatched)
                Assert.Fail("未能抛出PrimaryKeyException异常");
        }

        [TestMethod]
        [Description("测试合并类型没有定义主键的对象是否会抛出异常")]
        public void TestCombineNullPrimaryKeyDefingeClassWithXPatchSerializer()
        {
            var exceptionCatched = false;
            try
            {
                var a = new List<AuthorClass>();
                a.Add(new AuthorClass {Name = "Author A"});
                a.Add(new AuthorClass {Name = "Author B"});
                a.Add(new AuthorClass {Name = "Author C"});

                var serializer = new XPatchSerializer(typeof(List<AuthorClass>));
                using (var stream = new MemoryStream())
                {
                    serializer.Divide(stream, null, a);
                }
            }
            catch (AttributeMissException ex)
            {
                Assert.AreEqual("PrimaryKeyAttribute", ex.AttributeName);
                Assert.AreEqual(typeof(AuthorClass), ex.ErrorType);
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture, "类型 {0} 上没有定义 '{1}' Attribute .", ex.ErrorType.FullName,
                        ex.AttributeName), ex.Message);
                exceptionCatched = true;
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
            if (!exceptionCatched)
                Assert.Fail("未能抛出PrimaryKeyException异常");
        }

        [TestMethod]
        public void TestCombineNullPrimaryKeyDefingeClassWithXPatchSerializerRegisteredType()
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

            var serializer = new XPatchSerializer(typeof(List<AuthorClass>));

            var types = new Dictionary<Type, string[]>();
            types.Add(typeof(AuthorClass), new[] {"Name"});

            var context = string.Empty;

            serializer.RegisterTypes(types);
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, null, a);
                context = TestHelper.StreamToString(stream);
            }

            Assert.AreEqual(result, context);
        }

        [TestMethod]
        [Description("测试合并一个只有空白的根节点的内容")]
        public void TestCombineSameBookClassCollection()
        {
            var serializer = new XPatchSerializer(typeof(BookClassCollection));

            const string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<BookClassCollection />";

            var c = new BookClassCollection();
            c.Add(new BookClass {Name = "A"});
            c.Add(new BookClass {Name = "B"});
            c.Add(new BookClass {Name = "C"});
            c.Add(new BookClass {Name = "D"});

            using (var reader = new StringReader(result))
            {
                var b = serializer.Combine(reader, c) as BookClassCollection;

                Assert.AreEqual(b, c);
            }
        }

        [TestMethod]
        public void TestDivideAndSerializeBookClassCollection()
        {
            var b = new BookClassCollection();

            var serializer = new XPatchSerializer(typeof(BookClassCollection));

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

            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, null, b);
                var context = TestHelper.StreamToString(stream);
                Assert.AreEqual(result, context);
            }

            using (TextReader reader = new StringReader(result))
            {
                var b1 = serializer.Combine(reader, null) as BookClassCollection;
                Assert.IsNotNull(b1);
                Assert.AreEqual(b, b1);
            }

            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, new BookClassCollection(), b);
                var context = TestHelper.StreamToString(stream);
                Assert.AreEqual(result, context);
            }

            using (TextReader reader = new StringReader(result))
            {
                var b1 = serializer.Combine(reader, new BookClassCollection()) as BookClassCollection;
                Assert.IsNotNull(b1);
                Assert.AreEqual(b, b1);
            }
        }

        [TestMethod]
        public void TestDivideAndSerializeEmptyBookClassCollection()
        {
            var b = new BookClassCollection();

            var serializer = new XPatchSerializer(typeof(BookClassCollection));

            const string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<BookClassCollection />";

            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, null, b);
                var context = TestHelper.StreamToString(stream);
                Assert.AreEqual(result, context);
            }

            using (TextReader reader = new StringReader(result))
            {
                var b1 = serializer.Combine(reader, null) as BookClassCollection;
                Assert.IsNotNull(b1);
                Assert.AreEqual(b, b1);
            }

            using (var stream = new MemoryStream())
            {
                //b是空白集合，与空白集合产生增量内容，为空白内容。
                serializer.Divide(stream, new BookClassCollection(), b);
                var context = TestHelper.StreamToString(stream);
                Assert.AreEqual(TestHelper.XmlHeaderContext, context);
            }

            using (TextReader reader = new StringReader(result))
            {
                var b1 = serializer.Combine(reader, new BookClassCollection()) as BookClassCollection;
                Assert.IsNotNull(b1);
                Assert.AreEqual(b, b1);
            }
        }

        [TestMethod]
        [Description("测试拆分两个相同内容的集合，应该产生空白内容")]
        public void TestDivideAndSerializeSameBookClassCollection()
        {
            var b = new BookClassCollection();
            b.Add(new BookClass {Name = "A"});
            b.Add(new BookClass {Name = "B"});
            b.Add(new BookClass {Name = "C"});
            b.Add(new BookClass {Name = "D"});

            var serializer = new XPatchSerializer(typeof(BookClassCollection));

            using (var stream = new MemoryStream())
            {
                var c = new BookClassCollection();
                c.Add(new BookClass {Name = "A"});
                c.Add(new BookClass {Name = "B"});
                c.Add(new BookClass {Name = "C"});
                c.Add(new BookClass {Name = "D"});
                serializer.Divide(stream, c, b);
                var context = TestHelper.StreamToString(stream);
                Assert.AreEqual(TestHelper.XmlHeaderContext, context);
            }
        }

        [TestMethod]
        [Description("测试拆分集合类型但是传入的类型不是集合类型时，是否抛出ArgumentOutOfRangeException错误")]
        public void TestDivideNonCollectionType()
        {
            var exceptionCatched = false;
            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = XmlWriter.Create(stream, TestHelper.FlagmentSetting))
                    {
                        new DivideIEnumerable(writer, new TypeExtend(typeof(AuthorClass)));
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual(ex.ParamName, typeof(AuthorClass).FullName);
                exceptionCatched = true;
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出ArgumentOutOfRangeException异常");
            }
            if (!exceptionCatched)
                Assert.Fail("未能抛出PrimaryKeyException异常");
        }

        [TestMethod]
        [Description("测试拆分类型没有定义主键的对象是否会抛出异常")]
        public void TestDivideNullPrimaryKeyDefingeClass()
        {
            var exceptionCatched = false;
            try
            {
                new XPatchSerializer(typeof(List<AuthorClass>)).Divide(new MemoryStream(), null, new List<AuthorClass>());
            }
            catch (AttributeMissException ex)
            {
                Assert.AreEqual("PrimaryKeyAttribute", ex.AttributeName);
                Assert.AreEqual(typeof(AuthorClass), ex.ErrorType);
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture, "类型 {0} 上没有定义 '{1}' Attribute .", ex.ErrorType.FullName,
                        ex.AttributeName), ex.Message);
                exceptionCatched = true;
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
            if (!exceptionCatched)
                Assert.Fail("未能抛出PrimaryKeyException异常");
        }

        [TestMethod]
        public void TestSerializeBookClassCollection()
        {
            var b1 = new BookClassCollection();
            var b2 = new BookClassCollection();

            b1.Add(new BookClass {Name = "A"});
            b1.Add(new BookClass {Name = "B"});

            b2.Add(new BookClass {Name = "B"});
            b2.Add(new BookClass {Name = "C"});

            const string result =
                @"<BookClassCollection>
  <BookClass Action=""Remove"" Name=""A"" />
  <BookClass Action=""Add"">
    <Name>C</Name>
  </BookClass>
</BookClassCollection>";

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, TestHelper.FlagmentSetting))
                {
                    var ser = new DivideIEnumerable(writer, new TypeExtend(typeof(BookClassCollection)));

                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(BookClassCollection)), b1, b2));
                }
                stream.Position = 0;
                var ele = XElement.Load(stream);

                Assert.AreEqual(result, ele.ToString());
                Debug.WriteLine(ele.ToString());
                var com = new CombineIEnumerable(new TypeExtend(typeof(BookClassCollection)));
                stream.Position = 0;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    var b3 = com.Combine(reader,b1, ReflectionUtils.GetTypeFriendlyName(typeof(BookClassCollection))) as BookClassCollection;

                    Assert.AreEqual(b2, b3);
                }
            }
        }

        [TestMethod]
        public void TestSerializeBookClassCollectionSerializeDefaultValue()
        {
            var b1 = new BookClassCollection();
            var b2 = new BookClassCollection();

            b1.Add(new BookClass {Name = "A"});
            b1.Add(new BookClass {Name = "B"});

            b2.Add(new BookClass {Name = "B"});
            b2.Add(new BookClass {Name = "C"});

            const string result =
                @"<BookClassCollection>
  <BookClass Action=""Remove"" Name=""A"" />
  <BookClass Action=""Add"">
    <Name>C</Name>
    <Price>0</Price>
    <PublishYear>0</PublishYear>
  </BookClass>
</BookClassCollection>";
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, TestHelper.FlagmentSetting))
                {
                    var ser = new DivideIEnumerable(writer, new TypeExtend(typeof(BookClassCollection)), true);

                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(BookClassCollection)), b1, b2));
                }
                stream.Position = 0;
                var ele = XElement.Load(stream);


                Assert.AreEqual(result, ele.ToString());

                var com = new CombineIEnumerable(new TypeExtend(typeof(BookClassCollection)));
                stream.Position = 0;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    var b3 = com.Combine(reader,b1, ReflectionUtils.GetTypeFriendlyName(typeof(BookClassCollection))) as BookClassCollection;

                    Assert.AreEqual(b2, b3);
                }
            }
        }

        [TestMethod]
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
                using (var writer = XmlWriter.Create(stream, TestHelper.FlagmentSetting))
                {
                    var ser = new DivideIEnumerable(writer, new TypeExtend(typeof(string[])));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), s1, s2));
                }
                stream.Position = 0;
                var ele = XElement.Load(stream);

                Assert.AreEqual(result, ele.ToString());
                var com = new CombineIEnumerable(new TypeExtend(typeof(string[])));
                stream.Position = 0;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    var s3 = com.Combine(reader,s1, ReflectionUtils.GetTypeFriendlyName(s1.GetType())) as string[];
                    Assert.AreEqual(s2.Length, s3.Length);

                    Assert.IsTrue(s3.Contains(s2[0]));
                    Assert.IsTrue(s3.Contains(s2[1]));
                }
            }
        }

        [TestMethod]
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
                using (var writer = XmlWriter.Create(stream, TestHelper.FlagmentSetting))
                {
                    var ser = new DivideIEnumerable(writer, new TypeExtend(typeof(string[])));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), null, s1));
                }
                stream.Position = 0;
                var ele = XElement.Load(stream);

                Assert.AreEqual(result, ele.ToString());

                var com = new CombineIEnumerable(new TypeExtend(typeof(string[])));
                stream.Position = 0;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    var s2 =
                        com.Combine(reader, new string[] {}, ReflectionUtils.GetTypeFriendlyName(s1.GetType())) as
                            string[];
                    Assert.AreEqual(2, s2.Length);

                    Assert.IsTrue(s2.Contains("ABC"));
                    Assert.IsTrue(s2.Contains("DEF"));
                }
            }
        }

        [TestMethod]
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
                using (var writer = XmlWriter.Create(stream, TestHelper.FlagmentSetting))
                {
                    var ser = new DivideIEnumerable(writer, new TypeExtend(typeof(string[])));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), s1, s2));
                }
                stream.Position = 0;
                var ele = XElement.Load(stream);

                Assert.AreEqual(result, ele.ToString());

                stream.Position = 0;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    var com = new CombineIEnumerable(new TypeExtend(typeof(string[])));
                    var s3 = com.Combine(reader,s1, ReflectionUtils.GetTypeFriendlyName(s1.GetType())) as string[];

                    Assert.AreEqual(s2.Length, s3.Length);

                    Assert.AreEqual(s2[0], s3[0]);
                }
            }
        }

        [TestMethod]
        public void TestSerializeSimpleArraySetNull()
        {
            string[] s1 = {"ABC", "DEF"};

            const string result =
                @"<Array1OfString Action=""SetNull"" />";

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, TestHelper.FlagmentSetting))
                {
                    var ser = new DivideIEnumerable(writer, new TypeExtend(typeof(string[])));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), s1, null));
                }
                stream.Position = 0;
                var ele = XElement.Load(stream);

                Assert.AreEqual(result, ele.ToString());


                stream.Position = 0;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    var com = new CombineIEnumerable(new TypeExtend(typeof(BookClassCollection)));
                    var b3 = com.Combine(reader,s1, ReflectionUtils.GetTypeFriendlyName(s1.GetType())) as BookClassCollection;

                    Assert.AreEqual(null, b3);
                }
            }
        }

        #endregion Public Methods
    }
}