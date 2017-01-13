namespace XPatchLib.UnitTest.PetShopModelTests.Models
{
    [PrimaryKey("ItemId")]
    public class LineItemInfo
    {
        #region Public Constructors

        /// <summary>
        /// Default constructor This is required by web services serialization mechanism 
        /// </summary>
        public LineItemInfo()
        {
        }

        /// <summary>
        /// Constructor with specified initial values 
        /// </summary>
        /// <param name="id">
        /// Item Id 
        /// </param>
        /// <param name="line">
        /// Line number 
        /// </param>
        /// <param name="qty">
        /// Quanity in order 
        /// </param>
        /// <param name="price">
        /// Order item price 
        /// </param>
        public LineItemInfo(string id, string name, int line, int qty, decimal price)
        {
            this.id = id;
            this.productName = name;
            this.line = line;
            this.price = price;
            this.quantity = qty;
        }

        public override bool Equals(object obj)
        {
            LineItemInfo item = obj as LineItemInfo;
            if (item == null)
            {
                return false;
            }
            return this.ItemId.Equals(item.ItemId)
                && this.Name.Equals(item.Name)
                && this.Line.Equals(item.Line)
                && this.Price.Equals(item.Price)
                && this.Quantity.Equals(item.Quantity);
        }

        public override int GetHashCode()
        {
            int result = 0;
            if (this.ItemId != null)
            {
                result ^= this.ItemId.GetHashCode();
            }
            if (this.Name != null)
            {
                result ^= this.Name.GetHashCode();
            }

            result ^= this.Line.GetHashCode();

            result ^= this.Price.GetHashCode();

            result ^= this.Quantity.GetHashCode();
            return result;
        }

        #endregion Public Constructors

        #region Private Fields

        // Internal member variables 
        private string id;

        private int line;
        private decimal price;
        private string productName;
        private int quantity;

        #endregion Private Fields

        #region Public Properties

        // Properties 
        public string ItemId
        {
            get { return id; }
            set { id = value; }
        }

        public int Line
        {
            get { return line; }
            set { line = value; }
        }

        public string Name
        {
            get { return productName; }
            set { productName = value; }
        }

        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public decimal Subtotal
        {
            get { return price * quantity; }
        }

        #endregion Public Properties
    }
}