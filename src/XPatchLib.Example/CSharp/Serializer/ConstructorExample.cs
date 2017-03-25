using System.IO;
using XPatchLib;

namespace XmlSerializerExample
{
    public class ConstructorExample
    {
        private void SerializeObject(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(OrderedItem));

            OrderedItem i = new OrderedItem();

            i.ItemName = "Widget";
            i.Description = "Regular Widget";
            i.Quantity = 10;
            i.UnitPrice = (decimal) 2.30;

            TextWriter writer = new StreamWriter(filename);

            serializer.Divide(writer, null, i);
            writer.Close();
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