// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using XPatchLib.UnitTest.TestClass;
#if NUNIT
using NUnit.Framework;
#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest
{
    [TestFixture]
    public class TestEnumWrapper
    {
        [Test]
        public void TestEnumWrapperConstruction()
        {
            try
            {
                new EnumWrapper(typeof(AuthorClass));
                Assert.Fail("未能抛出 ArgumentException 异常。");
            }
            catch (Exception ex)
            {
                ArgumentException e=ex as ArgumentException;
                if (e != null)
                {
                    ArgumentException e1 =
                        new ArgumentException(
                            string.Format(CultureInfo.InvariantCulture, "类型 ' {0} ' 不是枚举类型。",
                                typeof(AuthorClass).FullName), "pType");
                    Assert.AreEqual(e.Message, e1.Message);
                    Assert.AreEqual("pType", e.ParamName);
                }
                else
                {
                    Assert.Fail("未能抛出 ArgumentException 异常。");
                }
            }
        }
    }
}