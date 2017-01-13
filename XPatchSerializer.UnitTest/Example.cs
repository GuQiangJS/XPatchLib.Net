using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XPatchLib.UnitTest
{
    /// <summary>
    /// 操作复杂类型 
    /// </summary>
    [TestClass]
    public class ExampleComplexClass
    {
        #region Private Fields

        private const string changedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<OrderInfo>
  <Date>2010-04-30T00:00:00</Date>
  <OrderId>2</OrderId>
  <OrderTotal>180.50</OrderTotal>
  <ShippingAddress>
    <City>Shanghai</City>
  </ShippingAddress>
</OrderInfo>";

        #endregion Private Fields

        #region Public Methods

        [TestMethod]
        public void ExampleCombine()
        {
            OrderInfo order1 = new OrderInfo()
            {
                OrderId = 1,
                OrderTotal = 200.45m,
                ShippingAddress = new AddressInfo()
                {
                    Country = "China",
                    Name = "Customer",
                    Phone = "138-1234-5678",
                    Zip = "100000",
                    City = "Beijing",
                    Address = "",
                },
                UserId = "1234",
                Date = new DateTime(2008, 8, 8)
            };

            OrderInfo order2 = new OrderInfo()
            {
                OrderId = 2,
                OrderTotal = 180.50m,
                ShippingAddress = new AddressInfo()
                {
                    Country = "China",
                    Name = "Customer",
                    Phone = "138-1234-5678",
                    Zip = "100000",
                    City = "Shanghai",
                    Address = "",
                },
                UserId = "1234",
                Date = new DateTime(2010, 4, 30)
            };

            OrderInfo order3 = null;
            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderInfo));
            using (StringReader reader = new StringReader(changedContext))
            {
                order3 = serializer.Combine(reader, order1) as OrderInfo;
            }

            Assert.AreEqual(order3, order2);
            Assert.AreNotEqual(order3.GetHashCode(), order2.GetHashCode());
        }

        [TestMethod]
        public void ExampleDivide()
        {
            OrderInfo order1 = new OrderInfo()
            {
                OrderId = 1,
                OrderTotal = 200.45m,
                ShippingAddress = new AddressInfo()
                {
                    Country = "China",
                    Name = "Customer",
                    Phone = "138-1234-5678",
                    Zip = "100000",
                    City = "Beijing",
                    Address = "",
                },
                UserId = "1234",
                Date = new DateTime(2008, 8, 8)
            };

            OrderInfo order2 = new OrderInfo()
            {
                OrderId = 2,
                OrderTotal = 180.50m,
                ShippingAddress = new AddressInfo()
                {
                    Country = "China",
                    Name = "Customer",
                    Phone = "138-1234-5678",
                    Zip = "100000",
                    City = "Shanghai",
                    Address = "",
                },
                UserId = "1234",
                Date = new DateTime(2010, 4, 30)
            };

            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderInfo));

            string context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, order1, order2);
                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(changedContext, context);
            Trace.WriteLine(context);
        }

        #endregion Public Methods

        #region Public Classes

        public class AddressInfo
        {
            #region Public Properties

            public string Address { get; set; }

            public string City { get; set; }

            public string Country { get; set; }

            public string Name { get; set; }

            public string Phone { get; set; }

            public string Zip { get; set; }

            #endregion Public Properties

            #region Public Methods

            public override bool Equals(object obj)
            {
                AddressInfo addr = obj as AddressInfo;
                if (addr == null)
                {
                    return false;
                }
                return string.Equals(this.Country, addr.Country)
                    && string.Equals(this.Name, addr.Name)
                    && string.Equals(this.Phone, addr.Phone)
                    && string.Equals(this.Zip, addr.Zip)
                    && string.Equals(this.City, addr.City)
                    && string.Equals(this.Address, addr.Address);
            }

            public override int GetHashCode()
            {
                int result = 0;
                if (Country != null)
                {
                    result ^= Country.GetHashCode();
                }
                if (Name != null)
                {
                    result ^= Name.GetHashCode();
                }
                if (Phone != null)
                {
                    result ^= Phone.GetHashCode();
                }
                if (Zip != null)
                {
                    result ^= Zip.GetHashCode();
                }
                if (City != null)
                {
                    result ^= City.GetHashCode();
                }
                if (Address != null)
                {
                    result ^= Address.GetHashCode();
                }
                return result;
            }

            #endregion Public Methods
        }

        public class OrderInfo
        {
            #region Public Properties

            public DateTime Date { get; set; }

            public int OrderId { get; set; }

            public decimal OrderTotal { get; set; }

            public AddressInfo ShippingAddress { get; set; }

            public string UserId { get; set; }

            #endregion Public Properties

            #region Public Methods

            public override bool Equals(object obj)
            {
                OrderInfo order = obj as OrderInfo;
                if (order == null)
                {
                    return false;
                }
                return int.Equals(this.OrderId, order.OrderId)
                    && decimal.Equals(this.OrderTotal, order.OrderTotal)
                    && AddressInfo.Equals(this.ShippingAddress, order.ShippingAddress)
                    && string.Equals(this.UserId, order.UserId)
                    && DateTime.Equals(this.Date, order.Date);
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }

    /// <summary>
    /// 操作复杂集合类型 
    /// </summary>
    [TestClass]
    public class ExampleComplexCollectionClass
    {
        #region Private Fields

        private const string changedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
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

        #endregion Private Fields

        #region Public Methods

        [TestMethod]
        public void ExampleCombine()
        {
            OrderList list1 = new OrderList()
            {
                UserId = "1234",
                Orders = new List<OrderInfo>(){
            new OrderInfo(){
                OrderId = 1,
                OrderTotal = 200.45m,
                UserId = "1234",
                Date = new DateTime(2008,8,8)
            },
            new OrderInfo(){
                OrderId = 2,
                OrderTotal = 450.23m,
                UserId = "1234",
                Date = new DateTime(2008,8,8)
            },
            new OrderInfo(){
                OrderId = 3,
                OrderTotal = 185.60m,
                UserId = "1234",
                Date = new DateTime(2008,8,8)
            }
        }
            };

            OrderList list2 = new OrderList()
            {
                UserId = "1234",
                Orders = new List<OrderInfo>(){
            new OrderInfo(){
                OrderId = 1,
                OrderTotal = 200.45m,
                UserId = "1234",
                Date = new DateTime(2008,8,8)
            },
            new OrderInfo(){
                OrderId = 2,
                OrderTotal = 230.89m,
                UserId = "1234",
                Date = new DateTime(2008,8,8)
            },
            new OrderInfo(){
                OrderId = 4,
                OrderTotal = 67.30m,
                UserId = "1234",
                Date = new DateTime(2008,8,8)
            }
        }
            };

            OrderList list3 = null;
            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderList));
            using (StringReader reader = new StringReader(changedContext))
            {
                list3 = serializer.Combine(reader, list1) as OrderList;
            }

            Assert.AreEqual(list3, list2);
            Assert.AreNotEqual(list3.GetHashCode(), list1.GetHashCode());
        }

        [TestMethod]
        public void ExampleDivide()
        {
            OrderList list1 = new OrderList()
            {
                UserId = "1234",
                Orders = new List<OrderInfo>(){
            new OrderInfo(){
                OrderId = 1,
                OrderTotal = 200.45m,
                UserId = "1234",
                Date = new DateTime(2008,8,8)
            },
            new OrderInfo(){
                OrderId = 2,
                OrderTotal = 450.23m,
                UserId = "1234",
                Date = new DateTime(2008,8,8)
            },
            new OrderInfo(){
                OrderId = 3,
                OrderTotal = 185.60m,
                UserId = "1234",
                Date = new DateTime(2008,8,8)
            }
        }
            };

            OrderList list2 = new OrderList()
            {
                UserId = "1234",
                Orders = new List<OrderInfo>(){
            new OrderInfo(){
                OrderId = 1,
                OrderTotal = 200.45m,
                UserId = "1234",
                Date = new DateTime(2008,8,8)
            },
            new OrderInfo(){
                OrderId = 2,
                OrderTotal = 230.89m,
                UserId = "1234",
                Date = new DateTime(2008,8,8)
            },
            new OrderInfo(){
                OrderId = 4,
                OrderTotal = 67.30m,
                UserId = "1234",
                Date = new DateTime(2008,8,8)
            }
        }
            };

            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderList));

            string context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, list1, list2);
                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(changedContext, context);
            Trace.WriteLine(context);
        }

        #endregion Public Methods

        #region Public Classes

        [PrimaryKey("OrderId")]
        public class OrderInfo
        {
            #region Public Properties

            public DateTime Date { get; set; }

            public int OrderId { get; set; }

            public decimal OrderTotal { get; set; }

            public ExampleComplexClass.AddressInfo ShippingAddress { get; set; }

            public string UserId { get; set; }

            #endregion Public Properties

            #region Public Methods

            public override bool Equals(object obj)
            {
                OrderInfo order = obj as OrderInfo;
                if (order == null)
                {
                    return false;
                }
                return int.Equals(this.OrderId, order.OrderId)
                    && decimal.Equals(this.OrderTotal, order.OrderTotal)
                    && ExampleComplexClass.AddressInfo.Equals(this.ShippingAddress, order.ShippingAddress)
                    && string.Equals(this.UserId, order.UserId)
                    && DateTime.Equals(this.Date, order.Date);
            }

            public override int GetHashCode()
            {
                int result = 0;
                if (OrderId != null)
                {
                    result ^= OrderId.GetHashCode();
                }
                if (OrderTotal != null)
                {
                    result ^= OrderTotal.GetHashCode();
                }
                if (ShippingAddress != null)
                {
                    result ^= ShippingAddress.GetHashCode();
                }
                if (UserId != null)
                {
                    result ^= UserId.GetHashCode();
                }
                if (Date != null)
                {
                    result ^= Date.GetHashCode();
                }
                return result;
            }

            #endregion Public Methods
        }

        public class OrderList
        {
            #region Public Properties

            public List<OrderInfo> Orders { get; set; }

            public string UserId { get; set; }

            #endregion Public Properties

            #region Public Methods

            public override bool Equals(object obj)
            {
                OrderList list = obj as OrderList;
                if (list == null)
                {
                    return false;
                }
                return string.Equals(this.UserId, list.UserId)
                    && (
                    (this.Orders == null && list.Orders == null)
                    ||
                    (this.Orders.Count.Equals(list.Orders.Count)
                    && this.Orders.Except(list.Orders).Count() == 0
                    && list.Orders.Except(this.Orders).Count() == 0
                    )
                    );
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }

    [TestClass]
    public class ExampleDateTimeSerializationMode
    {
        #region Private Fields

        private const string localChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>2017-05-01T20:00:00+08:00</CardExpiration>
</CreditCard>";

        private const string roundtripKindChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>2017-05-01T20:00:00</CardExpiration>
</CreditCard>";

        private const string unspecifiedChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>2017-05-01T20:00:00</CardExpiration>
</CreditCard>";

        private const string utcChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>2017-05-01T20:00:00Z</CardExpiration>
</CreditCard>";

        #endregion Private Fields

        #region Public Methods

        [TestMethod]
        public void ExampleCombine()
        {
            CreditCard card1 = new CreditCard()
            {
                CardExpiration = new DateTime(2017, 05, 01, 20, 0, 0)
            };

            XPatchSerializer serializer = new XPatchSerializer(typeof(CreditCard));

            CreditCard card2 = null;
            using (StringReader reader = new StringReader(roundtripKindChangedContext))
            {
                card2 = serializer.Combine(reader, null) as CreditCard;
            }

            Assert.AreEqual(card2, card1);
            Assert.AreNotEqual(card2.GetHashCode(), card1.GetHashCode());

            serializer = new XPatchSerializer(typeof(CreditCard), XmlDateTimeSerializationMode.Local);

            card2 = null;
            using (StringReader reader = new StringReader(localChangedContext))
            {
                card2 = serializer.Combine(reader, null) as CreditCard;
            }

            Assert.AreEqual(card2, card1);
            Assert.AreNotEqual(card2.GetHashCode(), card1.GetHashCode());

            serializer = new XPatchSerializer(typeof(CreditCard), XmlDateTimeSerializationMode.Unspecified);

            card2 = null;
            using (StringReader reader = new StringReader(unspecifiedChangedContext))
            {
                card2 = serializer.Combine(reader, null) as CreditCard;
            }

            Assert.AreEqual(card2, card1);
            Assert.AreNotEqual(card2.GetHashCode(), card1.GetHashCode());

            serializer = new XPatchSerializer(typeof(CreditCard), XmlDateTimeSerializationMode.Utc);

            card2 = null;
            using (StringReader reader = new StringReader(utcChangedContext))
            {
                card2 = serializer.Combine(reader, null) as CreditCard;
            }

            Assert.AreEqual(card2, card1);
            Assert.AreNotEqual(card2.GetHashCode(), card1.GetHashCode());
        }

        [TestMethod]
        public void ExampleDivide()
        {
            CreditCard card1 = new CreditCard()
            {
                CardExpiration = new DateTime(2017, 05, 01, 20, 0, 0)
            };

            XPatchSerializer serializer = new XPatchSerializer(typeof(CreditCard));

            string context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, card1);
                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(roundtripKindChangedContext, context);
            Trace.WriteLine(context);

            serializer = new XPatchSerializer(typeof(CreditCard), XmlDateTimeSerializationMode.Local);

            context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, card1);
                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(localChangedContext, context);
            Trace.WriteLine(context);

            serializer = new XPatchSerializer(typeof(CreditCard), XmlDateTimeSerializationMode.Unspecified);

            context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, card1);
                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(unspecifiedChangedContext, context);
            Trace.WriteLine(context);

            serializer = new XPatchSerializer(typeof(CreditCard), XmlDateTimeSerializationMode.Utc);

            context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, card1);
                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(utcChangedContext, context);
            Trace.WriteLine(context);
        }

        #endregion Public Methods

        #region Public Classes

        public class CreditCard
        {
            #region Public Properties

            public DateTime CardExpiration { get; set; }

            #endregion Public Properties

            #region Public Methods

            public override bool Equals(object obj)
            {
                CreditCard card = obj as CreditCard;
                if (card == null)
                {
                    return false;
                }
                return string.Equals(this.CardExpiration, card.CardExpiration);
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }

    /// <summary>
    /// 初级入门 
    /// </summary>
    [TestClassAttribute]
    public class ExampleSampleABC
    {
        #region Private Fields

        private const string changedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>05/17</CardExpiration>
  <CardNumber>9876543210</CardNumber>
</CreditCard>";

        #endregion Private Fields

        #region Public Methods

        [TestMethod]
        public void ExampleCombine()
        {
            CreditCard card1 = new CreditCard()
            {
                CardExpiration = "05/12",
                CardNumber = "0123456789"
            };
            CreditCard card2 = new CreditCard()
            {
                CardExpiration = "05/17",
                CardNumber = "9876543210"
            };

            CreditCard card3 = null;
            XPatchSerializer serializer = new XPatchSerializer(typeof(CreditCard));
            using (StringReader reader = new StringReader(changedContext))
            {
                card3 = serializer.Combine(reader, card1) as CreditCard;
            }

            Assert.AreEqual(card3, card2);
            Assert.AreNotEqual(card3.GetHashCode(), card1.GetHashCode());
        }

        [TestMethod]
        public void ExampleDivide()
        {
            CreditCard card1 = new CreditCard()
            {
                CardExpiration = "05/12",
                CardNumber = "0123456789"
            };
            CreditCard card2 = new CreditCard()
            {
                CardExpiration = "05/17",
                CardNumber = "9876543210"
            };

            XPatchSerializer serializer = new XPatchSerializer(typeof(CreditCard));

            string context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, card1, card2);
                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(changedContext, context);
            Trace.WriteLine(context);
        }

        #endregion Public Methods

        #region Public Classes

        public class CreditCard
        {
            #region Public Properties

            public string CardExpiration { get; set; }

            public string CardNumber { get; set; }

            #endregion Public Properties

            #region Public Methods

            public override bool Equals(object obj)
            {
                CreditCard card = obj as CreditCard;
                if (card == null)
                {
                    return false;
                }
                return string.Equals(this.CardNumber, card.CardNumber)
                    && string.Equals(this.CardExpiration, card.CardExpiration);
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }

    /// <summary>
    /// 操作简单类型 
    /// </summary>
    [TestClass]
    public class ExampleSampleClass
    {
        #region Private Fields

        private const string changedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Warehouse>
  <Items>
    <String Action=""Remove"">ItemB</String>
    <String Action=""Add"">ItemD</String>
  </Items>
  <Name>Company B</Name>
</Warehouse>";

        #endregion Private Fields

        #region Public Methods

        [TestMethod]
        public void ExampleCombine()
        {
            Warehouse w1 = new Warehouse()
            {
                Name = "Company A",
                Items = new string[] { "ItemA", "ItemB", "ItemC" }
            };

            Warehouse w2 = new Warehouse()
            {
                Name = "Company B",
                Items = new string[] { "ItemA", "ItemC", "ItemD" }
            };

            Warehouse w3 = null;
            XPatchSerializer serializer = new XPatchSerializer(typeof(Warehouse));
            using (StringReader reader = new StringReader(changedContext))
            {
                w3 = serializer.Combine(reader, w1) as Warehouse;
            }

            Assert.AreEqual(w3, w2);
            Assert.AreNotEqual(w3.GetHashCode(), w1.GetHashCode());
        }

        [TestMethod]
        public void ExampleDivide()
        {
            Warehouse w1 = new Warehouse()
            {
                Name = "Company A",
                Items = new string[] { "ItemA", "ItemB", "ItemC" }
            };

            Warehouse w2 = new Warehouse()
            {
                Name = "Company B",
                Items = new string[] { "ItemA", "ItemC", "ItemD" }
            };

            XPatchSerializer serializer = new XPatchSerializer(typeof(Warehouse));

            string context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, w1, w2);
                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(changedContext, context);
            Trace.WriteLine(context);
        }

        [TestMethod]
        public void ExampleDivideSetNull()
        {
            Warehouse w1 = new Warehouse()
            {
                Name = "Company A",
                Items = new string[] { "ItemA", "ItemB", "ItemC" }
            };

            Warehouse w2 = new Warehouse()
            {
                Name = "Company B",
                Items = null
            };

            XPatchSerializer serializer = new XPatchSerializer(typeof(Warehouse));

            string changedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Warehouse>
  <Items Action=""SetNull"" />
  <Name>Company B</Name>
</Warehouse>";

            string context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, w1, w2);
                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(changedContext, context);
            Trace.WriteLine(context);
        }

        #endregion Public Methods

        #region Public Classes

        public class Warehouse
        {
            #region Public Properties

            public string Address { get; set; }

            public string[] Items { get; set; }

            public string Name { get; set; }

            #endregion Public Properties

            #region Public Methods

            public override bool Equals(object obj)
            {
                Warehouse w = obj as Warehouse;
                if (w == null)
                {
                    return false;
                }
                return string.Equals(this.Name, w.Name)
                    && string.Equals(this.Address, w.Address)
                    && (
                    (this.Items == null && w.Items == null)
                    ||
                    (this.Items.Length.Equals(w.Items.Length)
                    && this.Items.Except(w.Items).Count() == 0
                    && w.Items.Except(this.Items).Count() == 0)
                    );
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }

    /// <summary>
    /// 是否序列化默认值设置 
    /// </summary>
    [TestClass]
    public class ExampleSerializeDefaultValue
    {
        #region Private Fields

        private const string notSerializeDefaultValueChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardExpiration>05/17</CardExpiration>
  <CardNumber>0123456789</CardNumber>
</CreditCard>";

        private const string serializeDefaultValueChangedContext = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CreditCard>
  <CardCode>0</CardCode>
  <CardExpiration>05/17</CardExpiration>
  <CardNumber>0123456789</CardNumber>
</CreditCard>";

        #endregion Private Fields

        #region Public Methods

        [TestMethod]
        public void ExampleCombine()
        {
            CreditCard card1 = new CreditCard()
            {
                CardNumber = "0123456789",
                CardExpiration = "05/17",
                CardCode = 0
            };

            XPatchSerializer serializer = new XPatchSerializer(typeof(CreditCard));

            CreditCard card2 = null;
            using (StringReader reader = new StringReader(notSerializeDefaultValueChangedContext))
            {
                card2 = serializer.Combine(reader, null) as CreditCard;
            }

            Assert.AreEqual(card2, card1);
            Assert.AreNotEqual(card2.GetHashCode(), card1.GetHashCode());

            serializer = new XPatchSerializer(typeof(CreditCard), true);

            card2 = null;
            using (StringReader reader = new StringReader(notSerializeDefaultValueChangedContext))
            {
                card2 = serializer.Combine(reader, null) as CreditCard;
            }

            Assert.AreEqual(card2, card1);
            Assert.AreNotEqual(card2.GetHashCode(), card1.GetHashCode());
        }

        [TestMethod]
        public void ExampleDivide()
        {
            CreditCard card1 = new CreditCard()
            {
                CardNumber = "0123456789",
                CardExpiration = "05/17",
                CardCode = 0
            };

            XPatchSerializer serializer = new XPatchSerializer(typeof(CreditCard));

            string context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, card1);
                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(notSerializeDefaultValueChangedContext, context);
            Trace.WriteLine(context);

            serializer = new XPatchSerializer(typeof(CreditCard), true);

            context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, card1);
                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(serializeDefaultValueChangedContext, context);
            Trace.WriteLine(context);
        }

        #endregion Public Methods

        #region Public Classes

        public class CreditCard
        {
            #region Public Properties

            public int CardCode { get; set; }

            public string CardExpiration { get; set; }

            public string CardNumber { get; set; }

            #endregion Public Properties

            #region Public Methods

            public override bool Equals(object obj)
            {
                CreditCard card = obj as CreditCard;
                if (card == null)
                {
                    return false;
                }
                return string.Equals(this.CardNumber, card.CardNumber)
                    && string.Equals(this.CardExpiration, card.CardExpiration)
                    && string.Equals(this.CardCode, card.CardCode);
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }

    [TestClass]
    public class XPatchSerialzerExample
    {
        #region Private Fields

        private const string context = @"<?xml version=""1.0"" encoding=""utf-8""?>
<MyClass>
  <MyObjectProperty>
    <ObjectName>My String</ObjectName>
  </MyObjectProperty>
</MyClass>";

        #endregion Private Fields

        #region Public Methods

        [TestMethod]
        public void ExampleClassDef()
        {
            XPatchSerializer serializer = new XPatchSerializer(typeof(MyClass));

            MyClass c1 = new MyClass();
            c1.MyObjectProperty.ObjectName = "My String";

            string changedContext = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, c1);
                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    changedContext = stremReader.ReadToEnd();
                }
            }

            Assert.AreEqual(context, changedContext);
        }

        #endregion Public Methods

        #region Public Classes

        public class MyClass
        {
            #region Public Constructors

            public MyClass()
            {
                MyObjectProperty = new MyObject();
            }

            #endregion Public Constructors

            #region Public Fields

            public MyObject MyObjectProperty;

            #endregion Public Fields
        }

        public class MyObject
        {
            #region Public Fields

            public string ObjectName;

            #endregion Public Fields
        }

        #endregion Public Classes
    }
}