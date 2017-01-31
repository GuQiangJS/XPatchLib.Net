using System.IO;
using XPatchLib;

namespace XPatchSerializerExample
{
    public class ConstructorSerializeDefalutValueExample
    {
        private void SerializeObject(string filename)
        {
            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderedItem), true);

            OrderedItem i = new OrderedItem();

            i.ItemName = "Widget";
            i.Description = "Regular Widget";
            //此处Quantity与默认值相同，但是标记了序列化默认值，所以Quantity也会被序列化至结果中。
            i.Quantity = 0;
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