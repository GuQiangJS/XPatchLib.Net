using System;
using System.IO;
using System.Text;
using System.Xml;

namespace XPatchLib.Example
{
    public class DivideExample
    {
        public static void Main(string[] args)
        {
            var t = new DivideExample();
            t.Divide();
        }

        private void Divide()
        {
            var serializer = new Serializer(typeof(OrderedItem));
            //原始对象
            var oldOrderItem = new OrderedItem();
            oldOrderItem.ItemName = "Widget";
            oldOrderItem.Description = "Small Widget";
            oldOrderItem.Quantity = 10;
            oldOrderItem.UnitPrice = (decimal) 2.30;
            oldOrderItem.Calculate();
            //更新后对象
            var newOrderItem = new OrderedItem();
            newOrderItem.ItemName = "Widget";
            newOrderItem.Description = "Big Widget";
            newOrderItem.Quantity = 15;
            newOrderItem.UnitPrice = (decimal) 7.80;
            newOrderItem.Calculate();

            using (var ms = new MemoryStream())
            {
                var settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;
                using (var writer = new XmlTextWriter(XmlWriter.Create(ms, settings)))
                {
                    serializer.Divide(writer, oldOrderItem, newOrderItem);
                }
                ms.Position = 0;
                using (var stremReader = new StreamReader(ms, Encoding.UTF8))
                {
                    Console.WriteLine(stremReader.ReadToEnd());
                }
            }
        }

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
    }
}