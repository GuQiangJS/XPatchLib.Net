// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
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
            var ser = new DivideBasic(
                new XmlTextWriter(new System.Xml.XmlTextWriter(new MemoryStream(), Encoding.UTF8)),
                new TypeExtend(typeof(string)));
            Assert.AreEqual(ser.Mode, XmlDateTimeSerializationMode.RoundtripKind);

            ser = new DivideBasic(new XmlTextWriter(new System.Xml.XmlTextWriter(new MemoryStream(), Encoding.UTF8)),
                new TypeExtend(typeof(string)), XmlDateTimeSerializationMode.Unspecified);
            Assert.AreEqual(ser.Mode, XmlDateTimeSerializationMode.Unspecified);
        }


        [TestMethod]
        public void BasicTypeSerializationTest()
        {
            var g1 = Guid.NewGuid();
            var g2 = Guid.NewGuid();

            var s1 = Guid.NewGuid().ToString();
            var s2 = Guid.NewGuid().ToString();

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
                , ushort.MinValue
                , uint.MinValue
                , ulong.MinValue
                , float.MinValue
                , decimal.MinValue
                , DateTime.MinValue
                , g1
                , 'a'
            };

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
                , ushort.MaxValue
                , uint.MaxValue
                , ulong.MaxValue
                , float.MaxValue
                , decimal.MaxValue
                , DateTime.MaxValue
                , g2
                , 'b'
            };

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
                , typeof(ushort)
                , typeof(uint)
                , typeof(ulong)
                , typeof(float)
                , typeof(decimal)
                , typeof(DateTime)
                , typeof(Guid)
                , typeof(char)
            };

            var serializedResults = new[]
            {
                "<Boolean>" + XmlConvert.ToString(true) + "</Boolean>"
                , "<Int32>" + int.MaxValue + "</Int32>"
                , "<Double>" + XmlConvert.ToString(double.MaxValue) + "</Double>"
                , "<String>" + s2 + "</String>"
                , "<Int64>" + long.MaxValue + "</Int64>"
                , "<Int16>" + short.MaxValue + "</Int16>"
                , "<SByte>" + sbyte.MaxValue + "</SByte>"
                , "<Byte>" + byte.MaxValue + "</Byte>"
                , "<UInt16>" + ushort.MaxValue + "</UInt16>"
                , "<UInt32>" + uint.MaxValue + "</UInt32>"
                , "<UInt64>" + ulong.MaxValue + "</UInt64>"
                , "<Single>" + XmlConvert.ToString(float.MaxValue) + "</Single>"
                , "<Decimal>" + XmlConvert.ToString(decimal.MaxValue) + "</Decimal>"
                ,
                "<DateTime>" + XmlConvert.ToString(DateTime.MaxValue, XmlDateTimeSerializationMode.RoundtripKind) +
                "</DateTime>"
                , "<Guid>" + XmlConvert.ToString(g2) + "</Guid>"
                , "<Char>" + XmlConvert.ToString((ushort) 'b') + "</Char>"
            };
            var serializedSetNullResults = new[]
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
                , @"<Char Action=""SetNull"" />"
            };
            //值类型默认值
            //https://msdn.microsoft.com/zh-cn/library/83fhsxwc.aspx
            var serializedDefaultValues = new[]
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
                ,
                "<DateTime>" + XmlConvert.ToString(new DateTime(), XmlDateTimeSerializationMode.RoundtripKind) +
                "</DateTime>"
                , "<Guid>" + XmlConvert.ToString(Guid.Empty) + "</Guid>"
                , "<Char>" + XmlConvert.ToString((ushort) '\0') + "</Char>"
            };

            for (var i = 0; i < types.Length; i++)
            {
                using (var stream = new MemoryStream())
                {
                    using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                    {
                        var ser = new DivideBasic(writer, new TypeExtend(types[i]));

                        //原始值与更新值不同时，序列化更新值
                        Assert.IsTrue(ser.Divide(types[i].Name, oriObjs[i], revObjs[i]));
                    }
                    stream.Position = 0;
                    var ele = XElement.Load(stream);

                    var der = new CombineBasic(new TypeExtend(types[i]));
                    var s = ele.ToString();
                    Debug.WriteLine(s);
                    Assert.AreEqual(serializedResults[i], s, "输出内容与预期不符");
                    stream.Position = 0;
                    using (XmlReader reader = XmlReader.Create(stream))
                    {
                        Assert.AreEqual(revObjs[i], der.Combine(reader, null, types[i].Name), "增量合并后的对象实例与预期不符");
                    }
                }

                using (var stream = new MemoryStream())
                {
                    using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                    {
                        var ser = new DivideBasic(writer, new TypeExtend(types[i]));
                        //原始值有值，更新值为null时，序列化为<XXX Action="SetNull" />
                        Assert.IsTrue(ser.Divide(types[i].Name, oriObjs[i], null));
                    }
                    stream.Position = 0;
                    var ele = XElement.Load(stream);

                    var der = new CombineBasic(new TypeExtend(types[i]));
                    var s = ele.ToString();
                    Debug.WriteLine(s);

                    Assert.AreEqual(serializedSetNullResults[i], s, "输出内容与预期不符");
                    stream.Position = 0;
                    using (XmlReader reader = XmlReader.Create(stream))
                    {
                        Assert.AreEqual(null, der.Combine(reader, null, types[i].Name), "增量合并后的对象实例与预期不符");
                    }
                }

                using (var stream = new MemoryStream())
                {
                    using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                    {
                        var ser = new DivideBasic(writer, new TypeExtend(types[i]));
                        //原始值为null，更新值有值时序列化更新值
                        Assert.IsTrue(ser.Divide(types[i].Name, null, revObjs[i]));
                    }
                    stream.Position = 0;
                    var ele = XElement.Load(stream);

                    var der = new CombineBasic(new TypeExtend(types[i]));
                    var s = ele.ToString();
                    Debug.WriteLine(s);

                    Assert.AreEqual(serializedResults[i], s, "输出内容与预期不符");
                    stream.Position = 0;
                    using (XmlReader reader = XmlReader.Create(stream))
                    {
                        Assert.AreEqual(revObjs[i], der.Combine(reader, null, types[i].Name), "增量合并后的对象实例与预期不符");
                    }
                }

                using (var stream = new MemoryStream())
                {
                    using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                    {
                        var ser = new DivideBasic(writer, new TypeExtend(types[i]));
                        //原始值与更新值相同时不做序列化
                        Assert.IsFalse(ser.Divide(types[i].Name, oriObjs[i], oriObjs[i]));
                        //原始值与更新值均为null时不做序列化
                        Assert.IsFalse(ser.Divide(types[i].Name, null, null));
                        //原始值为null，如果更新值为默认值时，如果默认不序列化默认值，则不做序列化
                        Assert.IsFalse(ser.Divide(types[i].Name, null, ReflectionUtils.GetDefaultValue(types[i])));
                    }
                }

                using (var stream = new MemoryStream())
                {
                    using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                    {
                        var ser = new DivideBasic(writer, new TypeExtend(types[i]));
                        ser.SerializeDefaultValue = true;
                        //原始值为null，如果更新值为默认值时，如果设置为序列化默认值，则做序列化
                        if (types[i] != typeof(string))
                        {
                            Assert.IsTrue(ser.Divide(types[i].Name, null, ReflectionUtils.GetDefaultValue(types[i])));
                            writer.WriteEndObject();
                            writer.Flush();
                            stream.Position = 0;
                            var ele = XElement.Load(stream);
                            Assert.IsNotNull(ele);
                            var s = ele.ToString();
                            Debug.WriteLine(s);
                            Assert.AreEqual(serializedDefaultValues[i], s);
                        }
                        else
                        {
                            Assert.IsFalse(ser.Divide(types[i].Name, null, ReflectionUtils.GetDefaultValue(types[i])));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void SerializeColor_Argb()
        {
            var c1 = new ColorClass {Color = Color.FromArgb(255, 255, 255)};
            var result = string.Format(@"<ColorClass>
  <Color>#{0:X}</Color>
</ColorClass>", c1.Color.ToArgb());
            var dser = new CombineObject(new TypeExtend(typeof(ColorClass)));
            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    var ser = new DivideObject(writer, new TypeExtend(typeof(ColorClass)));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(ColorClass)), null, c1));
                }
                stream.Position = 0;
                var ele = XElement.Load(stream);
                Debug.WriteLine(ele.ToString());
                Assert.AreEqual(result, ele.ToString());

                stream.Position = 0;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    var c2 =
                        dser.Combine(reader, ColorClass.GetSampleInstance(),
                            ReflectionUtils.GetTypeFriendlyName(typeof(ColorClass))) as ColorClass;
                    Debug.Assert(c2 != null, "c2 != null");
                    Assert.AreEqual(c1.Color, c2.Color);
                }
            }
        }

        [TestMethod]
        public void SerializeColor_KnownColor()
        {
            var result = @"<ColorClass>
  <Color>AliceBlue</Color>
</ColorClass>";
            using (var stream = new MemoryStream())
            {
                var c1 = ColorClass.GetSampleInstance();
                var dser = new CombineObject(new TypeExtend(typeof(ColorClass)));
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    var ser = new DivideObject(writer, new TypeExtend(typeof(ColorClass)));

                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(ColorClass)), null, c1));
                }
                stream.Position = 0;
                var ele = XElement.Load(stream);

                Assert.AreEqual(result, ele.ToString());
                Debug.WriteLine(ele.ToString());

                var c2 = ColorClass.GetSampleInstance();
                c2.Color = Color.AntiqueWhite;
                Assert.AreEqual(Color.AntiqueWhite, c2.Color);

                stream.Position = 0;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    //合并数据时会默认更新传入的原始对象
                    dser.Combine(reader, c2, ReflectionUtils.GetTypeFriendlyName(typeof(ColorClass)));

                    Assert.AreEqual(ColorClass.GetSampleInstance().Color, c2.Color);
                }
            }
        }

        [TestMethod]
        public void SerializeEnum()
        {
            var result = @"<EnumClass>
  <MultiQuarter>First, Second, Third, Fourth</MultiQuarter>
  <SingleQuarter>First</SingleQuarter>
</EnumClass>";
            var dser = new CombineObject(new TypeExtend(typeof(EnumClass)));
            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    var ser = new DivideObject(writer, new TypeExtend(typeof(EnumClass)));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(EnumClass)), null,
                        EnumClass.GetSampleInstance()));
                }

                stream.Position = 0;
                var ele = XElement.Load(stream);

                Assert.AreEqual(result, ele.ToString());
                Debug.WriteLine(ele.ToString());
            }

            var c1 = EnumClass.GetSampleInstance();
            c1.SingleQuarter = SingleQuarter.Second;
            c1.MultiQuarter = MultiQuarter.Second | MultiQuarter.Fourth;

            result = @"<EnumClass>
  <MultiQuarter>Second, Fourth</MultiQuarter>
  <SingleQuarter>Second</SingleQuarter>
</EnumClass>";
            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.FlagmentSetting))
                {
                    var ser = new DivideObject(writer, new TypeExtend(typeof(EnumClass)));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(EnumClass)),
                        EnumClass.GetSampleInstance(), c1));
                }

                stream.Position = 0;
                var ele = XElement.Load(stream);
                Assert.AreEqual(result, ele.ToString());
                Debug.WriteLine(ele.ToString());
                stream.Position = 0;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    var c2 =
                        dser.Combine(reader, EnumClass.GetSampleInstance(),
                            ReflectionUtils.GetTypeFriendlyName(typeof(EnumClass))) as EnumClass;
                    Assert.IsNotNull(c2);
                    Assert.AreEqual(SingleQuarter.Second, c2.SingleQuarter);
                    Assert.AreEqual(MultiQuarter.Second | MultiQuarter.Fourth, c2.MultiQuarter);
                }
            }
        }

        [TestMethod]
        public void SerializeErrorColor()
        {
            var result = @"<ColorClass>
  <Color>#xxxxxx</Color>
</ColorClass>";
            XElement xele;
            using (var reader = new StringReader(result))
            {
                xele = XElement.Load(reader);
            }

            var dser = new CombineObject(new TypeExtend(typeof(ColorClass)));
            try
            {
                using (XmlReader reader = XmlTextReader.Create(new StringReader(result)))
                {
                    dser.Combine(reader, ColorClass.GetSampleInstance(), "ColorClass");
                }
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