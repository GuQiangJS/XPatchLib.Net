using System;
using System.IO;
using System.Text;
using System.Xml;

namespace XPatchLib.Example
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
        public string Description { get; set; }
        public string ItemName { get; set; }

        public decimal LineTotal
        {
            get { return UnitPrice * Quantity; }
        }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class PurchaseOrder
    {
        public OrderedItem[] OrderedItems;
        public string OrderDate { get; set; }
        public decimal ShipCost { get; set; }
        public Address ShipTo { get; set; }

        public decimal SubTotal
        {
            get
            {
                var subTotal = new decimal();
                foreach (var oi in OrderedItems)
                    subTotal += oi.LineTotal;
                return subTotal;
            }
        }

        public decimal TotalCost
        {
            get { return SubTotal + ShipCost; }
        }
    }

    public class Test
    {
        public static void Main()
        {
            var t = new Test();

            t.Divide("patch.xml");
            t.Combine("patch.xml");
        }

        private void Divide(string filename)
        {
            //原始对象
            var order = CreatePuchaseOrder();
            //更新后的对象
            var newOrder = CreatePuchaseOrder();

            //改变更新后对象的内容
            //由于直接修改了主键的内容，将被视为删除了原有主键为Widget S的对象，并新增了主键为Widget B的对象。
            newOrder.OrderedItems[0].ItemName = "Widget B";
            newOrder.OrderedItems[0].Description = "Big widget";
            newOrder.OrderedItems[0].UnitPrice = (decimal) 26.78;
            newOrder.OrderedItems[0].Quantity = 5;

            newOrder.ShipCost = (decimal) 12.51;

            //产生增量内容
            var serializer = new Serializer(typeof(PurchaseOrder));
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
            {
                var settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;
                using (var xmlWriter = XmlWriter.Create(fs, settings))
                {
                    using (var writer = new XmlTextWriter(xmlWriter))
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
            foreach (var oi in order.OrderedItems)
                Console.WriteLine("\t" +
                                  oi.ItemName + "\t" +
                                  oi.Description + "\t" +
                                  oi.UnitPrice + "\t" +
                                  oi.Quantity + "\t" +
                                  oi.LineTotal);
            Console.WriteLine();
            Console.WriteLine("Subtotal:\t" + order.SubTotal);
            Console.WriteLine("Shipping:\t" + order.ShipCost);
            Console.WriteLine("Total:\t" + order.TotalCost);
        }

        private void Combine(string filename)
        {
            //合并增量内容至原始对象
            var serializer = new Serializer(typeof(PurchaseOrder));

            var oldOrder = CreatePuchaseOrder();
            PurchaseOrder newOrder = null;

            using (var fs = new FileStream(filename, FileMode.Open))
            {
                using (var xmlReader = XmlReader.Create(fs))
                {
                    using (var reader = new XmlTextReader(xmlReader))
                    {
                        newOrder = (PurchaseOrder) serializer.Combine(reader, oldOrder);
                    }
                }
            }

            Console.WriteLine("OldOrder: ");
            OuputPuchaseOrder(oldOrder);
            Console.WriteLine("---------- ");
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
            var result = new PurchaseOrder();

            var billAddress = new Address();
            billAddress.Name = "Teresa Atkinson";
            billAddress.Line1 = "1 Main St.";
            billAddress.City = "AnyTown";
            billAddress.State = "WA";
            billAddress.Zip = "00000";
            result.ShipTo = billAddress;
            result.OrderDate = DateTime.Now.ToLongDateString();

            var i1 = new OrderedItem();
            i1.ItemName = "Widget S";
            i1.Description = "Small widget";
            i1.UnitPrice = (decimal) 5.23;
            i1.Quantity = 3;

            OrderedItem[] items = {i1};
            result.OrderedItems = items;
            result.ShipCost = (decimal) 12.51;

            return result;
        }
    }
}