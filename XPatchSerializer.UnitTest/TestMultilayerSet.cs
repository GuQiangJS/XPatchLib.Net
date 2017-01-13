using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestMultilayerSet
    {
        [TestMethod]
        [Description("多层次的集合增加内容拆分及合并测试")]
        public void TestAppendDividendAndCombineMultilayerSet()
        {
            Root r1 = CreateRoot(10, 3);
            Root r2 = CreateRoot(10, 4);

            XPatchSerializer serializer = new XPatchSerializer(typeof(Root));

            string context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, r1, r2);
                context = TestHelper.StreamToString(stream);
            }
            using (StringReader reader = new StringReader(context))
            {
                Root r3 = serializer.Combine(reader, r1) as Root;
                Assert.AreEqual(r1, r3);
            }
        }

        [TestMethod]
        [Description("多层次的集合删除内容拆分及合并测试")]
        public void TestRemoveDividendAndCombineMultilayerSet()
        {
            Root r1 = CreateRoot(10, 4);
            Root r2 = CreateRoot(10, 3);

            XPatchSerializer serializer = new XPatchSerializer(typeof(Root));

            string context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, r1, r2);
                context = TestHelper.StreamToString(stream);
            }
            using (StringReader reader = new StringReader(context))
            {
                Root r3 = serializer.Combine(reader, r1) as Root;
                Assert.AreEqual(r1, r3);
            }
        }

        private Root CreateRoot(int pFirstNum, int pSecondNum)
        {
            Root result = new Root();
            for (int i = 0; i < pFirstNum; i++)
            {
                result.Childs.Add(new FirstChild() { Key = i.ToString(), Value = i.ToString() });
                for (int j = 0; j < pSecondNum; j++)
                {
                    result.Childs[i].Childs.Add(new SecondChild() { Key = j.ToString(), Value = j.ToString() });
                }
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
                FirstChild c = obj as FirstChild;
                if (c == null)
                {
                    return false;
                }
                return string.Equals(this.Key, c.Key) &&
                    string.Equals(this.Value, c.Value) &&
                    this.Childs.Except(c.Childs).Count() > 0 &&
                    c.Childs.Except(this.Childs).Count() > 0;
            }
        }

        [PrimaryKey("Key")]
        public class KeyValueClass
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public override bool Equals(object obj)
            {
                KeyValueClass c = obj as KeyValueClass;
                if (c == null)
                {
                    return false;
                }
                return string.Equals(this.Key, c.Key) &&
                    string.Equals(this.Value, c.Value);
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
                Root r = obj as Root;
                if (r == null)
                {
                    return false;
                }
                return this.Childs.Except(r.Childs).Count() > 0 && r.Childs.Except(this.Childs).Count() > 0;
            }
        }

        public class SecondChild : KeyValueClass
        {
        }
    }
}