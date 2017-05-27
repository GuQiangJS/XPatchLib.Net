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
    public class TestKeyValuePair
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "原始值的Key值与更新后的值的Key值不同时，抛出异常")]
        public void TestCombineSingleKeyValuePairByDifferentKey()
        {
            var k1 = new KeyValuePair<string, string>("1", "1");
            var k2 = new KeyValuePair<string, string>(null, null);

            var changedContext = @"<KeyValuePair_String_String Action=""Remove"">
  <Key>" + k2.Key + @"</Key>
</KeyValuePair_String_String>";

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(changedContext)))
            {
                using (XmlTextReader reader = new XmlTextReader(xmlReader))
                {
                    var combineObj = new CombineKeyValuePair(new TypeExtend(k1.GetType(), null)).Combine(reader, k1,
                        ReflectionUtils.GetTypeFriendlyName(k2.GetType()));
                    Assert.AreEqual(k2, combineObj);
                }
            }
        }

        [TestMethod]
        public void TestDivideAndCombineSingleKeyValuePairByNullOriObject()
        {
            var k2 = new KeyValuePair<string, string>("1", "2");
            var changedContext = @"<KeyValuePair_String_String>
  <Key>" + k2.Key + @"</Key>
  <Value>" + k2.Value + @"</Value>
</KeyValuePair_String_String>";

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideKeyValuePair(writer, new TypeExtend(k2.GetType(), writer.IgnoreAttributeType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(k2.GetType()), null, k2));
                }

                stream.Position = 0;
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(changedContext, changedEle.ToString());
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        var combineObj = new CombineKeyValuePair(new TypeExtend(k2.GetType(), null)).Combine(reader,
                            null,
                            ReflectionUtils.GetTypeFriendlyName(k2.GetType()));
                        Assert.AreEqual(k2, combineObj);
                    }
                }
            }
        }

        [TestMethod]
        public void TestDivideAndCombineSingleKeyValuePairByNullRevObject()
        {
            var k1 = new KeyValuePair<string, string>("1", "1");

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideKeyValuePair(writer, new TypeExtend(k1.GetType(), writer.IgnoreAttributeType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(k1.GetType()), k1, null));
                }

                stream.Position = 0;

                var changedContext = @"<KeyValuePair_String_String Action=""Remove"">
  <Key>" + k1.Key + @"</Key>
</KeyValuePair_String_String>";
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(changedContext, changedEle.ToString());

                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        var combineObj = new CombineKeyValuePair(new TypeExtend(k1.GetType(), null)).Combine(reader, k1,
                            ReflectionUtils.GetTypeFriendlyName(k1.GetType()));
                        Assert.AreEqual(null, combineObj);
                    }
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "原始值的Key值与更新后的值的Key值不同时，抛出异常")]
        public void TestDivideAndCombineSingleKeyValuePairByOriValueIsNull()
        {
            var k1 = new KeyValuePair<string, string>(null, null);
            var k2 = new KeyValuePair<string, string>("1", "2");

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideKeyValuePair(writer, new TypeExtend(k1.GetType(), writer.IgnoreAttributeType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(k1.GetType()), k1, k2));
                }

                stream.Position = 0;
                var changedContext = @"<KeyValuePair_String_String>
  <Key>" + k2.Key + @"</Key>
  <Value>" + k2.Value + @"</Value>
</KeyValuePair_String_String>";
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(changedContext, changedEle.ToString());

                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        var combineObj = new CombineKeyValuePair(new TypeExtend(k1.GetType(), null)).Combine(reader, k1,
                            ReflectionUtils.GetTypeFriendlyName(k1.GetType()));
                        Assert.AreEqual(k2, combineObj);
                    }
                }
            }
        }

        [TestMethod]
        public void TestDivideAndCombineSingleKeyValuePairBySameKey()
        {
            var k1 = new KeyValuePair<string, string>("1", "1");
            var k2 = new KeyValuePair<string, string>("1", "2");

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideKeyValuePair(writer, new TypeExtend(k1.GetType(), writer.IgnoreAttributeType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(k1.GetType()), k1, k2));
                }

                stream.Position = 0;
                var changedContext = @"<KeyValuePair_String_String>
  <Key>" + k2.Key + @"</Key>
  <Value>" + k2.Value + @"</Value>
</KeyValuePair_String_String>";
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(changedContext, changedEle.ToString());
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        var combineObj = new CombineKeyValuePair(new TypeExtend(k1.GetType(), null)).Combine(reader, k1,
                            ReflectionUtils.GetTypeFriendlyName(k1.GetType()));
                        Assert.AreEqual(k2, combineObj);
                    }
                }
            }
        }

        [TestMethod]
        public void TestDivideAndCombineSingleKeyValuePairBySameKeyAndRevValueIsNull()
        {
            var k1 = new KeyValuePair<string, string>("1", "1");
            var k2 = new KeyValuePair<string, string>("1", null);

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideKeyValuePair(writer, new TypeExtend(k1.GetType(), writer.IgnoreAttributeType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(k1.GetType()), k1, k2));
                }

                stream.Position = 0;
                var changedContext = @"<KeyValuePair_String_String Action=""SetNull"">
  <Key>" + k2.Key + @"</Key>
</KeyValuePair_String_String>";
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(changedContext, changedEle.ToString());
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        var combineObj = new CombineKeyValuePair(new TypeExtend(k1.GetType(), null)).Combine(reader, k1,
                            ReflectionUtils.GetTypeFriendlyName(k1.GetType()));
                        Assert.AreEqual(k2, combineObj);
                    }
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "当原始值不为Null同时更新后的值不为Null时，原始值的Key值就应该与更新后的值的Key值相同，否则不是同一个KeyValuePair对象,此时会抛出异常")]
        public void TestDivideSingleKeyValuePairByDifferentKey()
        {
            var k1 = new KeyValuePair<string, string>("1", "1");
            var k2 = new KeyValuePair<string, string>(null, null);

            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    Assert.IsTrue(
                        new DivideKeyValuePair(writer, new TypeExtend(k1.GetType(), writer.IgnoreAttributeType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(k1.GetType()), k1, k2));
                }

                stream.Position = 0;
                var changedContext = @"<KeyValuePair_String_String Action=""Remove"">
  <Key>" + k1.Key + @"</Key>
</KeyValuePair_String_String>";
                var changedEle = XElement.Load(new StreamReader(stream));
                Assert.AreEqual(changedContext, changedEle.ToString());
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        var combineObj = new CombineKeyValuePair(new TypeExtend(k1.GetType(), null)).Combine(reader, k1,
                            ReflectionUtils.GetTypeFriendlyName(k1.GetType()));
                        Assert.AreEqual(k2, combineObj);
                    }
                }
            }
        }
    }
}