using System;
using System.IO;
using XPatchLib;

namespace XPatchSerializerExample
{
    public class CombineTextReaderExample
    {
        public static void Main()
        {
            CombineTextReaderExample t = new CombineTextReaderExample();
            t.CombineObject("patch.xml");
        }

        private void CombineObject(string filename)
        {
            Console.WriteLine("Reading with TextReader");
            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderedItem));
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
            TextReader reader = new StreamReader(fs);

            OrderedItem oldOrderItem = new OrderedItem
            {
                Description = "Big Widget",
                ItemName = "Widgt",
                Quantity = 0,
                UnitPrice = (decimal) 4.7
            };

            // 采用不改动现有 oldOrderItem 合并方式，将增量内容与 oldOrderItem 内容进行合并，产生新的对象实例 newOrderItem
            // newOrderItem 与 oldOrderItem 非同一对象。
            OrderedItem newOrderItem = (OrderedItem) serializer.Combine(reader, oldOrderItem);

            fs.Close();

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