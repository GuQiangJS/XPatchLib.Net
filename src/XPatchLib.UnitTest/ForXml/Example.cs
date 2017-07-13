// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
#if NUNIT
using NUnit.Framework;
#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest.ForXml
{
    /// <summary>
    ///     操作复杂类型
    /// </summary>
    [TestFixture]
    public class ExampleComplexClass:TestBase
    {
        private const string ChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<OrderInfo>
  <Date>2010-04-30T00:00:00</Date>
  <OrderId>2</OrderId>
  <OrderTotal>180.50</OrderTotal>
  <ShippingAddress>
    <City>Shanghai</City>
  </ShippingAddress>
</OrderInfo>";

        public class AddressInfo
        {
            public string Address { get; set; }

            public string City { get; set; }

            public string Country { get; set; }

            public string Name { get; set; }

            public string Phone { get; set; }

            public string Zip { get; set; }

            public override bool Equals(object obj)
            {
                var addr = obj as AddressInfo;
                if (addr == null)
                    return false;
                return string.Equals(Country, addr.Country)
                       && string.Equals(Name, addr.Name)
                       && string.Equals(Phone, addr.Phone)
                       && string.Equals(Zip, addr.Zip)
                       && string.Equals(City, addr.City)
                       && string.Equals(Address, addr.Address);
            }

            public override int GetHashCode()
            {
                var result = 0;
                if (Country != null)
                    result ^= Country.GetHashCode();
                if (Name != null)
                    result ^= Name.GetHashCode();
                if (Phone != null)
                    result ^= Phone.GetHashCode();
                if (Zip != null)
                    result ^= Zip.GetHashCode();
                if (City != null)
                    result ^= City.GetHashCode();
                if (Address != null)
                    result ^= Address.GetHashCode();
                return result;
            }
        }

        public class OrderInfo
        {
            public DateTime Date { get; set; }

            public int OrderId { get; set; }

            public decimal OrderTotal { get; set; }

            public AddressInfo ShippingAddress { get; set; }

            public string UserId { get; set; }

            public override bool Equals(object obj)
            {
                var order = obj as OrderInfo;
                if (order == null)
                    return false;
                return Equals(OrderId, order.OrderId)
                       && decimal.Equals(OrderTotal, order.OrderTotal)
                       && Equals(ShippingAddress, order.ShippingAddress)
                       && string.Equals(UserId, order.UserId)
                       && DateTime.Equals(Date, order.Date);
            }
        }

        [Test]
        public void ExampleSerializeComplex()
        {
            var order1 = new OrderInfo
            {
                OrderId = 1,
                OrderTotal = 200.45m,
                ShippingAddress = new AddressInfo
                {
                    Country = "China",
                    Name = "Customer",
                    Phone = "138-1234-5678",
                    Zip = "100000",
                    City = "Beijing",
                    Address = ""
                },
                UserId = "1234",
                Date = new DateTime(2008, 8, 8)
            };

            var order2 = new OrderInfo
            {
                OrderId = 2,
                OrderTotal = 180.50m,
                ShippingAddress = new AddressInfo
                {
                    Country = "China",
                    Name = "Customer",
                    Phone = "138-1234-5678",
                    Zip = "100000",
                    City = "Shanghai",
                    Address = ""
                },
                UserId = "1234",
                Date = new DateTime(2010, 4, 30)
            };

            DoAssert(ChangedContext, order1, order2, true);

            DoAssert(ChangedContext, order1, order2, false);
        }
    }

    /// <summary>
    ///     操作复杂集合类型
    /// </summary>
    [TestFixture]
    public class ExampleComplexCollectionClass:TestBase
    {
        private const string ChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<OrderList>
  <Orders>
    <OrderInfo Action=""Remove"" OrderId=""3"" />
    <OrderInfo OrderId=""2"">
      <OrderTotal>230.89</OrderTotal>
    </OrderInfo>
    <OrderInfo Action=""Add"">
      <Date>2008-08-08T00:00:00</Date>
      <OrderId>4</OrderId>
      <OrderTotal>67.30</OrderTotal>
      <UserId>1234</UserId>
    </OrderInfo>
  </Orders>
</OrderList>";

        [PrimaryKey("OrderId")]
        public class OrderInfo
        {
            public DateTime Date { get; set; }

            public int OrderId { get; set; }

            public decimal OrderTotal { get; set; }

            public ExampleComplexClass.AddressInfo ShippingAddress { get; set; }

            public string UserId { get; set; }

            public override bool Equals(object obj)
            {
                var order = obj as OrderInfo;
                if (order == null)
                    return false;
                return Equals(OrderId, order.OrderId)
                       && decimal.Equals(OrderTotal, order.OrderTotal)
                       && Equals(ShippingAddress, order.ShippingAddress)
                       && string.Equals(UserId, order.UserId)
                       && DateTime.Equals(Date, order.Date);
            }

            public override int GetHashCode()
            {
                var result = 0;
                result ^= OrderId.GetHashCode();
                result ^= OrderTotal.GetHashCode();
                if (ShippingAddress != null)
                    result ^= ShippingAddress.GetHashCode();
                if (UserId != null)
                    result ^= UserId.GetHashCode();
                if (Date != null)
                    result ^= Date.GetHashCode();
                return result;
            }
        }

        public class OrderList
        {
            public List<OrderInfo> Orders { get; set; }

            public string UserId { get; set; }

            public override bool Equals(object obj)
            {
                var list = obj as OrderList;
                if (list == null)
                    return false;
                Debug.Assert(Orders != null, "Orders != null");
                return string.Equals(UserId, list.UserId)
                       && (
                           Orders == null && list.Orders == null
                           ||
                           Orders.Count.Equals(list.Orders.Count)
                           && Orders.Except(list.Orders).Count() == 0
                           && list.Orders.Except(Orders).Count() == 0
                       );
            }
        }

        [Test]
        public void ExampleSerializeComplexCollection()
        {
            var list1 = new OrderList
            {
                UserId = "1234",
                Orders = new List<OrderInfo>
                {
                    new OrderInfo
                    {
                        OrderId = 1,
                        OrderTotal = 200.45m,
                        UserId = "1234",
                        Date = new DateTime(2008, 8, 8)
                    },
                    new OrderInfo
                    {
                        OrderId = 2,
                        OrderTotal = 450.23m,
                        UserId = "1234",
                        Date = new DateTime(2008, 8, 8)
                    },
                    new OrderInfo
                    {
                        OrderId = 3,
                        OrderTotal = 185.60m,
                        UserId = "1234",
                        Date = new DateTime(2008, 8, 8)
                    }
                }
            };

            var list2 = new OrderList
            {
                UserId = "1234",
                Orders = new List<OrderInfo>
                {
                    new OrderInfo
                    {
                        OrderId = 1,
                        OrderTotal = 200.45m,
                        UserId = "1234",
                        Date = new DateTime(2008, 8, 8)
                    },
                    new OrderInfo
                    {
                        OrderId = 2,
                        OrderTotal = 230.89m,
                        UserId = "1234",
                        Date = new DateTime(2008, 8, 8)
                    },
                    new OrderInfo
                    {
                        OrderId = 4,
                        OrderTotal = 67.30m,
                        UserId = "1234",
                        Date = new DateTime(2008, 8, 8)
                    }
                }
            };

            DoAssert(ChangedContext, list1, list2, true);
            DoAssert(ChangedContext, list1, list2, false);
        }
    }

    [TestFixture]
    public class ExampleDateTimeSerializationMode:TestBase
    {
        private const string LocalChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>2017-05-01T20:00:00+08:00</CardExpiration>
</CreditCard>";

        private const string RoundtripKindChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>2017-05-01T20:00:00</CardExpiration>
</CreditCard>";

        private const string UnspecifiedChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>2017-05-01T20:00:00</CardExpiration>
</CreditCard>";

        private const string UtcChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>2017-05-01T20:00:00Z</CardExpiration>
</CreditCard>";

        public class CreditCard
        {
            public DateTime CardExpiration { get; set; }

            public override bool Equals(object obj)
            {
                var card = obj as CreditCard;
                if (card == null)
                    return false;
                return Equals(CardExpiration, card.CardExpiration);
            }
        }

        [Test]
        public void SerializeDateTimeSerializationMode()
        {


            var card1 = new CreditCard
            {
                CardExpiration = new DateTime(2017, 05, 01, 20, 0, 0)
            };

            DoAssert(typeof(CreditCard), RoundtripKindChangedContext, null, card1, true);

            DoAssert(typeof(CreditCard), LocalChangedContext, null, card1, true,
                new XmlSerializeSetting() {Mode = DateTimeSerializationMode.Local});

            DoAssert(typeof(CreditCard), UnspecifiedChangedContext, null, card1, true,
                new XmlSerializeSetting() { Mode = DateTimeSerializationMode.Unspecified });

            DoAssert(typeof(CreditCard), UtcChangedContext, null, card1, true,
                new XmlSerializeSetting() { Mode = DateTimeSerializationMode.Utc });
        }
    }

    /// <summary>
    ///     初级入门
    /// </summary>
    [TestFixture]
    public class ExampleSampleABC:TestBase
    {
        private const string ChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>05/17</CardExpiration>
  <CardNumber>9876543210</CardNumber>
</CreditCard>";

        public class CreditCard
        {
            public string CardExpiration { get; set; }

            public string CardNumber { get; set; }

            public override bool Equals(object obj)
            {
                var card = obj as CreditCard;
                if (card == null)
                    return false;
                return string.Equals(CardNumber, card.CardNumber)
                       && string.Equals(CardExpiration, card.CardExpiration);
            }
        }

        [Test]
        public void ExampleSample()
        {
            var card1 = new CreditCard
            {
                CardExpiration = "05/12",
                CardNumber = "0123456789"
            };
            var card2 = new CreditCard
            {
                CardExpiration = "05/17",
                CardNumber = "9876543210"
            };
            DoAssert(ChangedContext, card1, card2, true);
            DoAssert(ChangedContext, card1, card2, false);
        }
    }

    /// <summary>
    ///     操作简单类型
    /// </summary>
    [TestFixture]
    public class ExampleSampleClass:TestBase
    {
        private const string ChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Warehouse>
  <Items>
    <String Action=""Remove"">ItemB</String>
    <String Action=""Add"">ItemD</String>
  </Items>
  <Name>Company B</Name>
</Warehouse>";

        public class Warehouse
        {
            public string Address { get; set; }

            public string[] Items { get; set; }

            public string Name { get; set; }

            public override bool Equals(object obj)
            {
                var w = obj as Warehouse;
                if (w == null)
                    return false;
                return string.Equals(Name, w.Name)
                       && string.Equals(Address, w.Address)
                       && (
                           Items == null && w.Items == null
                           ||
                           Items.Length.Equals(w.Items.Length)
                           && Items.Except(w.Items).Count() == 0
                           && w.Items.Except(Items).Count() == 0
                       );
            }
        }

        [Test]
        public void ExampleSample()
        {
            var w1 = new Warehouse
            {
                Name = "Company A",
                Items = new[] {"ItemA", "ItemB", "ItemC"}
            };

            var w2 = new Warehouse
            {
                Name = "Company B",
                Items = new[] {"ItemA", "ItemC", "ItemD"}
            };

            DoAssert(ChangedContext, w1, w2, true);
            DoAssert(ChangedContext, w1, w2, false);
        }

        [Test]
        public void ExampleSampleSetNull()
        {
            var w1 = new Warehouse
            {
                Name = "Company A",
                Items = new[] {"ItemA", "ItemB", "ItemC"}
            };

            var w2 = new Warehouse
            {
                Name = "Company B",
                Items = null
            };
            
            var changedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Warehouse>
  <Items Action=""SetNull"" />
  <Name>Company B</Name>
</Warehouse>";

            DoAssert(changedContext, w1, w2, true);
            DoAssert(changedContext, w1, w2, false);
        }
    }

    /// <summary>
    ///     是否序列化默认值设置
    /// </summary>
    [TestFixture]
    public class ExampleSerializeDefaultValue:TestBase
    {
        private const string NotSerializeDefaultValueChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>05/17</CardExpiration>
  <CardNumber>0123456789</CardNumber>
</CreditCard>";

        private const string SerializeDefaultValueChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardCode>0</CardCode>
  <CardExpiration>05/17</CardExpiration>
  <CardNumber>0123456789</CardNumber>
</CreditCard>";

        public class CreditCard
        {
            public int CardCode { get; set; }

            public string CardExpiration { get; set; }

            public string CardNumber { get; set; }

            public override bool Equals(object obj)
            {
                var card = obj as CreditCard;
                if (card == null)
                    return false;
                return string.Equals(CardNumber, card.CardNumber)
                       && string.Equals(CardExpiration, card.CardExpiration)
                       && Equals(CardCode, card.CardCode);
            }
        }

        [Test]
        public void SerializeDefaultValue()
        {
            var card1 = new CreditCard
            {
                CardNumber = "0123456789",
                CardExpiration = "05/17",
                CardCode = 0
            };


            DoAssert(typeof(CreditCard), NotSerializeDefaultValueChangedContext, null, card1, true);
            XmlSerializeSetting setting=new XmlSerializeSetting();
            setting.SerializeDefalutValue = true;
            DoAssert(typeof(CreditCard), SerializeDefaultValueChangedContext, null, card1, true, setting);
        }
    }

    [TestFixture]
    public class XPatchSerialzerExample
    {
        private const string Context = @"<?xml version=""1.0"" encoding=""utf-8""?>
<MyClass>
  <MyObjectProperty>
    <ObjectName>My String</ObjectName>
  </MyObjectProperty>
</MyClass>";

        public class MyClass
        {
            public MyObject MyObjectProperty;

            public MyClass()
            {
                MyObjectProperty = new MyObject();
            }
        }

        public class MyObject
        {
            public string ObjectName;
        }

        [Test]
        public void ExampleClassDef()
        {
            var serializer = new Serializer(typeof(MyClass));

            var c1 = new MyClass {MyObjectProperty = {ObjectName = "My String"}};

            string changedContext;
            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = new XmlTextWriter(stream, new UTF8Encoding(false)))
                {
                    serializer.Divide(writer, null, c1);
                    stream.Position = 0;
                    using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        changedContext = stremReader.ReadToEnd();
                    }
                }
            }

            Assert.AreEqual(Context, changedContext);
        }
    }
}