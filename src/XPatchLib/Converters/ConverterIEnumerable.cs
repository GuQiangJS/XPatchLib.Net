// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
#if NET || NETSTANDARD_2_0_UP
using System.Runtime.Serialization;
#endif
#if (NET_35_UP || NETSTANDARD)
using System.Linq;
#endif

namespace XPatchLib
{
    internal class ConverterIEnumerable : ConverterBase
    {
        private static readonly string PRIMARY_KEY_MISS = typeof(PrimaryKeyAttribute).Name;

        internal ConverterIEnumerable(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
            InitType(pType);
        }

        internal ConverterIEnumerable(TypeExtend pType) : base(pType)
        {
            InitType(pType);
        }

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
        ///     获取集合中的元素类型。
        /// </summary>
        protected TypeExtend GenericArgumentType { get; private set; }

        protected PrimaryKeyAttribute GenericArgumentTypePrimaryKeyAttribute { get; private set; }

        private void InitType(TypeExtend pType)
        {
            Type t;
            if (ReflectionUtils.TryGetIEnumerableGenericArgument(pType.OriType, pType.InterfaceTypes, out t))
            {
                GenericArgumentType = TypeExtendContainer.GetTypeExtend(pType.Setting, t, null, pType);

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


        protected override void WriteEnd(Object obj)
        {
            Writer.WriteEndArray();
#if NET || NETSTANDARD_2_0_UP
            if (Writer.Setting.EnableOnSerializedAttribute)
                Type.InvokeOnSerialized(obj, new StreamingContext());
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
            IEnumerable<KeyValuesObject> oriObjects = KeyValuesObject.Translate(pOriItems, Writer.Setting);
            IEnumerable<KeyValuesObject> revObjects = KeyValuesObject.Translate(pRevItems, Writer.Setting);

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

        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            var kvs = KeyValuesObject.Translate(pOriObject as IEnumerable, pReader.Setting);
            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;

                //pReader.MoveToElement();

                if (pReader.Name == GenericArgumentType.TypeFriendlyName &&
                    (pReader.NodeType == NodeType.Element || pReader.NodeType == NodeType.FullElement))
                    CombineCore(pReader, ref pOriObject, kvs, GenericArgumentType.TypeFriendlyName);

                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;
                //pReader.MoveToNextElement(GenericArgumentType.TypeFriendlyName, pName);
                pReader.Read();
            }

            return pOriObject;
        }

        #region Divide

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
                ConverterCore ser = new ConverterCore(Writer, GenericArgumentType);
                ser.Assign(this);
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
                    pAttach.PrimaryKeys = new string[] { };
                }
            }

            return result;
        }

        #endregion


        #region Combine

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
            else if (listType.GetMethod(GetAddOperator()) != null)
            {
                //非数组类型时，在当前集合类型上调用Add方法
                Type.InvokeMember(GetAddOperator(), BindingFlags.InvokeMethod, pOriObject, new object[] {obj},
                    CultureInfo.InvariantCulture);
            }
            else
            {
                //TODO:未实现
                throw new NotImplementedException();
            }
        }

        private string GetAddOperator()
        {
            if (Type.IsConcurrentQueue || Type.IsQueue) return ConstValue.OPERATOR_ENQUEUE;
            if (Type.IsConcurrentStack || Type.IsStack) return ConstValue.OPERATOR_PUSH;
            return ConstValue.OPERATOR_ADD;
        }

        /// <summary>
        ///     增量内容数据合并核心方法。
        /// </summary>
        /// <param name="pReader">Xml读取器。</param>
        /// <param name="pOriObject">待合并数据的原始对象。</param>
        /// <param name="pOriEnumerable">当前正在处理的集合对象。</param>
        /// <param name="pName">当前正在解析的节点名称。</param>
        private void CombineCore(ITextReader pReader, ref object pOriObject,
            IEnumerable<KeyValuesObject> pOriEnumerable,
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
                    while (pName.Equals(pReader.Name) && pReader.NodeType != NodeType.EndElement &&
                           pReader.NodeType != NodeType.FullElement)
                        pReader.Read();
                    //pReader.MoveToCurrentElementEnd(pName);
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
                    o = pOriEnumerable.FirstOrDefault(x => x.Equals(pReader.GetValue()));
                else
                    o = pOriEnumerable.FirstOrDefault(x => x.EqualsByCombineAttr(pAttribute));
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
                    var value = new KeyValuesObject(ConverterBasic.CombineAction(GenericArgumentType.TypeCode,
                        GenericArgumentType.IsGuid, pReader.Setting.Mode, pReader.GetValue()), pReader.Setting);
                    //当时基础类型时，按照基础类型获取所有的方式查找索引。

                    o = pOriEnumerable.FirstOrDefault(x => x.Equals(value));
                    if (o != null)
                        foundItem = o.OriValue;
                }
                else
                {
                    o = pOriEnumerable.FirstOrDefault(x => x.EqualsByCombineAttr(pAttribute));
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
                        if (!Equals(oldList.GetValue(i), pFoundItem))
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
                    Type.InvokeMember(ConstValue.OPERATOR_REMOVE, BindingFlags.InvokeMethod, pOriObject,
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

        #endregion Combine
    }
}