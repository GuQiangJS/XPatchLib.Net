using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace XPatchLib
{
    /// <summary>
    /// 自定义属性未找到异常。 
    /// </summary>
    [Serializable]
    public class AttributeMissException : Exception
    {
        #region Public Constructors

        /// <summary>
        /// 使用指定的自定义属性名称初始化 <see cref="XPatchLib.AttributeMissException" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的未找到特性的类型。。 
        /// </param>
        /// <param name="pAttrName">
        /// 指定的自定义属性名称。 
        /// </param>
        public AttributeMissException(Type pType, string pAttrName)
            : base()
        {
            Guard.ArgumentNotNullOrEmpty(pAttrName, "pAttrName");
            Guard.ArgumentNotNull(pType, "pType");

            this.ErrorType = pType;
            this.AttributeName = pAttrName;
        }

        /// <summary>
        /// 初始化 <see cref="XPatchLib.AttributeMissException" /> 类的新实例。 
        /// </summary>
        public AttributeMissException()
            : base()
        {
        }

        /// <summary>
        /// 使用指定的错误信息初始化 <see cref="XPatchLib.AttributeMissException" /> 类的新实例。 
        /// </summary>
        /// <param name="message">
        /// 描述错误的消息。 
        /// </param>
        public AttributeMissException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="XPatchLib.AttributeMissException" /> 类的新实例。 
        /// </summary>
        /// <param name="message">
        /// 解释异常原因的错误信息。 
        /// </param>
        /// <param name="innerException">
        /// 导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。 
        /// </param>
        public AttributeMissException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        /// <summary>
        /// 用序列化数据初始化 <see cref="XPatchLib.AttributeMissException" /> 类的新实例。 
        /// </summary>
        /// <param name="info">
        /// <see cref="System.Runtime.Serialization.SerializationInfo" /> ，它存有有关所引发的异常的序列化对象数据。 
        /// </param>
        /// <param name="context">
        /// <see cref="System.Runtime.Serialization.StreamingContext" /> ，它包含有关源或目标的上下文信息。 
        /// </param>
        protected AttributeMissException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// 获取当前未找到的自定义特性的名称。 
        /// </summary>
        public string AttributeName { get; private set; }

        /// <summary>
        /// 获取当前未找到特性的类型。 
        /// </summary>
        public Type ErrorType { get; private set; }

        /// <summary>
        /// 获取描述当前异常的消息。 
        /// </summary>
        public override string Message
        {
            get
            {
                //TODO:多语言
                return string.Format(CultureInfo.InvariantCulture, "类型 {0} 上没有定义 '{1}' Attribute .", this.ErrorType.FullName, this.AttributeName);
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// 用关于异常的信息设置 <see cref="System.Runtime.Serialization.SerializationInfo" /> 。 
        /// </summary>
        /// <param name="info">
        /// </param>
        /// <param name="context">
        /// </param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("AttributeName", this.AttributeName);
            info.AddValue("ErrorType", this.ErrorType);
        }

        #endregion Public Methods
    }
}