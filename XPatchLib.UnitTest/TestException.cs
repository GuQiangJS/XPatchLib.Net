using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestException
    {
        #region Public Methods

        [TestMethod]
        public void TestAttributeMissException()
        {
            Type type = typeof(AuthorClass);
            string proName = "Comments";
            AttributeMissException attributeMissException = new AttributeMissException(type, proName);
            Assert.AreEqual(attributeMissException.ErrorType, type);
            Assert.AreEqual(attributeMissException.AttributeName, proName);
            string msg = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Exp_String_AttributeMiss, type,
                proName);
            Assert.AreEqual(attributeMissException.Message, msg);
        }

        #endregion Public Methods
    }
}