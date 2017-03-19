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

namespace XPatchLib.UnitTest.ForXml
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

        internal static ITextWriter CreateWriter(Stream output)
        {
            return CreateWriter(output, FlagmentSetting);
        }

        internal static ITextWriter CreateWriter(Stream output, XmlWriterSettings settings)
        {
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(output, settings.Encoding);
            writer.Formatting = System.Xml.Formatting.Indented;
            return new XmlTextWriter(writer);
        }

        #region Internal Methods

        internal static void PrivateAssert(Type pType, object pOriObj, object pChangedObj, string pChangedContext,
            string pAssert)
        {
            PrivateAssert(pType, pOriObj, pChangedObj, pChangedContext, pAssert,
                DateTimeSerializationMode.RoundtripKind);
        }

        internal static void PrivateAssert(Type pType, object pOriObj, object pChangedObj, string pChangedContext,
            string pAssert, DateTimeSerializationMode pMode)
        {
            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = CreateWriter(stream))
                {
                    writer.Setting.Mode = pMode;
                    Assert.IsTrue(
                        new DivideCore(writer, new TypeExtend(pType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(pType), pOriObj, pChangedObj));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(stream);
                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        reader.Setting.Mode = pMode;
                        var combinedObj = new CombineCore(new TypeExtend(pType)).Combine(reader, pOriObj,
                            ReflectionUtils.GetTypeFriendlyName(pType));

                        Trace.Write(pChangedContext);

                        Assert.AreEqual(pChangedContext, changedEle.ToString(), pAssert);

                        UnitTest.TestHelper.PrivateAssertObject(pChangedObj, combinedObj, pAssert);
                    }
                }
            }
        }

        internal static void PrivateAssertIEnumerable<T>(Type pType, object pOriObj, object pChangedObj,
            string pChangedContext, string pAssert)
        {
            PrivateAssertIEnumerable<T>(pType, pOriObj, pChangedObj, pChangedContext, pAssert,
                DateTimeSerializationMode.RoundtripKind);
        }

        internal static void PrivateAssertIEnumerable<T>(Type pType, object pOriObj, object pChangedObj,
            string pChangedContext, string pAssert, DateTimeSerializationMode pMode)
        {
            using (var stream = new MemoryStream())
            {
                using (ITextWriter writer = CreateWriter(stream))
                {
                    writer.Setting.Mode = pMode;
                    Assert.IsTrue(
                        new DivideCore(writer, new TypeExtend(pType)).Divide(
                            ReflectionUtils.GetTypeFriendlyName(pType), pOriObj, pChangedObj));
                }
                stream.Position = 0;
                var changedEle = XElement.Load(stream);

                stream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
                        reader.Setting.Mode = pMode;
                        var combinedObj = new CombineCore(new TypeExtend(pType)).Combine(reader, pOriObj,
                            ReflectionUtils.GetTypeFriendlyName(pType));

                        Trace.Write(pChangedContext);

                        Assert.AreEqual(pChangedContext, changedEle.ToString(), pAssert);

                        UnitTest.TestHelper.PrivateAssertIEnumerable<T>(pChangedObj, combinedObj, pType, pAssert);
                    }
                }
            }
        }

        #endregion Internal Methods
    }
}