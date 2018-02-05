
using System.IO;
using System.Text;
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
    public class TestIgnoreAttributeType:TestBase
    {
        [Test]
        [Description("测试将原本类型定义中标记的XmlIgnoreAttribute设为无效")]
        public void TestSetIgnoreAttributeToNull()
        {
            var c1 = new XmlIgnoreClass { A = "A", B = "B" };
            var c2 = new XmlIgnoreClass { A = "C", B = "D" };
            var changedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<XmlIgnoreClass>
  <A>C</A>
  <B>D</B>
</XmlIgnoreClass>";

            StringBuilder result=new StringBuilder();
            Serializer serializer = new Serializer(typeof(XmlIgnoreClass));
            using (TestBase.ExtentedStringWriter writer = new TestBase.ExtentedStringWriter(result,Encoding.UTF8))
            {
                using (XmlTextWriter xmlWriter=new XmlTextWriter(writer))
                {
                    //在本次序列化过程中，类型定义中的XmlIgnoreAttribute无效。
                    xmlWriter.Setting.IgnoreAttributeType = null;
                    serializer.Divide(xmlWriter, c1,c2);
                }
            }
            Assert.AreEqual(changedContext, result.ToString());
        }

        [Test]
        [Description("测试将原本类型定义中标记的XmlIgnoreAttribute设为其他特性")]
        public void TestSetIgnoreAttributeToOther()
        {
            var c1 = new XmlIgnoreClass { A = "A", B = "B" };
            var c2 = new XmlIgnoreClass { A = "C", B = "D" };
            var changedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<XmlIgnoreClass>
  <A>C</A>
</XmlIgnoreClass>";

            StringBuilder result = new StringBuilder();
            Serializer serializer = new Serializer(typeof(XmlIgnoreClass));
            using (TestBase.ExtentedStringWriter writer = new TestBase.ExtentedStringWriter(result, Encoding.UTF8))
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(writer))
                {
                    //在本次序列化过程中，使用 XPatchLibXmlIgnoreAttribute 作为忽略特性，而不是用默认的 XmlIgnoreAttribute
                    xmlWriter.Setting.IgnoreAttributeType = typeof(XPatchLibXmlIgnoreAttribute);
                    serializer.Divide(xmlWriter, c1, c2);
                }
            }
            Assert.AreEqual(changedContext, result.ToString());
        }
    }
}
