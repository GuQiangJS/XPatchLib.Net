using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestKeyValuePair
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "原始值的Key值与更新后的值的Key值不同时，抛出异常")]
        public void TestCombineSingleKeyValuePairByDifferentKey()
        {
            KeyValuePair<string, string> k1 = new KeyValuePair<string, string>("1", "1");
            KeyValuePair<string, string> k2 = new KeyValuePair<string, string>(null, null);

            string changedContext = @"<KeyValuePair_String_String Action=""Remove"">
  <Key>" + k1.Key + @"</Key>
</KeyValuePair_String_String>";
            XElement changedEle = new XElement("", changedContext);

            object combineObj = new CombineKeyValuePair(new TypeExtend(k1.GetType())).Combine(k1, changedEle);
            Assert.AreEqual(k2, combineObj);
        }

        [TestMethod]
        public void TestDivideAndCombineSingleKeyValuePairByNullOriObject()
        {
            KeyValuePair<string, string> k2 = new KeyValuePair<string, string>("1", "2");

            XElement changedEle = new DivideKeyValuePair(new TypeExtend(k2.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(k2.GetType()), null, k2);

            string changedContext = @"<KeyValuePair_String_String>
  <Key>" + k2.Key + @"</Key>
  <Value>" + k2.Value + @"</Value>
</KeyValuePair_String_String>";

            Assert.AreEqual(changedContext, changedEle.ToString());

            object combineObj = new CombineKeyValuePair(new TypeExtend(k2.GetType())).Combine(null, changedEle);
            Assert.AreEqual(k2, combineObj);
        }

        [TestMethod]
        public void TestDivideAndCombineSingleKeyValuePairByNullRevObject()
        {
            KeyValuePair<string, string> k1 = new KeyValuePair<string, string>("1", "1");

            XElement changedEle = new DivideKeyValuePair(new TypeExtend(k1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(k1.GetType()), k1, null);

            string changedContext = @"<KeyValuePair_String_String Action=""Remove"">
  <Key>" + k1.Key + @"</Key>
</KeyValuePair_String_String>";

            Assert.AreEqual(changedContext, changedEle.ToString());

            object combineObj = new CombineKeyValuePair(new TypeExtend(k1.GetType())).Combine(k1, changedEle);
            Assert.AreEqual(null, combineObj);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "原始值的Key值与更新后的值的Key值不同时，抛出异常")]
        public void TestDivideAndCombineSingleKeyValuePairByOriValueIsNull()
        {
            KeyValuePair<string, string> k1 = new KeyValuePair<string, string>(null, null);
            KeyValuePair<string, string> k2 = new KeyValuePair<string, string>("1", "2");

            XElement changedEle = new DivideKeyValuePair(new TypeExtend(k1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(k1.GetType()), k1, k2);

            string changedContext = @"<KeyValuePair_String_String>
  <Key>" + k2.Key + @"</Key>
  <Value>" + k2.Value + @"</Value>
</KeyValuePair_String_String>";

            Assert.AreEqual(changedContext, changedEle.ToString());

            object combineObj = new CombineKeyValuePair(new TypeExtend(k1.GetType())).Combine(k1, changedEle);
            Assert.AreEqual(k2, combineObj);
        }

        [TestMethod]
        public void TestDivideAndCombineSingleKeyValuePairBySameKey()
        {
            KeyValuePair<string, string> k1 = new KeyValuePair<string, string>("1", "1");
            KeyValuePair<string, string> k2 = new KeyValuePair<string, string>("1", "2");

            XElement changedEle = new DivideKeyValuePair(new TypeExtend(k1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(k1.GetType()), k1, k2);

            string changedContext = @"<KeyValuePair_String_String>
  <Key>" + k2.Key + @"</Key>
  <Value>" + k2.Value + @"</Value>
</KeyValuePair_String_String>";

            Assert.AreEqual(changedContext, changedEle.ToString());

            object combineObj = new CombineKeyValuePair(new TypeExtend(k1.GetType())).Combine(k1, changedEle);
            Assert.AreEqual(k2, combineObj);
        }

        [TestMethod]
        public void TestDivideAndCombineSingleKeyValuePairBySameKeyAndRevValueIsNull()
        {
            KeyValuePair<string, string> k1 = new KeyValuePair<string, string>("1", "1");
            KeyValuePair<string, string> k2 = new KeyValuePair<string, string>("1", null);

            XElement changedEle = new DivideKeyValuePair(new TypeExtend(k1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(k1.GetType()), k1, k2);

            string changedContext = @"<KeyValuePair_String_String Action=""SetNull"">
  <Key>" + k2.Key + @"</Key>
</KeyValuePair_String_String>";

            Assert.AreEqual(changedContext, changedEle.ToString());

            object combineObj = new CombineKeyValuePair(new TypeExtend(k1.GetType())).Combine(k1, changedEle);
            Assert.AreEqual(k2, combineObj);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "当原始值不为Null同时更新后的值不为Null时，原始值的Key值就应该与更新后的值的Key值相同，否则不是同一个KeyValuePair对象,此时会抛出异常")]
        public void TestDivideSingleKeyValuePairByDifferentKey()
        {
            KeyValuePair<string, string> k1 = new KeyValuePair<string, string>("1", "1");
            KeyValuePair<string, string> k2 = new KeyValuePair<string, string>(null, null);

            XElement changedEle = new DivideKeyValuePair(new TypeExtend(k1.GetType())).Divide(ReflectionUtils.GetTypeFriendlyName(k1.GetType()), k1, k2);

            string changedContext = @"<KeyValuePair_String_String Action=""Remove"">
  <Key>" + k1.Key + @"</Key>
</KeyValuePair_String_String>";

            Assert.AreEqual(changedContext, changedEle.ToString());

            object combineObj = new CombineKeyValuePair(new TypeExtend(k1.GetType())).Combine(k1, changedEle);
            Assert.AreEqual(k2, combineObj);
        }
    }
}