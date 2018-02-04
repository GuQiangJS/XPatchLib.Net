// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
#if NET || NETSTANDARD_2_0_UP
using System.Runtime.Serialization;
#endif
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
#if XUNIT
        public TestBase()
        {
            TestInitialize();
        }
#endif
#if NUNIT
        [SetUp]
#endif
        public virtual void TestInitialize()
        {
            TypeExtendContainer.ClearAll();
        }

        internal const string XmlHeaderContext = @"<?xml version=""1.0"" encoding=""utf-8""?>";

        protected ISerializeSetting DefaultXmlSerializeSetting = new XmlSerializeSetting();

        internal static XmlWriterSettings DocumentSetting
        {
            get
            {
                var settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = false;
                settings.Encoding = new UTF8Encoding(false);
                return settings;
            }
        }

        internal static XmlWriterSettings FlagmentSetting
        {
            get
            {
                var settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                settings.Indent = true;
                settings.Encoding = new UTF8Encoding(false);
                settings.OmitXmlDeclaration = false;
                return settings;
            }
        }

        protected virtual ITextWriter CreateWriter(StringBuilder output)
        {
            ExtentedStringWriter sw = new ExtentedStringWriter(output, new UTF8Encoding(false));
            return new XmlTextWriter(sw);
        }

        //protected void PrivateAssertIEnumerable<T>(Type pType, object pOriObj, object pChangedObj,
        //    string pChangedContext, string pAssert)
        //{
        //    PrivateAssertIEnumerable<T>(pType, pOriObj, pChangedObj, pChangedContext, pAssert,
        //        DateTimeSerializationMode.RoundtripKind);
        //}

        //protected void PrivateAssertIEnumerable<T>(Type pType, object pOriObj, object pChangedObj,
        //    string pChangedContext, string pAssert, DateTimeSerializationMode pMode)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    using (ITextWriter writer = CreateWriter(sb, FlagmentSetting))
        //    {
        //        writer.Setting.Mode = pMode;
        //        Assert.IsTrue(
        //            new DivideCore(writer, new TypeExtend(pType, writer.IgnoreAttributeType)).Divide(
        //                ReflectionUtils.GetTypeFriendlyName(pType), pOriObj, pChangedObj));
        //    }
        //    using (XmlReader xmlReader = XmlReader.Create(sb.ToString()))
        //    {
        //        using (XmlTextReader reader = new XmlTextReader(xmlReader))
        //        {
        //            reader.Setting.Mode = pMode;
        //            var combinedObj = new CombineCore(new TypeExtend(pType, null)).Combine(reader, pOriObj,
        //                ReflectionUtils.GetTypeFriendlyName(pType));
                    
        //            Assert.AreEqual(pChangedContext, sb.ToString());

        //            UnitTest.TestHelper.PrivateAssertIEnumerable<T>(pChangedObj, combinedObj, pType, pAssert);
        //        }
        //    }
        //}

        public static string ResolvePath(string path)
        {
#if !NETSTANDARD
            return Path.Combine(TestContext.CurrentContext.TestDirectory, path);
#else
            return path;
#endif
        }

        protected byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                    stream.Position = originalPosition;
            }
        }

        protected string StreamToString(Stream stream)
        {
            stream.Position = 0;


            byte[] bytes = ReadToEnd(stream);
            //int numBytesToRead = (int)stream.Length;
            //stream.Read(bytes, 0, numBytesToRead);
            //stream.Position = 0;
            Encoding encoding = new UTF8Encoding(false);
            return encoding.GetString(bytes);
        }

        protected Stream StringToStream(string str)
        {
            var strBytes = Encoding.UTF8.GetBytes(str);
            return new MemoryStream(strBytes);
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

        protected object DoCombineBasic_Combie(string context, Type type, object expected)
        {
            using (var reader = new XmlTextReader(new StringReader(context)))
            {
                return
                    new CombineBasic(new TypeExtend(DefaultXmlSerializeSetting,type, null)).Combine(reader, expected,
                        ReflectionUtils.GetTypeFriendlyName(type));
            }
        }

        protected T DoCombineBasic_Combie<T>(string context, T expected) where T : class
        {
            using (var reader = new XmlTextReader(new StringReader(context)))
            {
                return
                    new CombineBasic(new TypeExtend(DefaultXmlSerializeSetting,typeof(T), null)).Combine(reader, expected,
                        ReflectionUtils.GetTypeFriendlyName(typeof(T))) as T;
            }
        }

        protected object DoCombineObject_Combie(Type type, string context, object expected)
        {
            using (var reader = new XmlTextReader(new StringReader(context)))
            {
                return
                    new CombineObject(new TypeExtend(DefaultXmlSerializeSetting,type, null)).Combine(reader, expected,
                        ReflectionUtils.GetTypeFriendlyName(type));
            }
        }

        protected T DoCombineObject_Combie<T>(string context, T expected) where T : class
        {
            using (var reader = new XmlTextReader(new StringReader(context)))
            {
                return
                    new CombineObject(new TypeExtend(DefaultXmlSerializeSetting, typeof(T), null)).Combine(reader, expected,
                        ReflectionUtils.GetTypeFriendlyName(typeof(T))) as T;
            }
        }

        protected T DoCombineCore_Combie<T>(string context, T expected) where T : class
        {
            using (var reader = new XmlTextReader(new StringReader(context)))
            {
                return
                    new CombineCore(new TypeExtend(DefaultXmlSerializeSetting, typeof(T), null)).Combine(reader, expected,
                        ReflectionUtils.GetTypeFriendlyName(typeof(T))) as T;
            }
        }

        protected string DoDivideBasic_Divide<T>(T ori, T rev)
        {
            return DoDivideBasic_Divide(ori, rev, true);
        }

        protected string DoDivideBasic_Divide<T>(T ori, T rev,bool succeed)
        {
            StringBuilder result = new StringBuilder();
            using (var writer = CreateWriter(result))
            {
                TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(DefaultXmlSerializeSetting, typeof(T), null, null);
                var serializer = new DivideBasic(writer, typeExtend);
                bool succeeded = serializer.Divide(typeExtend.TypeFriendlyName, ori, rev);
                Assert.AreEqual(succeed, succeeded);
            }
            return result.ToString();
        }

#if NET || NETSTANDARD_2_0_UP
        protected string DoDivideISerializable_Divide<T>(T ori, T rev, bool succeed) where T:ISerializable
        {
            StringBuilder result = new StringBuilder();
            using (var writer = CreateWriter(result))
            {
                TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(DefaultXmlSerializeSetting, typeof(T), null, null);
                var serializer = new DivideISerializable(writer, typeExtend);
                bool succeeded = serializer.Divide(typeExtend.TypeFriendlyName, ori, rev);
                Assert.AreEqual(succeed, succeeded);
            }
            return result.ToString();
        }
#endif

        protected string DoDivideBasic_Divide(Type type,object ori, object rev, bool succeed)
        {
            return DoDivideBasic_Divide(type, ori, rev, succeed, null);
        }

        protected string DoDivideBasic_Divide(Type type, object ori, object rev, bool succeed,
            ISerializeSetting setting)
        {
            StringBuilder result = new StringBuilder();
            using (var writer = CreateWriter(result))
            {
                if (setting != null)
                {
                    writer.Setting = setting;
                }
                TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(DefaultXmlSerializeSetting, type, null, null);
                var serializer = new DivideBasic(writer, typeExtend);
                bool succeeded = serializer.Divide(typeExtend.TypeFriendlyName, ori, rev);
                Assert.AreEqual(succeed, succeeded);
            }
            return result.ToString();
        }

        protected string DoDivideCore_Divide<T>(T ori, T rev)
        {
            StringBuilder result = new StringBuilder();
            using (var writer = CreateWriter(result))
            {
                TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(DefaultXmlSerializeSetting, typeof(T), null, null);
                var serializer = new DivideCore(writer, typeExtend);
                bool succeeded = serializer.Divide(typeExtend.TypeFriendlyName, ori, rev);
                Assert.IsTrue(succeeded);
            }
            return result.ToString();
        }

        protected string DoDivideObject_Divide<T>(T ori, T rev)
        {
            StringBuilder result = new StringBuilder();
            using (var writer = CreateWriter(result))
            {
                TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(DefaultXmlSerializeSetting, typeof(T), null, null);
                var serializer = new DivideObject(writer, typeExtend);
                bool succeeded = serializer.Divide(typeExtend.TypeFriendlyName, ori, rev);
                Assert.IsTrue(succeeded);
            }
            return result.ToString();
        }

        protected string DoDivideIDictionary_Divide<T>(T ori, T rev)
        {
            StringBuilder result = new StringBuilder();
            using (var writer = CreateWriter(result))
            {
                TypeExtend typeExtend = new TypeExtend(DefaultXmlSerializeSetting, typeof(T), writer.IgnoreAttributeType);
                var serializer = new DivideIDictionary(writer, typeExtend);
                bool succeeded = serializer.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(T)), ori, rev);
                Assert.IsTrue(succeeded);
            }
            return result.ToString();
        }

        protected string DoDivideKeyValuePair_Divide<T>(T ori, T rev)
        {
            StringBuilder result = new StringBuilder();
            using (var writer = CreateWriter(result))
            {
                TypeExtend typeExtend = new TypeExtend(DefaultXmlSerializeSetting, typeof(T), writer.IgnoreAttributeType);
                var serializer = new DivideKeyValuePair(writer, typeExtend);
                bool succeeded = serializer.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(T)), ori, rev);
                Assert.IsTrue(succeeded);
            }
            return result.ToString();
        }

        protected string DoDivideIEnumerable_Divide<T>(T ori, T rev)
        {
            return DoDivideIEnumerable_Divide(ori, rev, null);
        }
        protected string DoDivideIEnumerable_Divide<T>(T ori, T rev,ISerializeSetting setting)
        {
            StringBuilder result = new StringBuilder();
            using (var writer = CreateWriter(result))
            {
                if (setting != null)
                    writer.Setting = setting;
                TypeExtend typeExtend = new TypeExtend(DefaultXmlSerializeSetting, typeof(T), writer.IgnoreAttributeType);
                var serializer = new DivideIEnumerable(writer, typeExtend);
                bool succeeded = serializer.Divide(ReflectionUtils.GetTypeFriendlyName(typeof(T)), ori, rev);
                Assert.IsTrue(succeeded);
            }
            return result.ToString();
        }

        /// <summary>
        ///     使用<see cref="Serializer" />合并增量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="expected"></param>
        /// <param name="deepClone"></param>
        /// <returns></returns>
        protected T DoSerializer_Combie<T>(string context, T expected, bool deepClone = false) where T : class
        {
            XmlReaderSettings settings = new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};

            using (var reader = new XmlTextReader(new StringReader(context)))
            {
                return new Serializer(typeof(T)).Combine(reader, expected, !deepClone) as T;
            }
        }
        protected T DoSerializer_Combie<T>(string context, T expected,ISerializeSetting setting, bool deepClone = false) where T : class
        {
            XmlReaderSettings settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

            using (var reader = new XmlTextReader(new StringReader(context)))
            {
                if (setting != null)
                    reader.Setting = setting;
                return new Serializer(typeof(T)).Combine(reader, expected, !deepClone) as T;
            }
        }

        protected object DoSerializer_Combie(Type type, string context, object expected, ISerializeSetting setting, bool deepClone = false)
        {
            XmlReaderSettings settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

            using (var reader = new XmlTextReader(new StringReader(context)))
            {
                if (setting != null)
                    reader.Setting = setting;
                return new Serializer(type).Combine(reader, expected, !deepClone);
            }
        }

        protected object DoSerializer_Combie(Type type, string context, object expected, bool deepClone = false)
        {
            XmlReaderSettings settings = new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};

            using (var reader = new XmlTextReader(new StringReader(context)))
            {
                return new Serializer(type).Combine(reader, expected, !deepClone);
            }
        }

        protected T DoCombineIDictionary_Combie<T>(string context, T expected) where T : class
        {
            using (var reader = new XmlTextReader(new StringReader(context)))
            {
                return
                    new CombineIDictionary(new TypeExtend(DefaultXmlSerializeSetting, typeof(T), null)).Combine(reader, expected,
                        ReflectionUtils.GetTypeFriendlyName(typeof(T))) as T;
            }
        }

        protected T DoCombineIEnumerable_Combie<T>(string context, T expected) where T : class
        {
            return DoCombineIEnumerable_Combie(context, expected, null);
        }

        protected T DoCombineIEnumerable_Combie<T>(string context, T expected, ISerializeSetting setting)
            where T : class
        {
            using (var reader = new XmlTextReader(new StringReader(context)))
            {
                if (setting != null)
                    reader.Setting = setting;
                return
                    new CombineIEnumerable(new TypeExtend(DefaultXmlSerializeSetting, typeof(T), null)).Combine(reader, expected,
                        ReflectionUtils.GetTypeFriendlyName(typeof(T))) as T;
            }
        }

        protected T DoCombineKeyValuePair_Combie<T>(string context, T expected)
        {
            using (var reader = new XmlTextReader(new StringReader(context)))
            {
                return (T)
                    new CombineKeyValuePair(new TypeExtend(DefaultXmlSerializeSetting, typeof(T), null)).Combine(reader, expected,
                        ReflectionUtils.GetTypeFriendlyName(typeof(T)));
            }
        }

        /// <summary>
        ///     使用<see cref="Serializer" />产生增量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ori"></param>
        /// <param name="rev"></param>
        /// <returns>增量内容</returns>
        protected string DoSerializer_Divide<T>(T ori, T rev)
        {
            return DoSerializer_Divide(ori, rev, null);
        }

        protected string DoSerializer_Divide<T>(T ori, T rev,ISerializeSetting setting)
        {
            StringBuilder result = new StringBuilder();
            using (var writer = CreateWriter(result))
            {
                if (setting != null)
                {
                    writer.Setting = setting;
                }
                var serializer = new Serializer(typeof(T));
                serializer.Divide(writer, ori, rev);
            }
            return result.ToString();
        }

        /// <summary>
        ///     通过 <see cref="Serializer" /> ，首先在 <paramref name="pOriValue" /> 和 <paramref name="pRevValue" />间产生增量，比较增量与
        ///     <paramref name="context" /> 是否一致，再使用增量产生新的对象实例，并比较新的对象实例与 <paramref name="pOriValue" /> 是否值相等。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">待比较的增量字符串。</param>
        /// <param name="pOriValue">原始对象实例。</param>
        /// <param name="pRevValue">更新后的对象实例。</param>
        /// <param name="createNew">是否在合并增量时创建新实例。</param>
        /// <remarks>不指定对象类型，使用泛型 <typeparamref name="T" /> 对应的类型。</remarks>
        protected T DoAssert<T>(string context, T pOriValue, T pRevValue, bool createNew)
        {
            return DoAssert(typeof(T), context, pOriValue, pRevValue, createNew);
        }

        /// <summary>
        ///     通过 <see cref="Serializer" /> ，首先在 <paramref name="pOriValue" /> 和 <paramref name="pRevValue" />间产生增量，比较增量与
        ///     <paramref name="context" /> 是否一致，再使用增量产生新的对象实例，并比较新的对象实例与 <paramref name="pOriValue" /> 是否值相等。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pType">对象类型。</param>
        /// <param name="context">待比较的增量字符串。</param>
        /// <param name="pOriValue">原始对象实例。</param>
        /// <param name="pRevValue">更新后的对象实例。</param>
        /// <param name="createNew">是否在合并增量时创建新实例。</param>
        protected T DoAssert<T>(Type pType, string context, T pOriValue, T pRevValue, bool createNew)
        {
            return DoAssert(pType, context, pOriValue, pRevValue, createNew, null);
        }

        /// <summary>
        ///     通过 <see cref="Serializer" /> ，首先在 <paramref name="pOriValue" /> 和 <paramref name="pRevValue" />间产生增量，比较增量与
        ///     <paramref name="context" /> 是否一致，再使用增量产生新的对象实例，并比较新的对象实例与 <paramref name="pOriValue" /> 是否值相等。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pType">对象类型。</param>
        /// <param name="context">待比较的增量字符串。如果传入 <see cref="string.Empty" /> 表示不比较产生的差量内容。</param>
        /// <param name="pOriValue">原始对象实例。</param>
        /// <param name="pRevValue">更新后的对象实例。</param>
        /// <param name="createNew">是否在合并增量时创建新实例。</param>
        /// <param name="setting">序列化设定，可以为 <b>null</b></param>
        protected T DoAssert<T>(Type pType, string context, T pOriValue, T pRevValue, bool createNew,
            ISerializeSetting setting)
        {
            return DoAssert(pType, context, pOriValue, pRevValue, createNew, setting, null);
        }

        protected string GetCompleteContext(string context)
        {
            if (!context.StartsWith(XmlHeaderContext))
            {
                return string.Concat(XmlHeaderContext, System.Environment.NewLine, context);
            }
            return context;
        }

        /// <summary>
        ///     通过 <see cref="Serializer" /> ，首先在 <paramref name="pOriValue" /> 和 <paramref name="pRevValue" />间产生增量，比较增量与
        ///     <paramref name="context" /> 是否一致，再使用增量产生新的对象实例，并比较新的对象实例与 <paramref name="pOriValue" /> 是否值相等。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pType">对象类型。</param>
        /// <param name="context">待比较的增量字符串。如果传入 <see cref="string.Empty" /> 表示不比较产生的差量内容。</param>
        /// <param name="pOriValue">原始对象实例。</param>
        /// <param name="pRevValue">更新后的对象实例。</param>
        /// <param name="createNew">是否在合并增量时创建新实例。</param>
        /// <param name="setting">序列化设定，可以为 <b>null</b></param>
        /// <param name="pTypes">待注册的类型与主键的键值对字典，可以为 <b>null</b></param>
        protected T DoAssert<T>(Type pType, string context, T pOriValue, T pRevValue, bool createNew,
            ISerializeSetting setting, IDictionary<Type, string[]> pTypes)
        {
            bool contextIsEmpty = string.IsNullOrEmpty(context);
            context = GetCompleteContext(context);

            Serializer serializer = new Serializer(pType);
            if (pTypes != null && pTypes.Count > 0)
            {
                foreach (KeyValuePair<Type, string[]> pair in pTypes)
                {
                    serializer.RegisterType(pair.Key, pair.Value);
                }
            }
            StringBuilder sb = new StringBuilder();
            using (ITextWriter writer = CreateWriter(sb))
            {
                if (setting != null)
                    writer.Setting = setting;
                serializer.Divide(writer, pOriValue, pRevValue);
            }
            if (!contextIsEmpty)
                Assert.AreEqual(context, sb.ToString(), "输出内容与预期不符");
            if (string.Equals(XmlHeaderContext, sb.ToString()))
                return default(T);
            using (XmlTextReader reader = new XmlTextReader(new StringReader(sb.ToString())))
            {
                if (setting != null)
                    reader.Setting = setting;
                T result = (T)serializer.Combine(reader,
                pOriValue,
                !createNew);
                if (createNew)
                {
                    Assert.AreNotEqual(result,
                    pOriValue);
                    if (pOriValue != null && pRevValue != null)
                        Assert.AreNotEqual(result.GetHashCode(),
                        pOriValue.GetHashCode());
                }
                else
                {
                    //类型如果是数组，始终会创建新实例
                    if (!IsValueType(pType) && !pType.IsArray)
                    {
                        if (pOriValue != null)
                            //如果pOriValue为null，那么会始终创建一个新的实例，所以肯定不相同
                            Assert.AreEqual(result, pOriValue);
                        if (pOriValue != null && pRevValue != null)
                            Assert.AreEqual(result.GetHashCode(), pOriValue.GetHashCode());
                    }
                }
                if (XPatchLib.ReflectionUtils.IsIEnumerable(pType, pType.GetInterfaces()))
                {
                    AssertIEnumerable(pRevValue, result, pType, string.Empty);
                }
                else
                {
                    Assert.AreEqual(pRevValue, result);
                }
                if (!IsValueType(pType) && pRevValue != null)
                    Assert.AreNotEqual(result.GetHashCode(), pRevValue.GetHashCode());
                return result;
            }
        }

        private void AssertIEnumerable(object A, object B, Type pType, string pAssert)
        {
            if (A == null && B == null)
            {
            }
            else if (A != null && B != null)
            {
                var aList = A as IEnumerable;
                var bList = B as IEnumerable;
                var aEnumerator = aList.GetEnumerator();
                var bEnumerator = bList.GetEnumerator();

                Assert.IsTrue(IEnumeratorEquals(aEnumerator, bEnumerator));
                Assert.IsTrue(IEnumeratorEquals(bEnumerator, aEnumerator));
            }
            else
            {
                Assert.Fail();
            }
        }

        private bool IEnumeratorEquals(IEnumerator aEnumerator, IEnumerator bEnumerator)
        {
            aEnumerator.Reset();
            while (aEnumerator.MoveNext())
            {
                var found = false;
                bEnumerator.Reset();
                while (bEnumerator.MoveNext())
                    if (aEnumerator.Current.Equals(bEnumerator.Current))
                    {
                        found = true;
                        break;
                    }
                if (!found)
                    return false;
            }
            return true;
        }

        private bool IsValueType(Type type)
        {
            if (ReflectionUtils.IsArray(type))
            {
                Type t;
                if (ReflectionUtils.TryGetArrayElementType(type, out t))
                    return t.IsValueType() || t == typeof(string);
            }
            if (ReflectionUtils.IsIEnumerable(type, type.GetInterfaces()))
            {
                Type t;
                if (ReflectionUtils.TryGetIEnumerableGenericArgument(type, type.GetInterfaces(), out t))
                    return t.IsValueType() || t == typeof(string);
            }
            return type.IsValueType() || type == typeof(string);
        }

        public sealed class ExtentedStringWriter : StringWriter
        {
            private readonly Encoding stringWriterEncoding;
            public ExtentedStringWriter(StringBuilder builder, Encoding desiredEncoding)
                : base(builder)
            {
                this.stringWriterEncoding = desiredEncoding;
            }

            public override Encoding Encoding
            {
                get
                {
                    return this.stringWriterEncoding;
                }
            }
        }
    }
}