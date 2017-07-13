// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using XPatchLib.UnitTest.TestClass;
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
    public class TestMulitPrimaryKeyClass:TestBase
    {
        [Test]
        public void TestMulitPrimaryKeyClassListRemoveDivideAndCombine()
        {
            var oriList = new List<MulitPrimaryKeyClass>();
            var revList = new List<MulitPrimaryKeyClass>();

            oriList.Add(new MulitPrimaryKeyClass {Name = "Name1", Id = 1});
            oriList.Add(new MulitPrimaryKeyClass {Name = "Name2", Id = 2});
            oriList.Add(new MulitPrimaryKeyClass {Name = "Name3", Id = 3});
            oriList.Add(new MulitPrimaryKeyClass {Name = "Name4", Id = 4});

            revList.Add(new MulitPrimaryKeyClass {Name = "Name1", Id = 1});
            revList.Add(new MulitPrimaryKeyClass {Name = "Name2", Id = 2});

            var changedContext = @"<" + ReflectionUtils.GetTypeFriendlyName(typeof(List<MulitPrimaryKeyClass>)) + @">
  <MulitPrimaryKeyClass Action=""Remove"" Id=""3"" Name=""Name3"" />
  <MulitPrimaryKeyClass Action=""Remove"" Id=""4"" Name=""Name4"" />
</" + ReflectionUtils.GetTypeFriendlyName(typeof(List<MulitPrimaryKeyClass>)) + @">";

            DoAssert(typeof(List<MulitPrimaryKeyClass>), changedContext, oriList, revList, true);
            DoAssert(typeof(List<MulitPrimaryKeyClass>), changedContext, oriList, revList, false);
        }
    }
}