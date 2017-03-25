using System;
using System.IO;
using System.Text;
using System.Xml;
using XPatchLib;
using XmlTextReader = XPatchLib.XmlTextReader;
using XmlTextWriter = XPatchLib.XmlTextWriter;

namespace SerializerExample
{
    public class Address
    {
        public string City;
        public string Line1;
        public string Name;
        public string State;
        public string Zip;
    }

    [PrimaryKey("ItemName")]
    public class OrderedItem
    {
        public string Description;
        public string ItemName;
        public decimal LineTotal;
        public int Quantity;
        public decimal UnitPrice;

        public void Calculate()
        {
            LineTotal = UnitPrice * Quantity;
        }
    }

    public class PurchaseOrder
    {
        public string OrderDate;
        public OrderedItem[] OrderedItems;
        public decimal ShipCost;
        public Address ShipTo;
        public decimal SubTotal;
        public decimal TotalCost;
    }

    public class Test
    {
        public static void Main()
        {
            Test t = new Test();

            t.Divide("po.xml");
            t.Combine("po.xml");
        }

        private void Divide(string filename)
        {
            //原始对象
            PurchaseOrder order = CreatePuchaseOrder();
            //更新后的对象
            PurchaseOrder newOrder = CreatePuchaseOrder();

            //改变更新后对象的内容
            newOrder.OrderedItems[0].ItemName = "Widget B";
            newOrder.OrderedItems[0].Description = "Big widget";
            newOrder.OrderedItems[0].UnitPrice = (decimal) 26.78;
            newOrder.OrderedItems[0].Quantity = 5;

            decimal subTotal = new decimal();
            foreach (OrderedItem oi in newOrder.OrderedItems)
                subTotal += oi.LineTotal;
            newOrder.SubTotal = subTotal;
            newOrder.ShipCost = (decimal) 12.51;
            newOrder.TotalCost = newOrder.SubTotal + newOrder.ShipCost;

            //产生增量内容
            Serializer serializer = new Serializer(typeof(PurchaseOrder));
            using (FileStream fs = new FileStream(filename, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                using (System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(fs, Encoding.UTF8))
                {
                    using (XmlTextWriter writer = new XmlTextWriter(xmlWriter))
                    {
                        serializer.Divide(writer, order, newOrder);
                    }
                }
            }
        }

        private void OuputPuchaseOrder(PurchaseOrder order)
        {
            Console.WriteLine("OrderDate: " + order.OrderDate);
            ReadAddress(order.ShipTo, "Ship To:");
            Console.WriteLine("Items to be shipped:");
            foreach (OrderedItem oi in order.OrderedItems)
                Console.WriteLine("\t" +
                                  oi.ItemName + "\t" +
                                  oi.Description + "\t" +
                                  oi.UnitPrice + "\t" +
                                  oi.Quantity + "\t" +
                                  oi.LineTotal);
            Console.WriteLine("\t\t Subtotal\t" + order.SubTotal);
            Console.WriteLine("\t\t Shipping\t" + order.ShipCost);
            Console.WriteLine("\t\t Total\t\t" + order.TotalCost);
        }

        private void Combine(string filename)
        {
            //合并增量内容至原始对象
            Serializer serializer = new Serializer(typeof(PurchaseOrder));

            PurchaseOrder oldOrder = CreatePuchaseOrder();
            PurchaseOrder newOrder = null;

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (XmlReader xmlReader = XmlReader.Create(fs))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        newOrder = (PurchaseOrder) serializer.Combine(reader, oldOrder);
                    }
                }
            }

            Console.WriteLine("OldOrder: ");
            OuputPuchaseOrder(oldOrder);
            Console.WriteLine("NewOrder: ");
            OuputPuchaseOrder(newOrder);
        }

        private void ReadAddress(Address a, string label)
        {
            Console.WriteLine(label);
            Console.WriteLine("\t" + a.Name);
            Console.WriteLine("\t" + a.Line1);
            Console.WriteLine("\t" + a.City);
            Console.WriteLine("\t" + a.State);
            Console.WriteLine("\t" + a.Zip);
            Console.WriteLine();
        }

        private PurchaseOrder CreatePuchaseOrder()
        {
            PurchaseOrder result = new PurchaseOrder();

            Address billAddress = new Address();
            billAddress.Name = "Teresa Atkinson";
            billAddress.Line1 = "1 Main St.";
            billAddress.City = "AnyTown";
            billAddress.State = "WA";
            billAddress.Zip = "00000";
            result.ShipTo = billAddress;
            result.OrderDate = DateTime.Now.ToLongDateString();

            OrderedItem i1 = new OrderedItem();
            i1.ItemName = "Widget S";
            i1.Description = "Small widget";
            i1.UnitPrice = (decimal) 5.23;
            i1.Quantity = 3;
            i1.Calculate();

            OrderedItem[] items = {i1};
            result.OrderedItems = items;

            decimal subTotal = new decimal();
            foreach (OrderedItem oi in items)
                subTotal += oi.LineTotal;
            result.SubTotal = subTotal;
            result.ShipCost = (decimal) 12.51;
            result.TotalCost = result.SubTotal + result.ShipCost;

            return result;
        }
    }
}