// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Xml;
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
    public class TestSpecialCharacters:TestBase
    {
        [Test]
        public void TestDivideAndCombineSpecialCharacters()
        {
            AuthorClass authorClass1 = new AuthorClass();
            authorClass1.Name = "<>";
            authorClass1.Comments = "&'\"";
            string context = @"<?xml version=""1.0"" encoding=""utf-8""?>
<AuthorClass>
  <Comments>&amp;'""</Comments>
  <Name>&lt;&gt;</Name>
</AuthorClass>";

            DoAssert(typeof(AuthorClass), context, null, authorClass1, true);
            DoAssert(typeof(AuthorClass), context, null, authorClass1, false);
        }
    }
}