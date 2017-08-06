// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
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
    public class TestReflectionUtils
    {
        public void GetNullTypeFriendlyNameTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ReflectionUtils.GetTypeFriendlyName(null);
            });
        }

        [Test]
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

        [Test]
        public void IsBasicTypeInCacheTest()
        {
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(string)));
            Assert.IsTrue(ReflectionUtils.IsBasicType(typeof(string)));
        }

        [Test]
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

        [Test]
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

        [Test]
        [Description("测试查找参与序列化的字段或属性")]
        public void TestGetSetProperty()
        {
            MemberWrapper[] publicMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassPublicPropertyGetSet), null);
            Assert.AreEqual(1, publicMember.Length);
            Assert.AreEqual(nameof(ClassPublicPropertyGetSet.A), publicMember[0].Name);
            Assert.IsNotNull(publicMember[0].MemberInfo as PropertyInfo);
            Assert.IsTrue(publicMember[0].IsProperty);

            MemberWrapper[] privateMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassPrivatePropertyGetSet), null);
            Assert.AreEqual(0, privateMember.Length);

            MemberWrapper[] protectedMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassProtectedPropertyGetSet), null);
            Assert.AreEqual(0, protectedMember.Length);

            MemberWrapper[] internalMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassInternalPropertyGetSet), null);
            Assert.AreEqual(0, internalMember.Length);
        }

        [Test]
        [Description("测试查找参与序列化的字段或属性")]
        public void TestOnlyGetProperty()
        {
            MemberWrapper[] publicMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassPublicPropertyGetOnly), null);
            Assert.AreEqual(0, publicMember.Length);

            MemberWrapper[] privateMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassPrivatePropertyGetOnly), null);
            Assert.AreEqual(0, privateMember.Length);

            MemberWrapper[] protectedMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassProtectedPropertyGetOnly), null);
            Assert.AreEqual(0, protectedMember.Length);

            MemberWrapper[] internalMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassInternalPropertyGetOnly), null);
            Assert.AreEqual(0, internalMember.Length);
        }

        [Test]
        [Description("测试查找参与序列化的字段或属性")]
        public void TestOnlySetProperty()
        {
            MemberWrapper[] publicMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassPublicPropertySetOnly), null);
            Assert.AreEqual(0, publicMember.Length);

            MemberWrapper[] privateMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassPrivatePropertySetOnly), null);
            Assert.AreEqual(0, privateMember.Length);

            MemberWrapper[] protectedMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassProtectedPropertySetOnly), null);
            Assert.AreEqual(0, protectedMember.Length);

            MemberWrapper[] internalMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassInternalPropertySetOnly), null);
            Assert.AreEqual(0, internalMember.Length);
        }
        
        [Test]
        [Description("测试查找参与序列化的字段或属性")]
        public void TestField()
        {
            MemberWrapper[] publicMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassPublicField), null);
            Assert.AreEqual(1, publicMember.Length);
            Assert.AreEqual(nameof(ClassPublicPropertyGetSet.A), publicMember[0].Name);
            Assert.IsNotNull(publicMember[0].MemberInfo as FieldInfo);
            Assert.IsFalse(publicMember[0].IsProperty);

            MemberWrapper[] privateMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassPrivateField), null);
            Assert.AreEqual(0, privateMember.Length);

            MemberWrapper[] protectedMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassProtectedField), null);
            Assert.AreEqual(0, protectedMember.Length);

            MemberWrapper[] internalMember =
                ReflectionUtils.GetFieldsToBeSerialized(typeof(ClassInternalField), null);
            Assert.AreEqual(0, internalMember.Length);
        }

        public class ClassPublicPropertyGetSet
        {
            public string A { get; set; }
        }

        public class ClassPrivatePropertyGetSet
        {
            private string A { get; set; }
        }

        public class ClassProtectedPropertyGetSet
        {
            protected string A { get; set; }
        }

        public class ClassInternalPropertyGetSet
        {
            internal string A { get; set; }
        }

        public class ClassPublicPropertyGetOnly
        {
            public string A { get; }
        }
        public class ClassPrivatePropertyGetOnly
        {
            private string A { get; }
        }
        public class ClassProtectedPropertyGetOnly
        {
            protected string A { get; }
        }
        public class ClassInternalPropertyGetOnly
        {
            internal string A { get; }
        }


        public class ClassPublicPropertySetOnly
        {
            public string A { get; }
        }
        public class ClassPrivatePropertySetOnly
        {
            private string A { get; }
        }
        public class ClassProtectedPropertySetOnly
        {
            protected string A { get; }
        }
        public class ClassInternalPropertySetOnly
        {
            internal string A { get; }
        }


        public class ClassPublicField
        {
            public string A;
        }
        public class ClassPrivateField
        {
            private string A;
        }
        public class ClassProtectedField
        {
            protected string A;
        }
        public class ClassInternalField
        {
            internal string A;
        }
    }
}