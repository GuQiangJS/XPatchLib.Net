using System;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    /// 对象比较产生增量内容结果的基础类。 
    /// </summary>
    internal abstract class DivideBase
    {
        #region Protected Constructors

        /// <summary>
        /// 使用指定的类型初始化 <see cref="XPatchLib.DivideBase" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <exception cref="PrimaryKeyException">
        /// 当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。 
        /// </exception>
        /// <remarks>
        /// <para> 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 </para>
        /// <para> 默认不序列化默认值。 </para>
        /// </remarks>
        protected DivideBase(TypeExtend pType)
            : this(pType, XmlDateTimeSerializationMode.RoundtripKind)
        {
        }

        /// <summary>
        /// 使用指定的类型及指定是否序列化默认值初始化 <see cref="XPatchLib.DivideBase" /> 类的新实例。 
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
        protected DivideBase(TypeExtend pType, Boolean pSerializeDefalutValue)
            : this(pType, XmlDateTimeSerializationMode.RoundtripKind, pSerializeDefalutValue)
        {
        }

        /// <summary>
        /// 使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.CombineBase" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <param name="pMode">
        /// 指定在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// </param>
        /// <remarks>
        /// <para> 默认不序列化默认值。 </para>
        /// </remarks>
        /// <exception cref="PrimaryKeyException">
        /// 当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。 
        /// </exception>
        protected DivideBase(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : this(pType, pMode, false)
        {
        }

        /// <summary>
        /// 使用指定的类型、指定是否序列化默认值和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.CombineBase" /> 类的新实例。
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
        protected DivideBase(TypeExtend pType, XmlDateTimeSerializationMode pMode, Boolean pSerializeDefalutValue)
        {
            this.Type = pType;
            this.SerializeDefaultValue = pSerializeDefalutValue;
            this.Mode = pMode;
        }

        #endregion Protected Constructors

        #region Internal Properties

        /// <summary>
        /// 获取或设置在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// </summary>
        internal XmlDateTimeSerializationMode Mode { get; set; }

        /// <summary>
        /// 获取或设置是否处理序列化默认值。 
        /// </summary>
        internal Boolean SerializeDefaultValue { get; set; }

        /// <summary>
        /// 获取或设置当前正在处理的类型。 
        /// </summary>
        internal TypeExtend Type { get; set; }

        #endregion Internal Properties
    }
}