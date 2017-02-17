using System;

namespace XPatchLib.UnitTest.PetShopModelTests.Models
{
    [PrimaryKey("CardId")]
    public class CreditCardInfo
    {
        #region Public Constructors

        /// <summary>
        ///     Default constructor
        /// </summary>
        public CreditCardInfo()
        {
        }

        /// <summary>
        ///     Constructor with specified initial values
        /// </summary>
        /// <param name="cardType">
        ///     Card type, e.g. Visa, Master Card, American Express
        /// </param>
        /// <param name="cardNumber">
        ///     Number on the card
        /// </param>
        /// <param name="cardExpiration">
        ///     Expiry Date, form MM/YY
        /// </param>
        public CreditCardInfo(string cardType, string cardNumber, string cardExpiration)
        {
            CardId = Guid.NewGuid();
            CardType = cardType;
            CardNumber = cardNumber;
            CardExpiration = cardExpiration;
        }

        public override bool Equals(object obj)
        {
            var card = obj as CreditCardInfo;
            if (card == null)
                return false;
            return CardId.Equals(card.CardId)
                   && CardType.Equals(card.CardType)
                   && CardNumber.Equals(card.CardNumber)
                   && CardExpiration.Equals(card.CardExpiration);
        }

        public override int GetHashCode()
        {
            var result = 0;
            if (CardId != null)
                result ^= CardId.GetHashCode();
            if (CardType != null)
                result ^= CardType.GetHashCode();
            if (CardNumber != null)
                result ^= CardNumber.GetHashCode();
            if (CardExpiration != null)
                result ^= CardExpiration.GetHashCode();
            return result;
        }

        #endregion Public Constructors

        #region Private Fields

        // Internal member variables 

        #endregion Private Fields

        #region Public Properties

        public string CardExpiration { get; set; }

        public Guid CardId { get; set; }

        public string CardNumber { get; set; }

        // Properties 
        public string CardType { get; set; }

        #endregion Public Properties
    }
}