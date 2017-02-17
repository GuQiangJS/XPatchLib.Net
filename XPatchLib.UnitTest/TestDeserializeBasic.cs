using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestDeserializeBasic
    {
        #region Public Methods

        [TestMethod]
        public void BasicDeserializeCtorTest()
        {
            var ser = new CombineBasic(new TypeExtend(typeof(string)));
            ser.Mode = XmlDateTimeSerializationMode.RoundtripKind;

            ser = new CombineBasic(new TypeExtend(typeof(string)), XmlDateTimeSerializationMode.Unspecified);
            ser.Mode = XmlDateTimeSerializationMode.Unspecified;
        }

        #endregion Public Methods
    }
}