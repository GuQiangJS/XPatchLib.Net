// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
}