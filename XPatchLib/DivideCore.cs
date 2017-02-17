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
                divide = new DivideBasic(Writer, Type, Mode, SerializeDefaultValue);
            else if (Type.IsIDictionary)
                divide = new DivideIDictionary(Writer, Type, Mode, SerializeDefaultValue);
            else if (Type.IsIEnumerable)
                divide = new DivideIEnumerable(Writer, Type, Mode, SerializeDefaultValue);
            else if (Type.IsGenericType && Type.OriType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                divide = new DivideKeyValuePair(Writer, Type, Mode, SerializeDefaultValue);
            else
                divide = new DivideObject(Writer, Type, Mode, SerializeDefaultValue);

            if (divide.Divide(pName, pOriObject, pRevObject, pAttach))
                return true;
            return false;
        }

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideCore" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">XML写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <remarks>
        ///     默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。
        /// </remarks>
        internal DivideCore(XmlWriter pWriter, TypeExtend pType)
            : base(pWriter, pType)
        {
        }

        /// <summary>
        ///     使用指定的类型及指定是否序列化默认值初始化 <see cref="XPatchLib.DivideCore" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">XML写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <param name="pSerializeDefalutValue">指定是否序列化默认值。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <remarks>
        ///     默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。
        /// </remarks>
        internal DivideCore(XmlWriter pWriter, TypeExtend pType, Boolean pSerializeDefalutValue)
            : base(pWriter, pType, pSerializeDefalutValue)
        {
        }

        /// <summary>
        ///     使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        ///     <see cref="XPatchLib.DivideCore" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">XML写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <param name="pMode">
        ///     指定在字符串与 System.DateTime 之间转换时，如何处理时间值。
        ///     <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <remarks>
        ///     默认不序列化默认值。
        /// </remarks>
        internal DivideCore(XmlWriter pWriter, TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : base(pWriter, pType, pMode)
        {
        }

        /// <summary>
        ///     使用指定的类型、指定是否序列化默认值和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        ///     <see cref="XPatchLib.DivideCore" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">XML写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <param name="pMode">
        ///     指定在字符串与 System.DateTime 之间转换时，如何处理时间值。
        ///     <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <param name="pSerializeDefalutValue">指定是否序列化默认值。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        internal DivideCore(XmlWriter pWriter, TypeExtend pType, XmlDateTimeSerializationMode pMode,
            Boolean pSerializeDefalutValue)
            : base(pWriter, pType, pMode, pSerializeDefalutValue)
        {
        }

        #endregion Internal Constructors
    }
}