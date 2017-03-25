using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace XPatchLib.Example
{
    public class RegisteTypesExample
    {
        public static void Main(string[] args)
        {
            var t = new RegisteTypesExample();

            t.Divide();
        }

        private void Divide()
        {
            var oldItems = new List<OrderedItem> {new OrderedItem {ItemName = "Item A", Quantity = 10}};
            var newItems = new List<OrderedItem> {new OrderedItem {ItemName = "Item A", Quantity = 5}};

            var serializer = new Serializer(typeof(List<OrderedItem>));
            //当OrderItem类型上未标记PrimaryKeyAttribute时，可以通过RegisterTypes方法向系统注册类型与主键的关系
            var types = new Dictionary<Type, string[]>();
            types.Add(typeof(OrderedItem), new[] {"ItemName"});
            serializer.RegisterTypes(types);

            using (var ms = new MemoryStream())
            {
                var settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;
                using (var writer = new XmlTextWriter(XmlWriter.Create(ms, settings)))
                {
                    serializer.Divide(writer, oldItems, newItems);
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