using System;
using XPatchLib.UnitTest.PetShopModelTests.Models;

namespace XPatchLib.UnitTest.ForXml.PetShopModelTests
{
    internal static class PetShopModelTestHelper
    {
        internal static string oriAddress1 = "1600 Pennsylvania Ave NW";
        internal static string oriAddress2 = "";
        internal static Guid oriAddressId = Guid.NewGuid();
        internal static string oriCity = "Washington, DC";
        internal static string oriCountry = "US";
        internal static string oriCreditCardExpiration = "07/12";
        internal static Guid oriCreditCardId = Guid.NewGuid();
        internal static string oriCreditCardNumber = "1234567890";
        internal static string oriCreditCardType = "Visa";
        internal static string oriEmail = "";
        internal static string oriFirstName = "Barack";
        internal static string oriLastName = "Obama";
        internal static DateTime oriOrderDate = DateTime.Now;
        internal static int oriOrderId = 123;
        internal static decimal oriOrderTotal = 1.23m;
        internal static string oriPhone = "202-456-1111";
        internal static string oriStatus = "";
        internal static string oriUserId = "OriUserId";
        internal static string oriZip = "20500";

        /// <summary>
        ///     使用处CreditCardInfo.CardId外其他的固定值创建OrderInfo类型对象。
        /// </summary>
        /// <returns>
        /// </returns>
        internal static OrderInfo CreateNewOriOrderInfo()
        {
            return CreateNewOriOrderInfo(oriOrderId);
        }

        internal static OrderInfo CreateNewOriOrderInfo(int pOrderId)
        {
            var addr1 = new AddressInfo(oriFirstName, oriLastName, oriAddress1, oriAddress2, oriCity, oriStatus, oriZip,
                oriCountry, oriPhone, oriEmail) {AddressId = oriAddressId};

            var result = new OrderInfo
            {
                OrderId = pOrderId,
                Date = oriOrderDate,
                UserId = oriUserId,
                CreditCard =
                    new CreditCardInfo(oriCreditCardType, oriCreditCardNumber, oriCreditCardExpiration)
                    {
                        CardId = oriCreditCardId
                    },
                BillingAddress = addr1,
                OrderTotal = oriOrderTotal
            };

            return result;
        }
    }
}