using System.Xml;

namespace XPatchLib
{
    /// <summary>
    /// 增量内容文档合并基础类。 
    /// </summary>
    internal abstract class CombineBase
    {
        #region Internal Constructors

        /// <summary>
        /// 使用指定的类型初始化 <see cref="XPatchLib.CombineBase" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <remarks>
        /// 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 
        /// </remarks>
        internal CombineBase(TypeExtend pType)
            : this(pType, XmlDateTimeSerializationMode.RoundtripKind)
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
        /// <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <exception cref="PrimaryKeyException">
        /// 当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。 
        /// </exception>
        internal CombineBase(TypeExtend pType, XmlDateTimeSerializationMode pMode)
        {
            Type = pType;
            Mode = pMode;
        }

        #endregion Internal Constructors

        #region Internal Properties

        /// <summary>
        /// 获取或设置在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// </summary>
        internal XmlDateTimeSerializationMode Mode { get; set; }

        /// <summary>
        /// 获取或设置当前正在处理的类型。 
        /// </summary>
        internal TypeExtend Type { get; set; }

        #endregion Internal Properties
    }
}