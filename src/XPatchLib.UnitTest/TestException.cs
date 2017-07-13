// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
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
    public class TestException:TestBase
    {
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
                string msg = string.Format(CultureInfo.CurrentCulture,
                    ResourceHelper.GetResourceString(LocalizationRes.Exp_String_AttributeMiss), author.GetType(),
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
            
            try
            {
                ErrorPrimaryKeyDefineClass c1 = new ErrorPrimaryKeyDefineClass();
                ErrorPrimaryKeyDefineClass c2 = new ErrorPrimaryKeyDefineClass();
                DoSerializer_Divide(c1, c2);
                Assert.Fail();
            }
            catch (PrimaryKeyException ex)
            {
                Assert.AreEqual(ex.ErrorType, type);
                Assert.AreEqual(ex.PrimaryKeyName, "Author");
                string msg = string.Format(CultureInfo.CurrentCulture,
                    ResourceHelper.GetResourceString(LocalizationRes.Exp_String_PrimaryKey),
                    type.FullName,
                    "Author");
                Assert.AreEqual(ex.Message, msg);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}