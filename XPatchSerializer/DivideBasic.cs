using System;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    /// <summary>
    /// 基础类型增量内容产生类。 
    /// </summary>
    internal class DivideBasic : DivideBase
    {
        #region Internal Constructors

        /// <summary>
        /// 使用指定的类型初始化 <see cref="XPatchLib.DivideBasic" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <remarks>
        /// 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 
        /// </remarks>
        internal DivideBasic(TypeExtend pType)
            : base(pType) { }

        /// <summary>
        /// 使用指定的类型及指定是否序列化默认值初始化 <see cref="XPatchLib.DivideBasic" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <param name="pSerializeDefalutValue">
        /// 指定是否序列化默认值。 
        /// </param>
        /// <exception cref="PrimaryKeyException">
        /// 当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。 
        /// </exception>
        /// <remarks>
        /// <para> 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 </para>
        /// </remarks>
        internal DivideBasic(TypeExtend pType, Boolean pSerializeDefalutValue)
            : base(pType, pSerializeDefalutValue) { }

        /// <summary>
        /// 使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.DivideBasic" /> 类的新实例。
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
        internal DivideBasic(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : base(pType, pMode) { }

        /// <summary>
        /// 使用指定的类型、指定是否序列化默认值和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.DivideBasic" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <param name="pMode">
        /// 指定在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <param name="pSerializeDefalutValue">
        /// 指定是否序列化默认值。 
        /// </param>
        /// <exception cref="PrimaryKeyException">
        /// 当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。 
        /// </exception>
        internal DivideBasic(TypeExtend pType, XmlDateTimeSerializationMode pMode, Boolean pSerializeDefalutValue)
            : base(pType, pMode, pSerializeDefalutValue) { }

        #endregion Internal Constructors

        #region Internal Methods

        /// <summary>
        /// 产生基础类型增量内容。 
        /// </summary>
        /// <param name="pName">
        /// 增量内容对象的名称。 
        /// </param>
        /// <param name="pOriObject">
        /// 原始对象。 
        /// </param>
        /// <param name="pRevObject">
        /// 更新后的对象。 
        /// </param>
        /// <returns>
        /// 返回 <paramref name="pOriObject" /> 与 <paramref name="pRevObject" /> 比较后需要记录的内容的
        /// <see cref="System.Xml.Linq.XElement" /> 的表现形式。
        /// </returns>
        internal XElement Divide(string pName, Object pOriObject, Object pRevObject)
        {
            switch (this.Type.TypeCode)
            {
                case TypeCode.Boolean:
                    return DivideCore<Boolean>(pName, pOriObject, pRevObject);

                case TypeCode.Char:
                    return DivideCore<Char>(pName, pOriObject, pRevObject);

                case TypeCode.SByte:
                    return DivideCore<sbyte>(pName, pOriObject, pRevObject);

                case TypeCode.Byte:
                    return DivideCore<byte>(pName, pOriObject, pRevObject);

                case TypeCode.Int16:
                    return DivideCore<short>(pName, pOriObject, pRevObject);

                case TypeCode.UInt16:
                    return DivideCore<ushort>(pName, pOriObject, pRevObject);

                case TypeCode.Int32:
                    return DivideCore<int>(pName, pOriObject, pRevObject);

                case TypeCode.UInt32:
                    return DivideCore<uint>(pName, pOriObject, pRevObject);

                case TypeCode.Int64:
                    return DivideCore<long>(pName, pOriObject, pRevObject);

                case TypeCode.UInt64:
                    return DivideCore<ulong>(pName, pOriObject, pRevObject);

                case TypeCode.Single:
                    return DivideCore<float>(pName, pOriObject, pRevObject);

                case TypeCode.Double:
                    return DivideCore<Double>(pName, pOriObject, pRevObject);

                case TypeCode.Decimal:
                    return DivideCore<Decimal>(pName, pOriObject, pRevObject);

                case TypeCode.DateTime:
                    return DivideCore<DateTime>(pName, pOriObject, pRevObject);

                case TypeCode.String:
                    return DivideCore<String>(pName, pOriObject, pRevObject);
            }
            if (this.Type.IsGuid)
            {
                return DivideCore<Guid>(pName, pOriObject, pRevObject);
            }
            return null;
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// 产生基础类型增量内容核心方法。 
        /// </summary>
        /// <param name="pElementName">
        /// 增量内容对象的名称。 
        /// </param>
        /// <param name="pElementValue">
        /// 更新后的对象数据。 
        /// </param>
        /// <param name="pAction">
        /// 更新操作类型，默认为Edit。 
        /// </param>
        /// <returns>
        /// <para> 返回以 <see cref="System.Xml.Linq.XElement" /> 的表现形式记录的增量内容内容。 </para>
        /// <para> 当 <paramref name="pElementName" /> 为空时，返回 null 。 </para>
        /// </returns>
        private static XElement DivideCore(string pElementName, string pElementValue, Action pAction = Action.Edit)
        {
            XElement result = null;
            if ((pElementName != null) && (pElementName.Length != 0))
            {
                result = new XElement(pElementName);
                result.AppendActionAttribute(pAction);
                if (pAction != Action.SetNull)
                {
                    result.Value = pElementValue;
                }
            }
            //TODO:异常可能
            return result;
        }

        /// <summary>
        /// 产生基础类型增量内容核心方法。 
        /// </summary>
        /// <typeparam name="T">
        /// 待产生增量内容的对象类型定义。 
        /// </typeparam>
        /// <param name="pName">
        /// 增量内容对象的名称。 
        /// </param>
        /// <param name="pOriObject">
        /// 原始对象。 
        /// </param>
        /// <param name="pRevObject">
        /// 更新后的对象。 
        /// </param>
        /// <returns>
        /// 返回 <paramref name="pOriObject" /> 与 <paramref name="pRevObject" /> 比较后需要记录的内容的
        /// <see cref="System.Xml.Linq.XElement" /> 的表现形式。
        /// </returns>
        private XElement DivideCore<T>(string pName, Object pOriObject, Object pRevObject)
        {
            XElement result = null;
            if (!ReflectionUtils.ObjectEquals(pOriObject, pRevObject))
            {
                //只有当原始对象与变更后的对象不相等时才需要产生增量内容。
                if (pRevObject == null)
                {
                    //当更新后对象为Null时设置为SetNull
                    return DivideCore(pName, string.Empty, Action.SetNull);
                }
                else if (pOriObject == null && ReflectionUtils.IsDefaultValue<T>((T)pRevObject))
                {
                    //原始对象为null，且更新后的对象为类型初始值时，需要判断_SerializeDefaultValue属性来确定是否序列化初始值。
                    if (this.SerializeDefaultValue)
                    {
                        return DivideCore(pName, TransToString<T>(pRevObject));
                    }
                }
                else
                {
                    return DivideCore(pName, TransToString<T>(pRevObject));
                }
            }
            return result;
        }

        /// <summary>
        /// 将待序列化的基础类型对象实例转换为可序列化的字符串。 
        /// </summary>
        /// <typeparam name="T">
        /// 待序列化的基础类型。 
        /// </typeparam>
        /// <param name="pObj">
        /// 待序列化的基础类型对象的值。 
        /// </param>
        /// <returns>
        /// 转换成功返回转换后的字符串，否则返回 <see cref="String.Empty" /> 。 
        /// </returns>
        private string TransToString<T>(Object pObj)
        {
            switch (System.Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                    return XmlConvert.ToString((Boolean)pObj);

                case TypeCode.Byte:
                    return XmlConvert.ToString((Byte)pObj);

                case TypeCode.Decimal:
                    return XmlConvert.ToString((Decimal)pObj);

                case TypeCode.Double:
                    return XmlConvert.ToString((Double)pObj);

                case TypeCode.Int16:
                    return XmlConvert.ToString((Int16)pObj);

                case TypeCode.Int32:
                    return XmlConvert.ToString((Int32)pObj);

                case TypeCode.Int64:
                    return XmlConvert.ToString((Int64)pObj);

                case TypeCode.SByte:
                    return XmlConvert.ToString((SByte)pObj);

                case TypeCode.Single:
                    return XmlConvert.ToString((Single)pObj);

                case TypeCode.UInt16:
                    return XmlConvert.ToString((UInt16)pObj);

                case TypeCode.UInt32:
                    return XmlConvert.ToString((UInt32)pObj);

                case TypeCode.UInt64:
                    return XmlConvert.ToString((UInt64)pObj);

                case TypeCode.DateTime:
                    return XmlConvert.ToString((DateTime)pObj, this.Mode);

                case TypeCode.String:
                    return (string)pObj;

                case TypeCode.Char:
                    return XmlConvert.ToString((ushort)((char)pObj));
            }

            if (typeof(T) == typeof(Guid))
            {
                return XmlConvert.ToString((Guid)pObj);
            }

            //TODO:异常可能
            return string.Empty;
        }

        #endregion Private Methods
    }
}