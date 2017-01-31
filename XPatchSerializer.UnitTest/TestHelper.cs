// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XPatchLib.UnitTest
{
    internal static class TestHelper
    {
        internal const string XmlHeaderContext = @"<?xml version=""1.0"" encoding=""utf-8""?>";

        internal static XmlWriterSettings FlagmentSetting
        {
            get
            {
                var settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                settings.OmitXmlDeclaration = false;
                return settings;
            }
        }

        #region Internal Methods

        internal static void PrivateAssert(Type pType, object pOriObj, object pChangedObj, string pChangedContext,
            string pAssert)
        {
            PrivateAssert(pType, pOriObj, pChangedObj, pChangedContext, pAssert,
                XmlDateTimeSerializationMode.RoundtripKind);
        }

        internal static void PrivateAssert(Type pType, object pOriObj, object pChangedObj, string pChangedContext,
            string pAssert, XmlDateTimeSerializationMode pMode)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, FlagmentSetting))
                {
                    Assert.IsTrue(
                        new DivideCore(writer, new TypeExtend(pType), pMode).Divide(
                            ReflectionUtils.GetTypeFriendlyName(pType), pOriObj, pChangedObj));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(stream);
                stream.Position = 0;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    var combinedObj = new CombineCore(new TypeExtend(pType), pMode).Combine(reader,pOriObj, ReflectionUtils.GetTypeFriendlyName(pType));

                    Trace.Write(pChangedContext);

                    Assert.AreEqual(pChangedContext, changedEle.ToString(), pAssert);

                    PrivateAssertObject(pChangedObj, combinedObj, pAssert);
                }
            }
        }

        internal static void PrivateAssertIEnumerable<T>(Type pType, object pOriObj, object pChangedObj,
            string pChangedContext, string pAssert)
        {
            PrivateAssertIEnumerable<T>(pType, pOriObj, pChangedObj, pChangedContext, pAssert,
                XmlDateTimeSerializationMode.RoundtripKind);
        }

        internal static void PrivateAssertIEnumerable<T>(Type pType, object pOriObj, object pChangedObj,
            string pChangedContext, string pAssert, XmlDateTimeSerializationMode pMode)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, FlagmentSetting))
                {
                    Assert.IsTrue(
                        new DivideCore(writer, new TypeExtend(pType), pMode).Divide(
                            ReflectionUtils.GetTypeFriendlyName(pType), pOriObj, pChangedObj));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(stream);

                stream.Position = 0;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    var combinedObj = new CombineCore(new TypeExtend(pType), pMode).Combine(reader, pOriObj,
                        ReflectionUtils.GetTypeFriendlyName(pType));

                    Trace.Write(pChangedContext);

                    Assert.AreEqual(pChangedContext, changedEle.ToString(), pAssert);

                    PrivateAssertIEnumerable<T>(pChangedObj, combinedObj, pType, pAssert);
                }
            }
        }

        internal static void PrivateAssertIEnumerable<T>(object A, object B, Type pType, string pAssert)
        {
            var aList = A as IEnumerable;
            var bList = B as IEnumerable;
            var aEnumerator = aList.GetEnumerator();
            var bEnumerator = bList.GetEnumerator();

            Assert.IsTrue(aEnumerator.IEnumeratorEquals(bEnumerator));
            Assert.IsTrue(bEnumerator.IEnumeratorEquals(aEnumerator));
        }

        internal static void PrivateAssertObject(object A, object B, string pAssert)
        {
            Assert.AreEqual(A, B, pAssert);
        }

        internal static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (var stremReader = new StreamReader(stream, Encoding.UTF8))
            {
                return stremReader.ReadToEnd();
            }
        }

        internal static Stream StringToStream(string str)
        {
            var strBytes = Encoding.UTF8.GetBytes(str);
            return new MemoryStream(strBytes);
        }

        private static bool IEnumeratorEquals(this IEnumerator aEnumerator, IEnumerator bEnumerator)
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

        #endregion Internal Methods
    }
}