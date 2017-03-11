using System;
using System.IO;
using System.Text;
using System.Xml;
using XPatchLib;

namespace XmlSerializerExample
{
    public class PerformanceAnalysis
    {
        #region Public Methods

        public static void Main()
        {
            Serializer serializer = new Serializer(typeof(OrderedItem));

            for (int i = 0; i < 100000; i++)
            {
                Console.WriteLine(i);
                PerformanceAnalysis t = new PerformanceAnalysis();
                string s = t.SerializeObject(serializer);

                using (TextReader r = new StringReader(s))
                {
                    OrderedItem item = t.DeserializeObject(r, serializer);
                    Console.WriteLine(Equals(item, t.CreateRevItem()));
                }
            }
        }

        public OrderedItem DeserializeObject(TextReader stream, Serializer serialzier)
        {
            return serialzier.Combine(stream, CreateOriItem()) as OrderedItem;
        }

        public string SerializeObject(Serializer serializer)
        {
            string result = string.Empty;

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Divide(ms, CreateOriItem(), CreateRevItem());
                ms.Position = 0;
                using (StreamReader stremReader = new StreamReader(ms, Encoding.UTF8))
                {
                    result = stremReader.ReadToEnd();
                }
            }

            return result;
        }

        private OrderedItem CreateOriItem()
        {
            OrderedItem _item = new OrderedItem();

            _item.ItemName = "Widget";
            _item.Description = "Small Widget";
            //此处Quantity与默认值相同将不做序列化
            _item.Quantity = 0;
            _item.UnitPrice = (decimal)2.30;
            _item.OrderDate = new DateTime(2015, 7, 8, 10, 0, 0);

            return _item;
        }

        private OrderedItem CreateRevItem()
        {
            OrderedItem _item = new OrderedItem();

            _item.ItemName = "Widget";
            _item.Description = "Big Widget";
            //此处Quantity与默认值相同将不做序列化
            _item.Quantity = 10;
            _item.UnitPrice = (decimal)4.30;
            _item.OrderDate = new DateTime(2015, 8, 9, 11, 0, 0);

            return _item;
        }

        #endregion Public Methods

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

            #region Public Methods

            public override bool Equals(object obj)
            {
                OrderedItem item = obj as OrderedItem;

                if (item == null)
                {
                    return false;
                }

                return string.Equals(Description, item.Description)
                    && string.Equals(ItemName, item.ItemName)
                    && Equals(Quantity, item.Quantity)
                    && decimal.Equals(UnitPrice, item.UnitPrice)
                    && DateTime.Equals(OrderDate, item.OrderDate);
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }
}