using System;

namespace XPatchLib.UnitTest.PetShopModelTests.Models
{
    [PrimaryKey("AddressId")]
    public class AddressInfo
    {
        #region Public Constructors

        /// <summary>
        /// Default constructor 
        /// </summary>
        public AddressInfo()
        {
        }

        /// <summary>
        /// Constructor with specified initial values 
        /// </summary>
        /// <param name="firstName">
        /// First Name 
        /// </param>
        /// <param name="lastName">
        /// Last Name 
        /// </param>
        /// <param name="address1">
        /// Address line 1 
        /// </param>
        /// <param name="address2">
        /// Address line 2 
        /// </param>
        /// <param name="city">
        /// City 
        /// </param>
        /// <param name="state">
        /// State 
        /// </param>
        /// <param name="zip">
        /// Postal Code 
        /// </param>
        /// <param name="country">
        /// Country 
        /// </param>
        /// <param name="phone">
        /// Phone number at this address 
        /// </param>
        /// <param name="email">
        /// Email at this address 
        /// </param>
        public AddressInfo(string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country, string phone, string email)
        {
            this.addressId = Guid.NewGuid();
            this.firstName = firstName;
            this.lastName = lastName;
            this.address1 = address1;
            this.address2 = address2;
            this.city = city;
            this.state = state;
            this.zip = zip;
            this.country = country;
            this.phone = phone;
            this.email = email;
        }

        public override bool Equals(object obj)
        {
            AddressInfo addr = obj as AddressInfo;
            if (addr == null)
            {
                return false;
            }
            return Guid.Equals(this.AddressId, addr.AddressId)
                && string.Equals(this.FirstName, addr.FirstName)
                && string.Equals(this.LastName, addr.LastName)
                && string.Equals(this.Address1, addr.Address1)
                && string.Equals(this.Address2, addr.Address2)
                && string.Equals(this.City, addr.City)
                && string.Equals(this.State, addr.State)
                && string.Equals(this.Zip, addr.Zip)
                && string.Equals(this.Country, addr.Country)
                && string.Equals(this.Phone, addr.Phone)
                && string.Equals(this.Email, addr.Email);
        }

        public override int GetHashCode()
        {
            int result = 0;
            if (this.AddressId != null)
            {
                result ^= this.AddressId.GetHashCode();
            }
            if (this.FirstName != null)
            {
                result ^= this.FirstName.GetHashCode();
            }
            if (this.LastName != null)
            {
                result ^= this.LastName.GetHashCode();
            }
            if (this.Address1 != null)
            {
                result ^= this.Address1.GetHashCode();
            }
            if (this.Address2 != null)
            {
                result ^= this.Address2.GetHashCode();
            }
            if (this.City != null)
            {
                result ^= this.City.GetHashCode();
            }
            if (this.State != null)
            {
                result ^= this.State.GetHashCode();
            }
            if (this.Zip != null)
            {
                result ^= this.Zip.GetHashCode();
            }
            if (this.Country != null)
            {
                result ^= this.Country.GetHashCode();
            }
            if (this.Phone != null)
            {
                result ^= this.Phone.GetHashCode();
            }
            if (this.Email != null)
            {
                result ^= this.Email.GetHashCode();
            }
            return result;
        }

        #endregion Public Constructors

        #region Private Fields

        private string address1;

        private string address2;

        // Internal member variables 
        private Guid addressId;

        private string city;
        private string country;
        private string email;
        private string firstName;
        private string lastName;
        private string phone;
        private string state;
        private string zip;

        #endregion Private Fields

        #region Public Properties

        public string Address1
        {
            get { return address1; }
            set { address1 = value; }
        }

        public string Address2
        {
            get { return address2; }
            set { address2 = value; }
        }

        public Guid AddressId
        {
            get { return addressId; }
            set { addressId = value; }
        }

        public string City
        {
            get { return city; }
            set { city = value; }
        }

        public string Country
        {
            get { return country; }
            set { country = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        // Properties 
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public string Zip
        {
            get { return zip; }
            set { zip = value; }
        }

        #endregion Public Properties
    }
}