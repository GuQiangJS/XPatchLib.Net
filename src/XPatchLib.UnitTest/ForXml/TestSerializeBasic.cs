// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using XPatchLib.UnitTest.TestClass;
#if (NET || NETSTANDARD_2_0_UP)
using System.Drawing;
#endif
#if (NET_35_UP || NETSTANDARD)
using System.Xml.Linq;
#endif
#if NUNIT
using NUnit.Framework;
#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest.ForXml
{
    [TestFixture]
    public class TestSerializeBasic
    {
        [Test]
        public void BasicSerializeCtorTest()
        {
            var writer = new XmlTextWriter(XmlWriter.Create(new MemoryStream(), TestHelper.DocumentSetting));
            Assert.AreEqual(writer.Setting.Mode, DateTimeSerializationMode.RoundtripKind);
            Assert.AreEqual(writer.Setting.Mode.Convert(), XmlDateTimeSerializationMode.RoundtripKind);

            writer.Setting.Mode = DateTimeSerializationMode.Unspecified;
            Assert.AreEqual(writer.Setting.Mode, DateTimeSerializationMode.Unspecified);
            Assert.AreEqual(writer.Setting.Mode.Convert(), XmlDateTimeSerializationMode.Unspecified);
        }


        [Test]
        public void BasicTypeSerializationTest()
        {
            var g1 = Guid.NewGuid();
            var g2 = Guid.NewGuid();

            var s1 = Guid.NewGuid().ToString();
            var s2 = Guid.NewGuid().ToString();

            var uri1 = new Uri("http://www.visualstudio.com");
            var uri2 = new Uri("http://www.msdn.com");

            var oriObjs = new object[]
            {
                false, int.MinValue, double.MinValue, s1, long.MinValue, short.MinValue, sbyte.MinValue, byte.MinValue,
                ushort.MinValue, uint.MinValue, ulong.MinValue, float.MinValue, decimal.MinValue, DateTime.MinValue, g1,
                'a', TimeSpan.MinValue, DateTimeOffset.MinValue
#if NET_40_UP || NETSTANDARD_1_1_UP
                ,System.Numerics.BigInteger.Zero
#endif
                ,uri1
            };

            var revObjs = new object[]
            {
                true, int.MaxValue, double.MaxValue, s2, long.MaxValue, short.MaxValue, sbyte.MaxValue, byte.MaxValue,
                ushort.MaxValue, uint.MaxValue, ulong.MaxValue, float.MaxValue, decimal.MaxValue, DateTime.MaxValue, g2,
                'b',TimeSpan.MaxValue, DateTimeOffset.MaxValue
#if NET_40_UP || NETSTANDARD_1_1_UP
                ,System.Numerics.BigInteger.MinusOne
#endif
                ,uri2
            };

            var types = new[]
            {
                typeof(bool), typeof(int), typeof(double), typeof(string), typeof(long), typeof(short), typeof(sbyte),
                typeof(byte), typeof(ushort), typeof(uint), typeof(ulong), typeof(float), typeof(decimal),
                typeof(DateTime), typeof(Guid), typeof(char), typeof(TimeSpan), typeof(DateTimeOffset)
#if NET_40_UP || NETSTANDARD_1_1_UP
                ,typeof(System.Numerics.BigInteger)
#endif
                ,typeof(Uri)
            };

            var serializedResults = new[]
            {
                "<Boolean>" + XmlConvert.ToString(true) + "</Boolean>", "<Int32>" + int.MaxValue + "</Int32>",
                "<Double>" + XmlConvert.ToString(double.MaxValue) + "</Double>", "<String>" + s2 + "</String>",
                "<Int64>" + long.MaxValue + "</Int64>", "<Int16>" + short.MaxValue + "</Int16>",
                "<SByte>" + sbyte.MaxValue + "</SByte>", "<Byte>" + byte.MaxValue + "</Byte>",
                "<UInt16>" + ushort.MaxValue + "</UInt16>", "<UInt32>" + uint.MaxValue + "</UInt32>",
                "<UInt64>" + ulong.MaxValue + "</UInt64>",
                "<Single>" + XmlConvert.ToString(float.MaxValue) + "</Single>",
                "<Decimal>" + XmlConvert.ToString(decimal.MaxValue) + "</Decimal>",
                "<DateTime>" + XmlConvert.ToString(DateTime.MaxValue, XmlDateTimeSerializationMode.RoundtripKind) +
                "</DateTime>",
                "<Guid>" + XmlConvert.ToString(g2) + "</Guid>",
                "<Char>" + XmlConvert.ToString((ushort) 'b') + "</Char>",
                "<TimeSpan>" + XmlConvert.ToString(TimeSpan.MaxValue) + "</TimeSpan>",
                "<DateTimeOffset>" + XmlConvert.ToString(DateTimeOffset.MaxValue) + "</DateTimeOffset>"
#if NET_40_UP || NETSTANDARD_1_1_UP
                ,"<BigInteger>" + System.Numerics.BigInteger.MinusOne.ToString(CultureInfo.InvariantCulture) + "</BigInteger>"
#endif
                ,"<Uri>" + uri2.ToString() + "</Uri>"
            };
            var serializedSetNullResults = new[]
            {
                @"<Boolean Action=""SetNull"" />", @"<Int32 Action=""SetNull"" />", @"<Double Action=""SetNull"" />",
                @"<String Action=""SetNull"" />", @"<Int64 Action=""SetNull"" />", @"<Int16 Action=""SetNull"" />",
                @"<SByte Action=""SetNull"" />", @"<Byte Action=""SetNull"" />", @"<UInt16 Action=""SetNull"" />",
                @"<UInt32 Action=""SetNull"" />", @"<UInt64 Action=""SetNull"" />", @"<Single Action=""SetNull"" />",
                @"<Decimal Action=""SetNull"" />", @"<DateTime Action=""SetNull"" />", @"<Guid Action=""SetNull"" />",
                @"<Char Action=""SetNull"" />", @"<TimeSpan Action=""SetNull"" />", @"<DateTimeOffset Action=""SetNull"" />"
#if NET_40_UP || NETSTANDARD_1_1_UP
                ,@"<BigInteger Action=""SetNull"" />"
#endif
                ,@"<Uri Action=""SetNull"" />"
            };
            //值类型默认值
            //https://msdn.microsoft.com/zh-cn/library/83fhsxwc.aspx
            var serializedDefaultValues = new[]
            {
                "<Boolean>" + XmlConvert.ToString(false) + "</Boolean>", "<Int32>" + 0 + "</Int32>",
                "<Double>" + XmlConvert.ToString(0D) + "</Double>", @"<String Action=""SetNull"" />",
                "<Int64>" + 0 + "</Int64>", "<Int16>" + 0 + "</Int16>", "<SByte>" + 0 + "</SByte>",
                "<Byte>" + 0 + "</Byte>", "<UInt16>" + 0 + "</UInt16>", "<UInt32>" + 0 + "</UInt32>",
                "<UInt64>" + 0 + "</UInt64>", "<Single>" + XmlConvert.ToString(0F) + "</Single>",
                "<Decimal>" + XmlConvert.ToString(0M) + "</Decimal>",
                "<DateTime>" + XmlConvert.ToString(new DateTime(), XmlDateTimeSerializationMode.RoundtripKind) +
                "</DateTime>",
                "<Guid>" + XmlConvert.ToString(Guid.Empty) + "</Guid>",
                "<Char>" + XmlConvert.ToString((ushort) '\0') + "</Char>",
                "<TimeSpan>" + XmlConvert.ToString(TimeSpan.Zero) + "</TimeSpan>",
                "<DateTimeOffset>" + XmlConvert.ToString((DateTimeOffset)ReflectionUtils.GetDefaultValue(typeof(DateTimeOffset))) + "</DateTimeOffset>"
#if NET_40_UP || NETSTANDARD_1_1_UP
                ,"<BigInteger>" + System.Numerics.BigInteger.Zero.ToString(CultureInfo.InvariantCulture) + "</BigInteger>"
#endif
                ,null
            };

            for (var i = 0; i < types.Length; i++)
            {
                using (var stream = new MemoryStream())
                {
                    using (ITextWriter writer = TestHelper.CreateWriter(stream))
                    {
                        var ser = new DivideBasic(writer, new TypeExtend(types[i], writer.IgnoreAttributeType));

                        //原始值与更新值不同时，序列化更新值
                        Assert.IsTrue(ser.Divide(types[i].Name, oriObjs[i], revObjs[i]));
                    }
                    stream.Position = 0;
                    var der = new CombineBasic(new TypeExtend(types[i], null));

                    AssertHelper.AreEqual(serializedResults[i], stream, "输出内容与预期不符");
                    using (XmlReader xmlReader = XmlReader.Create(stream))
                    {
                        using (XmlTextReader reader = new XmlTextReader(xmlReader))
                        {
                            Assert.AreEqual(revObjs[i], der.Combine(reader, null, types[i].Name), "增量合并后的对象实例与预期不符");
                        }
                    }
                }

                using (var stream = new MemoryStream())
                {
                    using (ITextWriter writer = TestHelper.CreateWriter(stream))
                    {
                        var ser = new DivideBasic(writer, new TypeExtend(types[i], writer.IgnoreAttributeType));
                        //原始值有值，更新值为null时，序列化为<XXX Action="SetNull" />
                        Assert.IsTrue(ser.Divide(types[i].Name, oriObjs[i], null));
                    }
                    stream.Position = 0;
                    var der = new CombineBasic(new TypeExtend(types[i], null));

                    AssertHelper.AreEqual(serializedSetNullResults[i], stream, "输出内容与预期不符");
                    using (XmlReader xmlReader = XmlReader.Create(stream))
                    {
                        using (XmlTextReader reader = new XmlTextReader(xmlReader))
                        {
                            Assert.AreEqual(null, der.Combine(reader, null, types[i].Name), "增量合并后的对象实例与预期不符");
                        }
                    }
                }

                using (var stream = new MemoryStream())
                {
                    using (ITextWriter writer = TestHelper.CreateWriter(stream))
                    {
                        var ser = new DivideBasic(writer, new TypeExtend(types[i], writer.IgnoreAttributeType));
                        //原始值为null，更新值有值时序列化更新值
                        Assert.IsTrue(ser.Divide(types[i].Name, null, revObjs[i]));
                    }
                    stream.Position = 0;
                    var der = new CombineBasic(new TypeExtend(types[i], null));

                    AssertHelper.AreEqual(serializedResults[i], stream, "输出内容与预期不符");
                    stream.Position = 0;
                    using (XmlReader xmlReader = XmlReader.Create(stream))
                    {
                        using (XmlTextReader reader = new XmlTextReader(xmlReader))
                        {
                            Assert.AreEqual(revObjs[i], der.Combine(reader, null, types[i].Name), "增量合并后的对象实例与预期不符");
                        }
                    }
                }

                using (var stream = new MemoryStream())
                {
                    using (ITextWriter writer = TestHelper.CreateWriter(stream))
                    {
                        var ser = new DivideBasic(writer, new TypeExtend(types[i], writer.IgnoreAttributeType));
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
                    using (ITextWriter writer = TestHelper.CreateWriter(stream))
                    {
                        writer.Setting.SerializeDefalutValue = true;
                        var ser = new DivideBasic(writer, new TypeExtend(types[i], writer.IgnoreAttributeType));
                        //原始值为null，如果更新值为默认值时，如果设置为序列化默认值，则做序列化

                        if (!string.IsNullOrEmpty(serializedDefaultValues[i]))
                        {
                            //只有当默认值不为null时，才做以下判断
                            Assert.IsTrue(ser.Divide(types[i].Name, null, ReflectionUtils.GetDefaultValue(types[i])));
                            //writer.WriteEndObject();
                            writer.Flush();
                            stream.Position = 0;
                            var der = new CombineBasic(new TypeExtend(types[i], null));

                            AssertHelper.AreEqual(serializedDefaultValues[i], stream, "输出内容与预期不符");
                        }
                    }
                }
            }
        }

        [Test]
        public void SerializeEnum()
        {
            var result = @"<EnumClass>
  <MultiQuarter>First, Second, Third, Fourth</MultiQuarter>
  <SingleQuarter>First</SingleQuarter>
</EnumClass>";
            var dser = new CombineObject(new TypeExtend(typeof(EnumClass), null));
            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    var ser = new DivideObject(writer, new TypeExtend(typeof(EnumClass), writer.IgnoreAttributeType));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(EnumClass)), null,
                        EnumClass.GetSampleInstance()));
                }
                AssertHelper.AreEqual(result, stream, "输出内容与预期不符");
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
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    var ser = new DivideObject(writer, new TypeExtend(typeof(EnumClass), writer.IgnoreAttributeType));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(EnumClass)),
                        EnumClass.GetSampleInstance(), c1));
                }
                AssertHelper.AreEqual(result, stream, "输出内容与预期不符");
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
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
        }

#if (NET || NETSTANDARD_2_0_UP)
        [Test]
        public void SerializeErrorColor()
        {
            var result = @"<ColorClass>
  <Color>#xxxxxx</Color>
</ColorClass>";

            var dser = new CombineObject(new TypeExtend(typeof(ColorClass), null));
            try
            {
                using (XmlReader xmlReader = XmlReader.Create(new StringReader(result)))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        dser.Combine(reader, ColorClass.GetSampleInstance(), "ColorClass");
                    }
                }
                Assert.Fail("未能抛出FormatException异常。");
            }
            catch (FormatException ex)
            {
                Assert.AreEqual(
                    string.Format(CultureInfo.InvariantCulture,
                        ResourceHelper.GetResourceString(LocalizationRes.Exp_String_IsNotColor), "#xxxxxx"),
                    ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("未能抛出FormatException异常。");
            }
        }
#endif

#if (NET || NETSTANDARD_2_0_UP)
        [Test]
        public void SerializeStruct()
        {
            Type[] typies = new Type[] { typeof(Size), typeof(SizeF), typeof(Point), typeof(PointF), typeof(Rectangle), typeof(RectangleF) };

            object[] values = new object[] { new Size(1, 2), new SizeF(1.1f, 2.2f), new Point(3, 4), new PointF(3.3f, 4.4f), new Rectangle(1, 2, 3, 4), new RectangleF(1.1f, 2.2f, 3.3f, 4.4f), };

            string[] results = new string[]
            {
                string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Size>
  <Height>{0}</Height>
  <Width>{1}</Width>
</Size>", ((Size)values[0]).Height,((Size)values[0]).Width),
                string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<SizeF>
  <Height>{0}</Height>
  <Width>{1}</Width>
</SizeF>", ((SizeF)values[1]).Height,((SizeF)values[1]).Width),
                string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Point>
  <X>{0}</X>
  <Y>{1}</Y>
</Point>", ((Point)values[2]).X,((Point)values[2]).Y),
                string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<PointF>
  <X>{0}</X>
  <Y>{1}</Y>
</PointF>", ((PointF)values[3]).X,((PointF)values[3]).Y),
                string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Rectangle>
  <Height>{0}</Height>
  <Location>
    <X>{2}</X>
    <Y>{3}</Y>
  </Location>
  <Size>
    <Height>{0}</Height>
    <Width>{1}</Width>
  </Size>
  <Width>{1}</Width>
  <X>{2}</X>
  <Y>{3}</Y>
</Rectangle>", ((Rectangle)values[4]).Height,((Rectangle)values[4]).Width,((Rectangle)values[4]).X,((Rectangle)values[4]).Y,((Rectangle)values[4]).Location,((Rectangle)values[4]).Size),
                string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<RectangleF>
  <Height>{0}</Height>
  <Location>
    <X>{2}</X>
    <Y>{3}</Y>
  </Location>
  <Size>
    <Height>{0}</Height>
    <Width>{1}</Width>
  </Size>
  <Width>{1}</Width>
  <X>{2}</X>
  <Y>{3}</Y>
</RectangleF>", ((RectangleF)values[5]).Height,((RectangleF)values[5]).Width,((RectangleF)values[5]).X,((RectangleF)values[5]).Y,((RectangleF)values[5]).Location,((RectangleF)values[5]).Size)
            };

            for (int i = 0; i < typies.Length; i++)
            {
                Serializer serializer = new Serializer(typies[i]);
                using (var stream = new MemoryStream())
                {
                    using (ITextWriter writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                    {
                        serializer.Divide(writer, null, values[i]);
                    }

                    AssertHelper.AreEqual(results[i], stream, "输出内容与预期不符");
                    using (XmlReader xmlReader = XmlReader.Create(stream))
                    {
                        using (XmlTextReader reader = new XmlTextReader(xmlReader))
                        {
                            var c2 =
                                serializer.Combine(reader, null);
                            Debug.Assert(c2 != null, "c2 != null");
                            Assert.AreEqual(values[i], c2);
                        }
                    }
                }
            }
        }

        [Test]
        public void SerializeColor_Argb()
        {
            var c1 = new ColorClass {Color = Color.FromArgb(255, 255, 255)};
            var result = string.Format(@"<ColorClass>
  <Color>#{0:X}</Color>
</ColorClass>", c1.Color.ToArgb());
            var dser = new CombineObject(new TypeExtend(typeof(ColorClass), null));
            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    var ser = new DivideObject(writer, new TypeExtend(typeof(ColorClass), writer.IgnoreAttributeType));
                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(ColorClass)), null, c1));
                }

                AssertHelper.AreEqual(result, stream, "输出内容与预期不符");
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        var c2 =
                            dser.Combine(reader, ColorClass.GetSampleInstance(),
                                ReflectionUtils.GetTypeFriendlyName(typeof(ColorClass))) as ColorClass;
                        Debug.Assert(c2 != null, "c2 != null");
                        Assert.AreEqual(c1.Color, c2.Color);
                    }
                }
            }
        }

        [Test]
        public void SerializeColor_KnownColor()
        {
            var result = @"<ColorClass>
  <Color>AliceBlue</Color>
</ColorClass>";
            using (var stream = new MemoryStream())
            {
                var c1 = ColorClass.GetSampleInstance();
                var dser = new CombineObject(new TypeExtend(typeof(ColorClass), null));
                using (ITextWriter writer = TestHelper.CreateWriter(stream))
                {
                    var ser = new DivideObject(writer, new TypeExtend(typeof(ColorClass), writer.IgnoreAttributeType));

                    Assert.IsTrue(ser.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(ColorClass)), null, c1));
                }
                AssertHelper.AreEqual(result, stream, "输出内容与预期不符");

                var c2 = ColorClass.GetSampleInstance();
                c2.Color = Color.AntiqueWhite;
                Assert.AreEqual(Color.AntiqueWhite, c2.Color);

                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        //合并数据时会默认更新传入的原始对象
                        dser.Combine(reader, c2, ReflectionUtils.GetTypeFriendlyName(typeof(ColorClass)));

                        Assert.AreEqual(ColorClass.GetSampleInstance().Color, c2.Color);
                    }
                }
            }
        }
#endif
    }
}