using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
#if NUNIT
using NUnit.Framework;

#elif XUNIT
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;

#endif

namespace XPatchLib.UnitTest.ForXml
{
    [TestFixture]
    public class TestSerializerCollections:TestBase
    {
#if NET_40_UP || NETSTANDARD_2_0_UP
        [Test]
        public void TestCombineConcurrentDictionaryWithNullValue()
        {
            ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();
            dic.TryAdd("KEY", null);

            string output = DoSerializer_Divide(null, dic);
//            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
//<ConcurrentDictionary_String_String>
//  <KeyValuePair_String_String Action=""Add"">
//    <Key>KEY</Key>
//  </KeyValuePair_String_String>
//</ConcurrentDictionary_String_String>", output);
            LogHelper.Debug(output);

            ConcurrentDictionary<string, string> dic_1 = DoSerializer_Combie<ConcurrentDictionary<string, string>>(output, null);

            Assert.AreEqual(dic, dic_1);
            Assert.IsTrue(dic_1.ContainsKey("KEY"));
            Assert.IsNull(dic_1["KEY"]);
        }

        [Test]
        public void TestCombineAndDivideConcurrentDictionary()
        {
            ConcurrentDictionary<string, string> dic_1 = new ConcurrentDictionary<string, string>();
            dic_1.TryAdd("1", "A");
            dic_1.TryAdd("2", "B");
            dic_1.TryAdd("3", "C");

            ConcurrentDictionary<string, string> dic_2 = new ConcurrentDictionary<string, string>();
            dic_2.TryAdd("1", "A");
            dic_2.TryAdd("2", "F");
            dic_2.TryAdd("4", "D");

            string output = DoSerializer_Divide(dic_1, dic_2);
//            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
//<ConcurrentDictionary_String_String>
//  <KeyValuePair_String_String Action=""Remove"">
//    <Key>3</Key>
//  </KeyValuePair_String_String>
//  <KeyValuePair_String_String>
//    <Key>2</Key>
//    <Value>F</Value>
//  </KeyValuePair_String_String>
//  <KeyValuePair_String_String Action=""Add"">
//    <Key>4</Key>
//    <Value>D</Value>
//  </KeyValuePair_String_String>
//</ConcurrentDictionary_String_String>", output);
            LogHelper.Debug(output);

            ConcurrentDictionary<string, string> dic_3 =
                DoSerializer_Combie<ConcurrentDictionary<string, string>>(output, dic_1, true);

            Assert.AreEqual(dic_2, dic_3);
            foreach (KeyValuePair<string, string> pair in dic_2)
            {
                Assert.AreEqual(dic_3[pair.Key], dic_2[pair.Key]);
            }
        }

        [Test]
        public void SerializeConcurrentQueue()
        {
            ConcurrentQueue<int> queue1 = new ConcurrentQueue<int>();
            queue1.Enqueue(1);

            string output = DoSerializer_Divide(null, queue1);
//            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
//<ConcurrentQueue_Int32>
//  <Int32 Action=""Add"">1</Int32>
//</ConcurrentQueue_Int32>", output);
            LogHelper.Debug(output);

            ConcurrentQueue<int> queue2 =
                DoSerializer_Combie<ConcurrentQueue<int>>(output, queue1, true);
            int i;
            Assert.IsTrue(queue2.TryDequeue(out i));
            Assert.AreEqual(1, i);
        }
        [Test]
        public void SerializeConcurrentBag()
        {
            ConcurrentBag<int> bag1 = new ConcurrentBag<int>();
            bag1.Add(1);

            string output = DoSerializer_Divide(null, bag1);
//            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
//<ConcurrentBag_Int32>
//  <Int32 Action=""Add"">1</Int32>
//</ConcurrentBag_Int32>", output);
            LogHelper.Debug(output);

            ConcurrentBag<int> bag2 =
                DoSerializer_Combie<ConcurrentBag<int>>(output, bag1, true);
            int i;
            Assert.IsTrue(bag2.TryTake(out i));
            Assert.AreEqual(1, i);
            
            ConcurrentBag<int> bag_1 = new ConcurrentBag<int>();
            bag_1.Add(1);
            bag_1.Add(2);

            ConcurrentBag<int> bag_2 = new ConcurrentBag<int>();
            bag_2.Add(1);
            bag_2.Add(3);

            output = DoSerializer_Divide(bag_1, bag_2);
            LogHelper.Debug(output);

            Assert.Throws<NotImplementedException>(() => DoSerializer_Combie<ConcurrentBag<int>>(output, bag_1, true));
        }

        [Test]
        public void SerializeConcurrentStack()
        {
            ConcurrentStack<int> stack1 = new ConcurrentStack<int>();
            stack1.Push(1);

            string output = DoSerializer_Divide(null, stack1);
//            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
//<ConcurrentStack_Int32>
//  <Int32 Action=""Add"">1</Int32>
//</ConcurrentStack_Int32>", output);
            LogHelper.Debug(output);

            ConcurrentStack<int> stack2 =
                DoSerializer_Combie<ConcurrentStack<int>>(output, stack1, true);
            int i;
            Assert.IsTrue(stack2.TryPop(out i));
            Assert.AreEqual(1, i);
        }

        [PrimaryKey("Text1")]
        public class SomeObject
        {
            public string Text1 { get; set; }
        }

        public class CustomConcurrentDictionary : ConcurrentDictionary<string, List<SomeObject>>
        {
            [OnDeserialized]
            internal void OnDeserializedMethod(StreamingContext context)
            {
                ((IDictionary)this).Add("key2", new List<SomeObject>
                {
                    new SomeObject
                    {
                        Text1 = "value2"
                    }
                });
            }
        }
        [Test]
        public void SerializeCustomConcurrentDictionary()
        {
            IDictionary d = new CustomConcurrentDictionary();
            d.Add("key", new List<SomeObject>
            {
                new SomeObject
                {
                    Text1 = "value1"
                }
            });

            string output = this.DoSerializer_Divide(null, d);
            LogHelper.Debug(output);

            CustomConcurrentDictionary d2 = this.DoSerializer_Combie<CustomConcurrentDictionary>(output, null, true);

            Assert.AreEqual(2, d2.Count);
            Assert.AreEqual("value1", d2["key"][0].Text1);
            Assert.AreEqual("value2", d2["key2"][0].Text1);
        }
#endif

        [Test]
        [Description("不支持包含Remove或Update的增量合并")]
        public void SerializeQueue()
        {
            Queue<int> queue1 = new Queue<int>();
            queue1.Enqueue(1);
            queue1.Enqueue(2);

            Queue<int> queue2 = new Queue<int>();
            queue2.Enqueue(1);
            queue2.Enqueue(3);

            string output = DoSerializer_Divide(queue1, queue2);
            LogHelper.Debug(output);

            Assert.Throws<NotImplementedException>(() => DoSerializer_Combie<Queue<int>>(output, queue1, true));
        }

        [Test]
        [Description("不支持包含Remove或Update的增量合并")]
        public void SerializeStack()
        {
            Stack<int> queue1 = new Stack<int>();
            queue1.Push(1);
            queue1.Push(2);

            Stack<int> queue2 = new Stack<int>();
            queue2.Push(1);
            queue2.Push(3);

            string output = DoSerializer_Divide(queue1, queue2);
            LogHelper.Debug(output);

            Assert.Throws<NotImplementedException>(() => DoSerializer_Combie<Stack<int>>(output, queue1, true));
        }
    }
}
