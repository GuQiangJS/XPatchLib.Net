using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            string str = DoSerializer_Divide(null, dic);

            Trace.WriteLine(str);

            ConcurrentDictionary<string, string> dic_1 = DoSerializer_Combie<ConcurrentDictionary<string, string>>(str, null);

            Assert.AreEqual(dic, dic_1);
            Assert.True(dic_1.ContainsKey("KEY"));
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

            string str = DoSerializer_Divide(dic_1, dic_2);

            Trace.WriteLine(str);

            ConcurrentDictionary<string, string> dic_3 =
                DoSerializer_Combie<ConcurrentDictionary<string, string>>(str, dic_1, true);

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

            string str = DoSerializer_Divide(null, queue1);
            //Assert.AreEqual(@"[1]", output);
            Trace.WriteLine(str);

            ConcurrentQueue<int> queue2 =
                DoSerializer_Combie<ConcurrentQueue<int>>(str, queue1, true);
            int i;
            Assert.IsTrue(queue2.TryDequeue(out i));
            Assert.AreEqual(1, i);
        }
        [Test]
        public void SerializeConcurrentBag()
        {
            ConcurrentBag<int> bag1 = new ConcurrentBag<int>();
            bag1.Add(1);

            string str = DoSerializer_Divide(null, bag1);
            //Assert.AreEqual(@"[1]", output);
            Trace.WriteLine(str);

            ConcurrentBag<int> bag2 =
                DoSerializer_Combie<ConcurrentBag<int>>(str, bag1, true);
            int i;
            Assert.IsTrue(bag2.TryTake(out i));
            Assert.AreEqual(1, i);
        }

        [Test]
        public void SerializeConcurrentStack()
        {
            ConcurrentStack<int> stack1 = new ConcurrentStack<int>();
            stack1.Push(1);

            string str = DoSerializer_Divide(null, stack1);
            //Assert.AreEqual(@"[1]", output);
            Trace.WriteLine(str);

            ConcurrentStack<int> stack2 =
                DoSerializer_Combie<ConcurrentStack<int>>(str, stack1, true);
            int i;
            Assert.IsTrue(stack2.TryPop(out i));
            Assert.AreEqual(1, i);
        }
#endif
    }
}
