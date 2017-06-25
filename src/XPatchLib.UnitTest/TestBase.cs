// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Xml;
#if NUNIT
using NUnit.Framework;

#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest
{
    public abstract class TestBase
    {
        /// <summary>
        ///     比较字典类型对象是否相同。
        /// </summary>
        /// <typeparam name="T">Key值类型。</typeparam>
        /// <typeparam name="W">Value值类型。</typeparam>
        /// <param name="expect">这是单元测试要求的对象实例。</param>
        /// <param name="actual">这是单元测试生成的对象实例。</param>
        /// <param name="message">消息。</param>
        /// <param name="actualIsNotNull"><paramref name="actual" />是否为 <b>null</b> 。</param>
        /// <param name="hashCodeEquals"><paramref name="expect" /> 和 <paramref name="actual" /> 是否 <b>hashCode</b> 值相同。</param>
        protected void AssertDictionary<T, W>(Dictionary<T, W> expect, Dictionary<T, W> actual, string message,
            bool actualIsNotNull, bool hashCodeEquals)
        {
            if (actualIsNotNull)
                Assert.IsNotNull(actual, message);
            else
                Assert.IsNull(actual, message);
            Assert.AreEqual(expect.Count, actual.Count, message);
            foreach (var key in expect.Keys)
                Assert.AreEqual(expect[key], actual[key], message);
            if (hashCodeEquals)
                Assert.AreEqual(expect.GetHashCode(), actual.GetHashCode(), message);
            else
                Assert.AreNotEqual(expect.GetHashCode(), actual.GetHashCode(), message);
        }

        protected T DoCombineCore_Combie<T>(string context, T expected) where T : class
        {
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(context)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    return
                        new CombineCore(new TypeExtend(typeof(T), null)).Combine(reader, expected,
                            ReflectionUtils.GetTypeFriendlyName(typeof(T))) as T;
                }
            }
        }

        protected string DoDivideCore_Divide<T>(T ori, T rev)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = ForXml.TestHelper.CreateWriter(stream, ForXml.TestHelper.FlagmentSetting))
                {
                    TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(typeof(T), null, null);
                    var serializer = new DivideCore(writer, typeExtend);
                    bool succeeded = serializer.Divide(typeExtend.TypeFriendlyName, ori, rev);
                    Assert.IsTrue(succeeded);
                    writer.Flush();
                    return TestHelper.StreamToString(stream);
                }
            }
        }

        protected string DoDivideIDictionary_Divide<T>(T ori, T rev)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = ForXml.TestHelper.CreateWriter(stream, ForXml.TestHelper.FlagmentSetting))
                {
                    TypeExtend typeExtend = new TypeExtend(typeof(T), writer.IgnoreAttributeType);
                    var serializer = new DivideIDictionary(writer, typeExtend);
                    bool succeeded = serializer.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(T)), ori, rev);
                    Assert.IsTrue(succeeded);
                    writer.Flush();
                    return TestHelper.StreamToString(stream);
                }
            }
        }

        protected T DoSerializer_Combie<T>(string context, T expected, bool deepClone = false) where T : class
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(context)))
            {
                using (var xmlTextReader = new XmlTextReader(reader))
                {
                    return new Serializer(typeof(T)).Combine(xmlTextReader, expected, !deepClone) as T;
                }
            }
        }

        protected T DoCombineIDictionary_Combie<T>(string context, T expected) where T : class
        {
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(context)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    return
                        new CombineIDictionary(new TypeExtend(typeof(T), null)).Combine(reader, expected,
                            ReflectionUtils.GetTypeFriendlyName(typeof(T))) as T;
                }
            }
        }

        protected string DoSerializer_Divide<T>(T ori, T rev)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = ForXml.TestHelper.CreateWriter(stream, ForXml.TestHelper.DocumentSetting))
                {
                    var serializer = new Serializer(typeof(T));
                    serializer.Divide(writer, ori, rev);
                    return TestHelper.StreamToString(stream);
                }
            }
        }
    }
}