#if NET || NETSTANDARD_2_0_UP

#if !NET_20
using System.Linq;
#endif
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace XPatchLib
{
    internal class CombineISerializable : CombineBase
    {
        internal static readonly StreamingContext DefaultContext;

        static CombineISerializable()
        {
            DefaultContext = new StreamingContext();
        }

        public CombineISerializable(TypeExtend pType) : base(pType)
        {
        }

        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            SerializationInfo serializationInfo = new SerializationInfo(Type.OriType, new XPatchLib.FormatterConverter(pReader.Setting));

            CombineCore combineCore = new CombineCore(Type);

            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;

                //pReader.MoveToElement();

                if (string.Equals(pName, pReader.Name))
                {
                    pReader.Read();
                    continue;
                }

                if (pReader.NodeType == NodeType.Element)
                {
                    string proName = pReader.Name;
                    object newValue =
                        new CombineCore(TypeExtendContainer.GetTypeExtend(pReader.Setting, typeof(string), null, null))
                            .Combine(pReader, null, proName);
                    serializationInfo.AddValue(proName, newValue);
                }
                pReader.Read();
            }

            return Type.CreateInstance(serializationInfo, DefaultContext);
        }

    }
}
#endif