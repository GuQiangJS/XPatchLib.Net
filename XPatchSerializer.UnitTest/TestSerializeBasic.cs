using System;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestSerializeBasic
    {
        #region Public Methods

        [TestMethod]
        public void BasicSerializeCtorTest()
        {
            DivideBasic ser = new DivideBasic(new TypeExtend(typeof(string)));
            ser.Mode = XmlDateTimeSerializationMode.RoundtripKind;

            ser = new DivideBasic(new TypeExtend(typeof(string)), XmlDateTimeSerializationMode.Unspecified);
            ser.Mode = XmlDateTimeSerializationMode.Unspecified;
        }

        [TestMethod]
        public void BasicTypeSerializationTest()
        {
            Guid g1 = Guid.NewGuid();
            Guid g2 = Guid.NewGuid();

            string s1 = Guid.NewGuid().ToString();
            string s2 = Guid.NewGuid().ToString();

            var oriObjs = new object[]
            {
                false
                , int.MinValue
                , double.MinValue
                , s1
                , long.MinValue
                , short.MinValue
                , sbyte.MinValue
                , byte.MinValue
                , UInt16.MinValue
                , UInt32.MinValue
                , UInt64.MinValue
                , Single.MinValue
                , Decimal.MinValue
                , DateTime.MinValue
                , g1
                , 'a'};

            var revObjs = new object[]
            {
                true
                , int.MaxValue
                , double.MaxValue
                , s2
                , long.MaxValue
                , short.MaxValue
                , sbyte.MaxValue
                , byte.MaxValue
                , UInt16.MaxValue
                , UInt32.MaxValue
                , UInt64.MaxValue
                , Single.MaxValue
                , Decimal.MaxValue
                , DateTime.MaxValue
                , g2
                , 'b' };

            var types = new[]
            {
                typeof(bool)
                , typeof(int)
                , typeof(double)
                , typeof(string)
                , typeof(long)
                , typeof(short)
                , typeof(sbyte)
                , typeof(byte)
                , typeof(UInt16)
                , typeof(UInt32)
                , typeof(UInt64)
                , typeof(float)
                , typeof(decimal)
                , typeof(DateTime)
                , typeof(Guid)
                , typeof(char) };

            var serializedResults = new string[]
            {
                "<Boolean>" + XmlConvert.ToString(true) + "</Boolean>"
                , "<Int32>" + int.MaxValue + "</Int32>"
                , "<Double>" + XmlConvert.ToString(double.MaxValue) + "</Double>"
                , "<String>"+s2+"</String>"
                , "<Int64>" + long.MaxValue + "</Int64>"
                , "<Int16>" + short.MaxValue + "</Int16>"
                , "<SByte>" + sbyte.MaxValue + "</SByte>"
                , "<Byte>" + byte.MaxValue + "</Byte>"
                , "<UInt16>" + ushort.MaxValue + "</UInt16>"
                , "<UInt32>" + uint.MaxValue + "</UInt32>"
                , "<UInt64>" + ulong.MaxValue + "</UInt64>"
                , "<Single>" + XmlConvert.ToString(float.MaxValue) + "</Single>"
                , "<Decimal>" + XmlConvert.ToString(decimal.MaxValue) + "</Decimal>"
                , "<DateTime>" + XmlConvert.ToString(DateTime.MaxValue,XmlDateTimeSerializationMode.RoundtripKind) + "</DateTime>"
                , "<Guid>" + XmlConvert.ToString(g2) + "</Guid>"
                , "<Char>" + XmlConvert.ToString((ushort)'b') + "</Char>" };
            var serializedSetNullResults = new string[]
            {
                @"<Boolean Action=""SetNull"" />"
                , @"<Int32 Action=""SetNull"" />"
                , @"<Double Action=""SetNull"" />"
                , @"<String Action=""SetNull"" />"
                , @"<Int64 Action=""SetNull"" />"
                , @"<Int16 Action=""SetNull"" />"
                , @"<SByte Action=""SetNull"" />"
                , @"<Byte Action=""SetNull"" />"
                , @"<UInt16 Action=""SetNull"" />"
                , @"<UInt32 Action=""SetNull"" />"
                , @"<UInt64 Action=""SetNull"" />"
                , @"<Single Action=""SetNull"" />"
                , @"<Decimal Action=""SetNull"" />"
                , @"<DateTime Action=""SetNull"" />"
                , @"<Guid Action=""SetNull"" />"
                , @"<Char Action=""SetNull"" />" };
            //值类型默认值
            //https://msdn.microsoft.com/zh-cn/library/83fhsxwc.aspx
            var serializedDefaultValues = new string[]
            {
                "<Boolean>" + XmlConvert.ToString(false) + "</Boolean>"
                , "<Int32>" + 0 + "</Int32>"
                , "<Double>" + XmlConvert.ToString(0D) + "</Double>"
                , ""
                , "<Int64>" + 0 + "</Int64>"
                , "<Int16>" + 0 + "</Int16>"
                , "<SByte>" + 0 + "</SByte>"
                , "<Byte>" + 0 + "</Byte>"
                , "<UInt16>" + 0 + "</UInt16>"
                , "<UInt32>" + 0 + "</UInt32>"
                , "<UInt64>" + 0 + "</UInt64>"
                , "<Single>" + XmlConvert.ToString(0F) + "</Single>"
                , "<Decimal>" + XmlConvert.ToString(0M) + "</Decimal>"
                , "<DateTime>" + XmlConvert.ToString(new DateTime(),XmlDateTimeSerializationMode.RoundtripKind) + "</DateTime>"
                , "<Guid>" + XmlConvert.ToString(Guid.Empty) + "</Guid>"
                , "<Char>" + XmlConvert.ToString((ushort)'\0') + "</Char>" };

            for (int i = 0; i < types.Length; i++)
            {
                DivideBasic ser = new DivideBasic(new TypeExtend(types[i]));
                CombineBasic der = new CombineBasic(new TypeExtend(types[i]));

                //原始值与更新值不同时，序列化更新值
                XElement ele = ser.Divide(types[i].Name, oriObjs[i], revObjs[i]);
                Assert.AreEqual(serializedResults[i], TestHelper.XElementToString(ele));

                Assert.AreEqual(revObjs[i], der.Combine(ele));

                //原始值有值，更新值为null时，序列化为<XXX Action="SetNull" />
                ele = ser.Divide(types[i].Name, oriObjs[i], null);
                Assert.AreEqual(serializedSetNullResults[i], TestHelper.XElementToString(ele));
                Assert.AreEqual(null, der.Combine(ele));

                //原始值为null，更新值有值时序列化更新值
                ele = ser.Divide(types[i].Name, null, revObjs[i]);
                Assert.AreEqual(serializedResults[i], TestHelper.XElementToString(ele));
                Assert.AreEqual(revObjs[i], der.Combine(ele));

                //原始值与更新值相同时不做序列化
                Assert.IsNull(ser.Divide(types[i].Name, oriObjs[i], oriObjs[i]));
                //原始值与更新值均为null时不做序列化
                Assert.IsNull(ser.Divide(types[i].Name, null, null));

                //原始值为null，如果更新值为默认值时，如果默认不序列化默认值，则不做序列化
                Assert.IsNull(ser.Divide(types[i].Name, null, ReflectionUtils.GetDefaultValue(types[i])));

                ser.SerializeDefaultValue = true;
                //原始值为null，如果更新值为默认值时，如果设置为序列化默认值，则做序列化
                ele = ser.Divide(types[i].Name, null, ReflectionUtils.GetDefaultValue(types[i]));
                if (types[i] == typeof(string))
                {
                    //string类型的默认值为null，不是String.Empty
                    Assert.IsNull(ele);
                }
                else
                {
                    Assert.IsNotNull(ele);
                    Assert.AreEqual(serializedDefaultValues[i], TestHelper.XElementToString(ele));
                }
            }
        }

        [TestMethod]
        public void SerializeColor_Argb()
        {
            DivideObject ser = new DivideObject(new TypeExtend(typeof(ColorClass)));
            CombineObject dser = new CombineObject(new TypeExtend(typeof(ColorClass)));

            ColorClass c1 = new ColorClass() { Color = Color.FromArgb(255, 255, 255) };
            XElement ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(ColorClass)), null, c1);

            string result =
string.Format(@"<ColorClass>
  <Color>#{0:X}</Color>
</ColorClass>", c1.Color.ToArgb());

            Assert.AreEqual(result, ele.ToString());

            ColorClass c2 = dser.Combine(ColorClass.GetSampleInstance(), ele) as ColorClass;
            Assert.AreEqual(c1.Color, c2.Color);
        }

        [TestMethod]
        public void SerializeColor_KnownColor()
        {
            DivideObject ser = new DivideObject(new TypeExtend(typeof(ColorClass)));
            CombineObject dser = new CombineObject(new TypeExtend(typeof(ColorClass)));

            ColorClass c1 = ColorClass.GetSampleInstance();
            XElement ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(ColorClass)), null, c1);

            string result =
string.Format(@"<ColorClass>
  <Color>AliceBlue</Color>
</ColorClass>");

            Assert.AreEqual(result, ele.ToString());

            ColorClass c2 = ColorClass.GetSampleInstance();
            c2.Color = Color.AntiqueWhite;
            Assert.AreEqual(Color.AntiqueWhite, c2.Color);

            c1 = dser.Combine(c2, ele) as ColorClass;

            Assert.AreEqual(ColorClass.GetSampleInstance().Color, c2.Color);
        }

        [TestMethod]
        public void SerializeEnum()
        {
            DivideObject ser = new DivideObject(new TypeExtend(typeof(EnumClass)));
            CombineObject dser = new CombineObject(new TypeExtend(typeof(EnumClass)));

            XElement ele = null;
            string result = string.Empty;

            ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(EnumClass)), null, EnumClass.GetSampleInstance());

            result =
@"<EnumClass>
  <MultiQuarter>First, Second, Third, Fourth</MultiQuarter>
  <SingleQuarter>First</SingleQuarter>
</EnumClass>";

            Assert.AreEqual(result, ele.ToString());

            EnumClass c1 = EnumClass.GetSampleInstance();
            c1.SingleQuarter = SingleQuarter.Second;
            c1.MultiQuarter = MultiQuarter.Second | MultiQuarter.Fourth;

            ele = ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(EnumClass)), EnumClass.GetSampleInstance(), c1);

            result =
@"<EnumClass>
  <MultiQuarter>Second, Fourth</MultiQuarter>
  <SingleQuarter>Second</SingleQuarter>
</EnumClass>";

            Assert.AreEqual(result, ele.ToString());
            EnumClass c2 = dser.Combine(EnumClass.GetSampleInstance(), ele) as EnumClass;
            Assert.IsNotNull(c2);
            Assert.AreEqual(SingleQuarter.Second, c2.SingleQuarter);
            Assert.AreEqual(MultiQuarter.Second | MultiQuarter.Fourth, c2.MultiQuarter);
        }

        [TestMethod]
        public void SerializeErrorColor()
        {
            string result = @"<ColorClass>
  <Color>#xxxxxx</Color>
</ColorClass>";
            XElement xele;
            using (StringReader reader = new StringReader(result))
            {
                xele = XElement.Load(reader);
            }

            CombineObject dser = new CombineObject(new TypeExtend(typeof(ColorClass)));
            try
            {
                ColorClass c2 = dser.Combine(ColorClass.GetSampleInstance(), xele) as ColorClass;
            }
            catch (FormatException ex)
            {
                Assert.AreEqual(string.Format("{0} 不能被转换成 Color 对象。", "#xxxxxx"), ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出FormatException异常。");
            }
        }

        #endregion Public Methods
    }
}