using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest.ForXml
{
    [TestClass]
    public class TestXmlSerializeSetting
    {
        [TestMethod]
        [Description("测试 XmlSerializeSetting 对象的属性定义")]
        public void TestXmlSerializeSettingValue()
        {
            XmlSerializeSetting setting = new XmlSerializeSetting();
            Assert.AreEqual("Action", setting.ActionName);
            Assert.AreEqual(DateTimeSerializationMode.RoundtripKind, setting.Mode);
            Assert.IsFalse(setting.SerializeDefalutValue);
        }

        [TestMethod]
        [Description("测试 XmlSerializeSetting 对象的属性变更后，是否播发事件")]
        public void TestXmlSerializeSettingValueChangedNotifyPropertyChanged()
        {
            XmlSerializeSetting setting = new XmlSerializeSetting();

            PropertyInfo[] propertyInfos = setting.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public |
                               BindingFlags.SetProperty);

            List<string> changedProNames=new List<string>();

            setting.PropertyChanged += delegate(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                changedProNames.Add(e.PropertyName);
            };
            
            setting.ActionName = "123";
            setting.Mode=DateTimeSerializationMode.Local;
            setting.SerializeDefalutValue = true;

            Assert.AreEqual(propertyInfos.Length, changedProNames.Count);
            foreach (var VARIABLE in propertyInfos)
            {
                if (!changedProNames.Contains(VARIABLE.Name))
                {
                    Assert.Fail();
                }
            }
            Assert.AreEqual("123", setting.ActionName);
            Assert.AreEqual(DateTimeSerializationMode.Local, setting.Mode);
            Assert.IsTrue(setting.SerializeDefalutValue);
        }
    }
}
