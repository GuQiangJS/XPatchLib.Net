// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace XPatchLib
{
    public class JsonSerializer
    {
        private readonly bool _serializeDefalutValue;
        private readonly TypeExtend _type;
        private readonly Encoding _encoding;

        public JsonSerializer(Type pType)
            : this(pType, Encoding.UTF8)
        {
        }

        public JsonSerializer(Type pType, Encoding pEncoding)
            : this(pType, pEncoding, false)
        {
        }

        public JsonSerializer(Type pType, Encoding pEncoding, bool pSerializeDefalutValue)
        {
            TypeExtendContainer.Clear();
            _type = TypeExtendContainer.GetTypeExtend(pType, null);
            _serializeDefalutValue = pSerializeDefalutValue;
            _encoding = pEncoding;
        }

        public void Divide(Stream pStream, object pOriValue, object pRevValue)
        {
            Guard.ArgumentNotNull(pStream, "pStream");

            JsonWriter writer = new JsonWriter(pStream, _encoding);

            writer.WriteStartDocument();
            if (new DivideCore(writer, _type, _serializeDefalutValue).Divide(_type.TypeFriendlyName,
                pOriValue, pRevValue))
                writer.WriteEndDocument();
            writer.Flush();
        }
    }

    public class JsonWriter : XmlTextWriter
    {
        public JsonWriter(Stream w, Encoding encoding) : base(w, encoding)
        {
        }

        public JsonWriter(string filename, Encoding encoding) : base(filename, encoding)
        {
        }

        public JsonWriter(TextWriter w) : base(w)
        {
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            WriteStartObject();
            WritePropertyName(localName);
        }

        public override void WriteEndElement()
        {
#if DEBUG
            Debug.Write("}");
#endif
            WriteChars(new char[] { '}' }, 0, 1);
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            WritePropertyName(localName);
            WritePropertyValueStart();
        }

        public override void WriteEndAttribute()
        {
            WritePropertyValueEnd();
        }

        public override void WriteStartDocument()
        {
        }

        public override void WriteEndDocument()
        {
        }
        
        private void WriteStartObject()
        {
#if DEBUG
            Debug.Write("{");
#endif
            WriteChars(new char[] { '{' }, 0, 1);
        }

        private void WriteStartArray()
        {
#if DEBUG
            Debug.Write("[");
#endif
            WriteChars(new char[] { '[' }, 0, 1);
        }

        private void WritePropertyValueStart()
        {
#if DEBUG
            Debug.Write("\"");
#endif
            WriteChars(new char[] { '"' }, 0, 1);
        }

        private void WritePropertyValueEnd()
        {
#if DEBUG
            Debug.Write("\"");
#endif
            WriteChars(new char[] { '"' }, 0, 1);
        }

        public override void WriteString(string text)
        {
            if (!text.StartsWith("\""))
            {
                WriteChars(new char[] { '"' }, 0, 1);
            }
            base.WriteString(text);
            if (!text.EndsWith("\""))
            {
                WriteChars(new char[] {'"'}, 0, 1);
            }
        }

        private void WritePropertyName(string name)
        {
            string pro = string.Format("\"{0}\"", name);
#if DEBUG
            Debug.Write(pro);
            Debug.Write(":");
#endif
            WriteValue(pro);

            WriteChars(new char[] { ':' }, 0, 1);
        }
    }
}