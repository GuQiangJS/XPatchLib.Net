using System;
using System.IO;
using XPatchLib;

namespace XPatchSerializerExample
{
    public class ConstructorDateModeExample
    {
        #region Private Methods

        private void SerializeObject(string filename)
        {
            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderedItem), System.Xml.XmlDateTimeSerializationMode.Local);

            OrderedItem i = new OrderedItem();

            i.ItemName = "Widget";
            i.Quantity = 0;
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

            public string ItemName;
            public DateTime OrderDate;
            public int Quantity;

            #endregion Public Fields
        }

        #endregion Public Classes
    }
}