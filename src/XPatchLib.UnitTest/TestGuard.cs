using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;

namespace XPatchLib.UnitTest
{
    [TestFixture]
    public class TestGuard
    {
        #region Public Methods

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
                Guard.ArgumentNotNullOrEmpty(new string[] {}, "TestArgumentName");
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

        #endregion Public Methods
    }
}