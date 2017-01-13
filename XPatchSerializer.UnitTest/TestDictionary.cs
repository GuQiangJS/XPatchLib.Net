using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XPatchLib.UnitTest
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

        private const string XMLHeaderContext = @"<?xml version=""1.0"" encoding=""utf-8""?>";

        [TestMethod]
        public void TestCombineString_StringDicComplexOperator_CombineCore()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "newBkey");
            dic2.Add("Dkey", null);
            dic2.Add("Ekey", "EValue");

            XElement changedEle = XElement.Load(new StringReader(ComplexOperatorChangedContext));

            Dictionary<string, string> newDic = new CombineCore(new TypeExtend(dic1.GetType())).Combine(dic1, changedEle) as Dictionary<string, string>;

            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, newDic.Count);
            Assert.AreEqual(dic2.Count, newDic.Count);
            foreach (string key in dic2.Keys)
            {
                Assert.AreEqual(dic1[key], newDic[key]);
                Assert.AreEqual(dic2[key], newDic[key]);
            }

            //在使用非XPatchSerializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
            Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicComplexOperator_CombineIDictionary()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "newBkey");
            dic2.Add("Dkey", null);
            dic2.Add("Ekey", "EValue");

            XElement changedEle = XElement.Load(new StringReader(ComplexOperatorChangedContext));

            Dictionary<string, string> newDic = new CombineIDictionary(new TypeExtend(dic1.GetType())).Combine(dic1, changedEle) as Dictionary<string, string>;

            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, newDic.Count);
            Assert.AreEqual(dic2.Count, newDic.Count);
            foreach (string key in dic2.Keys)
            {
                Assert.AreEqual(dic1[key], newDic[key]);
                Assert.AreEqual(dic2[key], newDic[key]);
            }

            //在使用非XPatchSerializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
            Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicComplexOperator_CombineXPatchSerializer()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "newBkey");
            dic2.Add("Dkey", null);
            dic2.Add("Ekey", "EValue");

            Dictionary<string, string> newDic = null;
            using (StringReader reader = new StringReader(ComplexOperatorChangedContext.ToString()))
            {
                newDic = new XPatchSerializer(dic1.GetType()).Combine(reader, dic1) as Dictionary<string, string>;
            }
            //newDic的长度应该为2，dic1的长度为4（因为dic1并未被改变）
            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, dic1.Count);
            Assert.AreEqual(4, newDic.Count);
            foreach (string key in newDic.Keys)
            {
                if (!key.Equals("Akey") && key.Equals("Ckey"))
                {
                    Assert.AreNotEqual(dic1[key], newDic[key]);
                }
                Assert.AreEqual(dic2[key], newDic[key]);
            }

            //在使用XPatchSerializer入口做增量内容合并时，会首先对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreNotEqual(dic1.GetHashCode(), newDic.GetHashCode());
            Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicComplexOperator_DivideXPatchSerializer()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "newBkey");
            dic2.Add("Dkey", null);
            dic2.Add("Ekey", "EValue");

            using (MemoryStream stream = new MemoryStream())
            {
                XPatchSerializer serializer = new XPatchSerializer(dic1.GetType());
                serializer.Divide(stream, dic1, dic2);
                string context = TestHelper.StreamToString(stream);
                Assert.AreEqual(XMLHeaderContext + System.Environment.NewLine + ComplexOperatorChangedContext, context);
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicCreate_CombineCore()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            XElement changedEle = XElement.Load(new StringReader(CreateChangedContext));

            Dictionary<string, string> oldDic = null;

            Dictionary<string, string> newDic = new CombineCore(new TypeExtend(dic1.GetType())).Combine(oldDic, changedEle) as Dictionary<string, string>;

            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(dic1.Count, newDic.Count);
            foreach (string key in dic1.Keys)
            {
                Assert.AreEqual(dic1[key], newDic[key]);
            }

            //原始值为Null时，内部重新创建对象实例，所以原始值依然为Null。
            Assert.IsNull(oldDic);
        }

        [TestMethod]
        public void TestCombineString_StringDicCreate_CombineIDictionary()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            XElement changedEle = XElement.Load(new StringReader(CreateChangedContext));

            Dictionary<string, string> oldDic = null;

            Dictionary<string, string> newDic = new CombineIDictionary(new TypeExtend(dic1.GetType())).Combine(oldDic, changedEle) as Dictionary<string, string>;

            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(dic1.Count, newDic.Count);
            foreach (string key in dic1.Keys)
            {
                Assert.AreEqual(dic1[key], newDic[key]);
            }

            //原始值为Null时，内部重新创建对象实例，所以原始值依然为Null。
            Assert.IsNull(oldDic);
        }

        [TestMethod]
        public void TestCombineString_StringDicCreate_CombineXPatchSerializer()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> oldDic = null;
            Dictionary<string, string> newDic = null;
            using (StringReader reader = new StringReader(CreateChangedContext))
            {
                newDic = new XPatchSerializer(dic1.GetType()).Combine(reader, oldDic) as Dictionary<string, string>;
            }
            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(dic1.Count, newDic.Count);
            foreach (string key in dic1.Keys)
            {
                Assert.AreEqual(dic1[key], newDic[key]);
            }

            //原始值为Null时，内部重新创建对象实例，所以原始值依然为Null。
            Assert.IsNull(oldDic);
        }

        [TestMethod]
        public void TestCombineString_StringDicCreate_DivideXPatchSerializer()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            using (MemoryStream stream = new MemoryStream())
            {
                XPatchSerializer serializer = new XPatchSerializer(dic1.GetType());
                serializer.Divide(stream, null, dic1);
                string context = TestHelper.StreamToString(stream);
                Assert.AreEqual(XMLHeaderContext + System.Environment.NewLine + CreateChangedContext, context);
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicEdit_CombineCore()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "newAvalue");
            dic2.Add("Bkey", "newBvalue");
            dic2.Add("Ckey", "newCvalue");
            dic2.Add("Dkey", "newDvalue");

            XElement changedEle = XElement.Load(new StringReader(EditChangedContext));

            Dictionary<string, string> newDic = new CombineCore(new TypeExtend(dic1.GetType())).Combine(dic1, changedEle) as Dictionary<string, string>;

            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, newDic.Count);
            Assert.AreEqual(dic2.Count, newDic.Count);
            foreach (string key in dic2.Keys)
            {
                Assert.AreEqual(dic1[key], newDic[key]);
                Assert.AreEqual(dic2[key], newDic[key]);
            }

            //在使用非XPatchSerializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
            Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicEdit_CombineIDictionary()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "newAvalue");
            dic2.Add("Bkey", "newBvalue");
            dic2.Add("Ckey", "newCvalue");
            dic2.Add("Dkey", "newDvalue");

            XElement changedEle = XElement.Load(new StringReader(EditChangedContext));

            Dictionary<string, string> newDic = new CombineIDictionary(new TypeExtend(dic1.GetType())).Combine(dic1, changedEle) as Dictionary<string, string>;

            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, newDic.Count);
            Assert.AreEqual(dic2.Count, newDic.Count);
            foreach (string key in dic2.Keys)
            {
                Assert.AreEqual(dic1[key], newDic[key]);
                Assert.AreEqual(dic2[key], newDic[key]);
            }

            //在使用非XPatchSerializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
            Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicEdit_CombineXPatchSerializer()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "newAvalue");
            dic2.Add("Bkey", "newBvalue");
            dic2.Add("Ckey", "newCvalue");
            dic2.Add("Dkey", "newDvalue");

            Dictionary<string, string> newDic = null;
            using (StringReader reader = new StringReader(EditChangedContext.ToString()))
            {
                newDic = new XPatchSerializer(dic1.GetType()).Combine(reader, dic1) as Dictionary<string, string>;
            }
            //newDic的长度应该为2，dic1的长度为4（因为dic1并未被改变）
            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, dic1.Count);
            Assert.AreEqual(4, newDic.Count);
            foreach (string key in newDic.Keys)
            {
                Assert.AreNotEqual(dic1[key], newDic[key]);
                Assert.AreEqual(dic2[key], newDic[key]);
            }

            //在使用XPatchSerializer入口做增量内容合并时，会首先对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreNotEqual(dic1.GetHashCode(), newDic.GetHashCode());
            Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicEdit_DivideXPatchSerializer()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "newAvalue");
            dic2.Add("Bkey", "newBvalue");
            dic2.Add("Ckey", "newCvalue");
            dic2.Add("Dkey", "newDvalue");

            using (MemoryStream stream = new MemoryStream())
            {
                XPatchSerializer serializer = new XPatchSerializer(dic1.GetType());
                serializer.Divide(stream, dic1, dic2);
                string context = TestHelper.StreamToString(stream);
                Assert.AreEqual(XMLHeaderContext + System.Environment.NewLine + EditChangedContext, context);
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicRemove_CombineCore()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            XElement changedEle = XElement.Load(new StringReader(RemoveChangedContext));

            Dictionary<string, string> newDic = new CombineCore(new TypeExtend(dic1.GetType())).Combine(dic1, changedEle) as Dictionary<string, string>;

            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(2, newDic.Count);
            Assert.AreEqual(dic1.Count, newDic.Count);
            foreach (string key in dic1.Keys)
            {
                Assert.AreEqual(dic1[key], newDic[key]);
            }

            //在使用非XPatchSerializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicRemove_CombineIDictionary()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            XElement changedEle = XElement.Load(new StringReader(RemoveChangedContext));

            Dictionary<string, string> newDic = new CombineIDictionary(new TypeExtend(dic1.GetType())).Combine(dic1, changedEle) as Dictionary<string, string>;

            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(2, newDic.Count);
            Assert.AreEqual(dic1.Count, newDic.Count);
            foreach (string key in dic1.Keys)
            {
                Assert.AreEqual(dic1[key], newDic[key]);
            }

            //在使用非XPatchSerializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicRemove_CombineXPatchSerializer()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> newDic = null;
            using (StringReader reader = new StringReader(RemoveChangedContext.ToString()))
            {
                newDic = new XPatchSerializer(dic1.GetType()).Combine(reader, dic1) as Dictionary<string, string>;
            }
            //newDic的长度应该为2，dic1的长度为4（因为dic1并未被改变）
            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, dic1.Count);
            Assert.AreEqual(2, newDic.Count);
            foreach (string key in newDic.Keys)
            {
                Assert.AreEqual(dic1[key], newDic[key]);
            }

            //在使用XPatchSerializer入口做增量内容合并时，会首先对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreNotEqual(dic1.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicRemove_DivideXPatchSerializer()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "Bvalue");

            using (MemoryStream stream = new MemoryStream())
            {
                XPatchSerializer serializer = new XPatchSerializer(dic1.GetType());
                serializer.Divide(stream, dic1, dic2);
                string context = TestHelper.StreamToString(stream);
                Assert.AreEqual(XMLHeaderContext + System.Environment.NewLine + RemoveChangedContext, context);
            }
        }

        [TestMethod]
        public void TestCombineString_StringDicSetNull_CombineCore()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", null);
            dic2.Add("Bkey", null);
            dic2.Add("Ckey", null);
            dic2.Add("Dkey", null);

            XElement changedEle = XElement.Load(new StringReader(SetNullChangedContext));

            Dictionary<string, string> newDic = new CombineCore(new TypeExtend(dic1.GetType())).Combine(dic1, changedEle) as Dictionary<string, string>;

            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, newDic.Count);
            Assert.AreEqual(dic2.Count, newDic.Count);
            foreach (string key in dic2.Keys)
            {
                Assert.AreEqual(dic1[key], newDic[key]);
                Assert.AreEqual(dic2[key], newDic[key]);
            }

            //在使用非XPatchSerializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
            Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicSetNull_CombineIDictionary()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", null);
            dic2.Add("Bkey", null);
            dic2.Add("Ckey", null);
            dic2.Add("Dkey", null);

            XElement changedEle = XElement.Load(new StringReader(SetNullChangedContext));

            Dictionary<string, string> newDic = new CombineIDictionary(new TypeExtend(dic1.GetType())).Combine(dic1, changedEle) as Dictionary<string, string>;

            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, newDic.Count);
            Assert.AreEqual(dic2.Count, newDic.Count);
            foreach (string key in dic2.Keys)
            {
                Assert.AreEqual(dic1[key], newDic[key]);
                Assert.AreEqual(dic2[key], newDic[key]);
            }

            //在使用非XPatchSerializer入口做增量内容合并时，不会对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreEqual(dic1.GetHashCode(), newDic.GetHashCode());
            Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicSetNull_CombineXPatchSerializer()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", null);
            dic2.Add("Bkey", null);
            dic2.Add("Ckey", null);
            dic2.Add("Dkey", null);

            Dictionary<string, string> newDic = null;
            using (StringReader reader = new StringReader(SetNullChangedContext.ToString()))
            {
                newDic = new XPatchSerializer(dic1.GetType()).Combine(reader, dic1) as Dictionary<string, string>;
            }
            //newDic的长度应该为2，dic1的长度为4（因为dic1并未被改变）
            Assert.IsInstanceOfType(newDic, typeof(Dictionary<string, string>));
            Assert.IsNotNull(newDic);
            Assert.AreEqual(4, dic1.Count);
            Assert.AreEqual(4, newDic.Count);
            foreach (string key in newDic.Keys)
            {
                Assert.AreNotEqual(dic1[key], newDic[key]);
                Assert.AreEqual(dic2[key], newDic[key]);
            }

            //在使用XPatchSerializer入口做增量内容合并时，会首先对原始对象进行深克隆，保证不影响到原有对象
            Assert.AreNotEqual(dic1.GetHashCode(), newDic.GetHashCode());
            Assert.AreNotEqual(dic2.GetHashCode(), newDic.GetHashCode());
        }

        [TestMethod]
        public void TestCombineString_StringDicSetNull_DivideXPatchSerializer()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", null);
            dic2.Add("Bkey", null);
            dic2.Add("Ckey", null);
            dic2.Add("Dkey", null);

            using (MemoryStream stream = new MemoryStream())
            {
                XPatchSerializer serializer = new XPatchSerializer(dic1.GetType());
                serializer.Divide(stream, dic1, dic2);
                string context = TestHelper.StreamToString(stream);
                Assert.AreEqual(XMLHeaderContext + System.Environment.NewLine + SetNullChangedContext, context);
            }
        }

        [TestMethod]
        public void TestDivideString_StringDicComplexOperator_DivideCore()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "newBkey");
            dic2.Add("Dkey", null);
            dic2.Add("Ekey", "EValue");

            XElement changedEle = new DivideCore(new TypeExtend(dic1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2);

            Assert.AreEqual(ComplexOperatorChangedContext, changedEle.ToString());
        }

        [TestMethod]
        public void TestDivideString_StringDicComplexOperator_DivideIDictionary()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "newBkey");
            dic2.Add("Dkey", null);
            dic2.Add("Ekey", "EValue");

            XElement changedEle = new DivideIDictionary(new TypeExtend(dic1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2);

            Assert.AreEqual(ComplexOperatorChangedContext, changedEle.ToString());
        }

        [TestMethod]
        public void TestDivideString_StringDicCreate_DivideCore()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            XElement changedEle = new DivideCore(new TypeExtend(dic1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), null, dic1);

            Assert.AreEqual(CreateChangedContext, changedEle.ToString());
        }

        [TestMethod]
        public void TestDivideString_StringDicCreate_DivideIDictionary()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            XElement changedEle = new DivideIDictionary(new TypeExtend(dic1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), null, dic1);

            Assert.AreEqual(CreateChangedContext, changedEle.ToString());
        }

        [TestMethod]
        public void TestDivideString_StringDicEdit_DivideCore()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "newAvalue");
            dic2.Add("Bkey", "newBvalue");
            dic2.Add("Ckey", "newCvalue");
            dic2.Add("Dkey", "newDvalue");

            XElement changedEle = new DivideCore(new TypeExtend(dic1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2);

            Assert.AreEqual(EditChangedContext, changedEle.ToString());
        }

        [TestMethod]
        public void TestDivideString_StringDicEdit_DivideIDictionary()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "newAvalue");
            dic2.Add("Bkey", "newBvalue");
            dic2.Add("Ckey", "newCvalue");
            dic2.Add("Dkey", "newDvalue");

            XElement changedEle = new DivideIDictionary(new TypeExtend(dic1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2);

            Assert.AreEqual(EditChangedContext, changedEle.ToString());
        }

        [TestMethod]
        public void TestDivideString_StringDicRemove_DivideCore()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "Bvalue");

            XElement changedEle = new DivideCore(new TypeExtend(dic1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2);

            Assert.AreEqual(RemoveChangedContext, changedEle.ToString());
        }

        [TestMethod]
        public void TestDivideString_StringDicRemove_DivideIDictionary()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", "Avalue");
            dic2.Add("Bkey", "Bvalue");

            XElement changedEle = new DivideIDictionary(new TypeExtend(dic1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2);

            Assert.AreEqual(RemoveChangedContext, changedEle.ToString());
        }

        [TestMethod]
        public void TestDivideString_StringDicSetNull_DivideCore()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", null);
            dic2.Add("Bkey", null);
            dic2.Add("Ckey", null);
            dic2.Add("Dkey", null);

            XElement changedEle = new DivideCore(new TypeExtend(dic1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2);

            Assert.AreEqual(SetNullChangedContext, changedEle.ToString());
        }

        [TestMethod]
        public void TestDivideString_StringDicSetNull_DivideIDictionary()
        {
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("Akey", "Avalue");
            dic1.Add("Bkey", "Bvalue");
            dic1.Add("Ckey", "Cvalue");
            dic1.Add("Dkey", "Dvalue");

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("Akey", null);
            dic2.Add("Bkey", null);
            dic2.Add("Ckey", null);
            dic2.Add("Dkey", null);

            XElement changedEle = new DivideIDictionary(new TypeExtend(dic1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(dic1.GetType()), dic1, dic2);

            Assert.AreEqual(SetNullChangedContext, changedEle.ToString());
        }
    }
}