// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XPatchLib.UnitTest.ForXml
{
    [TestClass]
    public class TestDictionary
    {
        private const string ComplexOperatorChangedContext = @"<Dictionary_String_String>
  <KeyValuePair_String_String Action=""Remove"">
    <Key>Ckey</Key>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String>
    <Key>Bkey</Key>
    <Value>newBkey</Value>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String Action=""SetNull"">
    <Key>Dkey</Key>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String Action=""Add"">
    <Key>Ekey</Key>
    <Value>EValue</Value>
  </KeyValuePair_String_String>
</Dictionary_String_String>";

        private const string CreateChangedContext = @"<Dictionary_String_String>
  <KeyValuePair_String_String Action=""Add"">
    <Key>Akey</Key>
    <Value>Avalue</Value>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String Action=""Add"">
    <Key>Bkey</Key>
    <Value>Bvalue</Value>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String Action=""Add"">
    <Key>Ckey</Key>
    <Value>Cvalue</Value>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String Action=""Add"">
    <Key>Dkey</Key>
    <Value>Dvalue</Value>
  </KeyValuePair_String_String>
</Dictionary_String_String>";

        private const string EditChangedContext = @"<Dictionary_String_String>
  <KeyValuePair_String_String>
    <Key>Akey</Key>
    <Value>newAvalue</Value>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String>
    <Key>Bkey</Key>
    <Value>newBvalue</Value>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String>
    <Key>Ckey</Key>
    <Value>newCvalue</Value>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String>
    <Key>Dkey</Key>
    <Value>newDvalue</Value>
  </KeyValuePair_String_String>
</Dictionary_String_String>";

        private const string RemoveChangedContext = @"<Dictionary_String_String>
  <KeyValuePair_String_String Action=""Remove"">
    <Key>Ckey</Key>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String Action=""Remove"">
    <Key>Dkey</Key>
  </KeyValuePair_String_String>
</Dictionary_String_String>";

        private const string SetNullChangedContext = @"<Dictionary_String_String>
  <KeyValuePair_String_String Action=""SetNull"">
    <Key>Akey</Key>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String Action=""SetNull"">
    <Key>Bkey</Key>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String Action=""SetNull"">
    <Key>Ckey</Key>
  </KeyValuePair_String_String>
  <KeyValuePair_String_String Action=""SetNull"">
    <Key>Dkey</Key>
  </KeyValuePair_String_String>
</Dictionary_String_String>";

        [TestMethod]
        public void TestCombineString_StringDicComplexOperator_CombineCore()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "newBkey");
            dic2.Add("Dkey", null);
            dic2.Add("Ekey", "EValue");

            var changedEle = XElement.Load(new StringReader(ComplexOperatorChangedContext));

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(ComplexOperatorChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    var newDic =
                        new CombineCore(new TypeExtend(dic1.GetType(), null)).Combine(reader, dic1,
                                ReflectionUtils.GetTypeFriendlyName(dic1.GetType())) as
                            Dictionary<string, string>;

                    Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
                    Assert.IsNotNull(newDic);
                    Assert.AreEqual(4, newDic.Count);
                    Assert.AreEqual(dic2.Count, newDic.Count);
                    foreach (var key in dic2.Keys)
                    {
                        Assert.AreEqual(dic1[key], newDic[key]);
                        Assert.AreEqual(dic2[key], newDic[key]);
                    }

                    //在使用非Serializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
                    Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
                    Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicComplexOperator_CombineIDictionary()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "newBkey");
            dic2.Add("Dkey", null);
            dic2.Add("Ekey", "EValue");

            var changedEle = XElement.Load(new StringReader(ComplexOperatorChangedContext));

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(ComplexOperatorChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    var newDic =
                        new CombineIDictionary(new TypeExtend(dic1.GetType(), null)).Combine(reader, dic1,
                                ReflectionUtils.GetTypeFriendlyName(dic1.GetType())) as
                            Dictionary<string, string>;

                    Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
                    Assert.IsNotNull(newDic);
                    Assert.AreEqual(4, newDic.Count);
                    Assert.AreEqual(dic2.Count, newDic.Count);
                    foreach (var key in dic2.Keys)
                    {
                        Assert.AreEqual(dic1[key], newDic[key]);
                        Assert.AreEqual(dic2[key], newDic[key]);
                    }

                    //在使用非Serializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
                    Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
                    Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicComplexOperator_CombineXmlSerializer()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "newBkey");
            dic2.Add("Dkey", null);
            dic2.Add("Ekey", "EValue");

            Dictionary<string, string> newDic = null;
            using (XmlReader reader = XmlReader.Create(new StringReader(ComplexOperatorChangedContext)))
            {
                using (var xmlTextReader = new XmlTextReader(reader))
                {
                    newDic = new Serializer(dic1.GetType()).Combine(xmlTextReader, dic1) as Dictionary<string, string>;
                }
            }
            //newDic的长度应该为2，dic1的长度为4（因为dic1并未被改变）
            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, dic1.Count);
            Assert.AreEqual(4, newDic.Count);
            foreach (var key in newDic.Keys)
            {
                if (!key.Equals("Akey") && key.Equals("Ckey"))
                    Assert.AreNotEqual(dic1[key], newDic[key]);
                Assert.AreEqual(dic2[key], newDic[key]);
            }

            //在使用Serializer入口做增量内容合并时，会首先对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreNotEqual(dic1.GetHashCode(), newDic.GetHashCode());
            Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicComplexOperator_DivideXmlSerializer()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "newBkey");
            dic2.Add("Dkey", null);
            dic2.Add("Ekey", "EValue");

            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    var serializer = new Serializer(dic1.GetType());
                    serializer.Divide(writer, dic1, dic2);
                    var context = UnitTest.TestHelper.StreamToString(stream);
                    Assert.AreEqual(TestHelper.XmlHeaderContext + Environment.NewLine + ComplexOperatorChangedContext,
                        context);
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicCreate_CombineCore()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var changedEle = XElement.Load(new StringReader(CreateChangedContext));

            Dictionary<string, string> oldDic = null;

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(CreateChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    var newDic =
                        new CombineCore(new TypeExtend(dic1.GetType(), null)).Combine(reader, oldDic,
                                ReflectionUtils.GetTypeFriendlyName(dic1.GetType())) as
                            Dictionary<string, string>;

                    Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
                    Assert.IsNotNull(newDic);
                    Assert.AreEqual(dic1.Count, newDic.Count);
                    foreach (var key in dic1.Keys)
                        Assert.AreEqual(dic1[key], newDic[key]);

                    //原始值为Null时，内部重新创建对象实例，所以原始值依然为Null。
                    Assert.IsNull(oldDic);
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicCreate_CombineIDictionary()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var changedEle = XElement.Load(new StringReader(CreateChangedContext));

            Dictionary<string, string> oldDic = null;

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(CreateChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    var newDic =
                        new CombineIDictionary(new TypeExtend(dic1.GetType(), null)).Combine(reader, oldDic,
                                ReflectionUtils.GetTypeFriendlyName(dic1.GetType())) as
                            Dictionary<string, string>;

                    Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
                    Assert.IsNotNull(newDic);
                    Assert.AreEqual(dic1.Count, newDic.Count);
                    foreach (var key in dic1.Keys)
                        Assert.AreEqual(dic1[key], newDic[key]);

                    //原始值为Null时，内部重新创建对象实例，所以原始值依然为Null。
                    Assert.IsNull(oldDic);
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicCreate_CombineXmlSerializer()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> oldDic = null;
            Dictionary<string, string> newDic = null;
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(CreateChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    newDic = new Serializer(dic1.GetType()).Combine(reader, oldDic) as Dictionary<string, string>;
                }
            }
            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(dic1.Count, newDic.Count);
            foreach (var key in dic1.Keys)
                Assert.AreEqual(dic1[key], newDic[key]);

            //原始值为Null时，内部重新创建对象实例，所以原始值依然为Null。
            Assert.IsNull(oldDic);
        }

        [TestMethod]
        public void TestCombineString_StringDicCreate_DivideXmlSerializer()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    var serializer = new Serializer(dic1.GetType());
                    serializer.Divide(writer, null, dic1);
                    var context = UnitTest.TestHelper.StreamToString(stream);
                    Assert.AreEqual(TestHelper.XmlHeaderContext + Environment.NewLine + CreateChangedContext, context);
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicEdit_CombineCore()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "newAvalue");
            dic2.Add("Bkey", "newBvalue");
            dic2.Add("Ckey", "newCvalue");
            dic2.Add("Dkey", "newDvalue");

            var changedEle = XElement.Load(new StringReader(EditChangedContext));

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(EditChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    var newDic =
                        new CombineCore(new TypeExtend(dic1.GetType(), null)).Combine(reader, dic1,
                                ReflectionUtils.GetTypeFriendlyName(dic1.GetType())) as
                            Dictionary<string, string>;

                    Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
                    Assert.IsNotNull(newDic);
                    Assert.AreEqual(4, newDic.Count);
                    Assert.AreEqual(dic2.Count, newDic.Count);
                    foreach (var key in dic2.Keys)
                    {
                        Assert.AreEqual(dic1[key], newDic[key]);
                        Assert.AreEqual(dic2[key], newDic[key]);
                    }

                    //在使用非Serializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
                    Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
                    Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicEdit_CombineIDictionary()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "newAvalue");
            dic2.Add("Bkey", "newBvalue");
            dic2.Add("Ckey", "newCvalue");
            dic2.Add("Dkey", "newDvalue");

            var changedEle = XElement.Load(new StringReader(EditChangedContext));

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(EditChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    var newDic =
                        new CombineIDictionary(new TypeExtend(dic1.GetType(), null)).Combine(reader, dic1,
                                ReflectionUtils.GetTypeFriendlyName(dic1.GetType())) as
                            Dictionary<string, string>;

                    Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
                    Assert.IsNotNull(newDic);
                    Assert.AreEqual(4, newDic.Count);
                    Assert.AreEqual(dic2.Count, newDic.Count);
                    foreach (var key in dic2.Keys)
                    {
                        Assert.AreEqual(dic1[key], newDic[key]);
                        Assert.AreEqual(dic2[key], newDic[key]);
                    }

                    //在使用非Serializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
                    Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
                    Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicEdit_CombineXmlSerializer()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "newAvalue");
            dic2.Add("Bkey", "newBvalue");
            dic2.Add("Ckey", "newCvalue");
            dic2.Add("Dkey", "newDvalue");

            Dictionary<string, string> newDic = null;
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(EditChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    newDic = new Serializer(dic1.GetType()).Combine(reader, dic1) as Dictionary<string, string>;
                }
            }
            //newDic的长度应该为2，dic1的长度为4（因为dic1并未被改变）
            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, dic1.Count);
            Assert.AreEqual(4, newDic.Count);
            foreach (var key in newDic.Keys)
            {
                Assert.AreNotEqual(dic1[key], newDic[key]);
                Assert.AreEqual(dic2[key], newDic[key]);
            }

            //在使用Serializer入口做增量内容合并时，会首先对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreNotEqual(dic1.GetHashCode(), newDic.GetHashCode());
            Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicEdit_DivideXmlSerializer()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "newAvalue");
            dic2.Add("Bkey", "newBvalue");
            dic2.Add("Ckey", "newCvalue");
            dic2.Add("Dkey", "newDvalue");

            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    var serializer = new Serializer(dic1.GetType());
                    serializer.Divide(writer, dic1, dic2);
                    var context = UnitTest.TestHelper.StreamToString(stream);
                    Assert.AreEqual(TestHelper.XmlHeaderContext + Environment.NewLine + EditChangedContext, context);
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicRemove_CombineCore()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var changedEle = XElement.Load(new StringReader(RemoveChangedContext));

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(RemoveChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    var newDic =
                        new CombineCore(new TypeExtend(dic1.GetType(), null)).Combine(reader, dic1,
                                ReflectionUtils.GetTypeFriendlyName(dic1.GetType())) as
                            Dictionary<string, string>;

                    Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
                    Assert.IsNotNull(newDic);
                    Assert.AreEqual(2, newDic.Count);
                    Assert.AreEqual(dic1.Count, newDic.Count);
                    foreach (var key in dic1.Keys)
                        Assert.AreEqual(dic1[key], newDic[key]);

                    //在使用非Serializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
                    Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicRemove_CombineIDictionary()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var changedEle = XElement.Load(new StringReader(RemoveChangedContext));

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(RemoveChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    var newDic =
                        new CombineIDictionary(new TypeExtend(dic1.GetType(), null)).Combine(reader, dic1,
                                ReflectionUtils.GetTypeFriendlyName(dic1.GetType())) as
                            Dictionary<string, string>;

                    Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
                    Assert.IsNotNull(newDic);
                    Assert.AreEqual(2, newDic.Count);
                    Assert.AreEqual(dic1.Count, newDic.Count);
                    foreach (var key in dic1.Keys)
                        Assert.AreEqual(dic1[key], newDic[key]);

                    //在使用非Serializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
                    Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicRemove_CombineXmlSerializer()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> newDic = null;
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(RemoveChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    newDic = new Serializer(dic1.GetType()).Combine(reader, dic1) as Dictionary<string, string>;
                }
            }
            //newDic的长度应该为2，dic1的长度为4（因为dic1并未被改变）
            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, dic1.Count);
            Assert.AreEqual(2, newDic.Count);
            foreach (var key in newDic.Keys)
                Assert.AreEqual(dic1[key], newDic[key]);

            //在使用Serializer入口做增量内容合并时，会首先对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreNotEqual(dic1.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicRemove_DivideXmlSerializer()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "Bvalue");

            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    var serializer = new Serializer(dic1.GetType());
                    serializer.Divide(writer, dic1, dic2);
                    var context = UnitTest.TestHelper.StreamToString(stream);
                    Assert.AreEqual(TestHelper.XmlHeaderContext + Environment.NewLine + RemoveChangedContext, context);
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicSetNull_CombineCore()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", null);
            dic2.Add("Bkey", null);
            dic2.Add("Ckey", null);
            dic2.Add("Dkey", null);

            var changedEle = XElement.Load(new StringReader(SetNullChangedContext));

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(SetNullChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    var newDic =
                        new CombineCore(new TypeExtend(dic1.GetType(), null)).Combine(reader, dic1,
                                ReflectionUtils.GetTypeFriendlyName(dic1.GetType())) as
                            Dictionary<string, string>;

                    Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
                    Assert.IsNotNull(newDic);
                    Assert.AreEqual(4, newDic.Count);
                    Assert.AreEqual(dic2.Count, newDic.Count);
                    foreach (var key in dic2.Keys)
                    {
                        Assert.AreEqual(dic1[key], newDic[key]);
                        Assert.AreEqual(dic2[key], newDic[key]);
                    }

                    //在使用非Serializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
                    Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
                    Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicSetNull_CombineIDictionary()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", null);
            dic2.Add("Bkey", null);
            dic2.Add("Ckey", null);
            dic2.Add("Dkey", null);

            var changedEle = XElement.Load(new StringReader(SetNullChangedContext));

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(SetNullChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    var newDic =
                        new CombineIDictionary(new TypeExtend(dic1.GetType(), null)).Combine(reader, dic1,
                                ReflectionUtils.GetTypeFriendlyName(dic1.GetType())) as
                            Dictionary<string, string>;

                    Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
                    Assert.IsNotNull(newDic);
                    Assert.AreEqual(4, newDic.Count);
                    Assert.AreEqual(dic2.Count, newDic.Count);
                    foreach (var key in dic2.Keys)
                    {
                        Assert.AreEqual(dic1[key], newDic[key]);
                        Assert.AreEqual(dic2[key], newDic[key]);
                    }

                    //在使用非Serializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
                    Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
                    Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
                }
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicSetNull_CombineXmlSerializer()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", null);
            dic2.Add("Bkey", null);
            dic2.Add("Ckey", null);
            dic2.Add("Dkey", null);

            Dictionary<string, string> newDic = null;
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(SetNullChangedContext)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    newDic = new Serializer(dic1.GetType()).Combine(reader, dic1) as Dictionary<string, string>;
                }
            }
            //newDic的长度应该为2，dic1的长度为4（因为dic1并未被改变）
            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, dic1.Count);
            Assert.AreEqual(4, newDic.Count);
            foreach (var key in newDic.Keys)
            {
                Assert.AreNotEqual(dic1[key], newDic[key]);
                Assert.AreEqual(dic2[key], newDic[key]);
            }

            //在使用Serializer入口做增量内容合并时，会首先对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreNotEqual(dic1.GetHashCode(), newDic.GetHashCode());
            Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicSetNull_DivideXmlSerializer()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", null);
            dic2.Add("Bkey", null);
            dic2.Add("Ckey", null);
            dic2.Add("Dkey", null);

            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    var serializer = new Serializer(dic1.GetType());
                    serializer.Divide(writer, dic1, dic2);
                    var context = UnitTest.TestHelper.StreamToString(stream);
                    Assert.AreEqual(TestHelper.XmlHeaderContext + Environment.NewLine + SetNullChangedContext, context);
                }
            }
        }

        [TestMethod]
        public void TestDivideString_StringDicComplexOperator_DivideCore()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "newBkey");
            dic2.Add("Dkey", null);
            dic2.Add("Ekey", "EValue");

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideCore(writer, new TypeExtend(dic1.GetType(), writer.IgnoreAttributeType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(ComplexOperatorChangedContext, changedEle.ToString());
            }
        }

        [TestMethod]
        public void TestDivideString_StringDicComplexOperator_DivideIDictionary()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "newBkey");
            dic2.Add("Dkey", null);
            dic2.Add("Ekey", "EValue");

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideIDictionary(writer, new TypeExtend(dic1.GetType(), writer.IgnoreAttributeType)).Divide
                        (
                            ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(ComplexOperatorChangedContext, changedEle.ToString());
            }
        }

        [TestMethod]
        public void TestDivideString_StringDicCreate_DivideCore()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");


            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideCore(writer, new TypeExtend(dic1.GetType(), writer.IgnoreAttributeType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), null, dic1));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(CreateChangedContext, changedEle.ToString());
            }
        }

        [TestMethod]
        public void TestDivideString_StringDicCreate_DivideIDictionary()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideIDictionary(writer, new TypeExtend(dic1.GetType(), writer.IgnoreAttributeType)).Divide
                        (
                            ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), null, dic1));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(CreateChangedContext, changedEle.ToString());
            }
        }

        [TestMethod]
        public void TestDivideString_StringDicEdit_DivideCore()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "newAvalue");
            dic2.Add("Bkey", "newBvalue");
            dic2.Add("Ckey", "newCvalue");
            dic2.Add("Dkey", "newDvalue");

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideCore(writer, new TypeExtend(dic1.GetType(), writer.IgnoreAttributeType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(EditChangedContext, changedEle.ToString());
            }
        }

        [TestMethod]
        public void TestDivideString_StringDicEdit_DivideIDictionary()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "newAvalue");
            dic2.Add("Bkey", "newBvalue");
            dic2.Add("Ckey", "newCvalue");
            dic2.Add("Dkey", "newDvalue");


            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideIDictionary(writer, new TypeExtend(dic1.GetType(), writer.IgnoreAttributeType)).Divide
                        (
                            ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(EditChangedContext, changedEle.ToString());
            }
        }

        [TestMethod]
        public void TestDivideString_StringDicRemove_DivideCore()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "Bvalue");

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideCore(writer, new TypeExtend(dic1.GetType(), writer.IgnoreAttributeType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(RemoveChangedContext, changedEle.ToString());
            }
        }

        [TestMethod]
        public void TestDivideString_StringDicRemove_DivideIDictionary()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "Bvalue");


            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideIDictionary(writer, new TypeExtend(dic1.GetType(), writer.IgnoreAttributeType)).Divide
                        (
                            ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(RemoveChangedContext, changedEle.ToString());
            }
        }

        [TestMethod]
        public void TestDivideString_StringDicSetNull_DivideCore()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", null);
            dic2.Add("Bkey", null);
            dic2.Add("Ckey", null);
            dic2.Add("Dkey", null);

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideCore(writer, new TypeExtend(dic1.GetType(), writer.IgnoreAttributeType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(SetNullChangedContext, changedEle.ToString());
            }
        }

        [TestMethod]
        public void TestDivideString_StringDicSetNull_DivideIDictionary()
        {
            var dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            var dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", null);
            dic2.Add("Bkey", null);
            dic2.Add("Ckey", null);
            dic2.Add("Dkey", null);


            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideIDictionary(writer, new TypeExtend(dic1.GetType(), writer.IgnoreAttributeType)).Divide
                        (
                            ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(SetNullChangedContext, changedEle.ToString());
            }
        }
    }
}