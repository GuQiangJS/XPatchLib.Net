using System;
using System.IO;
using System.Xml;
using XPatchLib;

namespace XPatchSerializerExample
{
    public class CombineXmlReaderOverrideExample
    {
        #region Public Methods

        public static void Main()
        {
            CombineXmlReaderOverrideExample t = new CombineXmlReaderOverrideExample();
            t.CombineObject("patch.xml");
        }

        #endregion Public Methods

        #region Private Methods

        private void CombineObject(string filename)
        {
            Console.WriteLine("Reading with XmlReader");
            XPatchSerializer serializer = new XPatchSerializer(typeof(OrderedItem));
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
            XmlReader reader = XmlReader.Create(fs);

            OrderedItem oldOrderItem = new OrderedItem()
            {
                Description = "Big Widget",
                ItemName = "Widgt",
                Quantity = 0,
                UnitPrice = (decimal)4.7
            };

            // 采用覆盖现有 oldOrderItem 合并方式，将增量内容与 oldOrderItem 内容进行合并，将增量内容直接变更至 oldOrderItem
            // newOrderItem 与 oldOrderItem 为同一对象。
            OrderedItem newOrderItem = (OrderedItem)serializer.Combine(reader, oldOrderItem, true);

            fs.Close();

            Console.Write(
            newOrderItem.ItemName + "\t" +
            newOrderItem.Description + "\t" +
            newOrderItem.UnitPrice + "\t" +
            newOrderItem.Quantity + "\t");

            Console.Write(oldOrderItem.GetHashCode());
            Console.Write(newOrderItem.GetHashCode());
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