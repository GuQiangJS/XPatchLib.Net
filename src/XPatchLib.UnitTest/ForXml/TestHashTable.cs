// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
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
    public class TestHashTable:TestBase
    {
#if (NET || NETSTANDARD_2_0_UP)
        [Test]
        public void TestHashTableDivideWithoutPrimaryKeyAttribute()
        {
            var table = new Hashtable();

            table.Add("aaa", "ccc");
            table.Add(123, "ddd");

            var serializer = new Serializer(typeof(Hashtable));

            var context = string.Empty;
            try
            {
                DoAssert(typeof(Hashtable), context, null, table, true);
                Assert.Fail("未能引发 AttributeMissException 异常。");
            }
            catch (AttributeMissException ex)
            {
                StringAssert.AreEqualIgnoringCase(typeof(PrimaryKeyAttribute).Name, ex.AttributeName);
                //Hashtable类型作为一种集合类型，在处理子元素时应该对子元素的类型标记 PrimaryKeyAttribute 。
                //但是由于Key值类型为Object，所以永远找不到PrimaryKeyAttribute
                StringAssert.AreEqualIgnoringCase(
                    string.Format(ResourceHelper.GetResourceString(LocalizationRes.Exp_String_AttributeMiss),
                        typeof(object), typeof(PrimaryKeyAttribute).Name), ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("未能引发 AttributeMissException 异常。");
            }
        }
#endif
    }
}