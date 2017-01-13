using System;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    /// <summary>
    /// 基础类型增量内容合并类。 
    /// </summary>
    internal class CombineBasic : CombineBase
    {
        #region Internal Constructors

        /// <summary>
        /// 使用指定的类型初始化 <see cref="XPatchLib.CombineBasic" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <remarks>
        /// 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 
        /// </remarks>
        internal CombineBasic(TypeExtend pType)
            : base(pType) { }

        /// <summary>
        /// 使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.CombineBasic" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <param name="pMode">
        /// 指定在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <exception cref="PrimaryKeyException">
        /// 当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。 
        /// </exception>
        internal CombineBasic(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : base(pType, pMode) { }

        #endregion Internal Constructors

        #region Internal Methods

        /// <summary>
        /// 根据增量内容创建基础类型实例。 
        /// </summary>
        /// <param name="pElement">
        /// 增量内容。 
        /// </param>
        /// <returns>
        /// 创建成功的基础类型实例。 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 当 <paramref name="pElement" /> is null 时。 
        /// </exception>
        internal Object Combine(XElement pElement)
        {
            Guard.ArgumentNotNull(pElement, "pElement");

            Action action;
            if (pElement.TryGetActionAttribute(out action) && action == Action.SetNull)
            {
                return null;
            }
            switch (this.Type.TypeCode)
            {
                case TypeCode.Boolean:
                    return CombineBoolean(pElement);

                case TypeCode.Char:
                    return CombineChar(pElement);

                case TypeCode.SByte:
                    return CombineByte(pElement);

                case TypeCode.Byte:
                    return CombineUnsignedByte(pElement);

                case TypeCode.Int16:
                    return CombineShort(pElement);

                case TypeCode.UInt16:
                    return CombineUnsignedShort(pElement);

                case TypeCode.Int32:
                    return CombineInt32(pElement);

                case TypeCode.UInt32:
                    return CombineUnsignedInt32(pElement);

                case TypeCode.Int64:
                    return CombineLong(pElement);

                case TypeCode.UInt64:
                    return CombineUnsignedLong(pElement);

                case TypeCode.Single:
                    return CombineFloat(pElement);

                case TypeCode.Double:
                    return CombineDouble(pElement);

                case TypeCode.Decimal:
                    return CombineDecimal(pElement);

                case TypeCode.DateTime:
                    return CombineDateTime(pElement, this.Mode);

                case TypeCode.String:
                    return CombineString(pElement);
            }
            if (this.Type.IsGuid)
            {
                return CombineGuid(pElement);
            }
            return null;
        }

        #endregion Internal Methods

        #region Private Methods

        private static Object CombineBoolean(XElement pElement)
        {
            return XmlConvert.ToBoolean(pElement.Value);
        }

        private static Object CombineByte(XElement pElement)
        {
            return XmlConvert.ToSByte(pElement.Value);
        }

        private static Object CombineChar(XElement pElement)
        {
            return (char)XmlConvert.ToUInt16(pElement.Value);
        }

        private static Object CombineDateTime(XElement pElement, XmlDateTimeSerializationMode pMode)
        {
            return XmlConvert.ToDateTime(pElement.Value, pMode);
        }

        private static Object CombineDecimal(XElement pElement)
        {
            return XmlConvert.ToDecimal(pElement.Value);
        }

        private static Object CombineDouble(XElement pElement)
        {
            return XmlConvert.ToDouble(pElement.Value);
        }

        private static Object CombineFloat(XElement pElement)
        {
            return XmlConvert.ToSingle(pElement.Value);
        }

        private static Object CombineGuid(XElement pElement)
        {
            return XmlConvert.ToGuid(pElement.Value);
        }

        private static Object CombineInt32(XElement pElement)
        {
            return XmlConvert.ToInt32(pElement.Value);
        }

        private static Object CombineLong(XElement pElement)
        {
            return XmlConvert.ToInt64(pElement.Value);
        }

        private static Object CombineShort(XElement pElement)
        {
            return XmlConvert.ToInt16(pElement.Value);
        }

        private static Object CombineString(XElement pElement)
        {
            return pElement.Value;
        }

        private static Object CombineUnsignedByte(XElement pElement)
        {
            return XmlConvert.ToByte(pElement.Value);
        }

        private static Object CombineUnsignedInt32(XElement pElement)
        {
            return XmlConvert.ToUInt32(pElement.Value);
        }

        private static Object CombineUnsignedLong(XElement pElement)
        {
            return XmlConvert.ToUInt64(pElement.Value);
        }

        private static Object CombineUnsignedShort(XElement pElement)
        {
            return XmlConvert.ToUInt16(pElement.Value);
        }

        #endregion Private Methods
    }
}