// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XPatchLib.UnitTest.ForXml
{
    /// <summary>
    ///     操作复杂类型
    /// </summary>
    [TestClass]
    public class ExampleComplexClass
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

        [TestMethod]
        public void ExampleCombine()
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

            OrderInfo order3;
            var serializer = new XmlSerializer(typeof(OrderInfo));
            using (var reader = new StringReader(ChangedContext))
            {
                order3 = serializer.Combine(reader, order1) as OrderInfo;
            }

            Assert.AreEqual(order3, order2);
            Debug.Assert(order3 != null, "order3 != null");
            Assert.AreNotEqual(order3.GetHashCode(), order2.GetHashCode());
        }

        [TestMethod]
        public void ExampleDivide()
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

            var serializer = new XmlSerializer(typeof(OrderInfo));

            string context;
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, order1, order2);
                stream.Position = 0;
                using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(ChangedContext, context);
            Trace.WriteLine(context);
        }

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
    }

    /// <summary>
    ///     操作复杂集合类型
    /// </summary>
    [TestClass]
    public class ExampleComplexCollectionClass
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

        [TestMethod]
        public void ExampleCombine()
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

            OrderList list3;
            var serializer = new XmlSerializer(typeof(OrderList));
            using (var reader = new StringReader(ChangedContext))
            {
                list3 = serializer.Combine(reader, list1) as OrderList;
            }

            Assert.AreEqual(list3, list2);
            Debug.Assert(list3 != null, "list3 != null");
            Assert.AreNotEqual(list3.GetHashCode(), list1.GetHashCode());
        }

        [TestMethod]
        public void ExampleDivide()
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

            var serializer = new XmlSerializer(typeof(OrderList));

            string context;
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, list1, list2);
                stream.Position = 0;
                using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(ChangedContext, context);
            Trace.WriteLine(context);
        }

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
    }

    [TestClass]
    public class ExampleDateTimeSerializationMode
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

        [TestMethod]
        public void ExampleCombine()
        {
            var card1 = new CreditCard
            {
                CardExpiration = new DateTime(2017, 05, 01, 20, 0, 0)
            };

            var serializer = new XmlSerializer(typeof(CreditCard));

            CreditCard card2;
            using (var reader = new StringReader(RoundtripKindChangedContext))
            {
                card2 = serializer.Combine(reader, null) as CreditCard;
            }

            Assert.AreEqual(card2, card1);
            Debug.Assert(card2 != null, "card2 != null");
            Assert.AreNotEqual(card2.GetHashCode(), card1.GetHashCode());

            serializer = new XmlSerializer(typeof(CreditCard), XmlDateTimeSerializationMode.Local);

            card2 = null;
            using (var reader = new StringReader(LocalChangedContext))
            {
                card2 = serializer.Combine(reader, null) as CreditCard;
            }

            Assert.AreEqual(card2, card1);
            Debug.Assert(card2 != null, "card2 != null");
            Assert.AreNotEqual(card2.GetHashCode(), card1.GetHashCode());

            serializer = new XmlSerializer(typeof(CreditCard), XmlDateTimeSerializationMode.Unspecified);

            card2 = null;
            using (var reader = new StringReader(UnspecifiedChangedContext))
            {
                card2 = serializer.Combine(reader, null) as CreditCard;
            }

            Assert.AreEqual(card2, card1);
            Debug.Assert(card2 != null, "card2 != null");
            Assert.AreNotEqual(card2.GetHashCode(), card1.GetHashCode());

            serializer = new XmlSerializer(typeof(CreditCard), XmlDateTimeSerializationMode.Utc);

            card2 = null;
            using (var reader = new StringReader(UtcChangedContext))
            {
                card2 = serializer.Combine(reader, null) as CreditCard;
            }

            Assert.AreEqual(card2, card1);
            Debug.Assert(card2 != null, "card2 != null");
            Assert.AreNotEqual(card2.GetHashCode(), card1.GetHashCode());
        }

        [TestMethod]
        public void ExampleDivide()
        {
            var card1 = new CreditCard
            {
                CardExpiration = new DateTime(2017, 05, 01, 20, 0, 0)
            };

            var serializer = new XmlSerializer(typeof(CreditCard));

            string context;
            context = String.Empty;
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, null, card1);
                stream.Position = 0;
                using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(RoundtripKindChangedContext, context);
            Trace.WriteLine(context);

            serializer = new XmlSerializer(typeof(CreditCard), XmlDateTimeSerializationMode.Local);

            context = String.Empty;
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, null, card1);
                stream.Position = 0;
                using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(LocalChangedContext, context);
            Trace.WriteLine(context);

            serializer = new XmlSerializer(typeof(CreditCard), XmlDateTimeSerializationMode.Unspecified);

            context = String.Empty;
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, null, card1);
                stream.Position = 0;
                using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(UnspecifiedChangedContext, context);
            Trace.WriteLine(context);

            serializer = new XmlSerializer(typeof(CreditCard), XmlDateTimeSerializationMode.Utc);

            context = String.Empty;
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, null, card1);
                stream.Position = 0;
                using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(UtcChangedContext, context);
            Trace.WriteLine(context);
        }

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
    }

    /// <summary>
    ///     初级入门
    /// </summary>
    [TestClass]
    public class ExampleSampleABC
    {
        private const string ChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>05/17</CardExpiration>
  <CardNumber>9876543210</CardNumber>
</CreditCard>";

        [TestMethod]
        public void ExampleCombine()
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

            CreditCard card3 = null;
            var serializer = new XmlSerializer(typeof(CreditCard));
            using (var reader = new StringReader(ChangedContext))
            {
                card3 = serializer.Combine(reader, card1) as CreditCard;
            }

            Assert.AreEqual(card3, card2);
            Assert.AreNotEqual(card3.GetHashCode(), card1.GetHashCode());
        }

        [TestMethod]
        public void ExampleDivide()
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

            var serializer = new XmlSerializer(typeof(CreditCard));

            var context = String.Empty;
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, card1, card2);
                stream.Position = 0;
                using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(ChangedContext, context);
            Trace.WriteLine(context);
        }

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
    }

    /// <summary>
    ///     操作简单类型
    /// </summary>
    [TestClass]
    public class ExampleSampleClass
    {
        private const string ChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Warehouse>
  <Items>
    <String Action=""Remove"">ItemB</String>
    <String Action=""Add"">ItemD</String>
  </Items>
  <Name>Company B</Name>
</Warehouse>";

        [TestMethod]
        public void ExampleCombine()
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

            Warehouse w3;
            var serializer = new XmlSerializer(typeof(Warehouse));
            using (var reader = new StringReader(ChangedContext))
            {
                w3 = serializer.Combine(reader, w1) as Warehouse;
            }

            Assert.AreEqual(w3, w2);
            Assert.AreNotEqual(w3.GetHashCode(), w1.GetHashCode());
        }

        [TestMethod]
        public void ExampleDivide()
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

            var serializer = new XmlSerializer(typeof(Warehouse));

            var context = String.Empty;
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, w1, w2);
                stream.Position = 0;
                using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(ChangedContext, context);
            Trace.WriteLine(context);
        }

        [TestMethod]
        public void ExampleDivideSetNull()
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

            var serializer = new XmlSerializer(typeof(Warehouse));

            var changedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Warehouse>
  <Items Action=""SetNull"" />
  <Name>Company B</Name>
</Warehouse>";

            var context = String.Empty;
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, w1, w2);
                stream.Position = 0;
                using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(changedContext, context);
            Trace.WriteLine(context);
        }

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
    }

    /// <summary>
    ///     是否序列化默认值设置
    /// </summary>
    [TestClass]
    public class ExampleSerializeDefaultValue
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

        [TestMethod]
        public void ExampleCombine()
        {
            var card1 = new CreditCard
            {
                CardNumber = "0123456789",
                CardExpiration = "05/17",
                CardCode = 0
            };

            var serializer = new XmlSerializer(typeof(CreditCard));

            CreditCard card2 = null;
            using (var reader = new StringReader(NotSerializeDefaultValueChangedContext))
            {
                card2 = serializer.Combine(reader, null) as CreditCard;
            }

            Assert.AreEqual(card2, card1);
            Debug.Assert(card2 != null, "card2 != null");
            Assert.AreNotEqual(card2.GetHashCode(), card1.GetHashCode());

            serializer = new XmlSerializer(typeof(CreditCard), true);

            card2 = null;
            using (var reader = new StringReader(NotSerializeDefaultValueChangedContext))
            {
                card2 = serializer.Combine(reader, null) as CreditCard;
            }

            Assert.AreEqual(card2, card1);
            Debug.Assert(card2 != null, "card2 != null");
            Assert.AreNotEqual(card2.GetHashCode(), card1.GetHashCode());
        }

        [TestMethod]
        public void ExampleDivide()
        {
            var card1 = new CreditCard
            {
                CardNumber = "0123456789",
                CardExpiration = "05/17",
                CardCode = 0
            };

            var serializer = new XmlSerializer(typeof(CreditCard));

            string context;
            context = "";
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, null, card1);
                stream.Position = 0;
                using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(NotSerializeDefaultValueChangedContext, context);
            Trace.WriteLine(context);

            serializer = new XmlSerializer(typeof(CreditCard), true);

            context = String.Empty;
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, null, card1);
                stream.Position = 0;
                using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(SerializeDefaultValueChangedContext, context);
            Trace.WriteLine(context);
        }

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
    }

    [TestClass]
    public class XPatchSerialzerExample
    {
        private const string Context = @"<?xml version=""1.0"" encoding=""utf-8""?>
<MyClass>
  <MyObjectProperty>
    <ObjectName>My String</ObjectName>
  </MyObjectProperty>
</MyClass>";

        [TestMethod]
        public void ExampleClassDef()
        {
            var serializer = new XmlSerializer(typeof(MyClass));

            var c1 = new MyClass {MyObjectProperty = {ObjectName = "My String"}};

            string changedContext;
            using (var stream = new MemoryStream())
            {
                serializer.Divide(stream, null, c1);
                stream.Position = 0;
                using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    changedContext = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(Context, changedContext);
        }

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
    }
}