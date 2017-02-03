// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     增量内容文档合并基础类。
    /// </summary>
    internal abstract class CombineBase
    {
        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pReader" /> is null 时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pReader" /> is null 时。</exception>
        /// <exception cref="ArgumentException">当参数 <paramref name="pName" /> 长度为 0 时。</exception>
        internal virtual object Combine(XmlReader pReader, Object pOriObject, string pName)
        {
            Guard.ArgumentNotNull(pReader, "pReader");
            Guard.ArgumentNotNullOrEmpty(pName, "pName");

            if (pReader.ReadState == ReadState.Initial)
                pReader.Read();

            InitAttributes(pReader, pName);

            if (CheckSetNullReturn())
                return null;
            return CombineAction(pReader, pOriObject, pName);
        }

        /// <summary>
        ///     检查当前Action是否为SetNull，如果是就退出
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean CheckSetNullReturn()
        {
            return Attributes.Action == Action.SetNull;
        }

        /// <summary>
        ///     初始化当前正在读取的节点的Attribute内容
        /// </summary>
        /// <param name="pReader">The p reader.</param>
        /// <param name="pName">Name of the p.</param>
        /// <remarks>执行此操作会移动到移动到包含当前属性节点的元素。<see cref="XmlReader.MoveToElement()" /></remarks>
        protected virtual void InitAttributes(XmlReader pReader, string pName)
        {
            Attributes = AnlysisAttributes(pReader, pName);
        }

        /// <summary>
        ///     分析XmlReader中的Attribute
        /// </summary>
        /// <param name="pReader">The p reader.</param>
        /// <param name="pName">Name of the p.</param>
        /// <returns></returns>
        /// <remarks>执行此操作会移动到移动到包含当前属性节点的元素。<see cref="XmlReader.MoveToElement()" /></remarks>
        protected virtual CombineAttribute AnlysisAttributes(XmlReader pReader, string pName)
        {
            CombineAttribute result = new CombineAttribute(Action.Edit, pReader.AttributeCount);
            if (pReader.NodeType == XmlNodeType.Element &&
                pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) && pReader.HasAttributes)
            {
                //读取除Action以外的所有Action，将其赋值给属性
                while (pReader.MoveToNextAttribute())
                {
#if DEBUG
                    Debug.WriteLine("Attributes of <" + pReader.Name + "," + pReader.Value + "<");
#endif
                    if (ConstValue.ACTION_NAME.Equals(pReader.Name))
                    {
                        Action action;
                        if (ActionHelper.TryParse(pReader.Value, out action))
                            result.Action = action;
                        continue;
                    }

                    result.KeysValuePairs.Add(pReader.Name, AnlysisKeyAttributeValueObject(pReader.Name, pReader.Value));
                }
                pReader.MoveToElement();
            }
            return result;
        }

        protected virtual Object AnlysisKeyAttributeValueObject(string pKeyName,string pKeyValue)
        {
            MemberWrapper member = FindMember(pKeyName);
            if(member!=null)
            {
                return CombineBasic.CombineAction(System.Type.GetTypeCode(member.Type), member.Type == typeof(Guid), Mode, pKeyValue);
            }
            return pKeyValue;
        }

        protected virtual MemberWrapper FindMember(string pMemberName)
        {
            for (int i = 0; i < Type.FieldsToBeSerialized.Length;i++ )
            {
                if(Type.FieldsToBeSerialized[i].Name.Equals(pMemberName, StringComparison.OrdinalIgnoreCase))
                {
                    return Type.FieldsToBeSerialized[i];
                }
            }
            return null;
        }

        protected virtual bool TryFindMember(string pMemberName, out MemberWrapper pMember)
        {
            pMember = FindMember(pMemberName);
            return pMember != null;
        }

        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        protected abstract object CombineAction(XmlReader pReader, Object pOriObject, string pName);

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.CombineBase" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        /// <remarks>
        ///     默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。
        /// </remarks>
        internal CombineBase(TypeExtend pType)
            : this(pType, XmlDateTimeSerializationMode.RoundtripKind)
        {
        }

        /// <summary>
        ///     使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        ///     <see cref="XPatchLib.CombineBase" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        /// <param name="pMode">
        ///     指定在字符串与 System.DateTime 之间转换时，如何处理时间值。
        ///     <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <exception cref="PrimaryKeyException">
        ///     当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。
        /// </exception>
        internal CombineBase(TypeExtend pType, XmlDateTimeSerializationMode pMode)
        {
            Type = pType;
            Mode = pMode;
            Attributes = new CombineAttribute(Action.Edit, 0);
        }

        #endregion Internal Constructors

        #region Internal Properties

        /// <summary>
        ///     获取或设置在字符串与 System.DateTime 之间转换时，如何处理时间值。
        /// </summary>
        internal XmlDateTimeSerializationMode Mode { get; set; }

        /// <summary>
        ///     获取或设置当前正在处理的类型。
        /// </summary>
        internal TypeExtend Type { get; set; }

        protected CombineAttribute Attributes { get; private set; }

        #endregion Internal Properties
    }
}