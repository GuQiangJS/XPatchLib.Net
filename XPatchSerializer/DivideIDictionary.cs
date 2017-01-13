using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    internal class DivideIDictionary : DivideBase
    {
        #region Internal Constructors

        /// <summary>
        /// 使用指定的类型初始化 <see cref="XPatchLib.DivideIEnumerable" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <exception cref="PrimaryKeyException">
        /// 默认在字符串与System.DateTime 之间转换时，转换时应保留时区信息。 
        /// </exception>
        /// <exception cref="ArgumentException">
        /// 待处理的类型不是字典类型时。 
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// 当 <paramref name="pType" /> 上无法获取元素类型时。 
        /// </exception>
        internal DivideIDictionary(TypeExtend pType)
            : this(pType, XmlDateTimeSerializationMode.RoundtripKind) { }

        /// <summary>
        /// 使用指定的类型及指定是否序列化默认值初始化 <see cref="XPatchLib.DivideIEnumerable" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <param name="pSerializeDefalutValue">
        /// 指定是否序列化默认值。 
        /// </param>
        /// <remarks>
        /// <para> 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 </para>
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// 待处理的类型不是字典类型时。 
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// 当 <paramref name="pType" /> 上无法获取元素类型时。 
        /// </exception>
        internal DivideIDictionary(TypeExtend pType, Boolean pSerializeDefalutValue)
            : this(pType, XmlDateTimeSerializationMode.RoundtripKind, pSerializeDefalutValue) { }

        /// <summary>
        /// 使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.DivideIEnumerable" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <param name="pMode">
        /// 指定在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <exception cref="ArgumentException">
        /// 待处理的类型不是字典类型时。 
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// 当 <paramref name="pType" /> 上无法获取元素类型时。 
        /// </exception>
        internal DivideIDictionary(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : this(pType, pMode, false) { }

        /// <summary>
        /// 使用指定的类型、指定是否序列化默认值和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.DivideIEnumerable" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <param name="pMode">
        /// 指定在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <param name="pSerializeDefalutValue">
        /// 指定是否序列化默认值。 
        /// </param>
        /// <exception cref="ArgumentException">
        /// 待处理的类型不是字典类型时。 
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// 当 <paramref name="pType" /> 上无法获取元素类型时。 
        /// </exception>
        internal DivideIDictionary(TypeExtend pType, XmlDateTimeSerializationMode pMode, Boolean pSerializeDefalutValue)
            : base(pType, pMode, pSerializeDefalutValue)
        {
            if (!this.Type.IsIDictionary)
            {
                //TODO:
                throw new ArgumentException("类型需要是字典类型");
            }
            Type t = null;
            if (ReflectionUtils.TryGetIEnumerableGenericArgument(pType.OriType, out t))
            {
                GenericArgumentType = TypeExtendContainer.GetTypeExtend(t);
            }
            else
            {
                //传入参数 pType 无法解析内部元素对象。
                throw new ArgumentOutOfRangeException(pType.OriType.FullName);
            }
        }

        #endregion Internal Constructors

        #region Protected Properties

        /// <summary>
        /// 集合类型中元素的类型。 
        /// </summary>
        protected TypeExtend GenericArgumentType { get; private set; }

        #endregion Protected Properties

        #region Internal Methods

        internal XElement Divide(string pName, Object pOriObject, Object pRevObject)
        {
            XElement result = null;

            IEnumerable pOriItems = pOriObject as IEnumerable;
            IEnumerable pRevItems = pRevObject as IEnumerable;

            if (pRevItems == null && pOriItems != null)
            {
                //当更新后对象为Null时设置为SetNull
                result = new XElement(pName);
                result.AppendActionAttribute(Action.SetNull);
            }
            else
            {
                IEnumerable<KeyValuesObject> oriKey = Translate(pOriItems);
                IEnumerable<KeyValuesObject> revKey = Translate(pRevItems);

                //顺序处理集合的删除、编辑、添加操作（顺序不能错）
                DivideItems(oriKey, revKey, Action.Remove, pName, ref result);
                DivideItems(oriKey, revKey, Action.Edit, pName, ref result);
                DivideItems(oriKey, revKey, Action.Add, pName, ref result);
            }

            return result;
        }

        #endregion Internal Methods

        #region Private Methods

        private static IEnumerable<KeyValuesObject> Translate(IEnumerable pValue)
        {
            if (pValue != null)
            {
                Queue<KeyValuesObject> result = new Queue<KeyValuesObject>();

                IEnumerator enumerator = pValue.GetEnumerator();
                if (enumerator != null)
                {
                    while (enumerator.MoveNext())
                    {
                        Object key = enumerator.Current.GetType().GetProperty(ConstValue.KEY).GetValue(enumerator.Current, null);

                        result.Enqueue(new KeyValuesObject(enumerator.Current, key));
                    }
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 尝试比较原始集合和更新后的集合，找到被添加的元素集合。 
        /// </summary>
        /// <param name="pOriItems">
        /// 原始集合。 
        /// </param>
        /// <param name="pRevItems">
        /// 更新后的集合。 
        /// </param>
        /// <param name="pFoundItems">
        /// 找到的被添加的元素集合。 
        /// </param>
        /// <returns>
        /// 当找到一个或多个被添加的元素时，返回 true 否则 返回 false 。 
        /// </returns>
        private static Boolean TryGetAddedItems(IEnumerable<KeyValuesObject> pOriItems, IEnumerable<KeyValuesObject> pRevItems, out IEnumerable<KeyValuesObject> pFoundItems)
        {
            //查找存在于更新后的集合中但是不存在于原始集合中的元素。
            if (pOriItems == null)
            {
                pFoundItems = pRevItems;
            }
            else if (pRevItems == null)
            {
                pFoundItems = null;
            }
            else
            {
                pFoundItems = pRevItems.Except(pOriItems, new KeyValuesObjectEqualityComparer());
            }

            return (pFoundItems != null);
        }

        /// <summary>
        /// 尝试比较原始集合和更新后的集合，找到可能被修改的元素集合。 (交集） 
        /// </summary>
        /// <param name="pOriItems">
        /// 原始集合。 
        /// </param>
        /// <param name="pRevItems">
        /// 更新后的集合。 
        /// </param>
        /// <param name="pFoundItems">
        /// 找到的被修改的元素集合。 
        /// </param>
        /// <returns>
        /// 当找到一个或多个被修改的元素时，返回 true 否则 返回 false 。 
        /// </returns>
        /// <remarks>
        /// 返回的集合是即存在于原始集合又存在于更新后集合的对象。 
        /// </remarks>
        private static Boolean TryGetEditedItems(IEnumerable<KeyValuesObject> pOriItems, IEnumerable<KeyValuesObject> pRevItems, out IEnumerable<KeyValuesObject> pFoundItems)
        {
            pFoundItems = null;
            if (pOriItems != null && pRevItems != null)
            {
                pFoundItems = pRevItems.Intersect(pOriItems, new KeyValuesObjectEqualityComparer());
            }

            return (pFoundItems != null);
        }

        /// <summary>
        /// 尝试比较原始集合和更新后的集合，找到被删除的元素集合。 
        /// </summary>
        /// <param name="pOriItems">
        /// 原始集合。 
        /// </param>
        /// <param name="pRevItems">
        /// 更新后的集合。 
        /// </param>
        /// <param name="pFoundItems">
        /// 找到的被删除的元素集合。 
        /// </param>
        /// <returns>
        /// 当找到一个或多个被删除的元素时，返回 true 否则 返回 false 。 
        /// </returns>
        private static Boolean TryGetRemovedItems(IEnumerable<KeyValuesObject> pOriItems, IEnumerable<KeyValuesObject> pRevItems, out IEnumerable<KeyValuesObject> pFoundItems)
        {
            //查找存在于原始集合中但是不存在于更新后的集合中的元素。
            //pFoundItems = pRevItems.Except(pOriItems, GenericArgumentType, GenericArgumentPrimaryKeys);
            if (pRevItems == null)
            {
                pFoundItems = pOriItems;
            }
            else if (pOriItems == null)
            {
                pFoundItems = null;
            }
            else
            {
                pFoundItems = pOriItems.Except(pRevItems, new KeyValuesObjectEqualityComparer());
            }
            return (pFoundItems != null);
        }

        /// <summary>
        /// 按照传入的操作方式产生集合类型增量内容。 
        /// </summary>
        /// <param name="pOriItems">
        /// 原始集合。 
        /// </param>
        /// <param name="pRevItems">
        /// 更新后的集合。 
        /// </param>
        /// <param name="pAction">
        /// 操作方式。 
        /// </param>
        /// <param name="pParentElementName">
        /// 父级增量内容名称。 
        /// </param>
        /// <param name="pParentElement">
        /// 父级增量内容对象。 
        /// </param>
        private void DivideItems(IEnumerable<KeyValuesObject> pOriItems, IEnumerable<KeyValuesObject> pRevItems, Action pAction, string pParentElementName, ref XElement pParentElement)
        {
            Queue<XElement> result = new Queue<XElement>();

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

            //找到待处理的元素集合时
            if (found)
            {
                IEnumerator<KeyValuesObject> items = pFoundItems.GetEnumerator();
                if (items != null)
                {
                    //开始遍历待处理的元素集合中的所有元素

                    //元素的类型未知，所以再次创建DivideCore实例，由此实例创建元素的增量结果。（递归方式）
                    DivideKeyValuePair ser = new DivideKeyValuePair(GenericArgumentType, this.Mode, this.SerializeDefaultValue);
                    while (items.MoveNext())
                    {
                        //当前被处理的元素的增量内容数据对象
                        XElement ele = null;
                        if (pAction == Action.Add)
                        {
                            //当前元素是新增操作时
                            //再次调用DivideCore.Divide的方法，传入空的原始对象，生成新增的增量节点。
                            KeyValuesObject obj = pRevItems.FirstOrDefault(x => x.Equals(items.Current));
                            ele = ser.Divide(GenericArgumentType.TypeFriendlyName, null, obj.OriValue);
                        }
                        else if (pAction == Action.Remove)
                        {
                            //当前元素是删除操作时
                            //再次调用DivideCore.Divide的方法，传入空的更新后对象，生成删除的增量节点。
                            KeyValuesObject obj = pOriItems.FirstOrDefault(x => x.Equals(items.Current));
                            ele = ser.Divide(GenericArgumentType.TypeFriendlyName, obj.OriValue, null);
                            //由于生成的增量对象会标记Attribute Action为SetNull，而实际需要的是Remove，所以先清除所有的Attribute。
                            ele.RemoveAttributes();
                        }
                        else if (pAction == Action.Edit)
                        {
                            //将此元素与当前正在遍历的元素作为参数调用序列化，看是否产生增量内容内容（如果没有产生增量内容内容则说明两个对象需要序列化的内容完全一样）
                            KeyValuesObject oldObj = pOriItems.FirstOrDefault(x => x.Equals(items.Current));
                            KeyValuesObject newObj = pRevItems.FirstOrDefault(x => x.Equals(items.Current));
                            ele = ser.Divide(GenericArgumentType.TypeFriendlyName, oldObj.OriValue, newObj.OriValue);
                        }
                        //当当前遍历的元素被产生了增量内容后，将其加入至增量内容序列中。等待整批加入父增量内容中。
                        if (ele != null)
                        {
                            ele.AppendActionAttribute(pAction);
                            result.Enqueue(ele);
                        }
                    }
                }
            }

            //当本次处理的增量内容序列中有元素时，将所有元素加入至父增量内容中。形成集合对象的增量结果。
            if (result != null && result.Count > 0)
            {
                if (pParentElement == null)
                {
                    pParentElement = new XElement(pParentElementName);
                }
                pParentElement.Add(result);
            }
        }

        #endregion Private Methods
    }
}