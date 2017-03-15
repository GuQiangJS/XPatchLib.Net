// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest.ForXml
{
    [TestClass]
    public class TestSerializeObject
    {
        #region Public Methods

        [TestMethod]
        public void CultureChangeTest()
        {
            CultureInfo curCulture = CultureInfo.CurrentCulture;

            Serializer serializer = null;

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            serializer = new Serializer(typeof(CultureClass));
            string frResult = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream))
                {
                    writer.Mode=DateTimeSerializationMode.Unspecified;
                    serializer.Divide(writer, null, CultureClass.GetSampleInstance());

                    stream.Position = 0;
                    using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        frResult = stremReader.ReadToEnd();
                    }
                }
            }
            Trace.WriteLine(frResult);

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fa-IR");
            string faResult = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream))
                {
                    writer.Mode = DateTimeSerializationMode.Unspecified;
                    serializer.Divide(writer, null, CultureClass.GetSampleInstance());

                    stream.Position = 0;
                    using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        faResult = stremReader.ReadToEnd();
                    }
                }
            }
            Trace.WriteLine(faResult);

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            string deResult = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream))
                {
                    writer.Mode = DateTimeSerializationMode.Unspecified;
                    serializer.Divide(writer, null, CultureClass.GetSampleInstance());

                    stream.Position = 0;
                    using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        deResult = stremReader.ReadToEnd();
                    }
                }
            }
            Trace.WriteLine(deResult);

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            string usResult = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream))
                {
                    writer.Mode = DateTimeSerializationMode.Unspecified;
                    serializer.Divide(writer, null, CultureClass.GetSampleInstance());

                    stream.Position = 0;
                    using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        usResult = stremReader.ReadToEnd();
                    }
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
            c.Items.Add(new FirstLevelClass {ID = "3", Second = new SecondLevelClass {SecondID = "3-1"}});

            using (MemoryStream stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(MultilevelClass)));
                    Assert.IsTrue(ser.Divide(typeof(MultilevelClass).Name, MultilevelClass.GetSampleInstance(), c));
                }
                stream.Position = 0;
                XElement ele = XElement.Load(stream);

                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());
                Debug.WriteLine(ele.ToString());

                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        CombineObject dSer = new CombineObject(new TypeExtend(typeof(MultilevelClass)));
                        MultilevelClass newItem =
                            dSer.Combine(reader, MultilevelClass.GetSampleInstance(), typeof(MultilevelClass).Name) as
                                MultilevelClass;

                        Debug.Assert(newItem != null, "newItem != null");
                        Assert.AreEqual(c.Items.Count, newItem.Items.Count);
                        Assert.IsTrue(c.Equals(newItem));
                    }
                }
            }
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
            c.Items[1].Second = new SecondLevelClass {SecondID = "3-1"};

            using (MemoryStream stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(MultilevelClass)));
                    Assert.IsTrue(ser.Divide(typeof(MultilevelClass).Name, MultilevelClass.GetSampleInstance(), c));
                }
                stream.Position = 0;
                XElement ele = XElement.Load(stream);
                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());

                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        CombineObject dSer = new CombineObject(new TypeExtend(typeof(MultilevelClass)));
                        MultilevelClass newItem =
                            dSer.Combine(reader, MultilevelClass.GetSampleInstance(), typeof(MultilevelClass).Name) as
                                MultilevelClass;

                        Debug.Assert(newItem != null, "newItem != null");
                        Assert.AreEqual(c.Items.Count, newItem.Items.Count);
                        Assert.AreEqual(c, newItem);
                    }
                }
            }
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

            using (MemoryStream stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(MultilevelClass)));
                    Assert.IsTrue(ser.Divide(typeof(MultilevelClass).Name, null, MultilevelClass.GetSampleInstance()));
                }
                stream.Position = 0;
                XElement ele = XElement.Load(stream);
                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());

                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        CombineObject dSer = new CombineObject(new TypeExtend(typeof(MultilevelClass)));
                        MultilevelClass newItem =
                            dSer.Combine(reader, null, typeof(MultilevelClass).Name) as MultilevelClass;

                        Debug.Assert(newItem != null, "newItem != null");
                        Assert.AreEqual(MultilevelClass.GetSampleInstance().Items.Count, newItem.Items.Count);
                        Assert.IsTrue(MultilevelClass.GetSampleInstance().Equals(newItem));
                    }
                }
            }
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

            using (MemoryStream stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(MultilevelClass)));
                    Assert.IsTrue(ser.Divide(typeof(MultilevelClass).Name, MultilevelClass.GetSampleInstance(), c));
                }
                stream.Position = 0;
                XElement ele = XElement.Load(stream);
                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());

                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        //Assert.Fail("缺少分序列化部分。");
                        CombineObject com = new CombineObject(new TypeExtend(typeof(MultilevelClass)));
                        MultilevelClass combineObj =
                            com.Combine(reader, MultilevelClass.GetSampleInstance(), typeof(MultilevelClass).Name) as
                                MultilevelClass;

                        Debug.Assert(combineObj != null, "combineObj != null");
                        Assert.AreEqual(c.Items.Count, combineObj.Items.Count);
                        Assert.IsTrue(c.Equals(combineObj));
                    }
                }
            }
        }

        [TestMethod]
        public void SerializeNullable_OriValueIsNotNull()
        {
            NullableClass b1 = NullableClass.GetSampleInstance();

            NullableClass b2 = NullableClass.GetSampleInstance();
            b2.PurchaseYear = 2003;
            string result = @"<NullableClass>
  <PurchaseYear>2003</PurchaseYear>
</NullableClass>";
            using (MemoryStream stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(NullableClass)));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(NullableClass)), b1, b2));
                }
                stream.Position = 0;
                XElement ele = XElement.Load(stream);
                CombineObject dser = new CombineObject(new TypeExtend(typeof(NullableClass)));

                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());

                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        NullableClass b3 =
                            dser.Combine(reader, b1, ReflectionUtils.GetTypeFriendlyName(typeof(NullableClass))) as
                                NullableClass;
                        Debug.Assert(b3 != null, "b3 != null");
                        Assert.AreEqual(b2.Title, b3.Title);
                        Assert.AreEqual(b2.PublishYear, b3.PublishYear);
                        Assert.AreEqual(b2.PurchaseYear, b3.PurchaseYear);
                    }
                }
            }
        }

        [TestMethod]
        public void SerializeNullable_OriValueIsNull()
        {
            NullableClass b1 = NullableClass.GetSampleInstance();
            string result =
                @"<NullableClass>
  <PublishYear>2002</PublishYear>
  <Title>Amazon</Title>
</NullableClass>";
            using (MemoryStream stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(NullableClass)));
                    //原始对象（null）与更新对象产生增量内容
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(NullableClass)), null, b1));
                }
                stream.Position = 0;
                XElement ele = XElement.Load(stream);
                CombineObject dser = new CombineObject(new TypeExtend(typeof(NullableClass)));

                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());

                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        NullableClass b2 =
                            dser.Combine(reader, NullableClass.GetSampleInstance(),
                                ReflectionUtils.GetTypeFriendlyName(typeof(NullableClass))) as NullableClass;
                        Debug.Assert(b2 != null, "b2 != null");
                        Assert.AreEqual(b1.Title, b2.Title);
                        Assert.AreEqual(b1.PublishYear, b2.PublishYear);
                        Assert.AreEqual(b1.PurchaseYear, b2.PurchaseYear);
                    }
                }
            }
        }

        [TestMethod]
        public void SerializeSimpleNestedType()
        {
            BookClass b1 = BookClass.GetSampleInstance();
            string result =
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

            using (MemoryStream stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(BookClass)));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(BookClass)), null, b1));
                }
                stream.Position = 0;
                XElement ele = XElement.Load(stream);

                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());
            }

            BookClass b2 = BookClass.GetSampleInstance();
            b2.Name = "耶路撒冷三千年";
            b2.Comments =
                @"《耶路撒冷三千年》内容简介：美国前总统克林顿2011年年度选书，基辛格视若珍宝的经典巨作。读《耶路撒冷三千年》，了解真实的耶路撒冷，就会明白世界为何演变成今天的模样。耶路撒冷曾被视为世界的中心，是基督教、犹太教和伊斯兰教三大宗教的圣地，是文明冲突的战略要冲，是让世人魂牵梦绕的去处，是惑人的阴谋、虚构的网络传说和二十四小时新闻发生的地方。
西蒙•蒙蒂菲奥里依年代顺序，以三大宗教围绕“圣城”的角逐，以几大家族的兴衰更迭为主线，生动讲述了耶路撒冷的前世今生；作者通过大量的田野调查和文献考据，以客观、中立的角度，透过士兵与先知、诗人与国王、农民与音乐家的生活，以及创造耶路撒冷的家族来呈现这座城市的三千年瑰丽历史，还原真实的耶路撒冷……";
            b2.Author.Name = "西蒙·蒙蒂菲奥里 (Simon Sebag Montefiore)";
            b2.Author.Comments = "";

            result =
                string.Format(@"<BookClass>
  <Author>
    <Comments />
    <Name>{2}</Name>
  </Author>
  <Comments>{1}</Comments>
  <Name>{0}</Name>
</BookClass>", b2.Name, b2.Comments, b2.Author.Name, b2.Author.Comments);

            using (MemoryStream stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(BookClass)));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(BookClass)), b1, b2));
                }
                stream.Position = 0;
                XElement ele = XElement.Load(stream);
                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());
            }
        }

        [TestMethod]
        public void SerializeSimpleNestedType_DecimalProperty()
        {
            BookClassWithDecimalPrice b1 = BookClassWithDecimalPrice.GetSampleInstance();
            string result =
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

            using (MemoryStream stream = new MemoryStream())
            {
                CombineObject dser = new CombineObject(new TypeExtend(typeof(BookClassWithDecimalPrice)));
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(BookClassWithDecimalPrice)));
                    //原始对象（null）与更新对象产生增量内容
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(BookClassWithDecimalPrice)),
                        null, b1));
                }
                stream.Position = 0;
                XElement ele = XElement.Load(stream);

                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        //创建另外一个原始对象
                        BookClassWithDecimalPrice b2 = BookClassWithDecimalPrice.GetSampleInstance();
                        b2 =
                            dser.Combine(reader, b2,
                                    ReflectionUtils.GetTypeFriendlyName(typeof(BookClassWithDecimalPrice)))
                                as BookClassWithDecimalPrice;
                        Debug.Assert(b2 != null, "b2 != null");
                        Assert.AreEqual(b1.Name, b2.Name);
                        Assert.AreEqual(b1.Comments, b2.Comments);
                        Assert.AreEqual(b1.Price, b2.Price);
                        Assert.AreEqual(b1.PublishYear, b2.PublishYear);
                    }
                }
            }
        }

        [TestMethod]
        [Description("测试原始对象为null，直接序列化")]
        public void SerializeSimpleStruct_OriValueIsNull()
        {
            AuthorStruct b1 = AuthorStruct.GetSampleInstance();

            string result =
                string.Format(@"<AuthorStruct>
  <Comments>{1}</Comments>
  <Name>{0}</Name>
</AuthorStruct>", b1.Name, b1.Comments);

            CombineObject dser = new CombineObject(new TypeExtend(typeof(AuthorStruct)));
            using (MemoryStream stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(AuthorStruct)));
                    //原始对象（null）与更新对象产生增量内容
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(AuthorStruct)), null, b1));
                }
                stream.Position = 0;
                XElement ele = XElement.Load(stream);

                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        //创建另外一个原始对象
                        AuthorStruct b2 = AuthorStruct.GetSampleInstance();
                        b2 =
                            dser.Combine(reader, b2, ReflectionUtils.GetTypeFriendlyName(typeof(AuthorStruct))) as
                                AuthorStruct;
                        Debug.Assert(b2 != null, "b2 != null");
                        Assert.AreEqual(b1.Name, b2.Name);
                        Assert.AreEqual(b1.Comments, b2.Comments);
                    }
                }
            }
        }

        [TestMethod]
        [Description("测试原始对象不为null，产生增量结果")]
        public void SerializeSimpleType_OriValueIsNotNull()
        {
            AuthorClass b1 = AuthorClass.GetSampleInstance();
            CombineObject dser = new CombineObject(new TypeExtend(typeof(AuthorClass)));

            AuthorClass b2 = AuthorClass.GetSampleInstance();
            b2.Name = "西蒙·蒙蒂菲奥里 (Simon Sebag Montefiore)";
            b2.Comments = @"";
            string result =
                string.Format(@"<AuthorClass>
  <Comments />
  <Name>{0}</Name>
</AuthorClass>", b2.Name, b2.Comments);

            using (MemoryStream stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(AuthorClass)));
                    //原始对象与更新对象产生增量内容
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(AuthorClass)), b1, b2));
                }
                stream.Position = 0;
                XElement ele = XElement.Load(stream);

                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        //创建原始对象
                        b1 = AuthorClass.GetSampleInstance();
                        //对原始对象进行增量内容数据合并
                        b1 =
                            dser.Combine(reader, b1, ReflectionUtils.GetTypeFriendlyName(typeof(AuthorClass))) as
                                AuthorClass;
                        //合并后的对象与更新对象的数据比较
                        Debug.Assert(b1 != null, "b1 != null");
                        Assert.AreEqual(b1.Name, b2.Name);
                        Assert.AreEqual(b1.Comments, b2.Comments);
                    }
                }
            }
        }

        [TestMethod]
        [Description("测试原始对象为null，直接序列化")]
        public void SerializeSimpleType_OriValueIsNull()
        {
            AuthorClass b1 = AuthorClass.GetSampleInstance();

            string result = string.Format(@"<AuthorClass>
  <Comments>{1}</Comments>
  <Name>{0}</Name>
</AuthorClass>", b1.Name, b1.Comments);

            CombineObject dser = new CombineObject(new TypeExtend(typeof(AuthorClass)));
            using (MemoryStream stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(AuthorClass)));

                    //原始对象（null）与更新对象产生增量内容
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(AuthorClass)), null, b1));
                }
                stream.Position = 0;
                XElement ele = XElement.Load(stream);
                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        //创建另外一个原始对象
                        AuthorClass b2 = AuthorClass.GetSampleInstance();
                        b2 =
                            dser.Combine(reader, b2, ReflectionUtils.GetTypeFriendlyName(typeof(AuthorClass))) as
                                AuthorClass;
                        Debug.Assert(b2 != null, "b2 != null");
                        Assert.AreEqual(b1.Name, b2.Name);
                        Assert.AreEqual(b1.Comments, b2.Comments);
                    }
                }
            }
        }

        [TestMethod]
        public void SerializeSimpleType_RevValueIsNull()
        {
            AuthorClass b1 = AuthorClass.GetSampleInstance();

            CombineObject dser = new CombineObject(new TypeExtend(typeof(AuthorClass)));
            string result = @"<AuthorClass Action=""SetNull"" />";

            using (MemoryStream stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    DivideObject ser = new DivideObject(writer, new TypeExtend(typeof(AuthorClass)));

                    //原始对象与更新对象产生增量内容
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(AuthorClass)), b1, null));
                }
                stream.Position = 0;

                XElement ele = XElement.Load(stream);
                Assert.IsNotNull(ele);
                Assert.AreEqual(result, ele.ToString());
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        //创建原始对象
                        b1 = AuthorClass.GetSampleInstance();
                        //对原始对象进行增量内容数据合并
                        b1 =
                            dser.Combine(reader, b1, ReflectionUtils.GetTypeFriendlyName(typeof(AuthorClass))) as
                                AuthorClass;
                        //合并后的对象与更新对象的数据比较
                        Assert.IsNull(b1);
                    }
                }
            }
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
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture, "'{0}' 的 主键 {1} 设置异常。", ex.SourceType.FullName,
                        ex.PrimaryKeyName), ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
        }

        [TestMethod]
        public void TestDivideAndSerializeSimpleType()
        {
            Serializer serializer = new Serializer(typeof(AuthorClass));
            AuthorClass b1 = AuthorClass.GetSampleInstance();
            AuthorClass b2 = AuthorClass.GetSampleInstance();

            using (MemoryStream stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream))
                {
                    serializer.Divide(writer, b1, b2);
                    string context = UnitTest.TestHelper.StreamToString(stream);
                    Assert.AreEqual(TestHelper.XmlHeaderContext, context);
                }
            }
        }

        [TestMethod]
        [Description("测试增量内容类型定义了错误的主键的对象是否会抛出异常")]
        public void TestDivideErrorPrimaryKeyDefineClass()
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                    {
                        new DivideObject(writer, new TypeExtend(typeof(ErrorPrimaryKeyDefineClass)));
                    }
                }
            }
            catch (PrimaryKeyException ex)
            {
                Assert.AreEqual(typeof(ErrorPrimaryKeyDefineClass), ex.SourceType);
                Assert.AreEqual("Author", ex.PrimaryKeyName);
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture, "'{0}' 的 主键 {1} 设置异常。", ex.SourceType.FullName,
                        ex.PrimaryKeyName), ex.Message);
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
                            Serializer serializer = new Serializer(typeof(BookClass));
                            string changedContext = string.Empty;
                            using (MemoryStream stream = new MemoryStream())
                            {
                                using (var writer = TestHelper.CreateWriter(stream))
                                {
                                    serializer.Divide(writer, null, BookClass.GetSampleInstance());

                                    stream.Position = 0;
                                    using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                                    {
                                        changedContext = stremReader.ReadToEnd();
                                    }
                                }
                            }
                            Trace.Write(changedContext);
                            Serializer deserializer = new Serializer(typeof(BookClass));
                            BookClass book = null;
                            using (XmlTextReader reader = new XmlTextReader(XmlReader.Create(new StringReader(changedContext))))
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