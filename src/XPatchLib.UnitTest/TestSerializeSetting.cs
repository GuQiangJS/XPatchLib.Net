// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
#if NUNIT
using NUnit.Framework;

#elif XUNIT
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;

#endif

namespace XPatchLib.UnitTest
{
    [TestFixture]
    public class TestSerializeSetting
    {
        [Test]
        public void TestClone()
        {
            XmlSerializeSetting c = new XmlSerializeSetting();
            c.IgnoreAttributeType = typeof(string);
            c.ActionName = new Random().Next(Int32.MinValue, Int32.MaxValue).ToString();
#if NET_40_UP || NETSTANDARD_2_0_UP
            c.AssemblyQualifiedName = new Random().Next(Int32.MinValue, Int32.MaxValue).ToString();
#endif
#if NET || NETSTANDARD_2_0_UP
            c.EnableOnDeserializedAttribute = !c.EnableOnDeserializedAttribute;
            c.EnableOnDeserializingAttribute = !c.EnableOnDeserializingAttribute;
            c.EnableOnSerializedAttribute = !c.EnableOnSerializedAttribute;
            c.EnableOnSerializingAttribute = !c.EnableOnSerializingAttribute;
#endif
            foreach (string name in Enum.GetNames(typeof(SerializeMemberType)))
            {
                SerializeMemberType t = (SerializeMemberType) Enum.Parse(typeof(SerializeMemberType), name);
                if (c.MemberType != t)
                    c.MemberType = t;
            }
            foreach (string name in Enum.GetNames(typeof(DateTimeSerializationMode)))
            {
                DateTimeSerializationMode t =
                    (DateTimeSerializationMode) Enum.Parse(typeof(DateTimeSerializationMode), name);
                if (c.Mode != t)
                    c.Mode = t;
            }
            foreach (string name in Enum.GetNames(typeof(SerializeMemberModifier)))
            {
                SerializeMemberModifier t = (SerializeMemberModifier) Enum.Parse(typeof(SerializeMemberModifier), name);
                if (c.Modifier != t)
                    c.Modifier = t;
            }
            c.SerializeDefalutValue = !c.SerializeDefalutValue;
            c.ActionName = new Random().Next(Int32.MinValue, Int32.MaxValue).ToString();

            XmlSerializeSetting c_new = c.Clone() as XmlSerializeSetting;
            PropertyInfo[] pis = typeof(XmlSerializeSetting).GetProperties();
            foreach (PropertyInfo pi in pis)
                Assert.AreEqual(pi.GetValue(c,null), pi.GetValue(c_new, null));
        }

        [Test]
        public void TestXmlSerializeSetting()
        {
            XmlSerializeSetting c = new XmlSerializeSetting();
            Assert.AreEqual(SerializeMemberType.Property, c.MemberType);

            c.MemberType = SerializeMemberType.Field;
            Assert.AreEqual(SerializeMemberType.Field, c.MemberType);

            c.MemberType = SerializeMemberType.Property;
            Assert.AreEqual(SerializeMemberType.Property, c.MemberType);

            c.MemberType = SerializeMemberType.All;
            Assert.AreEqual(SerializeMemberType.All, c.MemberType);
            Assert.IsTrue((SerializeMemberType.Property & c.MemberType) == SerializeMemberType.Property);
            Assert.IsTrue((SerializeMemberType.Field & c.MemberType) == SerializeMemberType.Field);
        }
    }
}