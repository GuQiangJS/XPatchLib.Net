// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if (NET_35_UP || NETSTANDARD)
using System.Xml.Linq;
#endif
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
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
    public class TestKeyValuePair : TestBase
    {
        [Test]
        [Description("原始值的Key值与更新后的值的Key值不同时，抛出异常")]
        public void TestCombineSingleKeyValuePairByDifferentKey()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var k1 = new KeyValuePair<string, string>("1", "1");
                var k2 = new KeyValuePair<string, string>(null, null);

                var changedContext = @"<KeyValuePair_String_String Action=""Remove"">
  <Key>" + k2.Key + @"</Key>
</KeyValuePair_String_String>";

                var combineObj = DoCombineKeyValuePair_Combie<KeyValuePair<string, string>>(changedContext, k1);
                Assert.AreEqual(k2, combineObj);
            }, string.Format(CultureInfo.InvariantCulture,
                ResourceHelper.GetResourceString(LocalizationRes.Exp_String_KeyValueChanged),
                "1", null));
        }

        [Test]
        public void TestDivideAndCombineSingleKeyValuePairByNullOriObject()
        {
            KeyValuePair<string, string>? k2 = new KeyValuePair<string, string>("1", "2");
            var changedContext = @"<KeyValuePair_String_String>
  <Key>" + k2.Value.Key + @"</Key>
  <Value>" + k2.Value.Value + @"</Value>
</KeyValuePair_String_String>";

            string context = DoDivideKeyValuePair_Divide(null, k2);
            Assert.AreEqual(changedContext, context);
            var combineObj = DoCombineKeyValuePair_Combie<KeyValuePair<string, string>?>(context, null);
            Assert.AreEqual(k2, combineObj);
        }

        [Test]
        public void TestDivideAndCombineSingleKeyValuePairByNullRevObject()
        {
            KeyValuePair<string, string>? k1 = new KeyValuePair<string, string>("1", "1");

            var changedContext = @"<KeyValuePair_String_String Action=""Remove"">
  <Key>" + k1.Value.Key + @"</Key>
</KeyValuePair_String_String>";

            string context = DoDivideKeyValuePair_Divide(k1, null);
            Assert.AreEqual(changedContext, context);
            var combineObj = DoCombineKeyValuePair_Combie<KeyValuePair<string, string>?>(context, k1);
            Assert.AreEqual(new KeyValuePair<string, string>("1", null), combineObj);
        }

        [Test]
        [Description("原始值的Key值与更新后的值的Key值不同时，抛出异常")]
        public void TestDivideAndCombineSingleKeyValuePairByOriValueIsNull()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var k1 = new KeyValuePair<string, string>(null, null);
                var k2 = new KeyValuePair<string, string>("1", "2");

                var changedContext = @"<KeyValuePair_String_String>
  <Key>" + k2.Key + @"</Key>
  <Value>" + k2.Value + @"</Value>
</KeyValuePair_String_String>";

                string context = DoDivideKeyValuePair_Divide(k1, k2);
                Assert.AreEqual(changedContext, context);
                var combineObj = DoCombineKeyValuePair_Combie(context, k1);
                Assert.AreEqual(k2, combineObj);
            }, string.Format(CultureInfo.InvariantCulture,
                ResourceHelper.GetResourceString(LocalizationRes.Exp_String_KeyValueChanged),
                null, "1"));
        }

        [Test]
        public void TestDivideAndCombineSingleKeyValuePairBySameKey()
        {
            var k1 = new KeyValuePair<string, string>("1", "1");
            var k2 = new KeyValuePair<string, string>("1", "2");

            var changedContext = @"<KeyValuePair_String_String>
  <Key>" + k2.Key + @"</Key>
  <Value>" + k2.Value + @"</Value>
</KeyValuePair_String_String>";

            string context = DoDivideKeyValuePair_Divide(k1, k2);
            Assert.AreEqual(changedContext, context);
            var combineObj = DoCombineKeyValuePair_Combie(context, k1);
            Assert.AreEqual(k2, combineObj);
        }

        [Test]
        public void TestDivideAndCombineSingleKeyValuePairBySameKeyAndRevValueIsNull()
        {
            var k1 = new KeyValuePair<string, string>("1", "1");
            var k2 = new KeyValuePair<string, string>("1", null);

            var changedContext = @"<KeyValuePair_String_String Action=""SetNull"">
  <Key>" + k2.Key + @"</Key>
</KeyValuePair_String_String>";

            string context = DoDivideKeyValuePair_Divide(k1, k2);
            Assert.AreEqual(changedContext, context);
            var combineObj = DoCombineKeyValuePair_Combie(context, k1);
            Assert.AreEqual(k2, combineObj);
        }
        

        [Test]
        [Description("当原始值不为Null同时更新后的值不为Null时，原始值的Key值就应该与更新后的值的Key值相同，否则不是同一个KeyValuePair对象,此时会抛出异常")]
        public void TestDivideSingleKeyValuePairByDifferentKey()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var k1 = new KeyValuePair<string, string>("1", "1");
                var k2 = new KeyValuePair<string, string>(null, null);

                var changedContext = @"<KeyValuePair_String_String Action=""Remove"">
  <Key>" + k1.Key + @"</Key>
</KeyValuePair_String_String>";

                string context = DoDivideKeyValuePair_Divide(k1, k2);
                Assert.AreEqual(changedContext, context);
                var combineObj = DoCombineKeyValuePair_Combie(context, k1);
                Assert.AreEqual(k2, combineObj);

            }, string.Format(CultureInfo.InvariantCulture,
                ResourceHelper.GetResourceString(LocalizationRes.Exp_String_KeyValueChanged),
                "1", null));
        }
    }
}