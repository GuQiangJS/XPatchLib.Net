// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
#if (NET_35_UP || NETSTANDARD)
using System.Xml.Linq;
#endif
using NUnit.Framework;

namespace XPatchLib.UnitTest
{
    internal static class TestHelper
    {


        internal static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (var stremReader = new StreamReader(stream, Encoding.UTF8))
            {
                return stremReader.ReadToEnd();
            }
        }

        /// <summary>
        /// 读取文本，构建XElement或XmlDocument对象，调试时方便查看内容
        /// </summary>
        /// <param name="text"></param>
        internal static void DebugTest(string text)
        {
#if (NET_35 || NETSTANDARD)
            var changedEle = XElement.Load(new StringReader(text));
#else
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(new StringReader(text));
            var changedEle = xDoc.OuterXml;
            xDoc = null;
#endif
            changedEle = null;
        }

        /// <summary>
        /// 读取Stream，构建XElement或XmlDocument对象，调试时方便查看内容
        /// </summary>
        /// <param name="stream"></param>
        internal static void DebugTest(Stream stream)
        {
#if (NET_35 || NETSTANDARD)
            var changedEle = XElement.Load(new StreamReader(stream));
#else
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(new StreamReader(stream));
            var changedEle = xDoc.OuterXml;
            xDoc = null;
#endif
            changedEle = null;
            stream.Position = 0;
        }

        internal static Stream StringToStream(string str)
        {
            var strBytes = Encoding.UTF8.GetBytes(str);
            return new MemoryStream(strBytes);
        }

        private static bool IEnumeratorEquals(this IEnumerator aEnumerator, IEnumerator bEnumerator)
        {
            aEnumerator.Reset();
            while (aEnumerator.MoveNext())
            {
                var found = false;
                bEnumerator.Reset();
                while (bEnumerator.MoveNext())
                    if (aEnumerator.Current.Equals(bEnumerator.Current))
                    {
                        found = true;
                        break;
                    }
                if (!found)
                    return false;
            }
            return true;
        }

        internal static void PrivateAssertIEnumerable<T>(object A, object B, Type pType, string pAssert)
        {
            var aList = A as IEnumerable;
            var bList = B as IEnumerable;
            var aEnumerator = aList.GetEnumerator();
            var bEnumerator = bList.GetEnumerator();

            Assert.IsTrue(aEnumerator.IEnumeratorEquals(bEnumerator));
            Assert.IsTrue(bEnumerator.IEnumeratorEquals(aEnumerator));
        }

        internal static void PrivateAssertObject(object A, object B, string pAssert)
        {
            Assert.AreEqual(A, B, pAssert);
        }
    }

    internal static class AssertHelper
    {
        internal static void AreEqual(string context, Stream value,string message)
        {
            value.Position = 0;
            UnitTest.TestHelper.DebugTest(value);
            value.Position = 0;
            NUnit.Framework.Assert.AreEqual(context, UnitTest.TestHelper.StreamToString(value), message);
            value.Position = 0;
        }


    }
}