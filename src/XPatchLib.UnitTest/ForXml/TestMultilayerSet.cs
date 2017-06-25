// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    public class TestMultilayerSet
    {
        private Root CreateRoot(int pFirstNum, int pSecondNum)
        {
            var result = new Root();
            for (var i = 0; i < pFirstNum; i++)
            {
                result.Childs.Add(new FirstChild {Key = i.ToString(), Value = i.ToString()});
                for (var j = 0; j < pSecondNum; j++)
                    result.Childs[i].Childs.Add(new SecondChild {Key = j.ToString(), Value = j.ToString()});
            }
            return result;
        }

        public class FirstChild : KeyValueClass
        {
            public FirstChild()
            {
                Childs = new List<SecondChild>();
            }

            public List<SecondChild> Childs { get; set; }

            public override bool Equals(object obj)
            {
                var c = obj as FirstChild;
                if (c == null)
                    return false;
                return string.Equals(Key, c.Key) &&
                       string.Equals(Value, c.Value) &&
                       Childs.SequenceEqual(c.Childs);
            }
        }

        [PrimaryKey("Key")]
        public class KeyValueClass
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public override bool Equals(object obj)
            {
                var c = obj as KeyValueClass;
                if (c == null)
                    return false;
                return string.Equals(Key, c.Key) &&
                       string.Equals(Value, c.Value);
            }
        }

        public class Root
        {
            public Root()
            {
                Childs = new List<FirstChild>();
            }

            public List<FirstChild> Childs { get; set; }

            public override bool Equals(object obj)
            {
                var r = obj as Root;
                if (r == null)
                    return false;
                return Childs.SequenceEqual(r.Childs);
            }
        }

        public class SecondChild : KeyValueClass
        {
        }

        [Test]
        [Description("多层次的集合增加内容拆分及合并测试")]
        public void TestAppendDividendAndCombineMultilayerSet()
        {
            var r1 = CreateRoot(10, 3);
            var r2 = CreateRoot(10, 4);

            var serializer = new Serializer(typeof(Root));

            string context;
            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    serializer.Divide(writer, r1, r2);
                    context = UnitTest.TestHelper.StreamToString(stream);
                    Debug.WriteLine(context);
                }
            }
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(context)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    var r3 = serializer.Combine(reader, r1) as Root;
                    Assert.AreEqual(r2, r3);
                }
            }
        }

        [Test]
        [Description("多层次的集合删除内容拆分及合并测试")]
        public void TestRemoveDividendAndCombineMultilayerSet()
        {
            var r1 = CreateRoot(10, 4);
            var r2 = CreateRoot(10, 3);

            var serializer = new Serializer(typeof(Root));

            string context;
            using (var stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    serializer.Divide(writer, r1, r2);
                    context = UnitTest.TestHelper.StreamToString(stream);
                    Debug.WriteLine(context);
                }
            }
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(context)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    var r3 = serializer.Combine(reader, r1) as Root;
                    Assert.AreEqual(r2, r3);
                }
            }
        }
    }
}