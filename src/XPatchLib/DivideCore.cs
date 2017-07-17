// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     增量内容产生入口类。
    /// </summary>
    /// <seealso cref="XPatchLib.DivideBase" />
    /// <remarks>
    ///     此类是增量内容产生的入口类，由此类区分待产生增量内容的对象类型，调用不同的增量内容产生类。
    /// </remarks>
    internal class DivideCore : DivideBase
    {
        /// <summary>
        ///     产生增量内容。
        /// </summary>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>
        ///     返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。
        /// </returns>
        public override bool Divide(string pName, object pOriObject, object pRevObject, DivideAttachment pAttach = null)
        {
            Guard.ArgumentNotNullOrEmpty(pName, "pName");

            //当前节点是被SetNull时，直接写入节点并增加SetNull Attribute，并返回写入成功。
            if (IsSetNull(pOriObject, pRevObject))
            {
                WriteParentElementStart(pAttach);
                Writer.WriteStartObject(pName);
                Writer.WriteActionAttribute(Action.SetNull);
                return true;
            }
            if (!TypeExtend.NeedSerialize(Type.DefaultValue, pOriObject, pRevObject, Writer.Setting.SerializeDefalutValue))
                return false;
            return DivideAction(pName, pOriObject, pRevObject, pAttach);
        }

        /// <summary>
        ///     产生增量内容的实际方法。
        /// </summary>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>
        ///     返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。
        /// </returns>
        protected override bool DivideAction(string pName, object pOriObject, object pRevObject,
            DivideAttachment pAttach = null)
        {
            IDivide divide;

            if (Type.IsBasicType)
                divide = new DivideBasic(Writer, Type);
            else if (Type.IsIDictionary)
                divide = new DivideIDictionary(Writer, Type);
            else if (Type.IsIEnumerable)
                divide = new DivideIEnumerable(Writer, Type);
            else if (Type.IsGenericType && Type.OriType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                divide = new DivideKeyValuePair(Writer, Type);
#if NET || NETSTANDARD_2_0_UP
            else if (Type.IsISerializable)
                divide=new DivideISerializable(Writer,Type);
#endif
            else
                divide = new DivideObject(Writer, Type);

            return divide.Divide(pName, pOriObject, pRevObject, pAttach);
        }

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideCore" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">写入器。</param>
        /// <param name="pType">指定的类型。</param>
        internal DivideCore(ITextWriter pWriter, TypeExtend pType)
            : base(pWriter, pType)
        {
        }

        #endregion Internal Constructors
    }
}