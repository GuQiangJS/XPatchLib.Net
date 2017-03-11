using System;
using System.IO;
using System.Xml;

namespace XPatchLib.Example
{
    public class CombineExample
    {
        public static void Main()
        {
            var example = new CombineExample();
            example.CombineObject("patch.xml");
        }

        private void CombineObject(string filename)
        {
            OrderedItem oldOrderItem = null;
            OrderedItem newOrderItem = null;

            var serializer = new Serializer(typeof(OrderedItem));
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                using (var xmlReader = XmlReader.Create(fs))
                {
                    oldOrderItem = new OrderedItem
                    {
                        Description = "Big Widget",
                        ItemName = "Widgt",
                        Quantity = 0,
                        UnitPrice = (decimal) 4.7
                    };

                    // 采用不改动现有 oldOrderItem 合并方式，将增量内容与 oldOrderItem 内容进行合并，产生新的对象实例 newOrderItem
                    // newOrderItem 与 oldOrderItem 非同一对象。
                    using (var reader = new XmlTextReader(xmlReader))
                    {
                        newOrderItem = (OrderedItem) serializer.Combine(reader, oldOrderItem);
                    }
                }
            }

            Console.WriteLine("OldInstance:");
            Write(oldOrderItem);
            Console.WriteLine("NewInstance:");
            Write(newOrderItem);
        }

        private void Write(OrderedItem item)
        {
            Console.WriteLine(item.ItemName);
            Console.WriteLine(item.Description);
            Console.WriteLine(item.UnitPrice);
            Console.WriteLine(item.Quantity);
            Console.WriteLine(item.GetHashCode());
            Console.WriteLine("-----");
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