// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if (NET || NETSTANDARD_2_0_UP)
using System.Drawing;
#endif
#if (NET_35_UP || NETSTANDARD)
#endif
#if (NETSTANDARD_1_0 || NETSTANDARD_1_1 || NETSTANDARD_1_3)
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Xml;
using XPatchLib.UnitTest.TestClass;
#if NUNIT
using NUnit.Framework;
#elif XUNIT
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;

#endif

namespace XPatchLib.UnitTest.ForXml
{
    [TestFixture]
    public class TestSerializeBasic : TestBase
    {
        [Test]
        public void BasicSerializeCtorTest()
        {
            var writer = new XmlTextWriter();
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
                , System.Numerics.BigInteger.Zero
#endif
                , uri1
            };

            var revObjs = new object[]
            {
                true, int.MaxValue, double.MaxValue, s2, long.MaxValue, short.MaxValue, sbyte.MaxValue, byte.MaxValue,
                ushort.MaxValue, uint.MaxValue, ulong.MaxValue, float.MaxValue, decimal.MaxValue, DateTime.MaxValue, g2,
                'b', TimeSpan.MaxValue, DateTimeOffset.MaxValue
#if NET_40_UP || NETSTANDARD_1_1_UP
                , System.Numerics.BigInteger.MinusOne
#endif
                , uri2
            };

            var types = new[]
            {
                typeof(bool), typeof(int), typeof(double), typeof(string), typeof(long), typeof(short), typeof(sbyte),
                typeof(byte), typeof(ushort), typeof(uint), typeof(ulong), typeof(float), typeof(decimal),
                typeof(DateTime), typeof(Guid), typeof(char), typeof(TimeSpan), typeof(DateTimeOffset)
#if NET_40_UP || NETSTANDARD_1_1_UP
                , typeof(System.Numerics.BigInteger)
#endif
                , typeof(Uri)
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
                , "<BigInteger>" + System.Numerics.BigInteger.MinusOne.ToString(CultureInfo.InvariantCulture) + "</BigInteger>"
#endif
                , "<Uri>" + uri2 + "</Uri>"
            };
            var serializedSetNullResults = new[]
            {
                @"<Boolean Action=""SetNull"" />", @"<Int32 Action=""SetNull"" />", @"<Double Action=""SetNull"" />",
                @"<String Action=""SetNull"" />", @"<Int64 Action=""SetNull"" />", @"<Int16 Action=""SetNull"" />",
                @"<SByte Action=""SetNull"" />", @"<Byte Action=""SetNull"" />", @"<UInt16 Action=""SetNull"" />",
                @"<UInt32 Action=""SetNull"" />", @"<UInt64 Action=""SetNull"" />", @"<Single Action=""SetNull"" />",
                @"<Decimal Action=""SetNull"" />", @"<DateTime Action=""SetNull"" />", @"<Guid Action=""SetNull"" />",
                @"<Char Action=""SetNull"" />", @"<TimeSpan Action=""SetNull"" />",
                @"<DateTimeOffset Action=""SetNull"" />"
#if NET_40_UP || NETSTANDARD_1_1_UP
                , @"<BigInteger Action=""SetNull"" />"
#endif
                , @"<Uri Action=""SetNull"" />"
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
                "<DateTimeOffset>" +
                XmlConvert.ToString((DateTimeOffset) ReflectionUtils.GetDefaultValue(typeof(DateTimeOffset))) +
                "</DateTimeOffset>"
#if NET_40_UP || NETSTANDARD_1_1_UP
                , "<BigInteger>" + System.Numerics.BigInteger.Zero.ToString(CultureInfo.InvariantCulture) + "</BigInteger>"
#endif
                , null
            };

            string context = string.Empty;

            for (var i = 0; i < types.Length; i++)
            {
                #region 原始值与更新值不同时

                context = DoDivideBasic_Divide(types[i], oriObjs[i], revObjs[i], true);
                Assert.AreEqual(serializedResults[i], context, "输出内容与预期不符");
                var newObj1 = DoCombineBasic_Combie(context, types[i], null);
                Assert.AreEqual(revObjs[i], newObj1);

                #endregion

                #region 原始值有值，更新值为null时，序列化为<XXX Action="SetNull" />

                context = DoDivideBasic_Divide(types[i], oriObjs[i], null, true);
                Assert.AreEqual(serializedSetNullResults[i], context, "输出内容与预期不符");
                var newObj2 = DoCombineBasic_Combie(context, types[i], null);
                Assert.AreEqual(null, newObj2);

                #endregion

                #region 原始值为null，更新值有值时序列化更新值

                context = DoDivideBasic_Divide(types[i], null, revObjs[i], true);
                Assert.AreEqual(serializedResults[i], context, "输出内容与预期不符");
                var newObj3 = DoCombineBasic_Combie(context, types[i], null);
                Assert.AreEqual(revObjs[i], newObj3);

                #endregion

                //原始值与更新值相同时不做序列化
                DoDivideBasic_Divide(types[i], oriObjs[i], oriObjs[i], false);
                //原始值与更新值均为null时不做序列化
                DoDivideBasic_Divide(types[i], null, null, false);
                //原始值为null，如果更新值为默认值时，如果默认不序列化默认值，则不做序列化
                DoDivideBasic_Divide(types[i], null, ReflectionUtils.GetDefaultValue(types[i]), false);


                //原始值为null，如果更新值为默认值时，如果设置为序列化默认值，则做序列化
                if (!string.IsNullOrEmpty(serializedDefaultValues[i]))
                {
                    //只有当默认值不为null时，才做以下判断
                    ISerializeSetting setting = new XmlSerializeSetting {SerializeDefalutValue = true};
                    string c = DoDivideBasic_Divide(types[i], null, ReflectionUtils.GetDefaultValue(types[i]), true,
                        setting);
                    Assert.AreEqual(serializedDefaultValues[i], c);
                    //var der=DoCombineBasic_Combie(c,types[i],)
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

            string context = DoDivideObject_Divide(null, EnumClass.GetSampleInstance());
            Assert.AreEqual(result, context);

            var c1 = EnumClass.GetSampleInstance();
            c1.SingleQuarter = SingleQuarter.Second;
            c1.MultiQuarter = MultiQuarter.Second | MultiQuarter.Fourth;

            result = @"<EnumClass>
  <MultiQuarter>Second, Fourth</MultiQuarter>
  <SingleQuarter>Second</SingleQuarter>
</EnumClass>";

            context = DoDivideObject_Divide(EnumClass.GetSampleInstance(), c1);
            Assert.AreEqual(result, context);
            var c2 = DoCombineObject_Combie(context, EnumClass.GetSampleInstance());
            Assert.IsNotNull(c2);
            Assert.AreEqual(SingleQuarter.Second, c2.SingleQuarter);
            Assert.AreEqual(MultiQuarter.Second | MultiQuarter.Fourth, c2.MultiQuarter);
        }

#if (NET || NETSTANDARD_2_0_UP)
        [Test]
        public void SerializeErrorColor()
        {
            var result = @"<ColorClass>
  <Color>#xxxxxx</Color>
</ColorClass>";
            
            try
            {
                DoCombineObject_Combie(result, ColorClass.GetSampleInstance());
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


        [Test]
        public void SerializeStruct()
        {
            Type[] typies = new Type[]
            {
                typeof(Size), typeof(SizeF), typeof(Point), typeof(PointF), typeof(Rectangle), typeof(RectangleF)
#if NET_35_UP
                , typeof(System.Windows.Vector), typeof(System.Windows.Thickness)
#endif
            };

            object[] values = new object[]
            {
                new Size(1, 2), new SizeF(1.1f, 2.2f), new Point(3, 4), new PointF(3.3f, 4.4f),
                new Rectangle(1, 2, 3, 4), new RectangleF(1.1f, 2.2f, 3.3f, 4.4f)
#if NET_35_UP
                , new System.Windows.Vector(2.345d, 3.456d),
                new System.Windows.Thickness(1.234d, 2.345d, 3.456d, 4.567d)
#endif
            };

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
</Rectangle>", ((Rectangle)values[4]).Height,((Rectangle)values[4]).Width,((Rectangle)values[4]).X,((Rectangle)values[4]).Y),
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
</RectangleF>", ((RectangleF)values[5]).Height,((RectangleF)values[5]).Width,((RectangleF)values[5]).X,((RectangleF)values[5]).Y)
#if NET_35_UP
                ,string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Vector>
  <X>{0}</X>
  <Y>{1}</Y>
</Vector>", ((System.Windows.Vector)values[6]).X, ((System.Windows.Vector)values[6]).Y)
                ,string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Thickness>
  <Bottom>{0}</Bottom>
  <Left>{1}</Left>
  <Right>{2}</Right>
  <Top>{3}</Top>
</Thickness>", ((System.Windows.Thickness)values[7]).Bottom, ((System.Windows.Thickness)values[7]).Left, ((System.Windows.Thickness)values[7]).Right, ((System.Windows.Thickness)values[7]).Top)
#endif
            };

            for (int i = 0; i < typies.Length; i++)
            {
                DoAssert(typies[i], results[i], null, values[i], true);
            }
        }

        [Test]
        public void SerializeColor_Argb()
        {
            var c1 = new ColorClass {Color = Color.FromArgb(255, 255, 255)};
            var result = string.Format(@"<ColorClass>
  <Color>#{0:X}</Color>
</ColorClass>", c1.Color.ToArgb());

            string context = DoDivideObject_Divide(null, c1);
            Assert.AreEqual(result, context);
            var c2 = DoCombineObject_Combie(context, ColorClass.GetSampleInstance());
            Debug.Assert(c2 != null, "c2 != null");
            Assert.AreEqual(c1.Color, c2.Color);
        }

        [Test]
        public void SerializeColor_KnownColor()
        {
            var result = @"<ColorClass>
  <Color>AliceBlue</Color>
</ColorClass>";

            var c1 = ColorClass.GetSampleInstance();

            string context = DoDivideObject_Divide(null, c1);
            Assert.AreEqual(result, context);

            var c2 = ColorClass.GetSampleInstance();
            c2.Color = Color.AntiqueWhite;
            Assert.AreEqual(Color.AntiqueWhite, c2.Color);

            DoCombineObject_Combie(context, c2);

            Assert.AreEqual(ColorClass.GetSampleInstance().Color, c2.Color);
            
        }

#endif

        internal void AssertPropertyValue(Type type, object objA, object objB, string proName, BindingFlags bindingAttr)
        {
            PropertyInfo pro = type.GetProperty(proName, bindingAttr);
            if (pro == null)
                Assert.Fail("pro==null");
            Assert.AreEqual(pro.GetValue(objA, null), pro.GetValue(objB, null));
        }

        internal void AssertFieldValue(Type type, object objA, object objB, string proName, BindingFlags bindingAttr)
        {
            FieldInfo fi = type.GetField(proName, bindingAttr);
            if (fi == null)
                Assert.Fail("fi==null");
            Assert.AreEqual(fi.GetValue(objA), fi.GetValue(objB));
        }

        [Test]
        public void TestSerializeMemberTypeAndModifier()
        {
            Type t = typeof(TestOverrideClass);

            List<ISerializeSetting> settings = new List<ISerializeSetting>();
            List<BindingFlags> flags = new List<BindingFlags>();
            List<string[]> proNames = new List<string[]>();
            List<string[]> fieldNames = new List<string[]>();

            //只序列化Public属性
            settings.Add(new XmlSerializeSetting());
            flags.Add(BindingFlags.Public | BindingFlags.Instance);
            proNames.Add(new[] {"PublicPro"});
            fieldNames.Add(new string[] { });

            //只序列化Public参数
            settings.Add(new XmlSerializeSetting {MemberType = SerializeMemberType.Field});
            flags.Add(BindingFlags.Public | BindingFlags.Instance);
            proNames.Add(new string[] { });
            fieldNames.Add(new[] {"PublicField"});

            //序列化Public属性+参数
            settings.Add(new XmlSerializeSetting {MemberType = SerializeMemberType.All});
            flags.Add(BindingFlags.Public | BindingFlags.Instance);
            proNames.Add(new[] {"PublicPro"});
            fieldNames.Add(new[] {"PublicField"});

            //只序列化Public属性
            settings.Add(new XmlSerializeSetting {Modifier = SerializeMemberModifier.Public});
            flags.Add(BindingFlags.Public | BindingFlags.Instance);
            proNames.Add(new[] {"PublicPro"});
            fieldNames.Add(new string[] { });

            //只序列化Public参数
            settings.Add(new XmlSerializeSetting
            {
                MemberType = SerializeMemberType.Field,
                Modifier = SerializeMemberModifier.Public
            });
            flags.Add(BindingFlags.Public | BindingFlags.Instance);
            proNames.Add(new string[] { });
            fieldNames.Add(new[] {"PublicField"});

            //序列化Public属性+参数
            settings.Add(new XmlSerializeSetting
            {
                MemberType = SerializeMemberType.All,
                Modifier = SerializeMemberModifier.Public
            });
            flags.Add(BindingFlags.Public | BindingFlags.Instance);
            proNames.Add(new[] {"PublicPro"});
            fieldNames.Add(new[] {"PublicField"});

            //只序列化Private属性
            settings.Add(new XmlSerializeSetting {Modifier = SerializeMemberModifier.Private});
            flags.Add(BindingFlags.NonPublic | BindingFlags.Instance);
            proNames.Add(new[] {"PrivatePro"});
            fieldNames.Add(new string[] { });

            //只序列化Private参数
            settings.Add(new XmlSerializeSetting
            {
                MemberType = SerializeMemberType.Field,
                Modifier = SerializeMemberModifier.Private
            });
            flags.Add(BindingFlags.NonPublic | BindingFlags.Instance);
            proNames.Add(new string[] { });
            fieldNames.Add(new[] {"PrivateField"});

            //序列化Private属性+参数
            settings.Add(new XmlSerializeSetting
            {
                MemberType = SerializeMemberType.All,
                Modifier = SerializeMemberModifier.Private
            });
            flags.Add(BindingFlags.NonPublic | BindingFlags.Instance);
            proNames.Add(new[] {"PrivatePro"});
            fieldNames.Add(new[] {"PrivateField"});

            //只序列化Protected属性
            settings.Add(new XmlSerializeSetting {Modifier = SerializeMemberModifier.Protected});
            flags.Add(BindingFlags.NonPublic | BindingFlags.Instance);
            proNames.Add(new[] {"ProtectedPro"});
            fieldNames.Add(new string[] { });

            //只序列化Protected参数
            settings.Add(new XmlSerializeSetting
            {
                MemberType = SerializeMemberType.Field,
                Modifier = SerializeMemberModifier.Protected
            });
            flags.Add(BindingFlags.NonPublic | BindingFlags.Instance);
            proNames.Add(new string[] { });
            fieldNames.Add(new[] {"ProtectedField"});

            //序列化Protected属性+参数
            settings.Add(new XmlSerializeSetting
            {
                MemberType = SerializeMemberType.All,
                Modifier = SerializeMemberModifier.Protected
            });
            flags.Add(BindingFlags.NonPublic | BindingFlags.Instance);
            proNames.Add(new[] {"ProtectedPro"});
            fieldNames.Add(new[] {"ProtectedField"});

            //只序列化Internal属性
            settings.Add(new XmlSerializeSetting {Modifier = SerializeMemberModifier.Internal});
            flags.Add(BindingFlags.NonPublic | BindingFlags.Instance);
            proNames.Add(new[] {"InternalPro"});
            fieldNames.Add(new string[] { });

            //只序列化Internal参数
            settings.Add(new XmlSerializeSetting
            {
                MemberType = SerializeMemberType.Field,
                Modifier = SerializeMemberModifier.Internal
            });
            flags.Add(BindingFlags.NonPublic | BindingFlags.Instance);
            proNames.Add(new string[] { });
            fieldNames.Add(new[] {"InternalField"});

            //序列化Internal属性+参数
            settings.Add(new XmlSerializeSetting
            {
                MemberType = SerializeMemberType.All,
                Modifier = SerializeMemberModifier.Internal
            });
            flags.Add(BindingFlags.NonPublic | BindingFlags.Instance);
            proNames.Add(new[] {"InternalPro"});
            fieldNames.Add(new[] {"InternalField"});

            //序列化NonPublic属性
            settings.Add(new XmlSerializeSetting {Modifier = SerializeMemberModifier.NonPublic});
            flags.Add(BindingFlags.NonPublic | BindingFlags.Instance);
            proNames.Add(new[] {"PrivatePro", "InternalPro", "ProtectedPro"});
            fieldNames.Add(new string[] { });

            //只序列化NonPublic参数
            settings.Add(new XmlSerializeSetting
            {
                MemberType = SerializeMemberType.Field,
                Modifier = SerializeMemberModifier.NonPublic
            });
            flags.Add(BindingFlags.NonPublic | BindingFlags.Instance);
            proNames.Add(new string[] { });
            fieldNames.Add(new[] { "PrivateField", "InternalField", "ProtectedField"});

            //序列化NonPublic属性+参数
            settings.Add(new XmlSerializeSetting
            {
                MemberType = SerializeMemberType.All,
                Modifier = SerializeMemberModifier.NonPublic
            });
            flags.Add(BindingFlags.NonPublic | BindingFlags.Instance);
            proNames.Add(new[] {"PrivatePro", "InternalPro", "ProtectedPro"});
            fieldNames.Add(new[] { "PrivateField", "InternalField", "ProtectedField"});

            for (int i = 0; i < settings.Count; i++)
            {
                string result = DoSerializer_Divide(new TestOverrideClass(), TestOverrideClass.CreateInstance(),
                    settings[i]);
                LogHelper.Debug(result);
                //Assert.AreEqual(GetCompleteContext(contexts[i]), result);
                TestOverrideClass newObj =
                    DoSerializer_Combie(t, result, new TestOverrideClass(), settings[i]) as TestOverrideClass;
                foreach (var proName in proNames[i])
                    AssertPropertyValue(t, newObj, TestOverrideClass.CreateInstance(), proName, flags[i]);
                foreach (var fieldName in fieldNames[i])
                    AssertFieldValue(t, newObj, TestOverrideClass.CreateInstance(), fieldName, flags[i]);
            }
        }

        private abstract class TestAbstractClass
        {
            internal string InternalField;
            protected string ProtectedField;

            public string PublicField;
            public virtual string PublicPro { get; set; }
            protected virtual string ProtectedPro { get; set; }
            internal virtual string InternalPro { get; set; }

            public override bool Equals(object obj)
            {
                TestAbstractClass c = obj as TestAbstractClass;
                if (c == null) return false;
                return string.Equals(PublicPro, c.PublicPro, StringComparison.Ordinal)
                       && string.Equals(ProtectedPro, c.ProtectedPro, StringComparison.Ordinal)
                       && string.Equals(InternalPro, c.InternalPro, StringComparison.Ordinal)
                       && string.Equals(PublicField, c.PublicField, StringComparison.Ordinal)
                       && string.Equals(ProtectedField, c.ProtectedField, StringComparison.Ordinal)
                       && string.Equals(InternalField, c.InternalField, StringComparison.Ordinal);
            }
        }

        private class TestOverrideClass : TestAbstractClass
        {
            private string PrivateField;
            private string PrivatePro { get; set; }

            public static TestOverrideClass CreateInstance()
            {
                TestOverrideClass result = new TestOverrideClass();
                result.PublicPro = "PublicPro";
                result.ProtectedPro = "ProtectedPro";
                result.InternalPro = "InternalPro";
                result.PrivatePro = "PrivatePro";
                result.PublicField = "PublicField";
                result.ProtectedField = "ProtectedField";
                result.InternalField = "InternalField";
                result.PrivateField = "PrivateField";
                return result;
            }

            public override bool Equals(object obj)
            {
                TestOverrideClass c = obj as TestOverrideClass;
                if (c == null) return false;
                return base.Equals(obj)
                       && string.Equals(PrivatePro, c.PrivatePro, StringComparison.Ordinal)
                       && string.Equals(PrivateField, c.PrivateField, StringComparison.Ordinal);
            }
        }
    }
}