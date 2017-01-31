namespace XPatchLib.UnitTest.PetShopModelTests.Models
{
    [PrimaryKey("ItemId")]
    public class LineItemInfo
    {
        #region Public Constructors

        /// <summary>
        ///     Default constructor This is required by web services serialization mechanism
        /// </summary>
        public LineItemInfo()
        {
        }

        /// <summary>
        ///     Constructor with specified initial values
        /// </summary>
        /// <param name="id">
        ///     Item Id
        /// </param>
        /// <param name="line">
        ///     Line number
        /// </param>
        /// <param name="qty">
        ///     Quanity in order
        /// </param>
        /// <param name="price">
        ///     Order item price
        /// </param>
        public LineItemInfo(string id, string name, int line, int qty, decimal price)
        {
            ItemId = id;
            Name = name;
            Line = line;
            Price = price;
            Quantity = qty;
        }

        public override bool Equals(object obj)
        {
            var item = obj as LineItemInfo;
            if (item == null)
                return false;
            return ItemId.Equals(item.ItemId)
                   && Name.Equals(item.Name)
                   && Line.Equals(item.Line)
                   && Price.Equals(item.Price)
                   && Quantity.Equals(item.Quantity);
        }

        public override int GetHashCode()
        {
            var result = 0;
            if (ItemId != null)
                result ^= ItemId.GetHashCode();
            if (Name != null)
                result ^= Name.GetHashCode();

            result ^= Line.GetHashCode();

            result ^= Price.GetHashCode();

            result ^= Quantity.GetHashCode();
            return result;
        }

        #endregion Public Constructors

        #region Private Fields

        // Internal member variables 

        #endregion Private Fields

        #region Public Properties

        // Properties 
        public string ItemId { get; set; }

        public int Line { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Subtotal
        {
            get { return Price * Quantity; }
        }

        #endregion Public Properties
    }
}