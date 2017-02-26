// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text;
using System.Xml;

namespace XPatchLib
{
    //TODO:Not Complete
    internal class JsonSerializer
    {
        private readonly XmlDateTimeSerializationMode _mode = XmlDateTimeSerializationMode.RoundtripKind;
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
            if (new DivideCore(pWriter, _type, _mode, _serializeDefalutValue).Divide(_type.TypeFriendlyName,
                pOriValue, pRevValue))
                pWriter.WriteEndDocument();
            pWriter.Flush();
        }
    }
}