// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Xml;
#if NETSTANDARD_1_0 || NETSTANDARD_1_1
using XmlConvert=XPatchLib.XmlConvert;
#else
using XmlConvert=System.Xml.XmlConvert;
#endif

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

                case TypeCode.TimeSpan:
                    return DivideAction<TimeSpan>(pName, pOriObject, pRevObject, pAttach);

                case TypeCode.DateTimeOffset:
                    return DivideAction<DateTimeOffset>(pName, pOriObject, pRevObject, pAttach);

#if NET_40_UP || NETSTANDARD_1_1_UP
                case TypeCode.BigInteger:
                    return DivideAction<System.Numerics.BigInteger>(pName, pOriObject, pRevObject, pAttach);
#endif

                case TypeCode.Uri:
                    return DivideAction<Uri>(pName, pOriObject, pRevObject, pAttach);
            }
            if (Type.IsGuid)
                return DivideAction<Guid>(pName, pOriObject, pRevObject, pAttach);
            return false;
        }

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideBasic" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">写入器。</param>
        /// <param name="pType">指定的类型。</param>
        internal DivideBasic(ITextWriter pWriter, TypeExtend pType)
            : base(pWriter, pType)
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
            WriteStart(Type, null, pElementName);
            if (pAttach != null && pAttach.CurrentAction != Action.Edit)
                Writer.WriteActionAttribute(pAttach.CurrentAction);
            else
                Writer.WriteActionAttribute(pAction);
            if (pAction != Action.SetNull)
                Writer.WriteValue(pElementValue);
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
            if (TypeExtend.NeedSerialize(Type.DefaultValue, pOriObject, pRevObject, Writer.Setting.SerializeDefalutValue))
            {
                //只有当原始对象与变更后的对象不相等时才需要产生增量内容。
                if (pRevObject == null)
                    return DivideAction(pName, string.Empty, pAttach, Action.SetNull);
                if (pOriObject == null && Equals(Type.DefaultValue, pRevObject))
                {
                    //原始对象为null，且更新后的对象为类型初始值时，需要判断_SerializeDefaultValue属性来确定是否序列化初始值。
                    if (Writer.Setting.SerializeDefalutValue)
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
            switch (Type.TypeCode)
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
                    return XmlConvert.ToString((DateTime) pObj, Writer.Setting.Mode.Convert());

                case TypeCode.String:
                    return (pObj == null) ? string.Empty : (string) pObj;

                case TypeCode.Char:
                    return XmlConvert.ToString((ushort) (char) pObj);

                case TypeCode.TimeSpan:
                    return XmlConvert.ToString((TimeSpan)pObj);

                case TypeCode.DateTimeOffset:
                    return XmlConvert.ToString((DateTimeOffset) pObj);

#if NET_40_UP || NETSTANDARD_1_1_UP
                case TypeCode.BigInteger:
                    return ((System.Numerics.BigInteger)pObj).ToString(CultureInfo.InvariantCulture);
#endif
                case TypeCode.Uri:
                    return ((Uri) pObj).ToString();
            }

            if (typeof(T) == typeof(Guid))
                return XmlConvert.ToString((Guid) pObj);

            //TODO:异常可能
            return string.Empty;
        }

#endregion Private Methods
    }
}