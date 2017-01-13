using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            bool exceptionCatched = false;
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
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
        }

        [TestMethod]
        [Description("测试合并类型没有定义主键的对象是否会抛出异常")]
        public void TestCombineNullPrimaryKeyDefingeClass()
        {
            bool exceptionCatched = false;
            try
            {
                new CombineIEnumerable(new TypeExtend(typeof(List<AuthorClass>)));
            }
            catch (AttributeMissException ex)
            {
                Assert.AreEqual("PrimaryKeyAttribute", ex.AttributeName);
                Assert.AreEqual(typeof(AuthorClass), ex.ErrorType);
                Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "类型 {0} 上没有定义 '{1}' Attribute .", ex.ErrorType.FullName, ex.AttributeName), ex.Message);
                exceptionCatched = true;
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
            if (!exceptionCatched)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
        }

        [TestMethod]
        [Description("测试合并类型没有定义主键的对象是否会抛出异常")]
        public void TestCombineNullPrimaryKeyDefingeClassWithXPatchSerializer()
        {
            bool exceptionCatched = false;
            try
            {
                List<AuthorClass> a = new List<AuthorClass>();
                a.Add(new AuthorClass() { Name = "Author A" });
                a.Add(new AuthorClass() { Name = "Author B" });
                a.Add(new AuthorClass() { Name = "Author C" });

                XPatchSerializer serializer = new XPatchSerializer(typeof(List<AuthorClass>));
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Divide(stream, null, a);
                }
            }
            catch (AttributeMissException ex)
            {
                Assert.AreEqual("PrimaryKeyAttribute", ex.AttributeName);
                Assert.AreEqual(typeof(AuthorClass), ex.ErrorType);
                Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "类型 {0} 上没有定义 '{1}' Attribute .", ex.ErrorType.FullName, ex.AttributeName), ex.Message);
                exceptionCatched = true;
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
            if (!exceptionCatched)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
        }

        [TestMethod]
        [Description("测试合并类型没有定义主键的对象是否会抛出异常")]
        public void TestCombineNullPrimaryKeyDefingeClassWithXPatchSerializerRegisteredType()
        {
            List<AuthorClass> a = new List<AuthorClass>();
            a.Add(new AuthorClass() { Name = "Author A" });
            a.Add(new AuthorClass() { Name = "Author B" });
            a.Add(new AuthorClass() { Name = "Author C" });

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

            XPatchSerializer serializer = new XPatchSerializer(typeof(List<AuthorClass>));

            Dictionary<Type, string[]> types = new Dictionary<Type, string[]>();
            types.Add(typeof(AuthorClass), new string[] { "Name" });

            string context = string.Empty;

            serializer.RegisterTypes(types);
            using (MemoryStream stream = new MemoryStream())
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
            XPatchSerializer serializer = new XPatchSerializer(typeof(BookClassCollection));

            const string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<BookClassCollection />";

            BookClassCollection c = new BookClassCollection();
            c.Add(new BookClass() { Name = "A" });
            c.Add(new BookClass() { Name = "B" });
            c.Add(new BookClass() { Name = "C" });
            c.Add(new BookClass() { Name = "D" });

            using (StringReader reader = new StringReader(result))
            {
                BookClassCollection b = serializer.Combine(reader, c) as BookClassCollection;

                Assert.AreEqual(b, c);
            }
        }

        [TestMethod]
        public void TestDivideAndSerializeBookClassCollection()
        {
            BookClassCollection b = new BookClassCollection();

            XPatchSerializer serializer = new XPatchSerializer(typeof(BookClassCollection));

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

            b.Add(new BookClass() { Name = "A" });
            b.Add(new BookClass() { Name = "B" });
            b.Add(new BookClass() { Name = "C" });
            b.Add(new BookClass() { Name = "D" });

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, b);
                string context = TestHelper.StreamToString(stream);
                Assert.AreEqual(result, context);
            }

            using (TextReader reader = new StringReader(result))
            {
                BookClassCollection b1 = serializer.Combine(reader, null) as BookClassCollection;
                Assert.IsNotNull(b1);
                Assert.AreEqual(b, b1);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, new BookClassCollection(), b);
                string context = TestHelper.StreamToString(stream);
                Assert.AreEqual(result, context);
            }

            using (TextReader reader = new StringReader(result))
            {
                BookClassCollection b1 = serializer.Combine(reader, new BookClassCollection()) as BookClassCollection;
                Assert.IsNotNull(b1);
                Assert.AreEqual(b, b1);
            }
        }

        [TestMethod]
        public void TestDivideAndSerializeEmptyBookClassCollection()
        {
            BookClassCollection b = new BookClassCollection();

            XPatchSerializer serializer = new XPatchSerializer(typeof(BookClassCollection));

            const string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<BookClassCollection />";

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, b);
                string context = TestHelper.StreamToString(stream);
                Assert.AreEqual(result, context);
            }

            using (TextReader reader = new StringReader(result))
            {
                BookClassCollection b1 = serializer.Combine(reader, null) as BookClassCollection;
                Assert.IsNotNull(b1);
                Assert.AreEqual(b, b1);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                //b是空白集合，与空白集合产生增量内容，为空白内容。
                serializer.Divide(stream, new BookClassCollection(), b);
                string context = TestHelper.StreamToString(stream);
                Assert.IsTrue(string.IsNullOrEmpty(context));
            }

            using (TextReader reader = new StringReader(result))
            {
                BookClassCollection b1 = serializer.Combine(reader, new BookClassCollection()) as BookClassCollection;
                Assert.IsNotNull(b1);
                Assert.AreEqual(b, b1);
            }
        }

        [TestMethod]
        [Description("测试拆分两个相同内容的集合，应该产生空白内容")]
        public void TestDivideAndSerializeSameBookClassCollection()
        {
            BookClassCollection b = new BookClassCollection();
            b.Add(new BookClass() { Name = "A" });
            b.Add(new BookClass() { Name = "B" });
            b.Add(new BookClass() { Name = "C" });
            b.Add(new BookClass() { Name = "D" });

            XPatchSerializer serializer = new XPatchSerializer(typeof(BookClassCollection));

            using (MemoryStream stream = new MemoryStream())
            {
                BookClassCollection c = new BookClassCollection();
                c.Add(new BookClass() { Name = "A" });
                c.Add(new BookClass() { Name = "B" });
                c.Add(new BookClass() { Name = "C" });
                c.Add(new BookClass() { Name = "D" });
                serializer.Divide(stream, c, b);
                string context = TestHelper.StreamToString(stream);
                Assert.AreEqual(string.Empty, context);
            }
        }

        [TestMethod]
        [Description("测试拆分集合类型但是传入的类型不是集合类型时，是否抛出ArgumentOutOfRangeException错误")]
        public void TestDivideNonCollectionType()
        {
            bool exceptionCatched = false;
            try
            {
                new DivideIEnumerable(new TypeExtend(typeof(AuthorClass)));
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
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
        }

        [TestMethod]
        [Description("测试拆分类型没有定义主键的对象是否会抛出异常")]
        public void TestDivideNullPrimaryKeyDefingeClass()
        {
            bool exceptionCatched = false;
            try
            {
                new XPatchSerializer(typeof(List<AuthorClass>)).Divide(new MemoryStream(), null, new List<AuthorClass>());
            }
            catch (AttributeMissException ex)
            {
                Assert.AreEqual("PrimaryKeyAttribute", ex.AttributeName);
                Assert.AreEqual(typeof(AuthorClass), ex.ErrorType);
                Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "类型 {0} 上没有定义 '{1}' Attribute .", ex.ErrorType.FullName, ex.AttributeName), ex.Message);
                exceptionCatched = true;
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
            if (!exceptionCatched)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
        }

        [TestMethod]
        public void TestSerializeBookClassCollection()
        {
            DivideIEnumerable ser = new DivideIEnumerable(new TypeExtend(typeof(BookClassCollection)));

            BookClassCollection b1 = new BookClassCollection();
            BookClassCollection b2 = new BookClassCollection();

            b1.Add(new BookClass() { Name = "A" });
            b1.Add(new BookClass() { Name = "B" });

            b2.Add(new BookClass() { Name = "B" });
            b2.Add(new BookClass() { Name = "C" });

            const string result =
@"<BookClassCollection>
  <BookClass Action=""Remove"" Name=""A"" />
  <BookClass Action=""Add"">
    <Name>C</Name>
  </BookClass>
</BookClassCollection>";

            XElement ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(BookClassCollection)), b1, b2);
            Assert.AreEqual(result, ele.ToString());

            CombineIEnumerable com = new CombineIEnumerable(new TypeExtend(typeof(BookClassCollection)));
            BookClassCollection b3 = com.Combine(b1, ele) as BookClassCollection;

            Assert.AreEqual(b2, b3);
        }

        [TestMethod]
        public void TestSerializeBookClassCollectionSerializeDefaultValue()
        {
            DivideIEnumerable ser = new DivideIEnumerable(new TypeExtend(typeof(BookClassCollection)), true);

            BookClassCollection b1 = new BookClassCollection();
            BookClassCollection b2 = new BookClassCollection();

            b1.Add(new BookClass() { Name = "A" });
            b1.Add(new BookClass() { Name = "B" });

            b2.Add(new BookClass() { Name = "B" });
            b2.Add(new BookClass() { Name = "C" });

            const string result =
@"<BookClassCollection>
  <BookClass Action=""Remove"" Name=""A"" />
  <BookClass Action=""Add"">
    <Name>C</Name>
    <Price>0</Price>
    <PublishYear>0</PublishYear>
  </BookClass>
</BookClassCollection>";

            XElement ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(BookClassCollection)), b1, b2);
            Assert.AreEqual(result, ele.ToString());

            CombineIEnumerable com = new CombineIEnumerable(new TypeExtend(typeof(BookClassCollection)));
            BookClassCollection b3 = com.Combine(b1, ele) as BookClassCollection;

            Assert.AreEqual(b2, b3);
        }

        [TestMethod]
        public void TestSerializeSimpleArray()
        {
            string[] s1 = new string[] { "ABC", "DEF" };
            string[] s2 = new string[] { "DEF", "HGI" };

            const string result =
@"<Array1OfString>
  <String Action=""Remove"">ABC</String>
  <String Action=""Add"">HGI</String>
</Array1OfString>";

            DivideIEnumerable ser = new DivideIEnumerable(new TypeExtend(typeof(string[])));
            XElement ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), s1, s2);

            Assert.AreEqual(result, ele.ToString());

            CombineIEnumerable com = new CombineIEnumerable(new TypeExtend(typeof(string[])));
            string[] s3 = com.Combine(s1, ele) as string[];
            Assert.AreEqual(s2.Length, s3.Length);

            Assert.IsTrue(s3.Contains(s2[0]));
            Assert.IsTrue(s3.Contains(s2[1]));
        }

        [TestMethod]
        public void TestSerializeSimpleArrayAdd()
        {
            string[] s1 = new string[] { "ABC", "DEF" };

            const string result =
@"<Array1OfString>
  <String Action=""Add"">ABC</String>
  <String Action=""Add"">DEF</String>
</Array1OfString>";

            DivideIEnumerable ser = new DivideIEnumerable(new TypeExtend(typeof(string[])));
            XElement ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), null, s1);

            Assert.AreEqual(result, ele.ToString());

            CombineIEnumerable com = new CombineIEnumerable(new TypeExtend(typeof(string[])));
            string[] s2 = com.Combine(new string[] { }, ele) as string[];
            Assert.AreEqual(2, s2.Length);

            Assert.IsTrue(s2.Contains("ABC"));
            Assert.IsTrue(s2.Contains("DEF"));
        }

        [TestMethod]
        public void TestSerializeSimpleArrayRemove()
        {
            string[] s1 = new string[] { "ABC", "DEF" };
            string[] s2 = new string[] { "DEF" };

            const string result =
@"<Array1OfString>
  <String Action=""Remove"">ABC</String>
</Array1OfString>";

            DivideIEnumerable ser = new DivideIEnumerable(new TypeExtend(typeof(string[])));
            XElement ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), s1, s2);

            Assert.AreEqual(result, ele.ToString());

            CombineIEnumerable com = new CombineIEnumerable(new TypeExtend(typeof(string[])));
            string[] s3 = com.Combine(s1, ele) as string[];

            Assert.AreEqual(s2.Length, s3.Length);

            Assert.AreEqual(s2[0], s3[0]);
        }

        [TestMethod]
        public void TestSerializeSimpleArraySetNull()
        {
            string[] s1 = new string[] { "ABC", "DEF" };

            const string result =
@"<Array1OfString Action=""SetNull"" />";

            DivideIEnumerable ser = new DivideIEnumerable(new TypeExtend(typeof(string[])));
            XElement ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(s1.GetType()), s1, null);

            Assert.AreEqual(result, ele.ToString());

            CombineIEnumerable com = new CombineIEnumerable(new TypeExtend(typeof(BookClassCollection)));
            BookClassCollection b3 = com.Combine(s1, ele) as BookClassCollection;

            Assert.AreEqual(null, b3);
        }

        #endregion Public Methods
    }
}