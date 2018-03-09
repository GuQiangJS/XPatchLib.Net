// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
#if (NET_35_UP || NETSTANDARD)
using System.Linq;

#endif

namespace XPatchLib
{
    internal class ConverterIDictionary : ConverterBase
    {
        internal ConverterIDictionary(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
            InitType(pType);
        }

        internal ConverterIDictionary(TypeExtend pType) : base(pType)
        {
            InitType(pType);
        }

        private void InitType(TypeExtend pType)
        {
            if (!Type.IsIDictionary)
                throw new ArgumentException("类型需要是字典类型");
            Type t;
            if (ReflectionUtils.TryGetIEnumerableGenericArgument(pType.OriType, pType.InterfaceTypes, out t))
                GenericArgumentType = TypeExtendContainer.GetTypeExtend(pType.Setting, t, null, pType);
            else
                throw new ArgumentOutOfRangeException(pType.OriType.FullName);
        }

        #region Divide

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

            KeyValuesObject[] oriKey = Translate(pOriItems, Writer.Setting);
            KeyValuesObject[] revKey = Translate(pRevItems, Writer.Setting);
            if (pAttach == null)
                pAttach = new DivideAttachment();
            //将当前节点加入附件中，如果遇到子节点被写入前，会首先根据队列先进先出写入附件中的节点的开始标记
            pAttach.ParentQuere.Enqueue(new ParentObject(pName, pOriObject, Type, GetType(pOriObject, pRevObject)));
            //顺序处理集合的删除、编辑、添加操作（顺序不能错）

            #region 处理删除

            bool removeItemsResult = DivideItems(oriKey, revKey, Action.Remove, pAttach);

            //只要有一个子节点写入成功，那么整个节点就是写入成功的
            result = removeItemsResult;

            #endregion 处理删除

            #region 处理编辑

            bool editItemsResult = DivideItems(oriKey, revKey, Action.Edit, pAttach);
            if (!result)
                result = editItemsResult;

            #endregion 处理编辑

            #region 处理新增

            bool addItemsResult = DivideItems(oriKey, revKey, Action.Add, pAttach);
            if (!result)
                result = addItemsResult;

            #endregion 处理新增

            return result;
        }

        private static KeyValuesObject[] Translate(IEnumerable pValue, ISerializeSetting pSetting)
        {
            if (pValue != null)
            {
                Queue<KeyValuesObject> result = new Queue<KeyValuesObject>();

                IEnumerator enumerator = pValue.GetEnumerator();
                if (enumerator != null)
                    while (enumerator.MoveNext())
                    {
                        Object key =
                            enumerator.Current.GetType().GetProperty(ConstValue.KEY).GetValue(enumerator.Current, null);

                        result.Enqueue(new KeyValuesObject(enumerator.Current, key, pSetting));
                    }

                return result.ToArray();
            }

            return null;
        }

        /// <summary>
        ///     尝试比较原始集合和更新后的集合，找到被添加的元素集合。
        /// </summary>
        /// <param name="pOriItems">原始集合。</param>
        /// <param name="pRevItems">更新后的集合。</param>
        /// <param name="pFoundItems">找到的被添加的元素集合。</param>
        /// <returns>
        ///     当找到一个或多个被添加的元素时，返回 true 否则 返回 false 。
        /// </returns>
        private static Boolean TryGetAddedItems(KeyValuesObject[] pOriItems,
            KeyValuesObject[] pRevItems, out IEnumerable<KeyValuesObject> pFoundItems)
        {
            //查找存在于更新后的集合中但是不存在于原始集合中的元素。
            if (pOriItems == null)
                pFoundItems = pRevItems;
            else if (pRevItems == null)
                pFoundItems = null;
            else
                pFoundItems = pRevItems.Except(pOriItems, new KeyValuesObjectEqualityComparer());

            return pFoundItems != null;
        }

        /// <summary>
        ///     尝试比较原始集合和更新后的集合，找到可能被修改的元素集合。 (交集）
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
        private static Boolean TryGetEditedItems(KeyValuesObject[] pOriItems,
            KeyValuesObject[] pRevItems, out IEnumerable<KeyValuesObject> pFoundItems)
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
        private static Boolean TryGetRemovedItems(KeyValuesObject[] pOriItems,
            KeyValuesObject[] pRevItems, out IEnumerable<KeyValuesObject> pFoundItems)
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

        /// <summary>
        ///     按照传入的操作方式产生集合类型增量内容。
        /// </summary>
        /// <param name="pOriItems">原始集合。</param>
        /// <param name="pRevItems">更新后的集合。</param>
        /// <param name="pAction">操作方式。</param>
        /// <param name="pAttach">The p attach.</param>
        private Boolean DivideItems(KeyValuesObject[] pOriItems, KeyValuesObject[] pRevItems,
            Action pAction, DivideAttachment pAttach)
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
                IEnumerator<KeyValuesObject> items = pFoundItems.GetEnumerator();

                //开始遍历待处理的元素集合中的所有元素

                //元素的类型未知，所以再次创建DivideCore实例，由此实例创建元素的增量结果。（递归方式）
                ConverterKeyValuePair ser = new ConverterKeyValuePair(Writer, GenericArgumentType);
                ser.Assign(this);
                while (items.MoveNext())
                {
                    pAttach.CurrentAction = pAction;
                    //当前被处理的元素的增量内容数据对象
                    if (pAction == Action.Add)
                    {
                        //当前元素是新增操作时
                        //再次调用DivideCore.Divide的方法，传入空的原始对象，生成新增的增量节点。
                        KeyValuesObject obj = pRevItems.FirstOrDefault(x => x.Equals(items.Current));
                        bool itemResult = ser.Divide(GenericArgumentType.TypeFriendlyName, null, obj.OriValue,
                            pAttach);
                        if (!result)
                            result = itemResult;
                    }
                    else if (pAction == Action.Remove)
                    {
                        //当前元素是删除操作时
                        //再次调用DivideCore.Divide的方法，传入空的更新后对象，生成删除的增量节点。
                        KeyValuesObject obj = pOriItems.FirstOrDefault(x => x.Equals(items.Current));

                        if (pAttach == null)
                            pAttach = new DivideAttachment();
                        //pAttach.ParentQuere.Enqueue(new ParentObject(GenericArgumentType.TypeFriendlyName, GenericArgumentType) { Action = Action.Remove });
                        bool itemResult = ser.Divide(GenericArgumentType.TypeFriendlyName, obj.OriValue, null,
                            pAttach);
                        if (!result)
                            result = itemResult;
                    }
                    else if (pAction == Action.Edit)
                    {
                        //将此元素与当前正在遍历的元素作为参数调用序列化，看是否产生增量内容内容（如果没有产生增量内容内容则说明两个对象需要序列化的内容完全一样）
                        KeyValuesObject oldObj = pOriItems.FirstOrDefault(x => x.Equals(items.Current));
                        KeyValuesObject newObj = pRevItems.FirstOrDefault(x => x.Equals(items.Current));
                        bool itemResult = ser.Divide(GenericArgumentType.TypeFriendlyName, oldObj.OriValue,
                            newObj.OriValue,
                            pAttach);
                        if (!result)
                            result = itemResult;
                    }
                }
            }

            return result;
        }

        #endregion Divide

        #region Combine

        /// <summary>
        ///     集合类型中元素的类型。
        /// </summary>
        protected TypeExtend GenericArgumentType { get; private set; }

        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;

                //pReader.MoveToElement();

                if (pReader.Name == GenericArgumentType.TypeFriendlyName && pReader.NodeType == NodeType.Element)
                    CombineCore(pReader, ref pOriObject, GenericArgumentType.TypeFriendlyName);
                pReader.Read();
            }

            return pOriObject;
        }


        /// <summary>
        ///     增量内容数据合并核心方法。
        /// </summary>
        /// <param name="pReader">Xml读取器。</param>
        /// <param name="pOriObject">待合并数据的原始对象。</param>
        /// <param name="pName">当前正在解析的节点名称。</param>
        private void CombineCore(ITextReader pReader, ref object pOriObject, string pName)
        {
            CombineAttribute attrs = AnlysisAttributes(pReader, pName);

            //生成增量内容实例
            object item = CombineInstanceContainer.GetCombineInstance(GenericArgumentType)
                .Combine(pReader, null, pName);

            //增量内容实例Key值
            object key = GenericArgumentType.GetMemberValue(item, ConstValue.KEY);

            //增量内容实例Value值
            object value = GenericArgumentType.GetMemberValue(item, ConstValue.VALUE);

            //当原始对象为null时，先创建一个实例。
            if (pOriObject == null)
                pOriObject = Type.CreateInstance();

            switch (attrs.Action)
            {
                case Action.Add:
                    Add(Type.IsConcurrentDictionary ? ConstValue.OPERATOR_TRY_ADD : ConstValue.OPERATOR_ADD, pOriObject,
                        key,
                        value);
                    break;

                case Action.Edit:
                    Update(Type.IsConcurrentDictionary ? ConstValue.OPERATOR_TRY_UPDATE : ConstValue.OPERATOR_SET,
                        pOriObject, key, value);
                    break;

                case Action.Remove:
                    Remove(pOriObject, key, value);
                    break;

                case Action.SetNull:
                    Update(ConstValue.OPERATOR_SET, pOriObject, key, null);
                    break;
            }
        }

        /// <summary>
        ///     执行增加操作。
        /// </summary>
        /// <param name="pOperatorName">操作名称。</param>
        /// <param name="pOriObject">待附加增量的对象实例。</param>
        /// <param name="pKey">Key值。</param>
        /// <param name="pValue">Value值。</param>
        private void Add(string pOperatorName, Object pOriObject, Object pKey, Object pValue)
        {
            Type.InvokeMember(pOperatorName, BindingFlags.InvokeMethod, pOriObject, new[] {pKey, pValue},
                CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     执行增加或更新操作。
        /// </summary>
        /// <param name="pOperatorName">操作名称。</param>
        /// <param name="pOriObject">待附加增量的对象实例。</param>
        /// <param name="pKey">Key值。</param>
        /// <param name="pValue">Value值。</param>
        private void Update(string pOperatorName, Object pOriObject, Object pKey, Object pValue)
        {
            if (Type.IsConcurrentDictionary)
                Type.InvokeMember(pOperatorName, BindingFlags.InvokeMethod, pOriObject,
                    new[] {pKey, pValue, ((IDictionary) pOriObject)[pKey]},
                    CultureInfo.InvariantCulture);
            else
                Type.InvokeMember(pOperatorName, BindingFlags.InvokeMethod, pOriObject, new[] {pKey, pValue},
                    CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     删除操作。
        /// </summary>
        /// <param name="pOriObject">待删除增量的对象实例。</param>
        /// <param name="pKey">待移除的Key值。</param>
        /// <param name="pValue">待移除的value值</param>
        private void Remove(Object pOriObject, Object pKey, Object pValue)
        {
            if (Type.IsConcurrentDictionary)
                Type.InvokeMember(ConstValue.OPERATOR_TRY_REMOVE, BindingFlags.InvokeMethod, pOriObject,
                    new[] {pKey, pValue},
                    CultureInfo.InvariantCulture);
            else
                Type.InvokeMember(ConstValue.OPERATOR_REMOVE, BindingFlags.InvokeMethod, pOriObject, new[] {pKey},
                    CultureInfo.InvariantCulture);
        }

        #endregion
    }
}