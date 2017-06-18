using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace XPatchLib.UnitTest
{
    public abstract class TestBase
    {
        /// <summary>
        /// 比较字典类型对象是否相同。
        /// </summary>
        /// <typeparam name="T">Key值类型。</typeparam>
        /// <typeparam name="W">Value值类型。</typeparam>
        /// <param name="expect">这是单元测试要求的对象实例。</param>
        /// <param name="actual">这是单元测试生成的对象实例。</param>
        /// <param name="message">消息。</param>
        /// <param name="actualIsNotNull"><paramref name="actual"/>是否为 <b>null</b> 。</param>
        /// <param name="hashCodeEquals"><paramref name="expect"/> 和 <paramref name="actual"/> 是否 <b>hashCode</b> 值相同。</param>
        protected void AssertDictionary<T, W>(Dictionary<T, W> expect, Dictionary<T, W> actual, string message,
            bool actualIsNotNull,bool hashCodeEquals)
        {
            if (actualIsNotNull)
                Assert.IsNotNull(actual, message);
            else
            {
                Assert.IsNull(actual, message);
            }
            Assert.AreEqual(expect.Count, actual.Count, message);
            foreach (var key in expect.Keys)
            {
                Assert.AreEqual(expect[key], actual[key], message);
            }
            if(hashCodeEquals)
                Assert.AreEqual(expect.GetHashCode(), actual.GetHashCode(), message);
            else
            {
                Assert.AreNotEqual(expect.GetHashCode(), actual.GetHashCode(), message);
            }
        }
        protected T DoCombineCore_Combie<T>(string context, T expected) where T : class
        {
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(context)))
            {
                using (var reader = new XmlTextReader(xmlReader))
                {
                    return
                        new CombineCore(new TypeExtend(expected.GetType(), null)).Combine(reader, expected,
                            ReflectionUtils.GetTypeFriendlyName(expected.GetType())) as T;
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
                    return UnitTest.TestHelper.StreamToString(stream);
                }
            }
        }

        protected string DoDivideIDictionary_Divide<T>(T ori, T rev)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = ForXml.TestHelper.CreateWriter(stream, ForXml.TestHelper.FlagmentSetting))
                {
                    TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(typeof(T), null, null);
                    var serializer = new DivideIDictionary(writer, typeExtend);
                    bool succeeded = serializer.Divide(typeExtend.TypeFriendlyName, ori, rev);
                    Assert.IsTrue(succeeded);
                    return UnitTest.TestHelper.StreamToString(stream);
                }
            }
        }

        protected T DoSerializer_Combie<T>(string context, T expected, bool deepClone = false) where T : class
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(context)))
            {
                using (var xmlTextReader = new XmlTextReader(reader))
                {
                    return new Serializer(expected.GetType()).Combine(xmlTextReader, expected, deepClone) as T;
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
                        new CombineIDictionary(new TypeExtend(expected.GetType(), null)).Combine(reader, expected,
                            ReflectionUtils.GetTypeFriendlyName(expected.GetType())) as T;
                }
            }
        }

        protected string DoSerializer_Divide<T>(T ori, T rev)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = ForXml.TestHelper.CreateWriter(stream, ForXml.TestHelper.DocumentSetting))
                {
                    var serializer = new Serializer(ori.GetType());
                    serializer.Divide(writer, ori, rev);
                    return UnitTest.TestHelper.StreamToString(stream);
                }
            }
        }
    }
}
