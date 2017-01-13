using System;
using System.Linq;

namespace XPatchLib.UnitTest.PetShopModelTests.Models
{
    [PrimaryKey("OrderId")]
    public class OrderInfo
    {
        #region Public Constructors

        /// <summary>
        /// Default constructor This is required by web services serialization mechanism 
        /// </summary>
        public OrderInfo()
        {
            this.LineItems = new LineItemInfo[] { };
        }

        /// <summary>
        /// Constructor with specified initial values 
        /// </summary>
        /// <param name="orderId">
        /// Unique identifier 
        /// </param>
        /// <param name="date">
        /// Order date 
        /// </param>
        /// <param name="userId">
        /// User placing order 
        /// </param>
        /// <param name="creditCard">
        /// Credit card used for order 
        /// </param>
        /// <param name="billing">
        /// Billing orderess for the order 
        /// </param>
        /// <param name="shipping">
        /// Shipping orderess for the order 
        /// </param>
        /// <param name="total">
        /// Order total value 
        /// </param>
        /// <param name="line">
        /// Ordered items 
        /// </param>
        /// <param name="authorization">
        /// Credit card authorization number 
        /// </param>
        public OrderInfo(int orderId, DateTime date, string userId, CreditCardInfo creditCard, AddressInfo billing, AddressInfo shipping, decimal total, LineItemInfo[] line, Nullable<int> authorization)
            : this()
        {
            this.orderId = orderId;
            this.date = date;
            this.userId = userId;
            this.creditCard = creditCard;
            this.billingAddress = billing;
            this.shippingAddress = shipping;
            this.orderTotal = total;
            this.lineItems = line;
            this.authorizationNumber = authorization;
        }

        public override bool Equals(object obj)
        {
            OrderInfo order = obj as OrderInfo;
            if (order == null)
            {
                return false;
            }
            return this.OrderId.Equals(order.OrderId)
                && this.Date.Equals(order.Date)
                && this.UserId.Equals(order.UserId)
                && this.CreditCard.Equals(order.CreditCard)
                && AddressInfo.Equals(this.BillingAddress, order.BillingAddress)
                && AddressInfo.Equals(this.ShippingAddress, order.ShippingAddress)
                && this.OrderTotal.Equals(order.OrderTotal)
                && this.LineItems.Length.Equals(order.LineItems.Length)
                && ((this.LineItems.Except(order.LineItems).Count() > 0) ? false : true)
                && this.AuthorizationNumber.Equals(order.AuthorizationNumber);
        }

        public override int GetHashCode()
        {
            int result = 0;

            result ^= this.OrderId.GetHashCode();

            if (this.Date != null)
            {
                result ^= this.Date.GetHashCode();
            }
            if (this.UserId != null)
            {
                result ^= this.UserId.GetHashCode();
            }
            if (this.CreditCard != null)
            {
                result ^= this.CreditCard.GetHashCode();
            }
            if (this.BillingAddress != null)
            {
                result ^= this.BillingAddress.GetHashCode();
            }
            if (this.ShippingAddress != null)
            {
                result ^= this.ShippingAddress.GetHashCode();
            }

            result ^= this.OrderTotal.GetHashCode();

            if (this.LineItems != null)
            {
                result ^= this.LineItems.GetHashCode();
            }
            if (this.AuthorizationNumber != null)
            {
                result ^= this.AuthorizationNumber.GetHashCode();
            }
            return result;
        }

        #endregion Public Constructors

        #region Private Fields

        private Nullable<int> authorizationNumber;

        private AddressInfo billingAddress;

        private CreditCardInfo creditCard;

        private DateTime date;

        private LineItemInfo[] lineItems;

        // Internal member variables 
        private int orderId;

        private decimal orderTotal;
        private AddressInfo shippingAddress;
        private string userId;

        #endregion Private Fields

        #region Public Properties

        public Nullable<int> AuthorizationNumber
        {
            get { return authorizationNumber; }
            set { authorizationNumber = value; }
        }

        public AddressInfo BillingAddress
        {
            get { return billingAddress; }
            set { billingAddress = value; }
        }

        public CreditCardInfo CreditCard
        {
            get { return creditCard; }
            set { creditCard = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public LineItemInfo[] LineItems
        {
            get { return lineItems; }
            set { lineItems = value; }
        }

        // Properties 
        public int OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }

        public decimal OrderTotal
        {
            get { return orderTotal; }
            set { orderTotal = value; }
        }

        public AddressInfo ShippingAddress
        {
            get { return shippingAddress; }
            set { shippingAddress = value; }
        }

        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        #endregion Public Properties
    }
}