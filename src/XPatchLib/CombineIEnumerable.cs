// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace XPatchLib
{
    /// <summary>
    ///     集合类型增量内容合并类。
    /// </summary>
    /// <seealso cref="XPatchLib.CombineBase" />
    internal class CombineIEnumerable : CombineBase
    {
        private static readonly string PRIMARY_KEY_MISS = typeof(PrimaryKeyAttribute).Name;

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.CombineIEnumerable" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        internal CombineIEnumerable(TypeExtend pType) : base(pType)
        {
            Type t;
            if (ReflectionUtils.TryGetIEnumerableGenericArgument(pType.OriType, out t))
            {
                GenericArgumentType = TypeExtendContainer.GetTypeExtend(t, null, pType);

                GenericArgumentTypePrimaryKeyAttribute = GenericArgumentType.PrimaryKeyAttr;

                if (GenericArgumentTypePrimaryKeyAttribute == null && !GenericArgumentType.IsBasicType)
                    throw new AttributeMissException(GenericArgumentType.OriType, PRIMARY_KEY_MISS);
            }
            else
            {
                //TODO：传入参数 pType 无法解析内部元素对象。
                throw new ArgumentOutOfRangeException(pType.OriType.FullName);
            }
        }

        #endregion Internal Constructors

        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            var kvs = KeyValuesObject.Translate(pOriObject as IEnumerable);
            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;

                pReader.MoveToElement();

                if (pReader.Name == GenericArgumentType.TypeFriendlyName && pReader.NodeType == NodeType.Element)
                    CombineCore(pReader, ref pOriObject, kvs, GenericArgumentType.TypeFriendlyName);
                pReader.Read();
            }
            return pOriObject;
        }

        #region Protected Properties

        /// <summary>
        ///     获取集合中的元素类型。
        /// </summary>
        protected TypeExtend GenericArgumentType { get; }

        protected PrimaryKeyAttribute GenericArgumentTypePrimaryKeyAttribute { get; }

        #endregion Protected Properties

        #region Private Methods

        /// <summary>
        ///     合并新增类型动作的增量内容。
        /// </summary>
        /// <param name="pReader">Xml读取器。</param>
        /// <param name="pOriObject">待合并数据的原始对象。</param>
        /// <param name="pName">当前正在解析的节点名称</param>
        private void CombineAddedItem(ITextReader pReader, ref object pOriObject,
            string pName)
        {
            //当原始对象为null时，先创建一个实例。
            if (pOriObject == null)
                pOriObject = Type.CreateInstance();

            //原始集合对象的类型。
            var listType = pOriObject.GetType();
            //创建集合元素实例,根据增量内容内容向集合元素实例赋值。
            var obj = CombineInstanceContainer.GetCombineInstance(GenericArgumentType).Combine(pReader, null, pName);

            if (Type.IsArray)
            {
                //当当前集合是数组类型时
                //将原始对象转换为Array
                var oldList = (Array) pOriObject;
                //创建长度比原始对象长度+1的新的Array
                var newList = Array.CreateInstance(GenericArgumentType.OriType, oldList.Length + 1);
                //将原始对象的Array全部复制至新的集合对象
                oldList.CopyTo(newList, 0);
                //将增量内容内容创建的元素实例赋值至新的集合对象的最后一个元素（空元素）
                newList.SetValue(obj, newList.Length - 1);
                //使用新的集合对象替换原有的集合对象。
                pOriObject = newList;
            }
            else if (listType.GetMethod(ConstValue.OPERATOR_ADD) != null)
            {
                //非数组类型时，在当前集合类型上调用Add方法
                Type.InvokeMember(ConstValue.OPERATOR_ADD, BindingFlags.InvokeMethod, null, pOriObject, new[] {obj},
                    CultureInfo.InvariantCulture);
            }
            else
            {
                //TODO:未实现
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     增量内容数据合并核心方法。
        /// </summary>
        /// <param name="pReader">Xml读取器。</param>
        /// <param name="pOriObject">待合并数据的原始对象。</param>
        /// <param name="pOriEnumerable">当前正在处理的集合对象。</param>
        /// <param name="pName">当前正在解析的节点名称。</param>
        private void CombineCore(ITextReader pReader, ref object pOriObject, IEnumerable<KeyValuesObject> pOriEnumerable,
            string pName)
        {
            var attrs = AnlysisAttributes(pReader, pName);

            switch (attrs.Action)
            {
                case Action.Add:
                    CombineAddedItem(pReader, ref pOriObject, pName);
                    break;

                case Action.Edit:
                    CombineEditItem(pReader, attrs, pOriEnumerable, pName);
                    break;

                case Action.Remove:
                    CombineRemovedItem(pReader, attrs, ref pOriObject, pOriEnumerable);
                    break;
            }
        }

        protected override MemberWrapper FindMember(string pMemberName)
        {
            for (int i = 0; i < GenericArgumentType.FieldsToBeSerialized.Length; i++)
                if (GenericArgumentType.FieldsToBeSerialized[i].Name.Equals(pMemberName,
                    StringComparison.OrdinalIgnoreCase))
                    return GenericArgumentType.FieldsToBeSerialized[i];
            return null;
        }

        /// <summary>
        ///     合并编辑类型动作的增量内容。
        /// </summary>
        /// <param name="pReader">Xml读取器。</param>
        /// <param name="pAttribute">当前正在解析的Attributes。（包含了Action和主键集合）</param>
        /// <param name="pOriEnumerable">当前正在处理的集合对象。</param>
        /// <param name="pName">当前正在解析的节点名称。</param>
        private void CombineEditItem(ITextReader pReader, CombineAttribute pAttribute,
            IEnumerable<KeyValuesObject> pOriEnumerable, string pName)
        {
            if (pOriEnumerable != null)
            {
                KeyValuesObject o = null;
                if (GenericArgumentType.IsBasicType && pOriEnumerable != null)
                {
                    var value = pReader.ReadString();
                    o = pOriEnumerable.FirstOrDefault(x => x.Equals(value));
                }
                else
                {
                    o = pOriEnumerable.FirstOrDefault(x => x.EqualsByKeys(pAttribute.Keys, pAttribute.Values));
                }
                if (o != null)
                {
                    object foundItem = o.OriValue;
                    foundItem = CombineInstanceContainer.GetCombineInstance(GenericArgumentType)
                        .Combine(pReader, foundItem, pName);
                }
            }
        }

        /// <summary>
        ///     合并删除类型动作的增量内容。
        /// </summary>
        /// <param name="pReader">Xml读取器。</param>
        /// <param name="pAttribute">当前正在解析的Attributes。（包含了Action和主键集合）</param>
        /// <param name="pOriObject">待合并数据的原始对象。</param>
        /// <param name="pOriEnumerable">当前正在处理的集合对象。</param>
        private void CombineRemovedItem(ITextReader pReader, CombineAttribute pAttribute, ref object pOriObject,
            IEnumerable<KeyValuesObject> pOriEnumerable)
        {
            //在集合中找到的待删除的元素实例。
            object foundItem = null;

            if (pOriEnumerable != null)
            {
                KeyValuesObject o = null;
                //当当前集合是数组类型时
                if (GenericArgumentType.IsBasicType)
                {
                    var value = pReader.ReadString();
                    //当时基础类型时，按照基础类型获取所有的方式查找索引。

                    o = pOriEnumerable.FirstOrDefault(x => x.Equals(value));
                    if (o != null)
                        foundItem = o.OriValue;
                }
                else
                {
                    o = pOriEnumerable.FirstOrDefault(x => x.EqualsByKeys(pAttribute.Keys, pAttribute.Values));
                }
                if (o != null)
                    foundItem = o.OriValue;
                CombineRemovedItem(ref pOriObject, foundItem);
            }
            else
            {
                //TODO:未实现
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     根据指定的<paramref name="pFoundItem" />更新对应的集合中的元素。
        /// </summary>
        /// <param name="pOriObject">待合并数据的原始对象。</param>
        /// <param name="pFoundItem">待删除的对象实例。</param>
        private void CombineRemovedItem(ref object pOriObject, object pFoundItem)
        {
            if (pFoundItem != null)
            {
                //原始集合对象的类型。
                var listType = pOriObject.GetType();
                //当在集合中找到了待删除的元素实例时
                if (Type.IsArray)
                {
                    //当当前集合是数组类型时
                    //将原始对象转换为Array
                    var oldList = (Array) pOriObject;
                    //创建长度比原始对象长度-1的新的Array
                    var newList = Array.CreateInstance(GenericArgumentType.OriType, oldList.Length - 1);
                    //遍历原始集合中的所有元素
                    for (int i = 0, j = 0; i < oldList.Length; i++)
                        if (oldList.GetValue(i) != pFoundItem)
                        {
                            newList.SetValue(oldList.GetValue(i), j);
                            j++;
                        }
                    //使用新的集合对象替换原有的集合对象。
                    pOriObject = newList;
                }
                else if (listType.GetMethod(ConstValue.OPERATOR_REMOVE) != null)
                {
                    //非数组类型时，在当前集合类型上调用Remove方法
                    Type.InvokeMember(ConstValue.OPERATOR_REMOVE, BindingFlags.InvokeMethod, null, pOriObject,
                        new[] {pFoundItem}, CultureInfo.InvariantCulture);
                }
                else
                {
                    //TODO:未实现
                    throw new NotImplementedException();
                }
            }
            else
            {
                //TODO:未实现
                throw new NotImplementedException();
            }
        }

        #endregion Private Methods
    }
}