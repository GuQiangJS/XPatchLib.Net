// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

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
    public class TestSerializeSetting
    {
        [Test]
        public void TestXmlSerializeSetting()
        {
            XmlSerializeSetting c = new XmlSerializeSetting();
            Assert.AreEqual(SerializeMemberType.Property, c.MemberType);

            c.MemberType = SerializeMemberType.Field;
            Assert.AreEqual(SerializeMemberType.Field, c.MemberType);

            c.MemberType = SerializeMemberType.Property;
            Assert.AreEqual(SerializeMemberType.Property, c.MemberType);

            c.MemberType=SerializeMemberType.All;
            Assert.AreEqual(SerializeMemberType.All, c.MemberType);
            Assert.IsTrue((SerializeMemberType.Property & c.MemberType) == SerializeMemberType.Property);
            Assert.IsTrue((SerializeMemberType.Field & c.MemberType) == SerializeMemberType.Field);
        }
    }
}