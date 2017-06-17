// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     KeyValuePair类型增量内容合并类。
    /// </summary>
    /// <seealso cref="XPatchLib.CombineBase" />
    internal class CombineKeyValuePair : CombineBase
    {
        /// <summary>
        ///     检查当前Action是否为SetNull，如果是就退出
        /// </summary>
        /// <returns></returns>
        protected override bool CheckSetNullReturn()
        {
            //KeyValuePair的SetNull，只是设置Value值为Null，不能退出
            return false;
        }

        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">原始Key值与更新后的Key值不符时。</exception>
        /// <exception cref="System.NotImplementedException">
        ///     待更新的Key值存在，但是更新操作被指定为<see cref="Action.Remove" />,
        ///     <see cref="Action.Edit" />,<see cref="Action.SetNull" />之外的操作时。
        /// </exception>
        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            //获取KeyValuePair类型对象的Key值与Value值的类型
            TypeExtend keyType = TypeExtendContainer.GetTypeExtend(Type.KeyArgumentType, null, Type);
            TypeExtend valueType = TypeExtendContainer.GetTypeExtend(Type.ValueArgumentType, null, Type);

            //获取原始值的Key值和Value值
            object oriKeyObj = null;

            object revKey = null;
            object revValue = null;

            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;
                if (pReader.Name.Equals(ConstValue.KEY) && pReader.NodeType == NodeType.Element)
                    revKey = CombineInstanceContainer.GetCombineInstance(keyType).Combine(pReader, null, pReader.Name);
                if (pReader.Name.Equals(ConstValue.VALUE) && pReader.NodeType == NodeType.Element)
                    revValue = CombineInstanceContainer.GetCombineInstance(valueType).Combine(pReader, null, pReader.Name);
                pReader.Read();
            }


            if (pOriObject == null)
            {
                //当原始对象为null时，先创建一个实例。并且赋予pElement转换的Key值和Value值
                //pOriObject = Type.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null,
                //    new[] {revKey != null ? revKey : oriKeyObj, revValue}, CultureInfo.InvariantCulture);
                pOriObject = this.Type.CreateInstance();
                Type.SetMemberValue(pOriObject, ConstValue.KEY, revKey != null ? revKey : oriKeyObj);
                Type.SetMemberValue(pOriObject, ConstValue.VALUE, revValue);
            }
            else
            {
                //当原始值不为空时，先获取原始值中的Key值和Value值
                oriKeyObj = Type.GetMemberValue(pOriObject, ConstValue.KEY);

                if (!oriKeyObj.Equals(revKey))
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                        "原始Key值:'{0}',更新后的Key值:'{1}'.",
                        oriKeyObj, revKey));

                if (Attributes.Action == Action.SetNull || Attributes.Action == Action.Edit)
                {
                    //pOriObject = Type.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null,
                    //    new[] {revKey != null ? revKey : oriKeyObj, revValue}, CultureInfo.InvariantCulture);
                    pOriObject = this.Type.CreateInstance();
                    Type.SetMemberValue(pOriObject, ConstValue.KEY, revKey != null ? revKey : oriKeyObj);
                    Type.SetMemberValue(pOriObject, ConstValue.VALUE, revValue);
                }
                else if (Attributes.Action == Action.Remove)
                    pOriObject = null;
                else
                    throw new NotImplementedException();
            }

            return pOriObject;
        }

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.CombineIEnumerable" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        internal CombineKeyValuePair(TypeExtend pType)
            : base(pType)
        {
        }

        #endregion Internal Constructors
    }
}