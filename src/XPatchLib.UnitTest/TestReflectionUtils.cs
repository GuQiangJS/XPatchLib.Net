// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class TestReflectionUtils:TestBase
    {
        public void GetNullTypeFriendlyNameTest()
        {
            Assert.Throws<ArgumentNullException>(() => { ReflectionUtils.GetTypeFriendlyName(null); });
        }
        

        private class TestReflectionClass
        {
            public string B1;

            private string B2;
            protected string B3;
            internal string B4;
            public string A1 { get; set; }
            private string A2 { get; set; }
            protected string A3 { get; set; }
            internal string A4 { get; set; }
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
            MemberWrapper[] member = null;
            member = ReflectionUtils.GetFieldsToBeSerialized(DefaultXmlSerializeSetting, typeof(TestReflectionClass),
                null);
            Assert.AreEqual(1, member.Length);
            Assert.AreEqual(nameof(TestReflectionClass.A1), member[0].Name);
            Assert.IsTrue(member[0].IsProperty);

            member = ReflectionUtils.GetFieldsToBeSerialized(
                new XmlSerializeSetting() {MemberType = SerializeMemberType.Property}, typeof(TestReflectionClass),
                null);
            Assert.AreEqual(1, member.Length);
            Assert.AreEqual(nameof(TestReflectionClass.A1), member[0].Name);
            Assert.IsTrue(member[0].IsProperty);

            member = ReflectionUtils.GetFieldsToBeSerialized(
                new XmlSerializeSetting { MemberType = SerializeMemberType.Field}, typeof(TestReflectionClass),
                null);
            Assert.AreEqual(1, member.Length);
            Assert.AreEqual(nameof(TestReflectionClass.B1), member[0].Name);
            Assert.IsFalse(member[0].IsProperty);

            member = ReflectionUtils.GetFieldsToBeSerialized(
                new XmlSerializeSetting { Modifier = SerializeMemberModifier.Private}, typeof(TestReflectionClass),
                null);
            Assert.AreEqual(1, member.Length);
            Assert.AreEqual("A2", member[0].Name);
            Assert.IsTrue(member[0].IsProperty);

            member = ReflectionUtils.GetFieldsToBeSerialized(
                new XmlSerializeSetting
                {
                    Modifier = SerializeMemberModifier.Private,
                    MemberType = SerializeMemberType.Property
                }, typeof(TestReflectionClass), null);
            Assert.AreEqual(1, member.Length);
            Assert.AreEqual("A2", member[0].Name);
            Assert.IsTrue(member[0].IsProperty);

            member = ReflectionUtils.GetFieldsToBeSerialized(
                new XmlSerializeSetting { Modifier = SerializeMemberModifier.NonPublic}, typeof(TestReflectionClass),
                null);
            Assert.AreEqual(3, member.Length);
            Assert.AreEqual("A2", member[0].Name);
            Assert.AreEqual("A3", member[1].Name);
            Assert.AreEqual("A4", member[2].Name);
            Assert.IsTrue(member[0].IsProperty);
            Assert.IsTrue(member[1].IsProperty);
            Assert.IsTrue(member[2].IsProperty);

            member = ReflectionUtils.GetFieldsToBeSerialized(
                new XmlSerializeSetting
                {
                    MemberType = SerializeMemberType.All,
                    Modifier = SerializeMemberModifier.NonPublic
                }, typeof(TestReflectionClass),
                null);
            Assert.AreEqual(6, member.Length);
            Assert.AreEqual("A2", member[0].Name);
            Assert.AreEqual("A3", member[1].Name);
            Assert.AreEqual("A4", member[2].Name);
            Assert.AreEqual("B2", member[3].Name);
            Assert.AreEqual("B3", member[4].Name);
            Assert.AreEqual("B4", member[5].Name);
            Assert.IsTrue(member[0].IsProperty);
            Assert.IsTrue(member[1].IsProperty);
            Assert.IsTrue(member[2].IsProperty);
            Assert.IsFalse(member[3].IsProperty);
            Assert.IsFalse(member[4].IsProperty);
            Assert.IsFalse(member[5].IsProperty);
        }
    }
}