using System;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestGuard
    {
        #region Public Methods

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void TestFileNotFound()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
            try
            {
                Guard.FileNotFound(tempFile);
            }
            catch (FileNotFoundException ex)
            {
                Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "文件 {0} 不存在。", tempFile), ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出 ArgumentNullException.");
            }
        }

        #endregion Public Methods
    }
}