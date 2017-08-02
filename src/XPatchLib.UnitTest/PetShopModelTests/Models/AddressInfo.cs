// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;

namespace XPatchLib.UnitTest.PetShopModelTests.Models
{
    [PrimaryKey("AddressId")]
    public class AddressInfo
    {
        #region Public Constructors

        /// <summary>
        ///     Default constructor
        /// </summary>
        public AddressInfo()
        {
            Address1 = string.Empty;
            Address2 = string.Empty;

            City = string.Empty;

            Country = string.Empty;

            Email = string.Empty;

            // Properties 
            FirstName = string.Empty;

            LastName = string.Empty;

            Phone = string.Empty;

            State = string.Empty;
            Zip = string.Empty;
        }

        /// <summary>
        ///     Constructor with specified initial values
        /// </summary>
        /// <param name="firstName">
        ///     First Name
        /// </param>
        /// <param name="lastName">
        ///     Last Name
        /// </param>
        /// <param name="address1">
        ///     Address line 1
        /// </param>
        /// <param name="address2">
        ///     Address line 2
        /// </param>
        /// <param name="city">
        ///     City
        /// </param>
        /// <param name="state">
        ///     State
        /// </param>
        /// <param name="zip">
        ///     Postal Code
        /// </param>
        /// <param name="country">
        ///     Country
        /// </param>
        /// <param name="phone">
        ///     Phone number at this address
        /// </param>
        /// <param name="email">
        ///     Email at this address
        /// </param>
        public AddressInfo(string firstName, string lastName, string address1, string address2, string city,
            string state, string zip, string country, string phone, string email)
        {
            AddressId = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Address1 = address1;
            Address2 = address2;
            City = city;
            State = state;
            Zip = zip;
            Country = country;
            Phone = phone;
            Email = email;
        }

        public override bool Equals(object obj)
        {
            var addr = obj as AddressInfo;
            if (addr == null)
                return false;
            return Equals(AddressId, addr.AddressId)
                   && string.Equals(FirstName, addr.FirstName)
                   && string.Equals(LastName, addr.LastName)
                   && string.Equals(Address1, addr.Address1)
                   && string.Equals(Address2, addr.Address2)
                   && string.Equals(City, addr.City)
                   && string.Equals(State, addr.State)
                   && string.Equals(Zip, addr.Zip)
                   && string.Equals(Country, addr.Country)
                   && string.Equals(Phone, addr.Phone)
                   && string.Equals(Email, addr.Email);
        }

        public override int GetHashCode()
        {
            var result = 0;
            if (AddressId != null)
                result ^= AddressId.GetHashCode();
            if (FirstName != null)
                result ^= FirstName.GetHashCode();
            if (LastName != null)
                result ^= LastName.GetHashCode();
            if (Address1 != null)
                result ^= Address1.GetHashCode();
            if (Address2 != null)
                result ^= Address2.GetHashCode();
            if (City != null)
                result ^= City.GetHashCode();
            if (State != null)
                result ^= State.GetHashCode();
            if (Zip != null)
                result ^= Zip.GetHashCode();
            if (Country != null)
                result ^= Country.GetHashCode();
            if (Phone != null)
                result ^= Phone.GetHashCode();
            if (Email != null)
                result ^= Email.GetHashCode();
            return result;
        }

        #endregion Public Constructors

        #region Private Fields

        // Internal member variables 

        #endregion Private Fields

        #region Public Properties

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public Guid AddressId { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Email { get; set; }

        // Properties 
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        #endregion Public Properties
    }
}