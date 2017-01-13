using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderInfo = XPatchLib.UnitTest.ExampleComplexCollectionClass.OrderInfo;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestKeyValuesObjectEqualityComparer
    {
        [TestMethod]
        public void TestKeyValuesObjectEqualityComparerEqualsAddForString()
        {
            string[] s1 = new string[] { "A", "B" };
            string[] s2 = new string[] { "A", "B", "D" };

            TestKeyValuesObjectEqualityComparerEqualsAdd(s1, s2);
        }

        [TestMethod]
        public void TestKeyValuesObjectEqualityComparerEqualsForChar()
        {
            Char[] s1 = new Char[] { 'A', 'B', 'C' };
            Char[] s2 = new Char[] { 'A', 'B', 'D' };

            TestKeyValuesObjectEqualityComparerEquals(s1, s2);
        }

        [TestMethod]
        public void TestKeyValuesObjectEqualityComparerEqualsForComplexClass()
        {
            OrderInfo[] s1 = new OrderInfo[] {
                new OrderInfo()
                {
                    OrderId=1,
                    OrderTotal=1.1m
                },
                new OrderInfo()
                {
                    OrderId=2,
                    OrderTotal=1.2m
                },
                new OrderInfo()
                {
                    OrderId=3,
                    OrderTotal=1.3m
                },
            };

            OrderInfo[] s2 = new OrderInfo[] {
                new OrderInfo()
                {
                    OrderId=1,
                    OrderTotal=1.1m
                },
                new OrderInfo()
                {
                    OrderId=2,
                    OrderTotal=1.2m
                },
                new OrderInfo()
                {
                    OrderId=4,
                    OrderTotal=1.4m
                },
            };

            TestKeyValuesObjectEqualityComparerEquals(s1, s2);
        }

        [TestMethod]
        public void TestKeyValuesObjectEqualityComparerEqualsForFloat()
        {
            float[] s1 = new float[] { 1.0f, 1.1f, 1.2f };
            float[] s2 = new float[] { 1.0f, 1.1f, 1.3f };

            TestKeyValuesObjectEqualityComparerEquals(s1, s2);
        }

        [TestMethod]
        public void TestKeyValuesObjectEqualityComparerEqualsForInt()
        {
            int[] s1 = new int[] { 1, 2, 3 };
            int[] s2 = new int[] { 1, 2, 4 };

            TestKeyValuesObjectEqualityComparerEquals(s1, s2);
        }

        private void TestKeyValuesObjectEqualityComparerEquals<T>(T[] s1, T[] s2)
        {
            IEnumerable<KeyValuesObject> l1 = KeyValuesObject.Translate(s1);
            IEnumerable<KeyValuesObject> l2 = KeyValuesObject.Translate(s2);

            List<KeyValuesObject> newList = l2.Except(l1, new KeyValuesObjectEqualityComparer()).ToList();
            Assert.AreEqual(newList.Count, 1);
            Assert.AreEqual(newList[0].OriValue, s2[2]);

            List<KeyValuesObject> removeList = l1.Except(l2, new KeyValuesObjectEqualityComparer()).ToList();
            Assert.AreEqual(removeList.Count, 1);
            Assert.AreEqual(removeList[0].OriValue, s1[2]);

            List<KeyValuesObject> sameList = l1.Intersect(l2, new KeyValuesObjectEqualityComparer()).ToList();
            Assert.AreEqual(sameList.Count, 2);
            Assert.AreEqual(sameList[0].OriValue, s1[0]);
            Assert.AreEqual(sameList[1].OriValue, s1[1]);
        }

        private void TestKeyValuesObjectEqualityComparerEqualsAdd<T>(T[] s1, T[] s2)
        {
            IEnumerable<KeyValuesObject> l1 = KeyValuesObject.Translate(s1);
            IEnumerable<KeyValuesObject> l2 = KeyValuesObject.Translate(s2);

            List<KeyValuesObject> newList = l2.Except(l1, new KeyValuesObjectEqualityComparer()).ToList();
            Assert.AreEqual(newList.Count, 1);
            Assert.AreEqual(newList[0].OriValue, s2[2]);
        }
    }
}