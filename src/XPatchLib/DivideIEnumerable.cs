// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
#if (NET_35_UP || NETSTANDARD)
using System.Linq;
#endif

namespace XPatchLib
{
    /// <summary>
    ///     集合类型增量内容产生类。
    /// </summary>
    /// <seealso cref="XPatchLib.DivideBase" />
    internal class DivideIEnumerable : DivideBase
    {
        private static readonly string PRIMARY_KEY_MISS = typeof(PrimaryKeyAttribute).Name;

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideIEnumerable" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 元素类型的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="pType" /> 上无法获取元素类型时。</exception>
        internal DivideIEnumerable(ITextWriter pWriter, TypeExtend pType)
            : base(pWriter, pType)
        {
            Type t = null;
            if (ReflectionUtils.TryGetIEnumerableGenericArgument(pType.OriType, out t))
            {
                GenericArgumentType = TypeExtendContainer.GetTypeExtend(t, Writer.IgnoreAttributeType, pType);

                GenericArgumentTypePrimaryKeyAttribute = GenericArgumentType.PrimaryKeyAttr;

                if (GenericArgumentTypePrimaryKeyAttribute == null && !GenericArgumentType.IsBasicType)
                    throw new AttributeMissException(GenericArgumentType.OriType, PRIMARY_KEY_MISS);
            }
            else
            {
                //传入参数 pType 无法解析内部元素对象。
                throw new ArgumentOutOfRangeException(pType.OriType.FullName);
            }
        }

        #endregion Internal Constructors

        protected override void WriteEnd(Object obj)
        {
            Writer.WriteEndArray();
#if NET || NETSTANDARD_2_0_UP
            Type.InvokeOnSerialized(obj, new System.Runtime.Serialization.StreamingContext());
#endif
        }

        /// <summary>
        ///     产生增量内容的实际方法。
        /// </summary>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>
        ///     返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。
        /// </returns>
        protected override bool DivideAction(string pName, object pOriObject, object pRevObject,
            DivideAttachment pAttach = null)
        {
            Boolean result;

            IEnumerable pOriItems = pOriObject as IEnumerable;
            IEnumerable pRevItems = pRevObject as IEnumerable;
            IEnumerable<KeyValuesObject> oriObjects = KeyValuesObject.Translate(pOriItems);
            IEnumerable<KeyValuesObject> revObjects = KeyValuesObject.Translate(pRevItems);

            if (pAttach == null)
                pAttach = new DivideAttachment();
            //将当前节点加入附件中，如果遇到子节点被写入前，会首先根据队列先进先出写入附件中的节点的开始标记
            pAttach.ParentQuere.Enqueue(new ParentObject(pName, pOriObject, Type));

            //顺序处理集合的删除、编辑、添加操作（顺序不能错）

            #region 处理删除

            bool removeItemsResult = DivideItems(oriObjects, revObjects, Action.Remove, pAttach);

            //只要有一个子节点写入成功，那么整个节点就是写入成功的
            result = removeItemsResult;

            #endregion 处理删除

            #region 处理编辑

            bool editItemsResult = DivideItems(oriObjects, revObjects, Action.Edit, pAttach);
            if (!result)
                result = editItemsResult;

            #endregion 处理编辑

            #region 处理新增

            bool addItemsResult = DivideItems(oriObjects, revObjects, Action.Add, pAttach);
            if (!result)
                result = addItemsResult;

            #endregion 处理新增

            if (result)
            {
                //Writer.WriteEndObject();
            }
            else if (pOriObject == null && pRevObject != null)
            {
                result = WriteParentElementStart(pAttach);
                if (!result)
                    pAttach.ParentQuere.Dequeue();
                return result;
            }

            if (pAttach != null) pAttach.CurrentAction = Action.Edit;

            return result;
        }

        #region Protected Properties

        /// <summary>
        ///     集合类型中元素的类型所标记的主键名称集合。
        /// </summary>
        protected string[] GenericArgumentPrimaryKeys
        {
            get
            {
                return GenericArgumentTypePrimaryKeyAttribute != null
                    ? GenericArgumentTypePrimaryKeyAttribute.GetPrimaryKeys()
                    : null;
            }
        }

        /// <summary>
        ///     集合类型中元素的类型。
        /// </summary>
        protected TypeExtend GenericArgumentType { get; private set; }

        /// <summary>
        ///     集合类型中元素的类型所标记的主键特性。
        /// </summary>
        protected PrimaryKeyAttribute GenericArgumentTypePrimaryKeyAttribute { get; private set; }

        #endregion Protected Properties

        #region Private Methods

        /// <summary>
        ///     尝试比较原始集合和更新后的集合，找到被添加的元素集合。
        /// </summary>
        /// <param name="pOriItems">原始集合。</param>
        /// <param name="pRevItems">更新后的集合。</param>
        /// <param name="pFoundItems">找到的被添加的元素集合。</param>
        /// <returns>
        ///     当找到一个或多个被添加的元素时，返回 true 否则 返回 false 。
        /// </returns>
        private static Boolean TryGetAddedItems(IEnumerable<KeyValuesObject> pOriItems,
            IEnumerable<KeyValuesObject> pRevItems, out IEnumerable<KeyValuesObject> pFoundItems)
        {
            //查找存在于更新后的集合中但是不存在于原始集合中的元素。
            //pFoundItems = pOriItems.Except(pRevItems, GenericArgumentType, GenericArgumentPrimaryKeys);
            if (pOriItems == null)
                pFoundItems = pRevItems;
            else if (pRevItems == null)
                pFoundItems = null;
            else
                pFoundItems = pRevItems.Except(pOriItems, new KeyValuesObjectEqualityComparer());

            return pFoundItems != null;
        }

        /// <summary>
        ///     尝试比较原始集合和更新后的集合，找到可能被修改的元素集合。
        /// </summary>
        /// <param name="pOriItems">原始集合。</param>
        /// <param name="pRevItems">更新后的集合。</param>
        /// <param name="pFoundItems">找到的被修改的元素集合。</param>
        /// <returns>
        ///     当找到一个或多个被修改的元素时，返回 true 否则 返回 false 。
        /// </returns>
        /// <remarks>
        ///     返回的集合是即存在于原始集合又存在于更新后集合的对象。
        /// </remarks>
        private static Boolean TryGetEditedItems(IEnumerable<KeyValuesObject> pOriItems,
            IEnumerable<KeyValuesObject> pRevItems, out IEnumerable<KeyValuesObject> pFoundItems)
        {
            pFoundItems = null;
            if (pOriItems != null && pRevItems != null)
                pFoundItems = pRevItems.Intersect(pOriItems, new KeyValuesObjectEqualityComparer());

            return pFoundItems != null;
        }

        /// <summary>
        ///     尝试比较原始集合和更新后的集合，找到被删除的元素集合。
        /// </summary>
        /// <param name="pOriItems">原始集合。</param>
        /// <param name="pRevItems">更新后的集合。</param>
        /// <param name="pFoundItems">找到的被删除的元素集合。</param>
        /// <returns>
        ///     当找到一个或多个被删除的元素时，返回 true 否则 返回 false 。
        /// </returns>
        private static Boolean TryGetRemovedItems(IEnumerable<KeyValuesObject> pOriItems,
            IEnumerable<KeyValuesObject> pRevItems, out IEnumerable<KeyValuesObject> pFoundItems)
        {
            //查找存在于原始集合中但是不存在于更新后的集合中的元素。
            //pFoundItems = pRevItems.Except(pOriItems, GenericArgumentType, GenericArgumentPrimaryKeys);
            if (pRevItems == null)
                pFoundItems = pOriItems;
            else if (pOriItems == null)
                pFoundItems = null;
            else
                pFoundItems = pOriItems.Except(pRevItems, new KeyValuesObjectEqualityComparer());
            return pFoundItems != null;
        }

        private Boolean DivideItems(IEnumerable<KeyValuesObject> pOriItems, IEnumerable<KeyValuesObject> pRevItems,
            Action pAction, DivideAttachment pAttach = null)
        {
            IEnumerable<KeyValuesObject> pFoundItems = null;

            //查找符合指定操作方式的元素。
            Boolean found = false;
            switch (pAction)
            {
                case Action.Add:
                    found = TryGetAddedItems(pOriItems, pRevItems, out pFoundItems);
                    break;

                case Action.Remove:
                    found = TryGetRemovedItems(pOriItems, pRevItems, out pFoundItems);
                    break;

                case Action.Edit:
                    found = TryGetEditedItems(pOriItems, pRevItems, out pFoundItems);
                    break;
            }

            Boolean result = false;

            //找到待处理的元素集合时
            if (found)
            {
                //IEnumerator<KeyValuesObject> items = pFoundItems.GetEnumerator();

                //开始遍历待处理的元素集合中的所有元素

                //元素的类型未知，所以再次创建DivideCore实例，由此实例创建元素的增量结果。（递归方式）
                DivideCore ser = new DivideCore(Writer, GenericArgumentType);
                foreach (var keyValuesObject in pFoundItems)
                {
                    bool itemResult = false;
                    pAttach.CurrentAction = pAction;
                    if (pAction == Action.Add)
                    {
                        //当前元素是新增操作时
                        //再次调用DivideCore.Divide的方法，传入空的原始对象，生成新增的增量节点。
                        itemResult = ser.Divide(GenericArgumentType.TypeFriendlyName, null,
                            keyValuesObject.OriValue, pAttach);
                        if (!result)
                            result = itemResult;
                    }
                    else if (pAction == Action.Remove)
                    {
                        //当前元素是删除操作时

                        WriteParentElementStart(pAttach);
                        Writer.WriteStartArrayItem(GenericArgumentType.TypeFriendlyName);
                        Writer.WriteActionAttribute(Action.Remove);

                        if (GenericArgumentType.IsBasicType)
                            Writer.WriteValue(keyValuesObject.OriValue.ToString());
                        else
                            WriteKeyAttributes(new DivideAttachment
                            {
                                CurrentObj = keyValuesObject.OriValue,
                                CurrentType = GenericArgumentType,
                                PrimaryKeys = GenericArgumentPrimaryKeys
                            });
                        result = itemResult = true;
                        Writer.WriteEndArrayItem();
                    }
                    else if (pAction == Action.Edit)
                    {
                        //当前元素是编辑操作时
                        //从原始集合中找到当前正在遍历的元素相同的元素
                        //pOriItems.GetEnumerator().TryGetItem(GenericArgumentType, items.Current, out oriItem, GenericArgumentPrimaryKeys);
                        KeyValuesObject oriItem = pOriItems.FirstOrDefault(x => x.Equals(keyValuesObject));

                        if (pAttach == null)
                            pAttach = new DivideAttachment();
                        pAttach.CurrentObj = oriItem.OriValue;
                        pAttach.CurrentType = GenericArgumentType;
                        pAttach.PrimaryKeys = GenericArgumentPrimaryKeys;

                        //将此元素与当前正在遍历的元素作为参数调用序列化，看是否产生增量内容内容（如果没有产生增量内容内容则说明两个对象需要序列化的内容完全一样）
                        itemResult = ser.Divide(GenericArgumentType.TypeFriendlyName, oriItem.OriValue,
                            keyValuesObject.OriValue, pAttach);

                        if (!result)
                            result = itemResult;
                    }
                    pAttach.CurrentObj = null;
                    pAttach.CurrentType = null;
                    pAttach.PrimaryKeys = new string[] {};
                }
            }
            return result;
        }

        #endregion Private Methods
    }
}