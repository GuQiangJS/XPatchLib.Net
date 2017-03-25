using System;
using System.IO;
using System.Xml;

namespace XPatchLib.Example
{
    public class ThrowAttributeMissException
    {
        public static void Main()
        {
            var t = new ThrowAttributeMissException();

            t.Divide();
        }

        private void Divide()
        {
            var order = CreatePuchaseOrder();
            var newOrder = CreatePuchaseOrder();

            newOrder.OrderedItems[0].ItemName = "Widget B";
            
            var serializer = new Serializer(typeof(PurchaseOrder));
            using (var fs = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(fs))
                {
                    using (var writer = new XmlTextWriter(xmlWriter))
                    {
                        try
                        {
                            //由于 OrderedItem 类型没有定义主键，所以在序列化/反序列化 集合对象时会抛出 AttributeMissException 异常。
                            serializer.Divide(writer, order, newOrder);
                        }
                        catch (AttributeMissException ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.AttributeName);
                            Console.WriteLine(ex.ErrorType);
                        }
                    }
                }
            }
        }

        private PurchaseOrder CreatePuchaseOrder()
        {
            var result = new PurchaseOrder();

            var i1 = new OrderedItem();
            i1.ItemName = "Widget S";

            OrderedItem[] items = {i1};
            result.OrderedItems = items;

            return result;
        }

        public class OrderedItem
        {
            public string ItemName { get; set; }
        }

        public class PurchaseOrder
        {
            public OrderedItem[] OrderedItems;
        }
    }
}