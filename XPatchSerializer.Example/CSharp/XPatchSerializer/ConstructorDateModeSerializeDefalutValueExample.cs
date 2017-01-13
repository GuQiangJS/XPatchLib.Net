using System;
using System.IO;
using XPatchLib;

namespace XPatchSerializerExample
{
    public class ConstructorDateModeSerializeDefalutValueExample
    {
        #region Private Methods

        private void SerializeObject(string filename)
        {
            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderedItem), System.Xml.XmlDateTimeSerializationMode.Utc, false);

            OrderedItem i = new OrderedItem();

            i.ItemName = "Widget";
            i.Description = "Regular Widget";
            //此处Quantity与默认值相同将不做序列化
            i.Quantity = 0;
            i.UnitPrice = (decimal)2.30;
            i.OrderDate = new DateTime(2015, 7, 8, 10, 0, 0);

            TextWriter writer = new StreamWriter(filename);

            serializer.Divide(writer, null, i);
            writer.Close();
        }

        #endregion Private Methods

        #region Public Classes

        public class OrderedItem
        {
            #region Public Fields

            public string Description;
            public string ItemName;
            public DateTime OrderDate;
            public int Quantity;
            public decimal UnitPrice;

            #endregion Public Fields
        }

        #endregion Public Classes
    }
}