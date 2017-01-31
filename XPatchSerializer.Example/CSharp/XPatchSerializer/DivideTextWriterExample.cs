using System;
using System.IO;
using System.Text;
using XPatchLib;

namespace XPatchSerializerExample
{
    public class DivideTextWriterExample
    {
        public static void Main(string[] args)
        {
            DivideTextWriterExample t = new DivideTextWriterExample();

            t.Divide("patch.xml");
        }

        private void Divide(string filename)
        {
            Console.WriteLine("Writing With TextWriter");

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

            FileStream fs = new FileStream(filename, FileMode.Create);
            TextWriter writer = new StreamWriter(fs, new UTF8Encoding());
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