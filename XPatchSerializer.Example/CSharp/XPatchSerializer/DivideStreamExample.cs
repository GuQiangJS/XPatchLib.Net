using System;
using System.IO;
using XPatchLib;

namespace XPatchSerializerExample
{
    public class DivideStreamExample
    {
        public static void Main(string[] args)
        {
            DivideStreamExample t = new DivideStreamExample();

            t.Divide("patch.xml");
        }

        private void Divide(string filename)
        {
            Console.WriteLine("Writing With Stream");

            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderedItem));
            //原始对象
            OrderedItem oldOrderItem = new OrderedItem();
            oldOrderItem.ItemName = "Widget";
            oldOrderItem.Description = "Small Widget";
            oldOrderItem.Quantity = 10;
            oldOrderItem.UnitPrice = (decimal) 2.30;
            oldOrderItem.Calculate();
            //更新后对象
            OrderedItem newOrderItem = new OrderedItem();
            newOrderItem.ItemName = "Widget";
            newOrderItem.Description = "Big Widget";
            newOrderItem.Quantity = 15;
            newOrderItem.UnitPrice = (decimal) 7.80;
            newOrderItem.Calculate();

            Stream writer = new FileStream(filename, FileMode.Create);
            serializer.Divide(writer, oldOrderItem, newOrderItem);
            writer.Close();
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