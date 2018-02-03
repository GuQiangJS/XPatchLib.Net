// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using XPatchLib.UnitTest.TestClass;
#if NUNIT
using NUnit.Framework;

#elif XUNIT
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;

#endif

namespace XPatchLib.UnitTest.ForXml
{
    [TestFixture]
    public class TestIncompleteXml : TestBase
    {
        [Test]
        [Description("测试反序列化不完整的XML，是否会抛出XmlException异常")]
        public void TestDeserializeIncompleteXml()
        {
            Serializer serializer = new Serializer(typeof(BookClassCollection));
            BookClassCollection books = new BookClassCollection();
            books.Add(new BookClass {Comments = "2", Name = "3", Price = 0.5d, PublishYear = 2018});
            books.Add(new BookClass {Comments = "4", Name = "5", Price = 5.5d, PublishYear = 2012});
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (XmlTextWriter writer = new XmlTextWriter(sw))
                {
                    serializer.Divide(writer, null, books);
                }
            }
            LogHelper.Debug(sb.ToString());
            using (StringReader sr = new StringReader(sb.ToString(0, sb.Length - 10)))
            {
                using (XmlTextReader reader = new XmlTextReader(sr))
                {
                    Assert.Throws<XmlException>(() => serializer.Combine(reader, null));
                }
            }
        }
    }
}