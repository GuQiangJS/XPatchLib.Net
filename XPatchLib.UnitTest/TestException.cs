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
            bool errorCatched = false;
            AuthorClass author = new AuthorClass();
            string attrName = typeof(PrimaryKeyAttribute).Name;
            try
            {
                new KeyValuesObject(author);
            }
            catch (AttributeMissException ex)
            {
                Assert.AreEqual(ex.ErrorType, author.GetType());
                Assert.AreEqual(ex.AttributeName, attrName);
                string msg = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Exp_String_AttributeMiss, author.GetType(),
                    attrName);
                Assert.AreEqual(ex.Message, msg);
                errorCatched = true;
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            Assert.IsTrue(errorCatched);

        }

        [TestMethod]
        public void TestPrimaryKeyException()
        {
            Type type = typeof(ErrorPrimaryKeyDefineClass);

            bool errorCatched = false;
            try
            {
                new Serializer(type);
            }
            catch (PrimaryKeyException ex)
            {
                Assert.AreEqual(ex.ErrorType, type);
                Assert.AreEqual(ex.PrimaryKeyName, "Author");
                string msg = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Exp_String_PrimaryKey, type.FullName,
                    "Author");
                Assert.AreEqual(ex.Message, msg);
                errorCatched = true;
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            Assert.IsTrue(errorCatched);
        }

        #endregion Public Methods
    }
}