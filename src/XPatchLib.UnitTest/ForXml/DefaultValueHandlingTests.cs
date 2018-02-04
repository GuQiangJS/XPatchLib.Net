
using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
#if NUNIT
using NUnit.Framework;

#elif XUNIT
using System;
using System.Text;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;

#endif

namespace XPatchLib.UnitTest.ForXml
{
    [TestFixture]
    public class DefaultValueHandlingTests : TestBase
    {
        public class MyClass
        {
            [XmlIgnore]
            public MyEnum Status { get; set; }

            private string _data;

            public string Data
            {
                get { return _data; }
                set
                {
                    _data = value;
                    if (_data != null && _data.StartsWith("Other"))
                    {
                        this.Status = MyEnum.Other;
                    }
                }
            }
        }

#if !NET && !NETSTANDARD_2_0_UP
        [AttributeUsage(AttributeTargets.Property)]
        public class XmlIgnoreAttribute : Attribute
        { }

        protected override ITextWriter CreateWriter(StringBuilder output)
        {
            ITextWriter writer = base.CreateWriter(output);
            writer.IgnoreAttributeType = typeof(XmlIgnoreAttribute);
            return writer;
        }
#endif

        public enum MyEnum
        {
            Default = 0,
            Other
        }

        [Test]
        public void PopulateWithIgnoreAttribute()
        {
            MyClass c1=new MyClass(){Data = "Other with some more text" };
            
            string output = this.DoSerializer_Divide(null, c1);
            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<MyClass>
  <Data>Other with some more text</Data>
</MyClass>",output);

            LogHelper.Debug(output);

            MyClass c2 = this.DoSerializer_Combie<MyClass>(output, null);

            Assert.AreEqual(MyEnum.Other, c2.Status);
        }
        public class Invoice
        {
            public string Company { get; set; }
            public decimal Amount { get; set; }

            // false is default value of bool
            public bool Paid { get; set; }

            // null is default value of nullable
            public DateTime? PaidDate { get; set; }

            // customize default values
            [DefaultValue(30)] public int FollowUpDays { get; set; }

            [DefaultValue("")] public string FollowUpEmailAddress { get; set; }
        }

        [Test]
        public void Include()
        {
            Invoice invoice = new Invoice
            {
                Company = "Acme Ltd.",
                Amount = 50.0m,
                Paid = false,
                FollowUpDays = 30,
                FollowUpEmailAddress = string.Empty,
                PaidDate = null
            };

            ISerializeSetting setting=new XmlSerializeSetting();
            setting.SerializeDefalutValue = true;
            string output = this.DoSerializer_Divide(null, invoice, setting);
            LogHelper.Debug(output);

            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Invoice>
  <Amount>50.0</Amount>
  <Company>Acme Ltd.</Company>
  <FollowUpDays>30</FollowUpDays>
  <FollowUpEmailAddress />
  <Paid>false</Paid>
  <PaidDate Action=""SetNull"" />
</Invoice>",output);

            Invoice i2 = this.DoSerializer_Combie<Invoice>(output, null);
            PropertyInfo[] pis = typeof(Invoice).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                Assert.AreEqual(pi.GetValue(invoice, null), pi.GetValue(invoice, null));
            }
        }

        [Test]
        public void SerializeInvoice()
        {
            Invoice invoice = new Invoice
            {
                Company = "Acme Ltd.",
                Amount = 50.0m,
                Paid = false,
                FollowUpDays = 30,
                FollowUpEmailAddress = string.Empty,
                PaidDate = null
            };

            ISerializeSetting setting = new XmlSerializeSetting();
            setting.SerializeDefalutValue = true;
            string output = this.DoSerializer_Divide(null, invoice, setting);
            LogHelper.Debug(output);

            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Invoice>
  <Amount>50.0</Amount>
  <Company>Acme Ltd.</Company>
  <FollowUpDays>30</FollowUpDays>
  <FollowUpEmailAddress />
  <Paid>false</Paid>
  <PaidDate Action=""SetNull"" />
</Invoice>", output);

            Invoice i2 = this.DoSerializer_Combie<Invoice>(output, null);
            PropertyInfo[] pis = typeof(Invoice).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                Assert.AreEqual(pi.GetValue(invoice, null), pi.GetValue(i2, null));
            }

            output = this.DoSerializer_Divide(null, invoice);
            LogHelper.Debug(output);

            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Invoice>
  <Amount>50.0</Amount>
  <Company>Acme Ltd.</Company>
  <FollowUpEmailAddress />
</Invoice>", output);
        }

        public class NetworkUser
        {
            [DefaultValue(-1)]
            public long GlobalId { get; set; }
            
            [DefaultValue(0)]
            public int Age { get; set; }
            
            [DefaultValue(0.0)]
            public decimal Amount { get; set; }
            
            [DefaultValue(-1.0d)]
            public float FloatGlobalId { get; set; }
            
            public string Firstname { get; set; }
            
            public string Lastname { get; set; }

            public NetworkUser()
            {
                GlobalId = -1;
                FloatGlobalId = -1.0f;
                Amount = 0.0m;
                Age = 0;
            }
        }

        [Test]
        public void IgnoreNumberTypeDifferencesWithDefaultValue()
        {
            NetworkUser user = new NetworkUser
            {
                Firstname = "blub"
            };

            string output = this.DoSerializer_Divide(new NetworkUser(), user);
            LogHelper.Debug(output);

            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<NetworkUser>
  <Firstname>blub</Firstname>
</NetworkUser>", output);


            NetworkUser n2 = this.DoSerializer_Combie<NetworkUser>(output, new NetworkUser());
            PropertyInfo[] pis = typeof(NetworkUser).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                Assert.AreEqual(pi.GetValue(user, null), pi.GetValue(n2, null));
            }
        }
    }
}
