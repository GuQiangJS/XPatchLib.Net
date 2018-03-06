// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Text.RegularExpressions;
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
    public class TestRegex:TestBase
    {
        void AssertEqual(Regex r1, Regex r2)
        {
            Assert.AreEqual(r1.ToString(), r2.ToString());
            Assert.AreEqual(r1.Options, r2.Options);
#if NET_45_UP || NETSTANDARD
            Assert.AreEqual(r1.MatchTimeout, r2.MatchTimeout);
#endif
        }

        [Test]
        [Description("测试只有pattern的Regex，是否能正常产生和合并增量，并且默认不写入默认值")]
        public void TestRegex_Empty()
        {
            Regex r1 = new Regex("abc");
            string output = DoSerializer_Divide(null, r1);
            LogHelper.Debug(output);

            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Regex>
  <pattern>abc</pattern>
</Regex>", output);

            Regex r2 = DoSerializer_Combie<Regex>(output, null, true);
            AssertEqual(r1, r2);
        }

        [Test]
        [Description("测试是否能正常产生和合并增量，并且默认不写入默认值")]
        public void TestRegex_Empty1()
        {
            Regex r1 = new Regex("abc", RegexOptions.Compiled);
            string output = DoSerializer_Divide(null, r1);
            LogHelper.Debug(output);

            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Regex>
  <Options>8</Options>
  <pattern>abc</pattern>
</Regex>", output);

            Regex r2 = DoSerializer_Combie<Regex>(output, null, true);
            AssertEqual(r1, r2);
        }

#if NET_45_UP || NETSTANDARD
        [Test]
        [Description("测试是否能正常产生和合并增量，并且默认不写入默认值")]
        public void TestRegex_Empty2()
        {
            Regex r1 = new Regex("abc", RegexOptions.Compiled, new TimeSpan(123l));
            string output = DoSerializer_Divide(null, r1);
            LogHelper.Debug(output);

            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Regex>
  <MatchTimeout>00:00:00.0000123</MatchTimeout>
  <Options>8</Options>
  <pattern>abc</pattern>
</Regex>", output);

            Regex r2 = DoSerializer_Combie<Regex>(output, null, true);
            AssertEqual(r1, r2);
        }

        [Test]
        public void TestRegex4()
        {
            Regex r1 = new Regex("abc");
            Regex r2 = new Regex("abc", RegexOptions.Compiled, new TimeSpan(123l));
            string output = DoSerializer_Divide(r1, r2);
            LogHelper.Debug(output);

            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Regex>
  <MatchTimeout>00:00:00.0000123</MatchTimeout>
  <Options>8</Options>
</Regex>", output);

            Regex r3 = DoSerializer_Combie<Regex>(output, r1, true);
            AssertEqual(r2, r3);
        }
#endif
    }
}