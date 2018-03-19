// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if NUNIT
using NUnit.Framework;

#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest.Issues
{
    [TestFixture]
    public class Issue1561 : TestBase
    {
        public class Data
        {
            public decimal? Value { get; set; }
        }

        [Test]
        public void Test()
        {
            Data data = new Data
            {
                Value = 1.1m
            };
            string output = DoSerializer_Divide(null, data);
            LogHelper.Debug(output);

            Data d = DoSerializer_Combie<Data>(output, null, true);
            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Data>
  <Value>1.1</Value>
</Data>", output);

            Assert.IsNotNull(d);
            Assert.IsTrue(d.Value.HasValue);
            Assert.AreEqual(data.Value.Value, d.Value.Value);
        }
    }
}