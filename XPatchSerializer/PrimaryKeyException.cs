// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace XPatchLib
{
    /// <summary>
    ///     主键定义异常。
    /// </summary>
    /// <remarks>
    ///     主键只能够设置为基础类型。
    /// </remarks>
    [Serializable]
    public class PrimaryKeyException : Exception
    {
        #region Public Constructors

        /// <summary>
        ///     使用指定的类型及指定的主键名称创建主键定义异常的实例。
        /// </summary>
        /// <param name="pType">
        ///     有问题的类型。
        /// </param>
        /// <param name="pKeyName">
        ///     有问题的主键名称。
        /// </param>
        public PrimaryKeyException(Type pType, string pKeyName)
        {
            SourceType = pType;
            PrimaryKeyName = pKeyName;
        }

        /// <summary>
        ///     使用指定的错误信息初始化 <see cref="XPatchLib.PrimaryKeyException" /> 类的新实例。
        /// </summary>
        /// <param name="pMessage">
        ///     描述错误的消息。
        /// </param>
        public PrimaryKeyException(string pMessage)
            : base(pMessage)
        {
        }

        /// <summary>
        ///     初始化 <see cref="XPatchLib.PrimaryKeyException" /> 类的新实例。
        /// </summary>
        public PrimaryKeyException()
        {
        }

        /// <summary>
        ///     使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="XPatchLib.PrimaryKeyException" /> 类的新实例。
        /// </summary>
        /// <param name="pMessage">
        ///     描述错误的消息。
        /// </param>
        /// <param name="pInnerException">
        ///     导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。
        /// </param>
        public PrimaryKeyException(string pMessage, Exception pInnerException)
            : base(pMessage, pInnerException)
        {
        }

        /// <summary>
        ///     用序列化数据初始化 <see cref="XPatchLib.PrimaryKeyException" /> 类的新实例。
        /// </summary>
        /// <param name="info">
        ///     <see cref="System.Runtime.Serialization.SerializationInfo" /> ，它存有有关所引发的异常的序列化对象数据。
        /// </param>
        /// <param name="context">
        ///     <see cref="System.Runtime.Serialization.StreamingContext" /> ，它包含有关源或目标的上下文信息。
        /// </param>
        protected PrimaryKeyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        ///     获取描述当前异常的消息。
        /// </summary>
        public override string Message
        {
            get
            {
                //TODO:多语言
                return string.Format(CultureInfo.InvariantCulture, " '{0}' 的 主键 {1} 设置异常.", SourceType.Name,
                    PrimaryKeyName);
            }
        }

        /// <summary>
        ///     获取有问题的主键名称。
        /// </summary>
        public string PrimaryKeyName { get; set; }

        /// <summary>
        ///     获取有问题的类型。
        /// </summary>
        public Type SourceType { get; set; }

        /// <summary>
        ///     用关于异常的信息设置 <see cref="System.Runtime.Serialization.SerializationInfo" /> 。
        /// </summary>
        /// <param name="info">
        /// </param>
        /// <param name="context">
        /// </param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("PrimaryKeyName", PrimaryKeyName);
            info.AddValue("SourceType", SourceType);
        }

        #endregion Public Properties
    }
}