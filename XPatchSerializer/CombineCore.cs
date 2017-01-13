using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    /// <summary>
    /// 增量内容合并入口类。 
    /// </summary>
    /// <remarks>
    /// 此类是增量内容合并的入口类，由此类区分待产生增量内容的对象类型，调用不同的增量内容产生类。 
    /// </remarks>
    internal class CombineCore : CombineBase
    {
        #region Internal Constructors

        /// <summary>
        /// 使用指定的类型初始化 <see cref="XPatchLib.CombineCore" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <remarks>
        /// 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 
        /// </remarks>
        internal CombineCore(TypeExtend pType)
            : base(pType) { }

        /// <summary>
        /// 使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.CombineCore" /> 类的新实例。
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
        internal CombineCore(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : base(pType, pMode) { }

        #endregion Internal Constructors

        #region Internal Methods

        /// <summary>
        /// 将增量内容对象合并至原始对象上。 
        /// </summary>
        /// <param name="pOriObject">
        /// 原始对象集合。 
        /// </param>
        /// <param name="pElement">
        /// 增量节点。 
        /// </param>
        /// <returns>
        /// 返回数据合并后的对象。 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 当 <paramref name="pElement" /> is null 时。 
        /// </exception>
        /// <exception cref="FormatException">
        /// 当增量内容中有需要转换为 <see cref="System.Drawing.Color" /> 类型的数据，而数据无法被转换为
        /// <see cref="System.Drawing.Color" /> 实例时。
        /// </exception>
        internal Object Combine(Object pOriObject, XElement pElement)
        {
            if (pOriObject == null)
            {
                pOriObject = this.Type.CreateInstance();
            }

            if (this.Type.IsBasicType)
            {
                return new CombineBasic(this.Type, this.Mode).Combine(pElement);
            }
            else if (this.Type.IsIDictionary)
            {
                //判断是否为字典(IsIDictionary)要放在判断是否为集合之前(IsIEnumerable)
                return new CombineIDictionary(this.Type, this.Mode).Combine(pOriObject, pElement);
            }
            else if (this.Type.IsIEnumerable)
            {
                return new CombineIEnumerable(this.Type, this.Mode).Combine(pOriObject, pElement);
            }
            else if (this.Type.IsGenericType && this.Type.OriType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                return new CombineKeyValuePair(this.Type, this.Mode).Combine(pOriObject, pElement);
            }
            else
            {
                return new CombineObject(this.Type, this.Mode).Combine(pOriObject, pElement);
            }
        }

        #endregion Internal Methods
    }
}