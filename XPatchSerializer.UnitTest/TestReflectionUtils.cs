using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestReflectionUtils
    {
        #region Public Methods

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNullTypeFriendlyNameTest()
        {
            Assert.AreEqual("String", ReflectionUtils.GetTypeFriendlyName(null));
        }

        [TestMethod]
        public void GetTypeFriendlyNameTest()
        {
            Assert.AreEqual("String", ReflectionUtils.GetTypeFriendlyName(typeof(string)));

            Assert.AreEqual("BookClass", ReflectionUtils.GetTypeFriendlyName(typeof(BookClass)));

            Assert.AreEqual("List_BookClass", ReflectionUtils.GetTypeFriendlyName(typeof(List<BookClass>)));

            Assert.AreEqual("Collection_BookClass", ReflectionUtils.GetTypeFriendlyName(typeof(Collection<BookClass>)));

            Assert.AreEqual("BookClassCollection", ReflectionUtils.GetTypeFriendlyName(typeof(BookClassCollection)));

            Assert.AreEqual("Array1OfBookClass", ReflectionUtils.GetTypeFriendlyName(typeof(BookClass[])));

            Assert.AreEqual("Nullable_DateTime", ReflectionUtils.GetTypeFriendlyName(typeof(Nullable<DateTime>)));
        }

        [TestMethod]
        public void IsBasicTypeInCacheTest()
        {
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(string)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(string)));
        }

        [TestMethod]
        public void IsBasicTypeTest()
        {
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(string)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(Guid)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(Int16)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(Int32)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(Int64)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(UInt16)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(UInt32)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(UInt64)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(Double)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(Decimal)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(Single)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(Char)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(Boolean)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(DateTime)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(Byte)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(SByte)));

            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(Nullable<SByte>)));

            Assert.IsFalse(ReflectionUtils.IsBasicType(typeof(BookClass)));

            Assert.IsFalse(ReflectionUtils.IsBasicType(typeof(List<BookClass>)));
        }

        [TestMethod]
        public void IsNullableTest()
        {
            Type valueType = null;
            Assert.IsFalse(ReflectionUtils.IsNullable(typeof(BookClass), out valueType));
            Assert.IsNull(valueType);

            Assert.IsFalse(ReflectionUtils.IsNullable(typeof(String), out valueType));
            Assert.IsNull(valueType);

            Assert.IsTrue(ReflectionUtils.IsNullable(typeof(Nullable<DateTime>), out valueType));
            Assert.IsNotNull(valueType);
            Assert.AreEqual(typeof(DateTime), valueType);
        }

        #endregion Public Methods
    }
}