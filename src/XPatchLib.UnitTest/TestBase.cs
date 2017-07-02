// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
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
        public static string ResolvePath(string path)
        {
#if !NETSTANDARD
            return Path.Combine(TestContext.CurrentContext.TestDirectory, path);
#else
            return path;
#endif
        }

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

        /// <summary>
        /// 通过 <see cref="Serializer"/> ，首先在 <paramref name="pOriValue"/> 和 <paramref name="pRevValue"/>间产生增量，比较增量与 <paramref name="context"/> 是否一致，再使用增量产生新的对象实例，并比较新的对象实例与 <paramref name="pOriValue"/> 是否值相等。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">待比较的增量字符串。</param>
        /// <param name="pOriValue">原始对象实例。</param>
        /// <param name="pRevValue">更新后的对象实例。</param>
        /// <param name="createNew">是否在合并增量时创建新实例。</param>
        /// <remarks>不指定对象类型，使用泛型 <typeparamref name="T"/> 对应的类型。</remarks>
        protected T DoAssert<T>(string context, T pOriValue, T pRevValue, bool createNew)
        {
            return DoAssert(typeof(T), context, pOriValue, pRevValue, createNew);
        }

        /// <summary>
        /// 通过 <see cref="Serializer"/> ，首先在 <paramref name="pOriValue"/> 和 <paramref name="pRevValue"/>间产生增量，比较增量与 <paramref name="context"/> 是否一致，再使用增量产生新的对象实例，并比较新的对象实例与 <paramref name="pOriValue"/> 是否值相等。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pType">对象类型。</param>
        /// <param name="context">待比较的增量字符串。</param>
        /// <param name="pOriValue">原始对象实例。</param>
        /// <param name="pRevValue">更新后的对象实例。</param>
        /// <param name="createNew">是否在合并增量时创建新实例。</param>
        protected T DoAssert<T>(Type pType, string context, T pOriValue,T pRevValue,bool createNew)
        {
            return DoAssert(pType, context, pOriValue, pRevValue, createNew, null);
        }

        /// <summary>
        /// 通过 <see cref="Serializer"/> ，首先在 <paramref name="pOriValue"/> 和 <paramref name="pRevValue"/>间产生增量，比较增量与 <paramref name="context"/> 是否一致，再使用增量产生新的对象实例，并比较新的对象实例与 <paramref name="pOriValue"/> 是否值相等。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pType">对象类型。</param>
        /// <param name="context">待比较的增量字符串。如果传入 <see cref="string.Empty"/> 表示不比较产生的差量内容。</param>
        /// <param name="pOriValue">原始对象实例。</param>
        /// <param name="pRevValue">更新后的对象实例。</param>
        /// <param name="createNew">是否在合并增量时创建新实例。</param>
        /// <param name="setting">序列化设定，可以为 <b>null</b></param>
        protected T DoAssert<T>(Type pType, string context, T pOriValue, T pRevValue, bool createNew,
            ISerializeSetting setting)
        {
            return DoAssert(pType, context, pOriValue, pRevValue, createNew, setting, null);
        }


        /// <summary>
        /// 通过 <see cref="Serializer"/> ，首先在 <paramref name="pOriValue"/> 和 <paramref name="pRevValue"/>间产生增量，比较增量与 <paramref name="context"/> 是否一致，再使用增量产生新的对象实例，并比较新的对象实例与 <paramref name="pOriValue"/> 是否值相等。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pType">对象类型。</param>
        /// <param name="context">待比较的增量字符串。如果传入 <see cref="string.Empty"/> 表示不比较产生的差量内容。</param>
        /// <param name="pOriValue">原始对象实例。</param>
        /// <param name="pRevValue">更新后的对象实例。</param>
        /// <param name="createNew">是否在合并增量时创建新实例。</param>
        /// <param name="setting">序列化设定，可以为 <b>null</b></param>
        /// <param name="setting">待注册的类型与主键的键值对字典，可以为 <b>null</b></param>
        protected T DoAssert<T>(Type pType, string context, T pOriValue, T pRevValue, bool createNew,
            ISerializeSetting setting, IDictionary<Type, string[]> pTypes)
        {
            T result = default(T);
            Serializer serializer = new Serializer(pType);
            if(pTypes!=null && pTypes.Count>0)
                serializer.RegisterTypes(pTypes);
            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer =
                    XPatchLib.UnitTest.ForXml.TestHelper.CreateWriter(stream,
                        XPatchLib.UnitTest.ForXml.TestHelper.DocumentSetting))
                {
                    if (setting != null)
                        writer.Setting = setting;
                    serializer.Divide(writer, pOriValue, pRevValue);
                }
                stream.Position = 0;
                if (!string.IsNullOrEmpty(context))
                    AssertHelper.AreEqual(context, stream, "输出内容与预期不符");
                if (string.Equals(ForXml.TestHelper.XmlHeaderContext, TestHelper.StreamToString(stream)))
                    return default(T);
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        if (setting != null)
                            reader.Setting = setting;
                        result = (T)serializer.Combine(reader, pOriValue, !createNew);
                        if (createNew)
                        {
                            Assert.AreNotEqual(result, pOriValue);
                            if (pOriValue != null && pRevValue != null)
                                Assert.AreNotEqual(result.GetHashCode(), pOriValue.GetHashCode());
                        }
                        else
                        {
                            if (!IsValueType(pType))
                            {
                                if (pOriValue != null)
                                    //如果pOriValue为null，那么会始终创建一个新的实例，所以肯定不相同
                                    Assert.AreEqual(result, pOriValue);
                                if (pOriValue != null && pRevValue != null)
                                    Assert.AreEqual(result.GetHashCode(), pOriValue.GetHashCode());
                            }
                        }
                        Assert.AreEqual(pRevValue, result);
                        if (!IsValueType(pType) && pRevValue != null)
                            Assert.AreNotEqual(result.GetHashCode(), pRevValue.GetHashCode());
                    }
                }
            }
            return result;
        }

        private bool IsValueType(Type type)
        {
            if (ReflectionUtils.IsArray(type))
            {
                Type t;
                if (ReflectionUtils.TryGetArrayElementType(type, out t))
                    return t.IsValueType() || (t == typeof(string));
            }
            if (ReflectionUtils.IsIEnumerable(type))
            {
                Type t;
                if (ReflectionUtils.TryGetIEnumerableGenericArgument(type, out t))
                    return t.IsValueType() || (t == typeof(string));
            }
            return type.IsValueType() || (type == typeof(string));
        }
    }
}