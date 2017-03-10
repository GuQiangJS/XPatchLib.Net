using System;
using System.IO;
using System.Xml;
using XPatchLib;

namespace XmlSerializerExample
{
    public class ConstructorDateModeSerializeDefalutValueExample
    {
        private void SerializeObject(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(OrderedItem), DateTimeSerializationMode.Utc,
                false);

            OrderedItem i = new OrderedItem();

            i.ItemName = "Widget";
            i.Description = "Regular Widget";
            //此处Quantity与默认值相同将不做序列化
            i.Quantity = 0;
            i.UnitPrice = (decimal) 2.30;
            i.OrderDate = new DateTime(2015, 7, 8, 10, 0, 0);

            TextWriter writer = new StreamWriter(filename);

            serializer.Divide(writer, null, i);
            writer.Close();
        }

        public class OrderedItem
        {
            public string Description;
            public string ItemName;
            public DateTime OrderDate;
            public int Quantity;
            public decimal UnitPrice;
        }
    }
}