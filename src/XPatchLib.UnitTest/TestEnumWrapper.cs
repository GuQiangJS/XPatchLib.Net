using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestEnumWrapper
    {
        #region Public Methods

        [TestMethod]
        public void TestEnumWrapperConstruction()
        {
            try
            {
                new EnumWrapper(typeof(AuthorClass));
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture, "类型 {0} 不是枚举类型。\r\n参数名: pType",
                        typeof(AuthorClass).FullName), ex.Message);
                Assert.AreEqual("pType", ex.ParamName);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出 ArgumentException 异常。");
            }
        }

        #endregion Public Methods
    }
}