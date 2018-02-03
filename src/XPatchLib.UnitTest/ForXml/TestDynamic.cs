// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if NET_40_UP

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
#if NUNIT
using NUnit.Framework;

#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest
{
    [TestFixture]
    public class TestDynamic : TestBase
    {
        private string GetTypeAssembly(dynamic o)
        {
            if (o == null)
                return typeof(object).AssemblyQualifiedName;
            return o.GetType().AssemblyQualifiedName;
        }

        private string CreateNonDefaultValueContext(ISerializeSetting setting, dynamic o)
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestDynamicObject>
  <Text " + setting.AssemblyQualifiedName + @"=""" + GetTypeAssembly(o.Text) + @""">Text!</Text>
  <Int " + setting.AssemblyQualifiedName + @"=""" + GetTypeAssembly(o.Int) + @""">2147483647</Int>
</TestDynamicObject>";
        }

        private string CreateDefaultValueContext(ISerializeSetting setting, dynamic o)
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestDynamicObject>
  <ChildObject " + setting.AssemblyQualifiedName + @"=""" + typeof(DynamicChildObject).AssemblyQualifiedName + @""" Action=""SetNull"" />
  <Text " + setting.AssemblyQualifiedName + @"=""" + GetTypeAssembly(o.Text) + @""">Text!</Text>
  <IntDefault " + setting.AssemblyQualifiedName + @"=""" + GetTypeAssembly(o.Int) + @""">0</IntDefault>
  <NUllableIntDefault " + setting.AssemblyQualifiedName + @"=""" + GetTypeAssembly(o.NUllableIntDefault) +
                   @""" Action=""SetNull"" />
  <DynamicChildObject " + setting.AssemblyQualifiedName + @"=""" + GetTypeAssembly(o.DynamicChildObject) +
                   @""" Action=""SetNull"" />
  <Int " + setting.AssemblyQualifiedName + @"=""" + GetTypeAssembly(o.Int) + @""">2147483647</Int>
</TestDynamicObject>";
        }

        private dynamic DoTest(string context, ISerializeSetting setting = null)
        {
            dynamic o = CreateInstance();
            string s = DoSerializer_Divide(null, o, setting);
            Assert.AreEqual(context, s);
            LogHelper.Debug(s);
            return DoSerializer_Combie(o.GetType(), s, null, setting);
        }

        private TestDynamicObject CreateInstance()
        {
            dynamic o = new TestDynamicObject();
            o.Text = "Text!";
            o.Int = int.MaxValue;
            o.IntDefault = 0;
            o.NUllableIntDefault = default(int?);
            o.ChildObject = null;
            o.DynamicChildObject = null;

            return o;
        }

        [Test]
        public void TesetDivideAndCombineBySpecifyAssemblyQualifiedName()
        {
            ISerializeSetting setting = new XmlSerializeSetting {AssemblyQualifiedName = "ASSEMBLY"};
            dynamic o = CreateInstance();
            dynamic o1 = DoTest(CreateNonDefaultValueContext(setting, o), setting);
            Assert.AreEqual(o.Text, o1.Text);
            Assert.AreEqual(o.Int, o1.Int);

            setting.SerializeDefalutValue = true;
            dynamic o2 = DoTest(CreateDefaultValueContext(setting, o), setting);
            Assert.AreEqual(o.Text, o2.Text);
            Assert.AreEqual(o.Int, o2.Int);
            Assert.AreEqual(o.IntDefault, o2.IntDefault);
            Assert.AreEqual(o.NUllableIntDefault, o2.NUllableIntDefault);
            Assert.AreEqual(o.ChildObject, o2.ChildObject);
            Assert.AreEqual(o.DynamicChildObject, o2.DynamicChildObject);
        }

        [Test]
        public void TestDivideAndCombine()
        {
            ISerializeSetting setting = new XmlSerializeSetting();
            dynamic o = CreateInstance();
            dynamic o1 = DoTest(CreateNonDefaultValueContext(setting, o), setting);
            Assert.AreEqual(o.Text, o1.Text);
            Assert.AreEqual(o.Int, o1.Int);

            setting.SerializeDefalutValue = true;
            dynamic o2 = DoTest(CreateDefaultValueContext(setting, o), setting);
            Assert.AreEqual(o.Text, o2.Text);
            Assert.AreEqual(o.Int, o2.Int);
            Assert.AreEqual(o.IntDefault, o2.IntDefault);
            Assert.AreEqual(o.NUllableIntDefault, o2.NUllableIntDefault);
            Assert.AreEqual(o.ChildObject, o2.ChildObject);
            Assert.AreEqual(o.DynamicChildObject, o2.DynamicChildObject);
        }

        [Test]
        public void TestSerializeDynamicObject()
        {
            TestDynamicObject dynamicObject = new TestDynamicObject();
            dynamicObject.Explicit = true;

            dynamic d = dynamicObject;
            d.Int = 1;
            d.Decimal = 99.9d;
            d.ChildObject = new DynamicChildObject();

            Dictionary<string, object> values = new Dictionary<string, object>();

            foreach (string memberName in dynamicObject.GetDynamicMemberNames())
            {
                object value = dynamicObject.GetMemberValue(memberName);

                values.Add(memberName, value);
            }

            Assert.AreEqual(d.Int, values["Int"]);
            Assert.AreEqual(d.Decimal, values["Decimal"]);
            Assert.AreEqual(d.ChildObject, values["ChildObject"]);

            ISerializeSetting settings = new XmlSerializeSetting()
            {
                MemberType = SerializeMemberType.All,
                SerializeDefalutValue = true
            };

            string result = DoSerializer_Divide(null, d, settings);
            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<TestDynamicObject>
  <ChildObject " + settings.AssemblyQualifiedName + @"=""" + GetTypeAssembly(d.ChildObject) + @""">
    <Integer>0</Integer>
    <Text Action=""SetNull"" />
  </ChildObject>
  <Explicit " + settings.AssemblyQualifiedName + @"=""" + GetTypeAssembly(d.Explicit) + @""">true</Explicit>
  <Int " + settings.AssemblyQualifiedName + @"=""" + GetTypeAssembly(d.Int) + @""">1</Int>
  <Decimal " + settings.AssemblyQualifiedName + @"=""" + GetTypeAssembly(d.Decimal) + @""">99.9</Decimal>
</TestDynamicObject>", result);

            TestDynamicObject newDynamicObject = DoSerializer_Combie<TestDynamicObject>(result, null);
            Assert.AreEqual(true, newDynamicObject.Explicit);

            dynamic n = newDynamicObject;

            Assert.AreEqual(99.9, n.Decimal);
            Assert.AreEqual(1, n.Int);
            Assert.AreEqual(dynamicObject.ChildObject.Integer, n.ChildObject.Integer);
            Assert.AreEqual(dynamicObject.ChildObject.Text, n.ChildObject.Text);
        }
    }
    

    public class DynamicChildObject
    {
        public string Text { get; set; }
        public int Integer { get; set; }
    }

    public class TestDynamicObject : DynamicObject
    {
        private readonly Dictionary<string, object> _members;

        public bool Explicit;

        public int Int;

        public TestDynamicObject()
        {
            _members = new Dictionary<string, object>();
        }

        public DynamicChildObject ChildObject { get; set; }

        internal Dictionary<string, object> Members
        {
            get { return _members; }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _members.Keys.Union(new[] {"Int", "ChildObject"});
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            Type targetType = binder.Type;

            if (targetType == typeof(IDictionary<string, object>) ||
                targetType == typeof(IDictionary))
            {
                result = new Dictionary<string, object>(_members);
                return true;
            }
            return base.TryConvert(binder, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _members.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _members[binder.Name] = value;
            return true;
        }
    }
}
#endif