// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if (NET_35_UP || NETSTANDARD)
using System.Xml.Linq;
#endif
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
#if NUNIT
using NUnit.Framework;

#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest
{
    internal static class TestHelper
    {

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
        
    }
}