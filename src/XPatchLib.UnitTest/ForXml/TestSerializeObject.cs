// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if (NET_35_UP || NETSTANDARD)
using System.Xml.Linq;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
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
    public class TestSerializeObject:TestBase
    {
        [Test]
        public void CultureChangeTest()
        {
            CultureInfo[] cultureInfos = new CultureInfo[]
            {
                new CultureInfo("fr-FR"), new CultureInfo("fa-IR"), new CultureInfo("de-DE"), new CultureInfo("en-US")
            };
            string[] results = new string[cultureInfos.Length];

            ISerializeSetting setting = new XmlSerializeSetting() {Mode = DateTimeSerializationMode.Unspecified};
            for (int i = 0; i < cultureInfos.Length; i++)
            {
#if NET
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = cultureInfos[i];
#else
                CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = cultureInfos[i];
#endif

                results[i] = DoSerializer_Divide(null, CultureClass.GetSampleInstance(), setting);
            }

            Assert.AreEqual(results[0], results[1], "Comparing FR and FA");
            Assert.AreEqual(results[1], results[2], "Comparing FA and DE");
            Assert.AreEqual(results[2], results[3], "Comparing DE and US");
        }

        [Test]
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

            string context=DoDivideObject_Divide(MultilevelClass.GetSampleInstance(), c);
            Assert.AreEqual(result, context);
            var newItem = DoCombineObject_Combie(context, MultilevelClass.GetSampleInstance());
            Assert.IsNotNull(newItem);
            Assert.AreEqual(c.Items.Count, newItem.Items.Count);
            Assert.IsTrue(c.Equals(newItem));

            result = string.Format("{0}{1}{2}", XmlHeaderContext, System.Environment.NewLine, result);
            DoAssert(typeof(MultilevelClass), result, MultilevelClass.GetSampleInstance(), c, true);
            DoAssert(typeof(MultilevelClass), result, MultilevelClass.GetSampleInstance(), c, false);
        }

        [Test]
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

            string context = DoDivideObject_Divide(MultilevelClass.GetSampleInstance(), c);
            Assert.AreEqual(result, context);
            var newItem = DoCombineObject_Combie(context, MultilevelClass.GetSampleInstance());
            Assert.IsNotNull(newItem);
            Assert.AreEqual(c.Items.Count, newItem.Items.Count);
            Assert.IsTrue(c.Equals(newItem));

            result = string.Format("{0}{1}{2}", XmlHeaderContext, System.Environment.NewLine, result);
            DoAssert(typeof(MultilevelClass), result, MultilevelClass.GetSampleInstance(), c, true);
            DoAssert(typeof(MultilevelClass), result, MultilevelClass.GetSampleInstance(), c, false);
        }

        [Test]
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

            string context = DoDivideObject_Divide(null, MultilevelClass.GetSampleInstance());
            Assert.AreEqual(result, context);
            MultilevelClass newItem = DoCombineObject_Combie(typeof(MultilevelClass), context, null) as MultilevelClass;
            Assert.IsNotNull(newItem);
            Assert.AreEqual(MultilevelClass.GetSampleInstance().Items.Count, newItem.Items.Count);
            Assert.IsTrue(MultilevelClass.GetSampleInstance().Equals(newItem));

            result = string.Format("{0}{1}{2}", XmlHeaderContext, System.Environment.NewLine, result);
            DoAssert(typeof(MultilevelClass), result, null,MultilevelClass.GetSampleInstance(), true);
            DoAssert(typeof(MultilevelClass), result, null,MultilevelClass.GetSampleInstance(), false);
        }

        [Test]
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
            
            string context = DoDivideObject_Divide(MultilevelClass.GetSampleInstance(), c);
            Assert.AreEqual(result, context);
            MultilevelClass newItem = DoCombineObject_Combie(typeof(MultilevelClass), context, MultilevelClass.GetSampleInstance()) as MultilevelClass;
            Assert.IsNotNull(newItem);
            Assert.AreEqual(c.Items.Count, newItem.Items.Count);
            Assert.IsTrue(c.Equals(newItem));

            result = string.Format("{0}{1}{2}", XmlHeaderContext, System.Environment.NewLine, result);
            DoAssert(typeof(MultilevelClass), result, MultilevelClass.GetSampleInstance(), c, true);
            DoAssert(typeof(MultilevelClass), result, MultilevelClass.GetSampleInstance(), c, false);
        }

        [Test]
        public void SerializeNullable_OriValueIsNotNull()
        {
            NullableClass b1 = NullableClass.GetSampleInstance();

            NullableClass b2 = NullableClass.GetSampleInstance();
            b2.PurchaseYear = 2003;
            string result = @"<NullableClass>
  <PurchaseYear>2003</PurchaseYear>
</NullableClass>";

            string context = DoDivideObject_Divide(b1, b2);
            Assert.AreEqual(result, context);
            var b3 = DoCombineObject_Combie(context, b1);
            Assert.IsNotNull(b3);
            Assert.AreEqual(b2.Title, b3.Title);
            Assert.AreEqual(b2.PublishYear, b3.PublishYear);
            Assert.AreEqual(b2.PurchaseYear, b3.PurchaseYear);
        }

        [Test]
        public void SerializeNullable_RevValueIsNotNull()
        {
            NullableClass b1 = NullableClass.GetSampleInstance();

            NullableClass b2 = NullableClass.GetSampleInstance();
            b2.PurchaseYear = 2003;
            string result = @"<NullableClass>
  <PurchaseYear Action=""SetNull"" />
</NullableClass>";

            string context = DoDivideObject_Divide(b2, b1);
            Assert.AreEqual(result, context);
            var b3 = DoCombineObject_Combie(context, b1);
            Assert.IsNotNull(b3);
            Assert.AreEqual(b1.Title, b3.Title);
            Assert.AreEqual(b1.PublishYear, b3.PublishYear);
            Assert.AreEqual(b1.PurchaseYear, b3.PurchaseYear);
        }

        [Test]
        public void SerializeNullable_OriValueIsNull()
        {
            NullableClass b1 = NullableClass.GetSampleInstance();
            string result =
                @"<NullableClass>
  <PublishYear>2002</PublishYear>
  <Title>Amazon</Title>
</NullableClass>";

            string context = DoDivideObject_Divide(null, b1);
            Assert.AreEqual(result, context);
            var b3 = DoCombineObject_Combie(context, NullableClass.GetSampleInstance());
            Assert.IsNotNull(b3);
            Assert.AreEqual(b1.Title, b3.Title);
            Assert.AreEqual(b1.PublishYear, b3.PublishYear);
            Assert.AreEqual(b1.PurchaseYear, b3.PurchaseYear);
        }

        [Test]
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

            string context = DoDivideObject_Divide(null, b1);
            Assert.AreEqual(result, context);
            
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
</BookClass>", b2.Name, b2.Comments, b2.Author.Name);

            context = DoDivideObject_Divide(b1, b2);
            Assert.AreEqual(result, context);
        }

        [Test]
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

            //原始对象（null）与更新对象产生增量内容
            string context = DoDivideObject_Divide(null, b1);
            Assert.AreEqual(result, context);
            //创建另外一个原始对象
            BookClassWithDecimalPrice b2 = BookClassWithDecimalPrice.GetSampleInstance();
            b2 = DoCombineObject_Combie(context, b2);
            Assert.IsNotNull(b2);
            Assert.AreEqual(b1.Name, b2.Name);
            Assert.AreEqual(b1.Comments, b2.Comments);
            Assert.AreEqual(b1.Price, b2.Price);
            Assert.AreEqual(b1.PublishYear, b2.PublishYear);
        }

        [Test]
        [Description("测试原始对象为null，直接序列化")]
        public void SerializeSimpleStruct_OriValueIsNull()
        {
            AuthorStruct b1 = AuthorStruct.GetSampleInstance();

            string result =
                string.Format(@"<AuthorStruct>
  <Comments>{1}</Comments>
  <Name>{0}</Name>
</AuthorStruct>", b1.Name, b1.Comments);

            //原始对象（null）与更新对象产生增量内容
            string context = DoDivideObject_Divide(null, b1);
            Assert.AreEqual(result, context);
            //创建另外一个原始对象
            AuthorStruct b2 = AuthorStruct.GetSampleInstance();
            b2 = DoCombineObject_Combie(context, b2);
            Assert.IsNotNull(b2);
            Assert.AreEqual(b1.Name, b2.Name);
            Assert.AreEqual(b1.Comments, b2.Comments);
            
        }

        [Test]
        [Description("测试原始对象不为null，产生增量结果")]
        public void SerializeSimpleType_OriValueIsNotNull()
        {
            AuthorClass b1 = AuthorClass.GetSampleInstance();

            AuthorClass b2 = AuthorClass.GetSampleInstance();
            b2.Name = "西蒙·蒙蒂菲奥里 (Simon Sebag Montefiore)";
            b2.Comments = @"";
            string result =
                string.Format(@"<AuthorClass>
  <Comments />
  <Name>{0}</Name>
</AuthorClass>", b2.Name, b2.Comments);

            //原始对象（null）与更新对象产生增量内容
            string context = DoDivideObject_Divide(b1, b2);
            Assert.AreEqual(result, context);
            //创建原始对象
            b1 = AuthorClass.GetSampleInstance();
            b1 = DoCombineObject_Combie(context, b1);
            Assert.IsNotNull(b1);
            Assert.AreEqual(b1.Name, b2.Name);
            Assert.AreEqual(b1.Comments, b2.Comments);
        }

        [Test]
        [Description("测试原始对象为null，直接序列化")]
        public void SerializeSimpleType_OriValueIsNull()
        {
            AuthorClass b1 = AuthorClass.GetSampleInstance();

            string result = string.Format(@"<AuthorClass>
  <Comments>{1}</Comments>
  <Name>{0}</Name>
</AuthorClass>", b1.Name, b1.Comments);

            //原始对象（null）与更新对象产生增量内容
            string context = DoDivideObject_Divide(null, b1);
            Assert.AreEqual(result, context);
            //创建原始对象
            AuthorClass b2 = AuthorClass.GetSampleInstance();
            b2 = DoCombineObject_Combie(context, b2);
            Assert.IsNotNull(b2);
            Assert.AreEqual(b1.Name, b2.Name);
            Assert.AreEqual(b1.Comments, b2.Comments);
        }

        [Test]
        public void SerializeSimpleType_RevValueIsNull()
        {
            AuthorClass b1 = AuthorClass.GetSampleInstance();

            CombineObject dser = new CombineObject(new TypeExtend(typeof(AuthorClass), null));
            string result = @"<AuthorClass Action=""SetNull"" />";
            
            string context = DoDivideObject_Divide(b1, null);
            Assert.AreEqual(result, context);
            //创建原始对象
            b1 = AuthorClass.GetSampleInstance();
            b1 = DoCombineObject_Combie(context, b1);
            //合并后的对象与更新对象的数据比较
            Assert.IsNull(b1);
        }

        [Test]
        [Description("测试合并类型定义了错误的主键的对象是否会抛出异常")]
        public void TestCombineErrorPrimaryKeyDefineClass()
        {
            try
            {
                new CombineObject(new TypeExtend(typeof(ErrorPrimaryKeyDefineClass), null));
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
            catch (PrimaryKeyException ex)
            {
                Assert.AreEqual(typeof(ErrorPrimaryKeyDefineClass), ex.ErrorType);
                Assert.AreEqual("Author", ex.PrimaryKeyName);
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture,
                        ResourceHelper.GetResourceString(LocalizationRes.Exp_String_PrimaryKey), ex.ErrorType.FullName,
                        ex.PrimaryKeyName), ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
        }

        [Test]
        public void TestDivideAndSerializeSimpleType()
        {
            AuthorClass b1 = AuthorClass.GetSampleInstance();
            AuthorClass b2 = AuthorClass.GetSampleInstance();

            DoAssert(typeof(AuthorClass), XmlHeaderContext, b1, b2, true);
            DoAssert(typeof(AuthorClass), XmlHeaderContext, b1, b2, false);
        }

        [Test]
        [Description("测试增量内容类型定义了错误的主键的对象是否会抛出异常")]
        public void TestDivideErrorPrimaryKeyDefineClass()
        {
            try
            {
                using (ITextWriter writer = CreateWriter(new StringBuilder()))
                {
                    new DivideObject(writer,
                        new TypeExtend(typeof(ErrorPrimaryKeyDefineClass), writer.IgnoreAttributeType));
                }
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
            catch (PrimaryKeyException ex)
            {
                Assert.AreEqual(typeof(ErrorPrimaryKeyDefineClass), ex.ErrorType);
                Assert.AreEqual("Author", ex.PrimaryKeyName);
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture,
                        ResourceHelper.GetResourceString(LocalizationRes.Exp_String_PrimaryKey), ex.ErrorType.FullName,
                        ex.PrimaryKeyName), ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出PrimaryKeyException异常");
            }
        }

        [Test]
        public void ThreadingTest()
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    var th = new Thread(() =>
                        {
                            DoAssert(typeof(BookClass), string.Empty, null, BookClass.GetSampleInstance(), true);
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
    }
}