using System;
using System.IO;
using XPatchLib;

namespace XPatchSerializerExample
{
    public class CombineStreamExample
    {
        public static void Main()
        {
            CombineStreamExample t = new CombineStreamExample();
            t.CombineObject("patch.xml");
        }

        private void CombineObject(string filename)
        {
            Console.WriteLine("Reading with Stream");
            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderedItem));
            Stream reader = new FileStream(filename, FileMode.Open);

            OrderedItem oldOrderItem = new OrderedItem
            {
                Description = "Small Widget",
                ItemName = "Widgt",
                Quantity = 0,
                UnitPrice = (decimal) 4.7
            };

            // 采用不改动现有 oldOrderItem 合并方式，将增量内容与 oldOrderItem 内容进行合并，产生新的对象实例 newOrderItem
            // newOrderItem 与 oldOrderItem 非同一对象。
            OrderedItem newOrderItem = (OrderedItem) serializer.Combine(reader, oldOrderItem);

            Console.Write(
                newOrderItem.ItemName + "\t" +
                newOrderItem.Description + "\t" +
                newOrderItem.UnitPrice + "\t" +
                newOrderItem.Quantity + "\t");

            Console.Write(oldOrderItem.GetHashCode());
            Console.Write(newOrderItem.GetHashCode());
        }

        public class OrderedItem
        {
            public string Description;
            public string ItemName;
            public int Quantity;
            public decimal UnitPrice;
        }
    }
}