using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest
{
    [TestFixture]
    public class TestException
    {
        #region Public Methods

        [Test]
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

        [Test]
        public void TestPrimaryKeyException()
        {
            Type type = typeof(ErrorPrimaryKeyDefineClass);

            bool errorCatched = false;
            try
            {
                Serializer serializer = new Serializer(type);
                using (var stream = new MemoryStream())
                {
                    using (var writer = ForXml.TestHelper.CreateWriter(stream, ForXml.TestHelper.DocumentSetting))
                    {
                        ErrorPrimaryKeyDefineClass c1 = new ErrorPrimaryKeyDefineClass();
                        ErrorPrimaryKeyDefineClass c2 = new ErrorPrimaryKeyDefineClass();
                        serializer.Divide(writer, c1, c2);
                    }
                }
            }
            catch (PrimaryKeyException ex)
            {
                Assert.AreEqual(ex.ErrorType, type);
                Assert.AreEqual(ex.PrimaryKeyName, "Author");
                string msg = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Exp_String_PrimaryKey,
                    type.FullName,
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