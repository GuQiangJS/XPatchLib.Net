using System;
using System.IO;
using XPatchLib;

namespace XPatchSerializerExample
{
    public class Address
    {
        #region Public Fields

        public string City;
        public string Line1;
        public string Name;
        public string State;
        public string Zip;

        #endregion Public Fields
    }

    [PrimaryKey("ItemName")]
    public class OrderedItem
    {
        #region Public Fields

        public string Description;
        public string ItemName;
        public decimal LineTotal;
        public int Quantity;
        public decimal UnitPrice;

        #endregion Public Fields

        #region Public Methods

        public void Calculate()
        {
            LineTotal = UnitPrice * Quantity;
        }

        #endregion Public Methods
    }

    public class PurchaseOrder
    {
        #region Public Fields

        public string OrderDate;
        public OrderedItem[] OrderedItems;
        public decimal ShipCost;
        public Address ShipTo;
        public decimal SubTotal;
        public decimal TotalCost;

        #endregion Public Fields
    }

    public class Test
    {
        #region Public Methods

        public static void Main()
        {
            Test t = new Test();

            t.Create("po.xml");
            t.Read("po.xml");
        }

        private void Create(string filename)
        {
            //原始对象
            PurchaseOrder order = CreatePuchaseOrder();
            //更新后的对象
            PurchaseOrder newOrder = CreatePuchaseOrder();

            //改变更新后对象的内容
            newOrder.OrderedItems[0].ItemName = "Widget B";
            newOrder.OrderedItems[0].Description = "Big widget";
            newOrder.OrderedItems[0].UnitPrice = (decimal)26.78;
            newOrder.OrderedItems[0].Quantity = 5;

            decimal subTotal = new decimal();
            foreach (OrderedItem oi in newOrder.OrderedItems)
            {
                subTotal += oi.LineTotal;
            }
            newOrder.SubTotal = subTotal;
            newOrder.ShipCost = (decimal)12.51;
            newOrder.TotalCost = newOrder.SubTotal + newOrder.ShipCost;

            //产生增量内容
            XPatchSerializer serializer = new XPatchSerializer(typeof(PurchaseOrder));
            TextWriter writer = new StreamWriter(filename);
            serializer.Divide(writer, order, newOrder);
            writer.Close();
        }

        private void OuputPuchaseOrder(PurchaseOrder order)
        {
            Console.WriteLine("OrderDate: " + order.OrderDate);
            ReadAddress(order.ShipTo, "Ship To:");
            Console.WriteLine("Items to be shipped:");
            foreach (OrderedItem oi in order.OrderedItems)
            {
                Console.WriteLine("\t" +
                oi.ItemName + "\t" +
                oi.Description + "\t" +
                oi.UnitPrice + "\t" +
                oi.Quantity + "\t" +
                oi.LineTotal);
            }
            Console.WriteLine("\t\t Subtotal\t" + order.SubTotal);
            Console.WriteLine("\t\t Shipping\t" + order.ShipCost);
            Console.WriteLine("\t\t Total\t\t" + order.TotalCost);
        }

        private void Read(string filename)
        {
            //合并增量内容至原始对象
            XPatchSerializer serializer = new XPatchSerializer(typeof(PurchaseOrder));
            FileStream fs = new FileStream(filename, FileMode.Open);
            PurchaseOrder oldOrder = CreatePuchaseOrder();
            PurchaseOrder newOrder = (PurchaseOrder)serializer.Combine(fs, oldOrder);

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

        #endregion Public Methods

        #region Private Methods

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
            result.OrderDate = System.DateTime.Now.ToLongDateString();

            OrderedItem i1 = new OrderedItem();
            i1.ItemName = "Widget S";
            i1.Description = "Small widget";
            i1.UnitPrice = (decimal)5.23;
            i1.Quantity = 3;
            i1.Calculate();

            OrderedItem[] items = { i1 };
            result.OrderedItems = items;

            decimal subTotal = new decimal();
            foreach (OrderedItem oi in items)
            {
                subTotal += oi.LineTotal;
            }
            result.SubTotal = subTotal;
            result.ShipCost = (decimal)12.51;
            result.TotalCost = result.SubTotal + result.ShipCost;

            return result;
        }

        #endregion Private Methods
    }
}