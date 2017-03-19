// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
        internal virtual object Combine(ITextReader pReader, Object pOriObject, string pName)
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
        protected virtual void InitAttributes(ITextReader pReader, string pName)
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
        protected virtual CombineAttribute AnlysisAttributes(ITextReader pReader, string pName)
        {
            CombineAttribute result = new CombineAttribute(Action.Edit, pReader.AttributeCount);
            if (pReader.AttributeCount > 0 && pReader.NodeType == NodeType.Element &&
                pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase))
            {
                //读取除Action以外的所有Action，将其赋值给属性
                while (pReader.MoveToNextAttribute())
                {
#if DEBUG
                    Debug.WriteLine("Attributes of <" + pReader.Name + "," + pReader.Value + "<");
#endif
                    if (pReader.Setting.ActionName.Equals(pReader.Name))
                    {
                        Action action;
                        if (ActionHelper.TryParse(pReader.Value, out action))
                            result.Action = action;
                        continue;
                    }

                    result.KeysValuePairs.Add(pReader.Name,
                        AnlysisKeyAttributeValueObject(pReader, pReader.Name, pReader.Value));
                }
                pReader.MoveToElement();
            }
            return result;
        }

        protected virtual Object AnlysisKeyAttributeValueObject(ITextReader pReader,string pKeyName, string pKeyValue)
        {
            MemberWrapper member = FindMember(pKeyName);
            if (member != null)
                return CombineBasic.CombineAction(System.Type.GetTypeCode(member.Type), member.Type == typeof(Guid),
                    pReader.Setting.Mode, pKeyValue);
            return pKeyValue;
        }

        protected virtual MemberWrapper FindMember(string pMemberName)
        {
            MemberWrapper result = null;
            if (_fieldsToBeSerialized != null && _fieldsToBeSerialized.Count > 0)
                _fieldsToBeSerialized.TryGetValue(pMemberName, out result);
            return result;
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
        protected abstract object CombineAction(ITextReader pReader, Object pOriObject, string pName);

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.CombineBase" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        internal CombineBase(TypeExtend pType)
        {
            Type = pType;

            if (Type.FieldsToBeSerialized.Length > 0)
            {
                _fieldsToBeSerialized = new Dictionary<string, MemberWrapper>(Type.FieldsToBeSerialized.Length);
                foreach (MemberWrapper mw in Type.FieldsToBeSerialized)
                {
                    _fieldsToBeSerialized.Add(mw.Name, mw);
                }
            }
            Attributes = new CombineAttribute(Action.Edit, 0);
        }

        #endregion Internal Constructors

        #region Internal Properties
        /// <summary>
        ///     获取或设置当前正在处理的类型。
        /// </summary>
        internal TypeExtend Type { get; set; }

        protected CombineAttribute Attributes { get; private set; }

        #endregion Internal Properties

        private Dictionary<string, MemberWrapper> _fieldsToBeSerialized;
    }
}