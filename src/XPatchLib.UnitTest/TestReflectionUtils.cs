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

            Assert.AreEqual("Nullable_DateTime", ReflectionUtils.GetTypeFriendlyName(typeof(DateTime?)));
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
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(short)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(int)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(long)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(ushort)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(uint)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(ulong)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(double)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(decimal)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(float)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(char)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(bool)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(DateTime)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(byte)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(sbyte)));

            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(sbyte?)));

            Assert.IsFalse(ReflectionUtils.IsBasicType(typeof(BookClass)));

            Assert.IsFalse(ReflectionUtils.IsBasicType(typeof(List<BookClass>)));
        }

        [TestMethod]
        public void IsNullableTest()
        {
            Type valueType = null;
            Assert.IsFalse(ReflectionUtils.IsNullable(typeof(BookClass), out valueType));
            Assert.IsNull(valueType);

            Assert.IsFalse(ReflectionUtils.IsNullable(typeof(string), out valueType));
            Assert.IsNull(valueType);

            Assert.IsTrue(ReflectionUtils.IsNullable(typeof(DateTime?), out valueType));
            Assert.IsNotNull(valueType);
            Assert.AreEqual(typeof(DateTime), valueType);
        }

        #endregion Public Methods
    }
}