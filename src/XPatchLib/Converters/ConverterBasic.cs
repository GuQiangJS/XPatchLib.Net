// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
#if NETSTANDARD_1_0 || NETSTANDARD_1_1
using XmlConvert = XPatchLib.XmlConvert;
#else
using XmlConvert = System.Xml.XmlConvert;

#endif


namespace XPatchLib
{
    /// <summary>
    ///     基础类型增量内容转换类。
    /// </summary>
    /// <seealso cref="XPatchLib.DivideBase" />
    internal class ConverterBasic : ConverterBase
    {
        internal ConverterBasic(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        internal ConverterBasic(TypeExtend pType) : base(pType)
        {
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

        /// <summary>
        ///     合并数据。
        /// </summary>
        /// <param name="pTypeCode">合并后的数据类型。</param>
        /// <param name="pIsGuid">是否为<see cref="Guid" />类型。</param>
        /// <param name="pMode">指定在字符串与 <see cref="T:System.DateTime" /> 之间转换时，如何处理时间值。</param>
        /// <param name="pValue">待合并的数据值。</param>
        /// <returns>返回合并后的数据。</returns>
        internal static Object CombineAction(TypeCode pTypeCode, bool pIsGuid, DateTimeSerializationMode pMode,
            string pValue)
        {
            switch (pTypeCode)
            {
                case TypeCode.Boolean:
                    return CombineBoolean(pValue);

                case TypeCode.Char:
                    return CombineChar(pValue);

                case TypeCode.SByte:
                    return CombineByte(pValue);

                case TypeCode.Byte:
                    return CombineUnsignedByte(pValue);

                case TypeCode.Int16:
                    return CombineShort(pValue);

                case TypeCode.UInt16:
                    return CombineUnsignedShort(pValue);

                case TypeCode.Int32:
                    return CombineInt32(pValue);

                case TypeCode.UInt32:
                    return CombineUnsignedInt32(pValue);

                case TypeCode.Int64:
                    return CombineLong(pValue);

                case TypeCode.UInt64:
                    return CombineUnsignedLong(pValue);

                case TypeCode.Single:
                    return CombineFloat(pValue);

                case TypeCode.Double:
                    return CombineDouble(pValue);

                case TypeCode.Decimal:
                    return CombineDecimal(pValue);

                case TypeCode.DateTime:
                    return CombineDateTime(pValue, pMode);

                case TypeCode.String:
                    return CombineString(pValue);

                case TypeCode.TimeSpan:
                    return CombineTimeSpan(pValue);

                case TypeCode.DateTimeOffset:
                    return CombineDateTimeOffset(pValue);

#if NET_40_UP || NETSTANDARD_1_1_UP
                case TypeCode.BigInteger:
                    return CombineBigInteger(pValue);
#endif

                case TypeCode.Uri:
                    return CombineUri(pValue);
            }

            if (pIsGuid)
                return CombineGuid(pValue);
            return null;
        }

        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            return CombineAction(Type.TypeCode, Type.IsGuid, pReader.Setting.Mode, pReader.GetValue());
        }

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
            if (TypeExtend.NeedSerialize(Type.DefaultValue, pOriObject, pRevObject,
                Writer.Setting.SerializeDefalutValue))
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
                    return pObj == null ? string.Empty : (string) pObj;

                case TypeCode.Char:
                    return XmlConvert.ToString((ushort) (char) pObj);

                case TypeCode.TimeSpan:
                    return ((TimeSpan) pObj).ToString();

                case TypeCode.DateTimeOffset:
                    return XmlConvert.ToString((DateTimeOffset) pObj);

#if NET_40_UP || NETSTANDARD_1_1_UP
                case TypeCode.BigInteger:
                    return ((System.Numerics.BigInteger) pObj).ToString(CultureInfo.InvariantCulture);
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

        #region Private Methods

        private static object CombineBoolean(string pValue)
        {
            return bool.Parse(pValue);
            //return XmlConvert.ToBoolean(pValue);
        }

        private static object CombineByte(string pValue)
        {
            return sbyte.Parse(pValue, NumberFormatInfo.InvariantInfo);
            //return XmlConvert.ToSByte(pValue);
        }

        private static object CombineChar(string pValue)
        {
            return (char) ushort.Parse(pValue, NumberFormatInfo.InvariantInfo);
            //return (char)XmlConvert.ToUInt16(pValue);
        }

        private static object CombineDateTime(string pValue, DateTimeSerializationMode pMode)
        {
            return XmlConvert.ToDateTime(pValue, pMode.Convert());
        }

        private static object CombineDecimal(string pValue)
        {
            return decimal.Parse(pValue, NumberFormatInfo.InvariantInfo);
            //return XmlConvert.ToDecimal(pValue);
        }

        private static object CombineDouble(string pValue)
        {
            return double.Parse(pValue, NumberFormatInfo.InvariantInfo);
            //return XmlConvert.ToDouble(pValue);
        }

        private static object CombineFloat(string pValue)
        {
            return float.Parse(pValue, NumberFormatInfo.InvariantInfo);
            //return XmlConvert.ToSingle(pValue);
        }

        private static object CombineGuid(string pValue)
        {
#if (NET_40_UP || NETSTANDARD)
            return Guid.Parse(pValue);
#else
            return ConvertHelper.ConvertGuidFromString(pValue);
#endif
            //return XmlConvert.ToGuid(pValue);
        }

        private static object CombineInt32(string pValue)
        {
            return int.Parse(pValue, NumberFormatInfo.InvariantInfo);
            //return XmlConvert.ToInt32(pValue);
        }

        private static object CombineLong(string pValue)
        {
            return long.Parse(pValue, NumberFormatInfo.InvariantInfo);
            //return XmlConvert.ToInt64(pValue);
        }

        private static object CombineShort(string pValue)
        {
            return short.Parse(pValue, NumberFormatInfo.InvariantInfo);
            //return XmlConvert.ToInt16(pValue);
        }

        private static object CombineString(string pValue)
        {
            //return pValue.Replace("\n", "\r\n");
            return pValue;
        }

        private static object CombineUnsignedByte(string pValue)
        {
            return byte.Parse(pValue, NumberFormatInfo.InvariantInfo);
            //return XmlConvert.ToByte(pValue);
        }

        private static object CombineUnsignedInt32(string pValue)
        {
            return uint.Parse(pValue, NumberFormatInfo.InvariantInfo);
            //return XmlConvert.ToUInt32(pValue);
        }

        private static object CombineUnsignedLong(string pValue)
        {
            return ulong.Parse(pValue, NumberFormatInfo.InvariantInfo);
            //return XmlConvert.ToUInt64(pValue);
        }

        private static object CombineUnsignedShort(string pValue)
        {
            return ushort.Parse(pValue, NumberFormatInfo.InvariantInfo);
            //return XmlConvert.ToUInt16(pValue);
        }

        private static object CombineTimeSpan(string pValue)
        {
            return TimeSpan.Parse(pValue);
        }

        private static object CombineDateTimeOffset(string pValue)
        {
            return XmlConvert.ToDateTimeOffset(pValue);
        }

#if NET_40_UP || NETSTANDARD_1_1_UP
        private static object CombineBigInteger(string pValue)
        {
            return System.Numerics.BigInteger.Parse(pValue, CultureInfo.InvariantCulture);
        }
#endif

        private static object CombineUri(string pValue)
        {
            return new Uri(pValue);
        }

        #endregion Private Methods
    }
}