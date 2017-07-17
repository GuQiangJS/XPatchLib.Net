
#if NET || NETSTANDARD_2_0_UP

using System;
using System.Runtime.Serialization;
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
    public class TestISerializable:TestBase
    {
        [Test]
        [Description("ISerializable 对象，OriValue=null，不设置任何其他参数，只使用 GetObjectData 方法中的内容")]
        public void TestISerializableOriIsNull1()
        {
            Person person = new Person("XXX", 9999);

            string context = string.Format(@"<Person>
  <AltName>{0}</AltName>
  <AltID>{1}</AltID>
</Person>", person.Name, person.IdNumber);
            string result = string.Empty;
            result = DoDivideISerializable_Divide(null, person, true);
            Assert.AreEqual(context, result);
        }

        [Test]
        public void TestISerializable1()
        {
            Person person = new Person("XXX", 9999);

            DoAssert(string.Empty, new Person(person.Name, person.IdNumber), person, true);
            DoAssert(string.Empty, new Person(person.Name, person.IdNumber), person, false);
        }

        [Test]
        public void TestISerializable2()
        {
            Person person1 = new Person("XXX", 9999);
            Person person2 = new Person("ZZZ", 0);

            string context = string.Format(@"<Person>
  <AltName>{0}</AltName>
  <AltID>{1}</AltID>
</Person>", person2.Name, person2.IdNumber);

            //ISerializable类型始终会创建实例
            DoAssert(context, person1, person2, true);
        }
    }

    public class Person : ISerializable
    {
        private string name_value;
        private int ID_value;

        private static readonly Random _random = new Random((int)DateTime.Now.Ticks);

        public Person(string name, int id)
        {
            name_value = name;
            ID_value = id;
        }

        protected Person(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");
            name_value = (string)info.GetValue("AltName", typeof(string));
            ID_value = (int)info.GetValue("AltID", typeof(int));
        }

        public virtual void GetObjectData(
        SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");
            info.AddValue("AltName", name_value);
            info.AddValue("AltID", ID_value);
        }

        public string Name
        {
            get { return name_value; }
        }

        public int IdNumber
        {
            get { return ID_value; }
        }

        public override bool Equals(object obj)
        {
            Person p=obj as Person;
            if (p == null)
                return false;
            return string.Equals(p.Name, Name) && IdNumber.Equals(p.IdNumber);
        }

        /// <summary>
        /// 由于子元素都是值类型，但是当前又是引用类型，所以在TestBase中判断HashCode值是否相同时会有错误
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            //ISerializable类型始终会创建实例，所以判断HashCode没有意义
            return _random.Next(int.MinValue, int.MaxValue);
        }
    }
}

#endif