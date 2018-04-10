// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if (NET_35_UP || NETSTANDARD)
#endif
using System;
using System.Globalization;
#if NUNIT
using NUnit.Framework;
#elif XUNIT
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;

#endif

namespace XPatchLib.UnitTest.ForXml
{
    [TestFixture]
    public class TestCultureInfo : TestBase
    {
        private class C
        {
            public C()
            {
                Date = new DateTime(2018, 1, 1);
            }

            public CultureInfo Culture { get; set; }

            public DateTime Date { get; }

            public override string ToString()
            {
                return Date.ToString(Culture);
            }
        }

        [Test]
        public void SerializeCultrueClass()
        {
            C c1 = new C {Culture = new CultureInfo("zh-CN")};
            C c2 = new C {Culture = new CultureInfo("de-DE")};
            LogHelper.Debug(c1.ToString());
            LogHelper.Debug(c2.ToString());
            string context = DoSerializer_Divide(c1, c2);
            LogHelper.Debug(context);
            C c3 = DoSerializer_Combie(context, c1, true);
            //Assert.AreEqual(c3, c2);
            Assert.AreEqual(c2.ToString(), c3.ToString());
        }
    }
}