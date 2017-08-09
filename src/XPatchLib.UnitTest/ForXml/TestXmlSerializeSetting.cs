// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using  XPatchLib;
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
    public class TestXmlSerializeSetting
    {
        [Test]
        [Description("测试 XmlSerializeSetting 对象的属性定义")]
        public void TestXmlSerializeSettingValue()
        {
            XmlSerializeSetting setting = new XmlSerializeSetting();
            Assert.AreEqual("Action", setting.ActionName);
            Assert.AreEqual(DateTimeSerializationMode.RoundtripKind, setting.Mode);
            Assert.IsFalse(setting.SerializeDefalutValue);
        }

        [Test]
        [Description("测试 XmlSerializeSetting 对象的属性变更后，是否播发事件")]
        public void TestXmlSerializeSettingValueChangedNotifyPropertyChanged()
        {
            XmlSerializeSetting setting = new XmlSerializeSetting();

#if (NETSTANDARD_1_0 || NETSTANDARD_1_1 || NETSTANDARD_1_2 || NETSTANDARD_1_3 || NETSTANDARD_1_4)
            PropertyInfo[] propertyInfos = setting.GetType()
                .GetProperties(XPatchLib.BindingFlags.Instance | XPatchLib.BindingFlags.GetProperty |
                               XPatchLib.BindingFlags.Public |
                               XPatchLib.BindingFlags.SetProperty);
#else
            PropertyInfo[] propertyInfos = setting.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.GetProperty |
                               BindingFlags.Public |
                               BindingFlags.SetProperty);
#endif

            List<string> changedProNames = new List<string>();

            setting.PropertyChanged +=
                delegate(object sender, System.ComponentModel.PropertyChangedEventArgs e) { changedProNames.Add(e.PropertyName); };

            setting.ActionName = "123";
            setting.Mode = DateTimeSerializationMode.Local;
            setting.SerializeDefalutValue = true;
            setting.MemberType = SerializeMemberType.All;
            setting.Modifier = SerializeMemberModifier.NonPublic;
#if NET || NETSTANDARD_2_0_UP
            setting.EnableOnDeserializedAttribute = false;
            setting.EnableOnDeserializingAttribute = false;
            setting.EnableOnSerializedAttribute = false;
            setting.EnableOnSerializingAttribute = false;
#endif

            Assert.AreEqual(propertyInfos.Length, changedProNames.Count);
            foreach (var VARIABLE in propertyInfos)
                if (!changedProNames.Contains(VARIABLE.Name))
                    Assert.Fail();
            Assert.AreEqual(SerializeMemberType.All, setting.MemberType);
            Assert.AreEqual("123", setting.ActionName);
            Assert.AreEqual(DateTimeSerializationMode.Local, setting.Mode);
            Assert.AreEqual(SerializeMemberModifier.NonPublic, setting.Modifier);
            Assert.IsTrue(setting.SerializeDefalutValue);
#if NET || NETSTANDARD_2_0_UP
            Assert.IsFalse(setting.EnableOnSerializedAttribute);
            Assert.IsFalse(setting.EnableOnDeserializingAttribute);
            Assert.IsFalse(setting.EnableOnSerializedAttribute);
            Assert.IsFalse(setting.EnableOnSerializingAttribute);
#endif
        }
    }

#if (NETSTANDARD && !NETSTANDARD_2_0_UP)
    internal static class TypeExtension
    {
#if (NETSTANDARD_1_0 || NETSTANDARD_1_1 || NETSTANDARD_1_2 || NETSTANDARD_1_3 || NETSTANDARD_1_4)
        internal static PropertyInfo[] GetProperties(this Type type, XPatchLib.BindingFlags bindingFlags)
        {
            IList<PropertyInfo> properties = (bindingFlags.HasFlag(XPatchLib.BindingFlags.DeclaredOnly))
                ? type.GetTypeInfo().DeclaredProperties.ToArray()
                : type.GetTypeInfo().GetPropertiesRecursive().ToArray();
#else
        internal static PropertyInfo[] GetProperties(this Type type, System.Reflection.BindingFlags bindingFlags)
        {
            IList<PropertyInfo> properties = (bindingFlags.HasFlag(System.Reflection.BindingFlags.DeclaredOnly))
                ? type.GetTypeInfo().DeclaredProperties.ToArray()
                : type.GetTypeInfo().GetPropertiesRecursive().ToArray();
#endif

            return properties.Where(p => XPatchLib.TypeExtension.TestAccessibility(p, bindingFlags)).ToArray();
        }

        private static IList<PropertyInfo> GetPropertiesRecursive(this TypeInfo type)
        {
            TypeInfo t = type;
            IList<PropertyInfo> properties = new List<PropertyInfo>();
            while (t != null)
            {
                foreach (PropertyInfo member in t.DeclaredProperties)
                {
                    if (!properties.Any(p => p.Name == member.Name))
                    {
                        properties.Add(member);
                    }
                }
                t = (t.BaseType != null) ? t.BaseType.GetTypeInfo() : null;
            }

            return properties;
        }
    }
#endif
}