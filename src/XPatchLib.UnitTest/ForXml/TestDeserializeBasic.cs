// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if NUNIT
using NUnit.Framework;

#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest.ForXml
{
    [TestFixture]
    public class TestDeserializeBasic
    {
        [Test]
        public void BasicDeserializeCtorTest()
        {
            //var ser = new CombineBasic(new TypeExtend(typeof(string)));
            //ser.Setting.Mode = DateTimeSerializationMode.RoundtripKind;

            //ser = new CombineBasic(new TypeExtend(typeof(string)), DateTimeSerializationMode.Unspecified);
            //ser.Setting.Mode = DateTimeSerializationMode.Unspecified;
        }
    }
}