using System;
using System.IO;
using System.Xml;
using XPatchLib;

namespace XPatchSerializerExample
{
    public class ConstructorDateModeExample
    {
        private void SerializeObject(string filename)
        {
            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderedItem), XmlDateTimeSerializationMode.Local);

            OrderedItem i = new OrderedItem();

            i.ItemName = "Widget";
            i.Quantity = 0;
            i.OrderDate = new DateTime(2015, 7, 8, 10, 0, 0);

            TextWriter writer = new StreamWriter(filename);

            serializer.Divide(writer, null, i);
            writer.Close();
        }

        public class OrderedItem
        {
            public string ItemName;
            public DateTime OrderDate;
            public int Quantity;
        }
    }
}