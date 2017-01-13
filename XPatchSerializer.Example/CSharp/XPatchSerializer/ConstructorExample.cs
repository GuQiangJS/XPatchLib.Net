using System.IO;
using XPatchLib;

namespace XPatchSerializerExample
{
    public class ConstructorExample
    {
        #region Private Methods

        private void SerializeObject(string filename)
        {
            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderedItem));

            OrderedItem i = new OrderedItem();

            i.ItemName = "Widget";
            i.Description = "Regular Widget";
            i.Quantity = 10;
            i.UnitPrice = (decimal)2.30;

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
            public int Quantity;
            public decimal UnitPrice;

            #endregion Public Fields
        }

        #endregion Public Classes
    }
}