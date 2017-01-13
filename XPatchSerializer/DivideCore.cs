using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    /// <summary>
    /// 增量内容产生入口类。 
    /// </summary>
    /// <remarks>
    /// 此类是增量内容产生的入口类，由此类区分待产生增量内容的对象类型，调用不同的增量内容产生类。 
    /// </remarks>
    internal class DivideCore : DivideBase
    {
        #region Internal Constructors

        /// <summary>
        /// 使用指定的类型初始化 <see cref="XPatchLib.DivideCore" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <remarks>
        /// 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 
        /// </remarks>
        internal DivideCore(TypeExtend pType)
            : base(pType) { }

        /// <summary>
        /// 使用指定的类型及指定是否序列化默认值初始化 <see cref="XPatchLib.DivideCore" /> 类的新实例。 
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
        internal DivideCore(TypeExtend pType, Boolean pSerializeDefalutValue)
            : base(pType, pSerializeDefalutValue) { }

        /// <summary>
        /// 使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.DivideCore" /> 类的新实例。
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
        internal DivideCore(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : base(pType, pMode) { }

        /// <summary>
        /// 使用指定的类型、指定是否序列化默认值和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.DivideCore" /> 类的新实例。
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
        internal DivideCore(TypeExtend pType, XmlDateTimeSerializationMode pMode, Boolean pSerializeDefalutValue)
            : base(pType, pMode, pSerializeDefalutValue) { }

        #endregion Internal Constructors

        #region Internal Methods

        /// <summary>
        /// 产生增量内容。 
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
            if (this.Type.IsBasicType)
            {
                return new DivideBasic(this.Type, this.Mode, this.SerializeDefaultValue).Divide(pName, pOriObject, pRevObject);
            }
            else if (this.Type.IsIDictionary)
            {
                //判断是否为字典(IsIDictionary)要放在判断是否为集合之前(IsIEnumerable)
                return new DivideIDictionary(this.Type, this.Mode, this.SerializeDefaultValue).Divide(pName, pOriObject, pRevObject);
            }
            else if (this.Type.IsIEnumerable)
            {
                return new DivideIEnumerable(this.Type, this.Mode, this.SerializeDefaultValue).Divide(pName, pOriObject, pRevObject);
            }
            else if (this.Type.IsGenericType && this.Type.OriType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                return new DivideKeyValuePair(this.Type, this.Mode, this.SerializeDefaultValue).Divide(pName, pOriObject, pRevObject);
            }
            else
            {
                return new DivideObject(this.Type, this.Mode, this.SerializeDefaultValue).Divide(pName, pOriObject, pRevObject);
            }
        }

        #endregion Internal Methods
    }
}