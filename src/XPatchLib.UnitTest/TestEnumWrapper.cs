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
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture, "类型 ' {0} ' 不是枚举类型。\r\n参数名: pType",
                        typeof(AuthorClass).FullName), ex.Message);
                Assert.AreEqual("pType", ex.ParamName);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出 ArgumentException 异常。");
            }
        }
    }
}