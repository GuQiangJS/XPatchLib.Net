using System;
using System.Linq;

namespace XPatchLib.UnitTest.PetShopModelTests.Models
{
    [PrimaryKey("OrderId")]
    public class OrderInfo
    {
        #region Public Constructors

        /// <summary>
        ///     Default constructor This is required by web services serialization mechanism
        /// </summary>
        public OrderInfo()
        {
            LineItems = new LineItemInfo[] {};
        }

        /// <summary>
        ///     Constructor with specified initial values
        /// </summary>
        /// <param name="orderId">
        ///     Unique identifier
        /// </param>
        /// <param name="date">
        ///     Order date
        /// </param>
        /// <param name="userId">
        ///     User placing order
        /// </param>
        /// <param name="creditCard">
        ///     Credit card used for order
        /// </param>
        /// <param name="billing">
        ///     Billing orderess for the order
        /// </param>
        /// <param name="shipping">
        ///     Shipping orderess for the order
        /// </param>
        /// <param name="total">
        ///     Order total value
        /// </param>
        /// <param name="line">
        ///     Ordered items
        /// </param>
        /// <param name="authorization">
        ///     Credit card authorization number
        /// </param>
        public OrderInfo(int orderId, DateTime date, string userId, CreditCardInfo creditCard, AddressInfo billing,
            AddressInfo shipping, decimal total, LineItemInfo[] line, int? authorization)
            : this()
        {
            OrderId = orderId;
            Date = date;
            UserId = userId;
            CreditCard = creditCard;
            BillingAddress = billing;
            ShippingAddress = shipping;
            OrderTotal = total;
            LineItems = line;
            AuthorizationNumber = authorization;
        }

        public override bool Equals(object obj)
        {
            var order = obj as OrderInfo;
            if (order == null)
                return false;
            return OrderId.Equals(order.OrderId)
                   && Date.Equals(order.Date)
                   && UserId.Equals(order.UserId)
                   && CreditCard.Equals(order.CreditCard)
                   && Equals(BillingAddress, order.BillingAddress)
                   && Equals(ShippingAddress, order.ShippingAddress)
                   && OrderTotal.Equals(order.OrderTotal)
                   && LineItems.Length.Equals(order.LineItems.Length)
                   && (LineItems.Except(order.LineItems).Count() > 0 ? false : true)
                   && AuthorizationNumber.Equals(order.AuthorizationNumber);
        }

        public override int GetHashCode()
        {
            var result = 0;

            result ^= OrderId.GetHashCode();

            if (Date != null)
                result ^= Date.GetHashCode();
            if (UserId != null)
                result ^= UserId.GetHashCode();
            if (CreditCard != null)
                result ^= CreditCard.GetHashCode();
            if (BillingAddress != null)
                result ^= BillingAddress.GetHashCode();
            if (ShippingAddress != null)
                result ^= ShippingAddress.GetHashCode();

            result ^= OrderTotal.GetHashCode();

            if (LineItems != null)
                result ^= LineItems.GetHashCode();
            if (AuthorizationNumber != null)
                result ^= AuthorizationNumber.GetHashCode();
            return result;
        }

        #endregion Public Constructors

        #region Private Fields

        // Internal member variables 

        #endregion Private Fields

        #region Public Properties

        public int? AuthorizationNumber { get; set; }

        public AddressInfo BillingAddress { get; set; }

        public CreditCardInfo CreditCard { get; set; }

        public DateTime Date { get; set; }

        public LineItemInfo[] LineItems { get; set; }

        // Properties 
        public int OrderId { get; set; }

        public decimal OrderTotal { get; set; }

        public AddressInfo ShippingAddress { get; set; }

        public string UserId { get; set; }

        #endregion Public Properties
    }
}