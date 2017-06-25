// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
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
    public class TestGuard
    {
        [Test]
        public void TestArgumentNotNull()
        {
            try
            {
                Guard.ArgumentNotNull(null, "TestArgumentName");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("TestArgumentName", ex.ParamName);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出 ArgumentNullException.");
            }
        }

        [Test]
        public void TestArgumentNotNullOrEmptyForArray()
        {
            try
            {
                Guard.ArgumentNotNullOrEmpty(new string[] { }, "TestArgumentName");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("TestArgumentName", ex.ParamName);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出 ArgumentNullException.");
            }
        }

        [Test]
        public void TestArgumentNotNullOrEmptyForString()
        {
            try
            {
                Guard.ArgumentNotNullOrEmpty("", "TestArgumentName");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("TestArgumentName", ex.ParamName);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出 ArgumentNullException.");
            }
            try
            {
                Guard.ArgumentNotNullOrEmpty(string.Empty, "TestArgumentName");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("TestArgumentName", ex.ParamName);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出 ArgumentNullException.");
            }
        }
    }
}