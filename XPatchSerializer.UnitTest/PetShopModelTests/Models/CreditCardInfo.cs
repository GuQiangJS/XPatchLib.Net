using System;

namespace XPatchLib.UnitTest.PetShopModelTests.Models
{
    [PrimaryKey("CardId")]
    public class CreditCardInfo
    {
        #region Public Constructors

        /// <summary>
        /// Default constructor 
        /// </summary>
        public CreditCardInfo()
        {
        }

        /// <summary>
        /// Constructor with specified initial values 
        /// </summary>
        /// <param name="cardType">
        /// Card type, e.g. Visa, Master Card, American Express 
        /// </param>
        /// <param name="cardNumber">
        /// Number on the card 
        /// </param>
        /// <param name="cardExpiration">
        /// Expiry Date, form MM/YY 
        /// </param>
        public CreditCardInfo(string cardType, string cardNumber, string cardExpiration)
        {
            this.cardId = Guid.NewGuid();
            this.cardType = cardType;
            this.cardNumber = cardNumber;
            this.cardExpiration = cardExpiration;
        }

        public override bool Equals(object obj)
        {
            CreditCardInfo card = obj as CreditCardInfo;
            if (card == null)
            {
                return false;
            }
            return this.CardId.Equals(card.CardId)
                && this.CardType.Equals(card.CardType)
                && this.CardNumber.Equals(card.CardNumber)
                && this.CardExpiration.Equals(card.CardExpiration);
        }

        public override int GetHashCode()
        {
            int result = 0;
            if (this.CardId != null)
            {
                result ^= this.CardId.GetHashCode();
            }
            if (this.CardType != null)
            {
                result ^= this.CardType.GetHashCode();
            }
            if (this.CardNumber != null)
            {
                result ^= this.CardNumber.GetHashCode();
            }
            if (this.CardExpiration != null)
            {
                result ^= this.CardExpiration.GetHashCode();
            }
            return result;
        }

        #endregion Public Constructors

        #region Private Fields

        private string cardExpiration;

        // Internal member variables 
        private Guid cardId;

        private string cardNumber;
        private string cardType;

        #endregion Private Fields

        #region Public Properties

        public string CardExpiration
        {
            get { return cardExpiration; }
            set { cardExpiration = value; }
        }

        public Guid CardId
        {
            get { return cardId; }
            set { cardId = value; }
        }

        public string CardNumber
        {
            get { return cardNumber; }
            set { cardNumber = value; }
        }

        // Properties 
        public string CardType
        {
            get { return cardType; }
            set { cardType = value; }
        }

        #endregion Public Properties
    }
}