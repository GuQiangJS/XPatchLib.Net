// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     基础类型增量内容产生类。
    /// </summary>
    /// <seealso cref="XPatchLib.DivideBase" />
    internal class DivideBasic : DivideBase
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
            switch (Type.TypeCode)
            {
                case TypeCode.Boolean:
                    return DivideAction<Boolean>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.Char:
                    return DivideAction<Char>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.SByte:
                    return DivideAction<sbyte>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.Byte:
                    return DivideAction<byte>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.Int16:
                    return DivideAction<short>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.UInt16:
                    return DivideAction<ushort>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.Int32:
                    return DivideAction<int>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.UInt32:
                    return DivideAction<uint>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.Int64:
                    return DivideAction<long>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.UInt64:
                    return DivideAction<ulong>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.Single:
                    return DivideAction<float>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.Double:
                    return DivideAction<Double>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.Decimal:
                    return DivideAction<Decimal>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.DateTime:
                    return DivideAction<DateTime>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.String:
                    return DivideAction<String>(pName, pOriObject, pRevObject, pAttach);
            }
            if (Type.IsGuid)
                return DivideAction<Guid>(pName, pOriObject, pRevObject, pAttach);
            return false;
        }

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideBasic" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">XML写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <remarks>
        ///     默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。
        /// </remarks>
        internal DivideBasic(XmlWriter pWriter, TypeExtend pType)
            : base(pWriter, pType)
        {
        }

        /// <summary>
        ///     使用指定的类型及指定是否序列化默认值初始化 <see cref="XPatchLib.DivideBasic" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">XML写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <param name="pSerializeDefalutValue">指定是否序列化默认值。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <remarks>
        ///     默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。
        /// </remarks>
        internal DivideBasic(XmlWriter pWriter, TypeExtend pType, Boolean pSerializeDefalutValue)
            : base(pWriter, pType, pSerializeDefalutValue)
        {
        }

        /// <summary>
        ///     使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        ///     <see cref="XPatchLib.DivideBasic" /> 类的新实例。
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
        internal DivideBasic(XmlWriter pWriter, TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : base(pWriter, pType, pMode)
        {
        }

        /// <summary>
        ///     使用指定的类型、指定是否序列化默认值和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        ///     <see cref="XPatchLib.DivideBasic" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">XML写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <param name="pMode">
        ///     指定在字符串与 System.DateTime 之间转换时，如何处理时间值。
        ///     <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <param name="pSerializeDefalutValue">指定是否序列化默认值。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        internal DivideBasic(XmlWriter pWriter, TypeExtend pType, XmlDateTimeSerializationMode pMode,
            Boolean pSerializeDefalutValue)
            : base(pWriter, pType, pMode, pSerializeDefalutValue)
        {
        }

        #endregion Internal Constructors

        #region Private Methods

        /// <summary>
        ///     产生基础类型增量内容核心方法。
        /// </summary>
        /// <param name="pElementName">增量内容对象的名称。</param>
        /// <param name="pElementValue">更新后的对象数据。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <param name="pAction">更新操作类型，默认为Edit。</param>
        /// <returns>
        ///     返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。
        /// </returns>
        private Boolean DivideAction(string pElementName, string pElementValue, DivideAttachment pAttach,
            Action pAction = Action.Edit)
        {
            if (string.IsNullOrEmpty(pElementName))
                return false;
            WriteParentElementStart(pAttach);
            Writer.WriteStartElement(pElementName);
            if (pAttach != null && pAttach.CurrentAction != Action.Edit)
            {
                Writer.WriteActionAttribute(pAttach.CurrentAction);
#if DEBUG
                Debug.WriteLine("Write DivideAttachment ActionAttribute:{0}.", pAttach.CurrentAction);
#endif
            }
            else
            {
                Writer.WriteActionAttribute(pAction);
            }
#if DEBUG
            Debug.WriteLine(string.Format("WriteStartElement:{0}.", pElementName));
            Debug.WriteLine("WriteActionAttribute:{0}.", pAction);
#endif
            if (pAction != Action.SetNull)
            {
                Writer.WriteString(pElementValue);
#if DEBUG
                Debug.WriteLine(string.Format("WriteString:{0}.", pElementValue));
#endif
            }
            return true;
        }

        /// <summary>
        ///     产生基础类型增量内容核心方法。
        /// </summary>
        /// <typeparam name="T">待产生增量内容的对象类型定义。</typeparam>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>
        ///     返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。
        /// </returns>
        private Boolean DivideAction<T>(string pName, Object pOriObject, Object pRevObject, DivideAttachment pAttach)
        {
            if (!TypeExtend.Equals(pOriObject, pRevObject))
            {
                //只有当原始对象与变更后的对象不相等时才需要产生增量内容。
                if (pRevObject == null)
                    return DivideAction(pName, string.Empty, pAttach, Action.SetNull);
                if (pOriObject == null && TypeExtend.Equals(Type.DefaultValue, pRevObject))
                {
                    //原始对象为null，且更新后的对象为类型初始值时，需要判断_SerializeDefaultValue属性来确定是否序列化初始值。
                    if (SerializeDefaultValue)
                        return DivideAction(pName, TransToString<T>(pRevObject), pAttach);
                }
                else
                {
                    return DivideAction(pName, TransToString<T>(pRevObject), pAttach);
                }
            }
            return false;
        }

        /// <summary>
        ///     将待序列化的基础类型对象实例转换为可序列化的字符串。
        /// </summary>
        /// <typeparam name="T">待序列化的基础类型。</typeparam>
        /// <param name="pObj">待序列化的基础类型对象的值。</param>
        /// <returns>
        ///     转换成功返回转换后的字符串，否则返回 <see cref="String.Empty" /> 。
        /// </returns>
        private string TransToString<T>(Object pObj)
        {
            switch (System.Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                    return XmlConvert.ToString((Boolean) pObj);

                case TypeCode.Byte:
                    return XmlConvert.ToString((Byte) pObj);

                case TypeCode.Decimal:
                    return XmlConvert.ToString((Decimal) pObj);

                case TypeCode.Double:
                    return XmlConvert.ToString((Double) pObj);

                case TypeCode.Int16:
                    return XmlConvert.ToString((Int16) pObj);

                case TypeCode.Int32:
                    return XmlConvert.ToString((Int32) pObj);

                case TypeCode.Int64:
                    return XmlConvert.ToString((Int64) pObj);

                case TypeCode.SByte:
                    return XmlConvert.ToString((SByte) pObj);

                case TypeCode.Single:
                    return XmlConvert.ToString((Single) pObj);

                case TypeCode.UInt16:
                    return XmlConvert.ToString((UInt16) pObj);

                case TypeCode.UInt32:
                    return XmlConvert.ToString((UInt32) pObj);

                case TypeCode.UInt64:
                    return XmlConvert.ToString((UInt64) pObj);

                case TypeCode.DateTime:
                    return XmlConvert.ToString((DateTime) pObj, Mode);

                case TypeCode.String:
                    return (string) pObj;

                case TypeCode.Char:
                    return XmlConvert.ToString((ushort) (char) pObj);
            }

            if (typeof(T) == typeof(Guid))
                return XmlConvert.ToString((Guid) pObj);

            //TODO:异常可能
            return string.Empty;
        }

        #endregion Private Methods
    }
}