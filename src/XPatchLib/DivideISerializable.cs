// Copyright © 2013-2018 - GuQiang55
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if NET || NETSTANDARD_2_0_UP

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XPatchLib
{
    internal class DivideISerializable : DivideBase
    {
        internal static readonly StreamingContext DefaultContext;

        static DivideISerializable()
        {
            DefaultContext = new StreamingContext();
        }

        public DivideISerializable(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        protected override bool DivideAction(string pName, object pOriObject, object pRevObject,
            DivideAttachment pAttach = null)
        {
            Boolean result;

            SerializationInfo oriSerializationInfo = null;
            SerializationInfo revSerializationInfo = null;
            ISerializable oriSerializable = pOriObject as ISerializable;
            ISerializable revSerializable = pRevObject as ISerializable;
            if (oriSerializable != null)
            {
                oriSerializationInfo = new SerializationInfo(Type.OriType, new FormatterConverter(Writer.Setting));
                oriSerializable.GetObjectData(oriSerializationInfo, DefaultContext);
            }
            if (revSerializable != null)
            {
                revSerializationInfo = new SerializationInfo(Type.OriType, new FormatterConverter(Writer.Setting));
                revSerializable.GetObjectData(revSerializationInfo, DefaultContext);
            }

            if (pAttach == null)
                pAttach = new DivideAttachment();
            //将当前节点加入附件中，如果遇到子节点被写入前，会首先根据队列先进先出写入附件中的节点的开始标记
            pAttach.ParentQuere.Enqueue(new ParentObject(pName, pOriObject, Type));

            result = DivideItems(oriSerializationInfo, revSerializationInfo, pAttach);

            if (result)
            {
                //Writer.WriteEndObject();
            }
            else if (pOriObject == null && pRevObject != null)
            {
                result = WriteParentElementStart(pAttach);
                if (!result)
                    pAttach.ParentQuere.Dequeue();
                return result;
            }

            if (pAttach != null) pAttach.CurrentAction = Action.Edit;

            return result;
        }

        private bool DivideItems(SerializationInfo oriSerializationInfo, SerializationInfo revSerializationInfo,
            DivideAttachment pAttach = null)
        {
            Queue<string> s = new Queue<string>();
            bool result = false;
            if (revSerializationInfo != null)
                foreach (SerializationEntry revPro in revSerializationInfo)
                {
                    TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(Writer.Setting, revPro.ObjectType,
                        Writer.Setting.IgnoreAttributeType, null);
                    DivideCore ser = new DivideCore(Writer, typeExtend);
                    ser.Assign(this);

                    s.Enqueue(revPro.Name);

                    SerializationEntry oriEntry;
                    Boolean b = ser.Divide(revPro.Name,
                        TryGetSerializationEntry(oriSerializationInfo, revPro, out oriEntry) ? oriEntry.Value : null,
                        revPro.Value, pAttach);
                    if (!result)
                        result = b;
                }
            return result;
        }

        private bool TryGetSerializationEntry(SerializationInfo oriSerializationInfo, SerializationEntry revPro,
            out SerializationEntry oriPro)
        {
            oriPro = default(SerializationEntry);
            if (oriSerializationInfo == null)
                return false;
            foreach (SerializationEntry entry in oriSerializationInfo)
                if (string.Equals(revPro.Name, entry.Name))
                {
                    oriPro = entry;
                    return true;
                }
            return false;
        }
    }
}
#endif