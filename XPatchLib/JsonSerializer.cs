// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    //TODO:Not Complete
    internal class JsonSerializer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private readonly DateTimeSerializationMode _mode = DateTimeSerializationMode.RoundtripKind;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private readonly bool _serializeDefalutValue;
        private readonly TypeExtend _type;

        public JsonSerializer(Type pType, bool pSerializeDefalutValue)
        {
            TypeExtendContainer.Clear();
            _type = TypeExtendContainer.GetTypeExtend(pType, null);
            _serializeDefalutValue = pSerializeDefalutValue;
        }
        public void Divide(Stream pStream, object pOriValue, object pRevValue)
        {
            StreamWriter streamWriter = new StreamWriter(pStream, Encoding.UTF8);
            Divide(streamWriter, pOriValue, pRevValue);
        }

        public void Divide(TextWriter pWriter, object pOriValue, object pRevValue)
        {
            Guard.ArgumentNotNull(pWriter, "pWriter");

            Divide(new JsonTextWriter(pWriter), pOriValue, pRevValue);
        }

        private void Divide(ITextWriter pWriter, object pOriValue, object pRevValue)
        {
            Guard.ArgumentNotNull(pWriter, "pWriter");

            pWriter.WriteStartDocument();
            if (new DivideCore(pWriter, _type).Divide(_type.TypeFriendlyName,
                pOriValue, pRevValue))
                pWriter.WriteEndDocument();
            pWriter.Flush();
        }

        public object Combine(Stream pStream, object pOriValue)
        {
            return Combine(pStream, pOriValue, false);
        }
        public object Combine(Stream pStream, object pOriValue, bool pOverride)
        {
            var xmlReader = XmlReader.Create(pStream);
            //xmlReader.WhitespaceHandling = WhitespaceHandling.Significant;
            //xmlReader.Normalization = true;
            //xmlReader.XmlResolver = null;
            return Combine(xmlReader, pOriValue, pOverride);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:不要多次释放对象")]
        public object Combine(XmlReader pReader, object pOriValue, bool pOverride)
        {
            Guard.ArgumentNotNull(pReader, "pReader");

            object cloneObjValue = null;
            //当原始值不为Null时，需要先对原始值进行克隆，否则做数据合并时会侵入到原始数据
            if (pOriValue != null)
                if (pOverride)
                {
                    cloneObjValue = pOriValue;
                }
                else
                {
                    MemoryStream stream = null;
                    try
                    {
                        stream = new MemoryStream();
                        var settings = new XmlWriterSettings();
                        settings.ConformanceLevel = ConformanceLevel.Fragment;
                        settings.Indent = true;
                        settings.Encoding = Encoding.UTF8;
                        settings.OmitXmlDeclaration = false;
                        using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
                        {
                            ITextWriter writer = new JsonTextWriter(streamWriter);
                            new DivideCore(writer, _type).Divide(_type.TypeFriendlyName, null, pOriValue);
                            stream.Position = 0;
                            using (JsonTextReader reader = new JsonTextReader(stream))
                            {
                                cloneObjValue = new CombineCore(_type).Combine(reader, null, _type.TypeFriendlyName);
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Dispose();
                    }
                }
            else
                cloneObjValue = _type.CreateInstance();

            Stream inputStream = null;
            try
            {
                string s = pReader.ReadOuterXml();
                inputStream = GenerateStreamFromString(s);
                return new CombineCore(_type).Combine(new JsonTextReader(inputStream), cloneObjValue,
                    _type.TypeFriendlyName);
            }
            finally
            {
                if (inputStream != null)
                    inputStream.Dispose();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}