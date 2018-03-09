// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace XPatchLib
{
    internal class ConverterCore : ConverterBase
    {
        internal ConverterCore(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        internal ConverterCore(TypeExtend pType) : base(pType)
        {
        }

        /// <summary>
        ///     产生增量内容。
        /// </summary>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。</returns>
        public override bool Divide(string pName, object pOriObject, object pRevObject, DivideAttachment pAttach = null)
        {
            Guard.ArgumentNotNullOrEmpty(pName, "pName");

            //当前节点是被SetNull时，直接写入节点并增加SetNull Attribute，并返回写入成功。
            if (IsSetNull(pOriObject, pRevObject))
            {
                WriteParentElementStart(pAttach);
                Writer.WriteStartObject(pName);
                if (Type.OriType.IsInterface())
                    WriteAssemby(GetType(pOriObject, pRevObject));
                Writer.WriteActionAttribute(Action.SetNull);
                return true;
            }

            if (!TypeExtend.NeedSerialize(Type.DefaultValue, pOriObject, pRevObject,
                Writer.Setting.SerializeDefalutValue))
                return false;
            return DivideAction(pName, pOriObject, pRevObject, pAttach);
        }

        protected override bool DivideAction(string pName, object pOriObject, object pRevObject,
            DivideAttachment pAttach = null)
        {
            IDivide divide;

            if (!OtherConverterContainer.TryGetDivide(Type, Writer, out divide))
                if (Type.IsBasicType)
                    divide = new ConverterBasic(Writer, Type);
                else if (Type.IsIDictionary)
                    divide = new ConverterIDictionary(Writer, Type);
                else if (Type.IsIEnumerable)
                    divide = new ConverterIEnumerable(Writer, Type);
                else if (Type.IsGenericType && Type.OriType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                    divide = new ConverterKeyValuePair(Writer, Type);
#if NET || NETSTANDARD_2_0_UP
                else if (Type.IsISerializable)
                    divide = new ConverterISerializable(Writer, Type);
#endif
#if NET_40_UP || NETSTANDARD_2_0_UP
                else if (Type.IsDynamicObject)
                    divide = new ConverterDynamic(Writer, Type);
#endif
                else
                    divide = new ConverterObject(Writer, Type);

            divide.Assign(this);
            return divide.Divide(pName, pOriObject, pRevObject, pAttach);
        }

        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            if (pOriObject == null
#if NET || NETSTANDARD_2_0_UP
                && Type.IsISerializable
#endif
            )
                pOriObject = Type.CreateInstance();

            return CombineInstanceContainer.GetCombineInstance(Type).Combine(pReader, pOriObject, pName);
        }
    }
}