// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Xml.Serialization;
#if NUNIT
using NUnit.Framework;
#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif
#if NET
using System.Runtime.Serialization;
#endif

namespace XPatchLib.UnitTest
{
    [TestFixture]
    public class TestSerializeAttribute : TestBase
    {
#if NET
        [Test]
        [Description("测试是否支持 OnSerializingAttribute，OnSerializedAttribute，OnDeserializingAttribute，OnDeserializedAttribute 特性标签")]
        public void TestOnSerializeEvent()
        {
            string Context = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestSimpleObject>
  <member2>123</member2>
  <member4>123</member4>
</TestSimpleObject>";

            TestSimpleObject oriObj = new TestSimpleObject();
            TestSimpleObject revObj = new TestSimpleObject();
            revObj.member2 = "123";
            revObj.member3 = "123";
            revObj.member4 = "123";

            ISerializeSetting setting = new XmlSerializeSetting() {MemberType = SerializeMemberType.Field};

            string result = DoSerializer_Divide(oriObj, revObj, setting);

            Assert.AreEqual(Context, result);
            Assert.AreEqual(TestSimpleObject.Member2Serialized, revObj.member2);

            //反序列化时，使用深克隆模式。返回对象与传入的原始对象不相同
            TestSimpleObject newObj1 =
                DoSerializer_Combie(typeof(TestSimpleObject), Context, oriObj, setting, true) as TestSimpleObject;
            //反序列化完成后，member4的值应该是 OnDeserializedMethod 后的值
            Assert.AreEqual(TestSimpleObject.Member4Deserialized, newObj1.member4);
            //OnDeserializingMethod 是在反序列化之前执行，所以member3的值应该是Context中的值
            Assert.AreEqual(revObj.member3, revObj.member3);
            Assert.AreNotEqual(TestSimpleObject.Member3Default, revObj.member3);
            Assert.AreEqual(TestSimpleObject.Member3Deserializing, newObj1.member3);
            //反序列化时，使用深克隆模式。原始传入的对象不会被改变
            Assert.AreEqual(TestSimpleObject.Member3Default, oriObj.member3);
            Assert.AreEqual(TestSimpleObject.Member4Default, oriObj.member4);

            //反序列化时，不使用深克隆模式。返回对象与传入的原始对象相同
            TestSimpleObject newObj2 =
                DoSerializer_Combie(typeof(TestSimpleObject), Context, oriObj, setting, false) as TestSimpleObject;
            //反序列化完成后，member4的值应该是 OnDeserializedMethod 后的值
            Assert.AreEqual(TestSimpleObject.Member4Deserialized, newObj1.member4);
            //OnDeserializingMethod 是在反序列化之前执行，所以member3的值应该是Context中的值
            Assert.AreEqual(revObj.member3, revObj.member3);
            Assert.AreNotEqual(TestSimpleObject.Member3Default, revObj.member3);
            Assert.AreEqual(TestSimpleObject.Member3Deserializing, newObj1.member3);
            //反序列化时，不使用深克隆模式。原始传入的对象会被改变
            Assert.AreEqual(newObj2.member3, oriObj.member3);
            Assert.AreEqual(newObj2.member4, oriObj.member4);
        }
#endif
    }

#if NET
    //https://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.onserializingattribute(v=vs.110).aspx
    public class TestSimpleObject
    {
        internal const string Member2Serializing = "This value went into the data file during serialization.";
        internal const string Member2Serialized = "This value was reset after serialization.";
        internal const string Member3Deserializing = "This value was set during deserialization";
        internal const string Member4Deserialized = "This value was set after deserialization.";
        internal const int MemberDefault = 11;
        internal const string Member2Default = "Hello World!";
        internal const string Member3Default = "This is a nonserialized value";
        internal const string Member4Default = null;

        // This member is serialized and deserialized with no change.
        public int member1;

        // The value of this field is set and reset during and 
        // after serialization.
        public string member2;

        // This field is not serialized. The OnDeserializedAttribute 
        // is used to set the member value after serialization.
        [XmlIgnore] public string member3;

        // This field is set to null, but populated after deserialization.
        public string member4;

        // Constructor for the class.
        public TestSimpleObject()
        {
            member1 = MemberDefault;
            member2 = Member2Default;
            member3 = Member3Default;
            member4 = Member4Default;
        }

        public void Print()
        {
            LogHelper.Debug("member1 = '{0}'", member1);
            LogHelper.Debug("member2 = '{0}'", member2);
            LogHelper.Debug("member3 = '{0}'", member3);
            LogHelper.Debug("member4 = '{0}'", member4);
        }

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            member2 = Member2Serializing;
        }

        [OnSerialized]
        internal void OnSerializedMethod(StreamingContext context)
        {
            member2 = Member2Serialized;
        }

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            member3 = Member3Deserializing;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            member4 = Member4Deserialized;
        }
    }
#endif
}