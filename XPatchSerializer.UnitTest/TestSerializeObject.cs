using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestSerializeObject
    {
        #region Public Methods

        [TestMethod]
        public void CultureChangeTest()
        {
            CultureInfo curCulture = CultureInfo.CurrentCulture;

            XPatchSerializer serializer = null;

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            serializer = new XPatchSerializer(typeof(CultureClass), System.Xml.XmlDateTimeSerializationMode.Unspecified);
            string frResult = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, CultureClass.GetSampleInstance());

                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    frResult = stremReader.ReadToEnd();
                }
            }
            Trace.WriteLine(frResult);

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fa-IR");
            serializer = new XPatchSerializer(typeof(CultureClass), System.Xml.XmlDateTimeSerializationMode.Unspecified);
            string faResult = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, CultureClass.GetSampleInstance());

                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    faResult = stremReader.ReadToEnd();
                }
            }
            Trace.WriteLine(faResult);

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            serializer = new XPatchSerializer(typeof(CultureClass), System.Xml.XmlDateTimeSerializationMode.Unspecified);
            string deResult = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, CultureClass.GetSampleInstance());

                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    deResult = stremReader.ReadToEnd();
                }
            }
            Trace.WriteLine(deResult);

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            serializer = new XPatchSerializer(typeof(CultureClass), System.Xml.XmlDateTimeSerializationMode.Unspecified);
            string usResult = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, CultureClass.GetSampleInstance());

                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    usResult = stremReader.ReadToEnd();
                }
            }
            Trace.WriteLine(usResult);

            Thread.CurrentThread.CurrentCulture = curCulture;

            Assert.AreEqual(faResult, frResult, "Comparing FR and FA");
            Assert.AreEqual(deResult, faResult, "Comparing FA and DE");
            Assert.AreEqual(usResult, deResult, "Comparing DE and US");
        }

        [TestMethod]
        [Description("测试多层级对象的增量和数据合并-增加元素")]
        public void SerializeMulitiLevelClass_AddItem()
        {
            string result =
@"<MultilevelClass>
  <Items>
    <FirstLevelClass Action=""Add"">
      <ID>3</ID>
      <Second>
        <SecondID>3-1</SecondID>
      </Second>
    </FirstLevelClass>
  </Items>
</MultilevelClass>";

            MultilevelClass c = MultilevelClass.GetSampleInstance();
            c.Items.Add(new FirstLevelClass() { ID = "3", Second = new SecondLevelClass() { SecondID = "3-1" } });

            DivideObject ser = new DivideObject(new TypeExtend(typeof(MultilevelClass)));
            XElement ele = ser.Divide(typeof(MultilevelClass).Name, MultilevelClass.GetSampleInstance(), c);
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());

            CombineObject dSer = new CombineObject(new TypeExtend(typeof(MultilevelClass)));
            MultilevelClass newItem = dSer.Combine(MultilevelClass.GetSampleInstance(), ele) as MultilevelClass;

            Assert.AreEqual(c.Items.Count, newItem.Items.Count);
            Assert.IsTrue(c.Equals(newItem));
        }

        [TestMethod]
        [Description("测试多层级对象的增量和数据合并-编辑元素")]
        public void SerializeMulitiLevelClass_EditItem()
        {
            string result =
@"<MultilevelClass>
  <Items>
    <FirstLevelClass ID=""2"">
      <Second>
        <SecondID>3-1</SecondID>
      </Second>
    </FirstLevelClass>
  </Items>
</MultilevelClass>";

            MultilevelClass c = MultilevelClass.GetSampleInstance();
            c.Items[1].Second = new SecondLevelClass() { SecondID = "3-1" };

            DivideObject ser = new DivideObject(new TypeExtend(typeof(MultilevelClass)));
            XElement ele = ser.Divide(typeof(MultilevelClass).Name, MultilevelClass.GetSampleInstance(), c);
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());

            CombineObject dSer = new CombineObject(new TypeExtend(typeof(MultilevelClass)));
            MultilevelClass newItem = dSer.Combine(MultilevelClass.GetSampleInstance(), ele) as MultilevelClass;

            Assert.AreEqual(c.Items.Count, newItem.Items.Count);
            Assert.AreEqual(c, newItem);
        }

        [TestMethod]
        [Description("测试多层级对象的增量和数据合并-原始值为null")]
        public void SerializeMulitiLevelClass_OriValueIsNull()
        {
            string result =
@"<MultilevelClass>
  <Items>
    <FirstLevelClass Action=""Add"">
      <ID>1</ID>
      <Second>
        <SecondID>1-2</SecondID>
      </Second>
    </FirstLevelClass>
    <FirstLevelClass Action=""Add"">
      <ID>2</ID>
    </FirstLevelClass>
  </Items>
</MultilevelClass>";

            DivideObject ser = new DivideObject(new TypeExtend(typeof(MultilevelClass)));
            XElement ele = ser.Divide(typeof(MultilevelClass).Name, null, MultilevelClass.GetSampleInstance());
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());

            CombineObject dSer = new CombineObject(new TypeExtend(typeof(MultilevelClass)));
            MultilevelClass newItem = dSer.Combine(null, ele) as MultilevelClass;

            Assert.AreEqual(MultilevelClass.GetSampleInstance().Items.Count, newItem.Items.Count);
            Assert.IsTrue(MultilevelClass.GetSampleInstance().Equals(newItem));
        }

        [TestMethod]
        [Description("测试多层级对象的增量和数据合并-删除元素")]
        public void SerializeMulitiLevelClass_RemoveItem()
        {
            string result =
@"<MultilevelClass>
  <Items>
    <FirstLevelClass Action=""Remove"" ID=""1"" />
  </Items>
</MultilevelClass>";

            MultilevelClass c = MultilevelClass.GetSampleInstance();
            c.Items.RemoveAt(0);

            DivideObject ser = new DivideObject(new TypeExtend(typeof(MultilevelClass)));
            XElement ele = ser.Divide(typeof(MultilevelClass).Name, MultilevelClass.GetSampleInstance(), c);
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());

            //Assert.Fail("缺少分序列化部分。");
            CombineObject com = new CombineObject(new TypeExtend(typeof(MultilevelClass)));
            MultilevelClass combineObj = com.Combine(MultilevelClass.GetSampleInstance(), ele) as MultilevelClass;

            Assert.AreEqual(c.Items.Count, combineObj.Items.Count);
            Assert.IsTrue(c.Equals(combineObj));
        }

        [TestMethod]
        public void SerializeNullable_OriValueIsNotNull()
        {
            NullableClass b1 = NullableClass.GetSampleInstance();
            DivideObject ser = new DivideObject(new TypeExtend(typeof(NullableClass)));
            CombineObject dser = new CombineObject(new TypeExtend(typeof(NullableClass)));
            string result = string.Empty;
            XElement ele = null;

            NullableClass b2 = NullableClass.GetSampleInstance();
            b2.PurchaseYear = 2003;
            result =
@"<NullableClass>
  <PurchaseYear>2003</PurchaseYear>
</NullableClass>";

            ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(NullableClass)), b1, b2);
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());

            NullableClass b3 = dser.Combine(b1, ele) as NullableClass;
            Assert.AreEqual(b2.Title, b3.Title);
            Assert.AreEqual(b2.PublishYear, b3.PublishYear);
            Assert.AreEqual(b2.PurchaseYear, b3.PurchaseYear);
        }

        [TestMethod]
        public void SerializeNullable_OriValueIsNull()
        {
            NullableClass b1 = NullableClass.GetSampleInstance();
            DivideObject ser = new DivideObject(new TypeExtend(typeof(NullableClass)));
            CombineObject dser = new CombineObject(new TypeExtend(typeof(NullableClass)));
            string result = string.Empty;

            result =
@"<NullableClass>
  <PublishYear>2002</PublishYear>
  <Title>Amazon</Title>
</NullableClass>";

            XElement ele = null;
            //原始对象（null）与更新对象产生增量内容
            ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(NullableClass)), null, b1);
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());

            NullableClass b2 = dser.Combine(NullableClass.GetSampleInstance(), ele) as NullableClass;
            Assert.AreEqual(b1.Title, b2.Title);
            Assert.AreEqual(b1.PublishYear, b2.PublishYear);
            Assert.AreEqual(b1.PurchaseYear, b2.PurchaseYear);
        }

        [TestMethod]
        public void SerializeSimpleNestedType()
        {
            BookClass b1 = BookClass.GetSampleInstance();

            DivideObject ser = new DivideObject(new TypeExtend(typeof(BookClass)));

            string result = string.Empty;
            XElement ele = null;

            result =
string.Format(@"<BookClass>
  <Author>
    <Comments>{5}</Comments>
    <Name>{4}</Name>
  </Author>
  <Comments>{3}</Comments>
  <Name>{0}</Name>
  <Price>{1}</Price>
  <PublishYear>{2}</PublishYear>
</BookClass>", b1.Name, b1.Price, b1.PublishYear, b1.Comments, b1.Author.Name, b1.Author.Comments);
            string testValue = string.Empty;

            ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(BookClass)), null, b1);
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());

            BookClass b2 = BookClass.GetSampleInstance();
            b2.Name = "耶路撒冷三千年";
            b2.Comments = @"《耶路撒冷三千年》内容简介：美国前总统克林顿2011年年度选书，基辛格视若珍宝的经典巨作。读《耶路撒冷三千年》，了解真实的耶路撒冷，就会明白世界为何演变成今天的模样。耶路撒冷曾被视为世界的中心，是基督教、犹太教和伊斯兰教三大宗教的圣地，是文明冲突的战略要冲，是让世人魂牵梦绕的去处，是惑人的阴谋、虚构的网络传说和二十四小时新闻发生的地方。
西蒙•蒙蒂菲奥里依年代顺序，以三大宗教围绕“圣城”的角逐，以几大家族的兴衰更迭为主线，生动讲述了耶路撒冷的前世今生；作者通过大量的田野调查和文献考据，以客观、中立的角度，透过士兵与先知、诗人与国王、农民与音乐家的生活，以及创造耶路撒冷的家族来呈现这座城市的三千年瑰丽历史，还原真实的耶路撒冷……";
            b2.Author.Name = "西蒙·蒙蒂菲奥里 (Simon Sebag Montefiore)";
            b2.Author.Comments = "";

            result =
string.Format(@"<BookClass>
  <Author>
    <Comments>{3}</Comments>
    <Name>{2}</Name>
  </Author>
  <Comments>{1}</Comments>
  <Name>{0}</Name>
</BookClass>", b2.Name, b2.Comments, b2.Author.Name, b2.Author.Comments);

            ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(BookClass)), b1, b2);
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());
        }

        [TestMethod]
        public void SerializeSimpleNestedType_DecimalProperty()
        {
            BookClassWithDecimalPrice b1 = BookClassWithDecimalPrice.GetSampleInstance();

            DivideObject ser = new DivideObject(new TypeExtend(typeof(BookClassWithDecimalPrice)));
            CombineObject dser = new CombineObject(new TypeExtend(typeof(BookClassWithDecimalPrice)));

            string result = string.Empty;
            XElement ele = null;

            result =
string.Format(@"<BookClassWithDecimalPrice>
  <Author>
    <Comments>{5}</Comments>
    <Name>{4}</Name>
  </Author>
  <Comments>{3}</Comments>
  <Name>{0}</Name>
  <Price>{1}</Price>
  <PublishYear>{2}</PublishYear>
</BookClassWithDecimalPrice>", b1.Name, b1.Price, b1.PublishYear, b1.Comments, b1.Author.Name, b1.Author.Comments);

            string testValue = string.Empty;

            //原始对象（null）与更新对象产生增量内容
            ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(BookClassWithDecimalPrice)), null, b1);
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());
            //创建另外一个原始对象
            BookClassWithDecimalPrice b2 = BookClassWithDecimalPrice.GetSampleInstance();
            b2 = dser.Combine(b2, ele) as BookClassWithDecimalPrice;
            Assert.AreEqual(b1.Name, b2.Name);
            Assert.AreEqual(b1.Comments, b2.Comments);
            Assert.AreEqual(b1.Price, b2.Price);
            Assert.AreEqual(b1.PublishYear, b2.PublishYear);
        }

        [TestMethod]
        [Description("测试原始对象为null，直接序列化")]
        public void SerializeSimpleStruct_OriValueIsNull()
        {
            AuthorStruct b1 = AuthorStruct.GetSampleInstance();

            DivideObject ser = new DivideObject(new TypeExtend(typeof(AuthorStruct)));
            CombineObject dser = new CombineObject(new TypeExtend(typeof(AuthorStruct)));

            string result = string.Empty;
            XElement ele = null;

            result =
string.Format(@"<AuthorStruct>
  <Comments>{1}</Comments>
  <Name>{0}</Name>
</AuthorStruct>", b1.Name, b1.Comments);
            string testValue = string.Empty;

            //原始对象（null）与更新对象产生增量内容
            ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(AuthorStruct)), null, b1);
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());
            //创建另外一个原始对象
            AuthorStruct b2 = AuthorStruct.GetSampleInstance();
            b2 = dser.Combine(b2, ele) as AuthorStruct;
            Assert.AreEqual(b1.Name, b2.Name);
            Assert.AreEqual(b1.Comments, b2.Comments);
        }

        [TestMethod]
        [Description("测试原始对象不为null，产生增量结果")]
        public void SerializeSimpleType_OriValueIsNotNull()
        {
            AuthorClass b1 = AuthorClass.GetSampleInstance();

            DivideObject ser = new DivideObject(new TypeExtend(typeof(AuthorClass)));
            CombineObject dser = new CombineObject(new TypeExtend(typeof(AuthorClass)));

            string result = string.Empty;
            XElement ele = null;

            AuthorClass b2 = AuthorClass.GetSampleInstance();
            b2.Name = "西蒙·蒙蒂菲奥里 (Simon Sebag Montefiore)";
            b2.Comments = @"";

            result =
string.Format(@"<AuthorClass>
  <Comments>{1}</Comments>
  <Name>{0}</Name>
</AuthorClass>", b2.Name, b2.Comments);

            //原始对象与更新对象产生增量内容
            ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(AuthorClass)), b1, b2);
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());
            //创建原始对象
            b1 = AuthorClass.GetSampleInstance();
            //对原始对象进行增量内容数据合并
            b1 = dser.Combine(b1, ele) as AuthorClass;
            //合并后的对象与更新对象的数据比较
            Assert.AreEqual(b1.Name, b2.Name);
            Assert.AreEqual(b1.Comments, b2.Comments);
        }

        [TestMethod]
        [Description("测试原始对象为null，直接序列化")]
        public void SerializeSimpleType_OriValueIsNull()
        {
            AuthorClass b1 = AuthorClass.GetSampleInstance();

            DivideObject ser = new DivideObject(new TypeExtend(typeof(AuthorClass)));
            CombineObject dser = new CombineObject(new TypeExtend(typeof(AuthorClass)));

            string result = string.Empty;
            XElement ele = null;

            result =
string.Format(@"<AuthorClass>
  <Comments>{1}</Comments>
  <Name>{0}</Name>
</AuthorClass>", b1.Name, b1.Comments);
            string testValue = string.Empty;

            //原始对象（null）与更新对象产生增量内容
            ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(AuthorClass)), null, b1);
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());
            //创建另外一个原始对象
            AuthorClass b2 = AuthorClass.GetSampleInstance();
            b2 = dser.Combine(b2, ele) as AuthorClass;
            Assert.AreEqual(b1.Name, b2.Name);
            Assert.AreEqual(b1.Comments, b2.Comments);
        }

        [TestMethod]
        public void SerializeSimpleType_RevValueIsNull()
        {
            AuthorClass b1 = AuthorClass.GetSampleInstance();

            DivideObject ser = new DivideObject(new TypeExtend(typeof(AuthorClass)));
            CombineObject dser = new CombineObject(new TypeExtend(typeof(AuthorClass)));

            string result = string.Empty;
            XElement ele = null;

            AuthorClass b2 = null;

            result =
@"<AuthorClass Action=""SetNull"" />";

            //原始对象与更新对象产生增量内容
            ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(AuthorClass)), b1, b2);
            Assert.IsNotNull(ele);
            Assert.AreEqual(result, ele.ToString());
            //创建原始对象
            b1 = AuthorClass.GetSampleInstance();
            //对原始对象进行增量内容数据合并
            b1 = dser.Combine(b1, ele) as AuthorClass;
            //合并后的对象与更新对象的数据比较
            Assert.IsNull(b1);
        }

        [TestMethod]
        [Description("测试合并类型定义了错误的主键的对象是否会抛出异常")]
        public void TestCombineErrorPrimaryKeyDefineClass()
        {
            try
            {
                new CombineObject(new TypeExtend(typeof(ErrorPrimaryKeyDefineClass)));
            }
            catch (PrimaryKeyException ex)
            {
                Assert.AreEqual(typeof(ErrorPrimaryKeyDefineClass), ex.SourceType);
                Assert.AreEqual("Author", ex.PrimaryKeyName);
                Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, " '{0}' 的 主键 {1} 设置异常.", ex.SourceType.Name, ex.PrimaryKeyName), ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
        }

        [TestMethod]
        public void TestDivideAndSerializeSimpleType()
        {
            XPatchSerializer serializer = new XPatchSerializer(typeof(AuthorClass));
            AuthorClass b1 = AuthorClass.GetSampleInstance();
            AuthorClass b2 = AuthorClass.GetSampleInstance();

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, b1, b2);
                string context = TestHelper.StreamToString(stream);
                Assert.AreEqual(string.Empty, context);
            }
        }

        [TestMethod]
        [Description("测试增量内容类型定义了错误的主键的对象是否会抛出异常")]
        public void TestDivideErrorPrimaryKeyDefineClass()
        {
            try
            {
                new DivideObject(new TypeExtend(typeof(ErrorPrimaryKeyDefineClass)));
            }
            catch (PrimaryKeyException ex)
            {
                Assert.AreEqual(typeof(ErrorPrimaryKeyDefineClass), ex.SourceType);
                Assert.AreEqual("Author", ex.PrimaryKeyName);
                Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, " '{0}' 的 主键 {1} 设置异常.", ex.SourceType.Name, ex.PrimaryKeyName), ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
        }

        [TestMethod]
        public void ThreadingTest()
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    var th = new Thread(() =>
                    {
                        XPatchSerializer serializer = new XPatchSerializer(typeof(BookClass));
                        string changedContext = string.Empty;
                        using (MemoryStream stream = new MemoryStream())
                        {
                            serializer.Divide(stream, null, BookClass.GetSampleInstance());

                            stream.Position = 0;
                            using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                            {
                                changedContext = stremReader.ReadToEnd();
                            }
                        }
                        Trace.Write(changedContext);
                        XPatchSerializer deserializer = new XPatchSerializer(typeof(BookClass));
                        BookClass book = null;
                        using (TextReader reader = new StringReader(changedContext))
                        {
                            book = deserializer.Combine(reader, null) as BookClass;
                        }
                        Assert.IsNotNull(book);
                        Assert.IsInstanceOfType(book, typeof(BookClass));
                        Assert.AreEqual(BookClass.GetSampleInstance(), book);
                    }
                    );

                    th.Start();
                }
            }
            catch
            {
                Assert.Fail("Exception fired in threading method");
            }
        }

        #endregion Public Methods
    }
}