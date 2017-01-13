using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XPatchLib.UnitTest.PetShopModelTests.Models;

namespace XPatchLib.UnitTest.PetShopModelTests
{
    [TestClass]
    public class SingleModelTest
    {
        #region Public Methods

        [TestMethod]
        [Description("测试两个同一类型的复杂对象间改变值的增量内容是否产生正确，是否能够正确合并，并且合并后值相等")]
        public void TestOrderInfoDivideAndCombine()
        {
            OrderInfo oriObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            OrderInfo changedObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            string changedContext = string.Empty;

            //更新了一个全新的CreditCard信息，并且重新赋值了UserId属性
            changedObj.UserId = "UserId-3";
            changedObj.CreditCard = new CreditCardInfo("American Express", "0123456789", "12/15");

            changedContext = @"<OrderInfo>
  <CreditCard>
    <CardExpiration>12/15</CardExpiration>
    <CardId>" + changedObj.CreditCard.CardId + @"</CardId>
    <CardNumber>0123456789</CardNumber>
    <CardType>American Express</CardType>
  </CreditCard>
  <UserId>UserId-3</UserId>
</OrderInfo>";

            TestHelper.PrivateAssert(typeof(OrderInfo), oriObj, changedObj, changedContext, "更新了一个全新的CreditCard信息，并且重新赋值了UserId属性");

            oriObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            changedObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            changedObj.Date = changedObj.Date.AddDays(1);

            changedContext = @"<OrderInfo>
  <Date>" + XmlConvert.ToString(changedObj.Date, XmlDateTimeSerializationMode.RoundtripKind) + @"</Date>
</OrderInfo>";

            TestHelper.PrivateAssert(typeof(OrderInfo), oriObj, changedObj, changedContext, "更新OrderInfo中的Date信息(RoundtripKind)");

            oriObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            changedObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            changedObj.Date = changedObj.Date.AddDays(1);
            changedContext = @"<OrderInfo>
  <Date>" + XmlConvert.ToString(changedObj.Date, XmlDateTimeSerializationMode.Local) + @"</Date>
</OrderInfo>";

            TestHelper.PrivateAssert(typeof(OrderInfo), oriObj, changedObj, changedContext, "更新OrderInfo中的Date信息(Local)", XmlDateTimeSerializationMode.Local);

            oriObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            changedObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            changedObj.Date = changedObj.Date.AddDays(1);
            changedContext = @"<OrderInfo>
  <Date>" + XmlConvert.ToString(changedObj.Date, XmlDateTimeSerializationMode.Unspecified) + @"</Date>
</OrderInfo>";

            TestHelper.PrivateAssert(typeof(OrderInfo), oriObj, changedObj, changedContext, "更新OrderInfo中的Date信息(Unspecified)", XmlDateTimeSerializationMode.Unspecified);

            oriObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            changedObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            LineItemInfo item = new LineItemInfo("22", "NewLineItemInfo", 23, 34, 45m);
            changedObj.LineItems = new LineItemInfo[] { item };

            changedContext = @"<OrderInfo>
  <LineItems>
    <LineItemInfo Action=""Add"">
      <ItemId>" + item.ItemId + @"</ItemId>
      <Line>" + item.Line + @"</Line>
      <Name>" + item.Name + @"</Name>
      <Price>" + item.Price + @"</Price>
      <Quantity>" + item.Quantity + @"</Quantity>
    </LineItemInfo>
  </LineItems>
</OrderInfo>";

            TestHelper.PrivateAssert(typeof(OrderInfo), oriObj, changedObj, changedContext, "增加一个LineItemInfo");

            oriObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            changedObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            oriObj.LineItems = new LineItemInfo[] { new LineItemInfo("22", "NewLineItemInfo", 23, 34, 45m) };
            changedObj.LineItems = new LineItemInfo[] { new LineItemInfo("22", "ChangedLineItemInfo", 23, 45, 45m) };

            changedContext = @"<OrderInfo>
  <LineItems>
    <LineItemInfo ItemId=""" + oriObj.LineItems[0].ItemId + @""">
      <Name>" + changedObj.LineItems[0].Name + @"</Name>
      <Quantity>" + changedObj.LineItems[0].Quantity + @"</Quantity>
    </LineItemInfo>
  </LineItems>
</OrderInfo>";

            TestHelper.PrivateAssert(typeof(OrderInfo), oriObj, changedObj, changedContext, "编辑首个LineItemInfo");

            oriObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            changedObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            oriObj.LineItems = new LineItemInfo[] { new LineItemInfo("11", "NewLineItemInfo1", 123, 134, 145m), new LineItemInfo("22", "NewLineItemInfo2", 223, 234, 245m), new LineItemInfo("33", "NewLineItemInfo3", 323, 334, 345m) };
            changedObj.LineItems = new LineItemInfo[] { new LineItemInfo("11", "NewLineItemInfo1", 123, 134, 145m), new LineItemInfo("22", "NewLineItemInfo2", 223, 234, 245m), new LineItemInfo("33", "NewLineItemInfo3", 323, 334, 345m) };

            changedObj.LineItems[1].Name = "ChangedLineItemInfo2";
            changedObj.LineItems[1].Price = 245.2222m;
            changedObj.LineItems[1].Quantity = 2222;

            changedContext = @"<OrderInfo>
  <LineItems>
    <LineItemInfo ItemId=""" + oriObj.LineItems[1].ItemId + @""">
      <Name>" + changedObj.LineItems[1].Name + @"</Name>
      <Price>" + changedObj.LineItems[1].Price + @"</Price>
      <Quantity>" + changedObj.LineItems[1].Quantity + @"</Quantity>
    </LineItemInfo>
  </LineItems>
</OrderInfo>";

            TestHelper.PrivateAssert(typeof(OrderInfo), oriObj, changedObj, changedContext, "编辑非首个LineItemInfo");

            oriObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            changedObj = PetShopModelTestHelper.CreateNewOriOrderInfo();
            oriObj.LineItems = new LineItemInfo[] { new LineItemInfo("11", "NewLineItemInfo1", 123, 134, 145m), new LineItemInfo("22", "NewLineItemInfo2", 223, 234, 245m), new LineItemInfo("33", "NewLineItemInfo3", 323, 334, 345m) };
            changedObj.LineItems = new LineItemInfo[] { new LineItemInfo("11", "NewLineItemInfo1", 123, 134, 145m), new LineItemInfo("33", "NewLineItemInfo3", 323, 334, 345m) };

            changedContext = @"<OrderInfo>
  <LineItems>
    <LineItemInfo Action=""Remove"" ItemId=""" + oriObj.LineItems[1].ItemId + @""" />
  </LineItems>
</OrderInfo>";

            TestHelper.PrivateAssert(typeof(OrderInfo), oriObj, changedObj, changedContext, "删除非首个LineItemInfo");
        }

        #endregion Public Methods
    }
}