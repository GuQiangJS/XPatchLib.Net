using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    /// <summary>
    /// 集合类型增量内容合并类。 
    /// </summary>
    internal class CombineIEnumerable : CombineBase
    {
        #region Internal Constructors

        /// <summary>
        /// 使用指定的类型初始化 <see cref="XPatchLib.CombineIEnumerable" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <remarks>
        /// 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 
        /// </remarks>
        internal CombineIEnumerable(TypeExtend pType)
            : this(pType, XmlDateTimeSerializationMode.RoundtripKind) { }

        /// <summary>
        /// 使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.CombineIEnumerable" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <param name="pMode">
        /// 指定在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// 当无法取得 <paramref name="pType" /> 定义的集合内部的元素类型时。 
        /// </exception>
        /// <exception cref="PrimaryKeyException">
        /// 当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。 
        /// </exception>
        /// <exception cref="AttributeMissException">
        /// 当 <paramref name="pType" /> 定义的集合内部的元素类型不是基础类型，并且没有被标记
        /// <see cref="XPatchLib.PrimaryKeyAttribute" /> 时。
        /// </exception>
        internal CombineIEnumerable(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : base(pType, pMode)
        {
            Type t = null;
            if (ReflectionUtils.TryGetIEnumerableGenericArgument(pType.OriType, out t))
            {
                GenericArgumentType = TypeExtendContainer.GetTypeExtend(t);

                GenericArgumentTypePrimaryKeyAttribute = GenericArgumentType.PrimaryKeyAttribute;

                if (GenericArgumentTypePrimaryKeyAttribute == null && !this.GenericArgumentType.IsBasicType)
                {
                    //内部元素对象没有被标记 PrimaryKeyAttribute 时
                    throw new AttributeMissException(GenericArgumentType.OriType, "PrimaryKeyAttribute");
                }
            }
            else
            {
                //TODO：传入参数 pType 无法解析内部元素对象。
                throw new ArgumentOutOfRangeException(pType.OriType.FullName);
            }
        }

        #endregion Internal Constructors

        #region Protected Properties

        /// <summary>
        /// 获取集合中的元素类型。 
        /// </summary>
        protected TypeExtend GenericArgumentType { get; private set; }

        protected PrimaryKeyAttribute GenericArgumentTypePrimaryKeyAttribute { get; private set; }

        #endregion Protected Properties

        #region Internal Methods

        /// <summary>
        /// 将增量内容对象合并至原始对象上。 
        /// </summary>
        /// <param name="pOriObject">
        /// 原始对象集合。 
        /// </param>
        /// <param name="pElement">
        /// 增量节点。 
        /// </param>
        /// <returns>
        /// 返回数据合并后的对象。 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 当 <paramref name="pElement" /> is null 时。 
        /// </exception>
        internal Object Combine(Object pOriObject, XElement pElement)
        {
            Guard.ArgumentNotNull(pElement, "pElement");

            IEnumerable<KeyValuesObject> kvs = KeyValuesObject.Translate(pOriObject as IEnumerable);

            //元素类型是否为基础类型。
            if (this.GenericArgumentType.IsBasicType)
            {
                //如果是基础类型时，逐个遍历增量节点，将各个节点附加至原始对象上。
                if (pElement.HasElements)
                {
                    //当元素类型是基础类型，但是增量内容下还有子节点时，说明增量内容当前的节点还不是基础类型数据的增量内容，还需要继续递归操作。
                    foreach (XElement childEle in pElement.Elements())
                    {
                        pOriObject = Combine(pOriObject, childEle);
                    }
                }
                else
                {
                    //当元素类型是基础类型，并且没有增量内容内容没有子节点时，认为该增量内容内容即为一条基础类型的增量内容，直接调用合并核心方法，进行合并。
                    CombineCore(ref pOriObject, kvs, pElement);
                }
            }
            else
            {
                //如果不是基础类型，也不是集合类型时，将按照复杂类型进行操作。
                if (this.GenericArgumentType.TypeFriendlyName.Equals(pElement.Name.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    //如果当前元素类型的名称与增量内容的节点名称相符，那么就应该使用当前的增量节向当前的原始数据进行合并。
                    CombineCore(ref pOriObject, kvs, pElement);
                }
                else
                {
                    //如果当前元素类型的名称与增量内容的节点名称不相符，则认为当前节点不是集合的元素增量内容，遍历增量节点的子节点进行合并。
                    foreach (XElement childEle in pElement.Elements())
                    {
                        CombineCore(ref pOriObject, kvs, childEle);
                    }
                }
            }
            return pOriObject;
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// 合并新增类型动作的增量内容。 
        /// </summary>
        /// <param name="pOriObject">
        /// 待合并数据的原始对象。 
        /// </param>
        /// <param name="pElement">
        /// 待合并的增量内容数据。 
        /// </param>
        private void CombineAddedItem(ref Object pOriObject, XElement pElement)
        {
            //当原始对象为null时，先创建一个实例。
            if (pOriObject == null)
            {
                pOriObject = this.Type.CreateInstance();
            }

            //原始集合对象的类型。
            Type listType = pOriObject.GetType();
            //创建集合元素实例,根据增量内容内容向集合元素实例赋值。
            Object obj = new CombineCore(GenericArgumentType).Combine(null, pElement);

            //将增量内容数据合并至新增的元素实例。
            if (this.GenericArgumentType.IsBasicType)
            {
                obj = new CombineBasic(GenericArgumentType).Combine(pElement);
            }
            else
            {
                obj = GenericArgumentType.CreateInstance();
                //根据增量内容内容向集合元素实例赋值。
                obj = new CombineCore(GenericArgumentType).Combine(obj, pElement);
            }

            if (this.Type.IsArray)
            {
                //当当前集合是数组类型时
                //将原始对象转换为Array
                Array oldList = (Array)pOriObject;
                //创建长度比原始对象长度+1的新的Array
                Array newList = Array.CreateInstance(GenericArgumentType.OriType, oldList.Length + 1);
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
                listType.InvokeMember(ConstValue.OPERATOR_ADD, BindingFlags.InvokeMethod, null, pOriObject, new object[] { obj }, CultureInfo.InvariantCulture);
            }
            else
            {
                //TODO:未实现
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 增量内容数据合并核心方法。 
        /// </summary>
        /// <param name="pOriObject">
        /// 待合并数据的原始对象。 
        /// </param>
        /// <param name="pElement">
        /// 待合并的增量内容数据。 
        /// </param>
        private void CombineCore(ref Object pOriObject, IEnumerable<KeyValuesObject> pOriEnumerable, XElement pElement)
        {
            //当增量内容数据中不含有名为Action的Attribute时，默认该增量内容记录的是Edit类型的操作。
            Action action;
            if (!pElement.TryGetActionAttribute(out action))
            {
                action = Action.Edit;
            }

            switch (action)
            {
                case Action.Add:
                    CombineAddedItem(ref pOriObject, pElement);
                    break;

                case Action.Edit:
                    CombineEditItem(ref pOriObject, pOriEnumerable, pElement);
                    break;

                case Action.Remove:
                    CombineRemovedItem(ref pOriObject, pOriEnumerable, pElement);
                    break;
            }
        }

        /// <summary>
        /// 合并编辑类型动作的增量内容。 
        /// </summary>
        /// <param name="pOriObject">
        /// 待合并数据的原始对象。 
        /// </param>
        /// <param name="pElement">
        /// 待合并的增量内容数据。 
        /// </param>
        private void CombineEditItem(ref Object pOriObject, IEnumerable<KeyValuesObject> pOriEnumerable, XElement pElement)
        {
            //当前编辑的对象在原始集合对象中以零开始的索引。
            int index = -1;
            if (this.GenericArgumentType.IsBasicType && pOriEnumerable != null)
            {
                //当集合的元素类型是基础类型时，按照基础类型获取所有的方式查找索引。
                //基础类型不存在Edit动作，所以看上去永远不会执行到这段
                //index = existsList.GetIndex(GenericArgumentType, pElement.Value);
                index = pOriEnumerable.FindIndex(x => x.Equals(pElement.Value));
            }
            else
            {
                //当集合的元素类型不是基础类型时，先获取当前集合元素类型上标记的PrimaryKeyAttribute，然后根据PrimaryKeyAttribute查找索引。
                int[] primaryKeys;
                int[] primaryKeyValues;
                pElement.TryGetPrimaryKeyValues(this.GenericArgumentType, out primaryKeys, out primaryKeyValues);

                index = pOriEnumerable.FindIndex(x => x.EqualsByKeys(primaryKeys, primaryKeyValues));
            }
            CombineEditItem(ref pOriObject, index, pElement);
            //}
        }

        private void CombineEditItem(ref Object pOriObject, int pIndex, XElement pElement)
        {
            if (pIndex >= 0)
            {
                //原始集合对象的类型。
                Type listType = pOriObject.GetType();
                //当在原始集合中找到了索引时
                if (this.Type.IsArray)
                {
                    //当当前集合是数组类型时
                    Object obj = ((Array)pOriObject).GetValue(pIndex);

                    //创建集合元素实例,根据增量内容内容向集合元素实例赋值。
                    obj = new CombineCore(GenericArgumentType).Combine(obj, pElement);

                    ((Array)pOriObject).SetValue(obj, pIndex);
                }
                else if (listType.GetMethod(ConstValue.OPERATOR_SET) != null)
                {
                    //当当前集合不是数组类型时
                    Object obj = listType.InvokeMember(ConstValue.OPERATOR_GET, BindingFlags.InvokeMethod, null, pOriObject, new object[] { pIndex }, CultureInfo.InvariantCulture);

                    //创建集合元素实例,根据增量内容内容向集合元素实例赋值。
                    obj = new CombineCore(GenericArgumentType).Combine(obj, pElement);

                    listType.InvokeMember(ConstValue.OPERATOR_SET, BindingFlags.InvokeMethod, null, pOriObject, new object[] { pIndex, obj }, CultureInfo.InvariantCulture);
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

        /// <summary>
        /// 合并删除类型动作的增量内容。 
        /// </summary>
        /// <param name="pOriObject">
        /// 待合并数据的原始对象。 
        /// </param>
        /// <param name="pElement">
        /// 待合并的增量内容数据。 
        /// </param>
        private void CombineRemovedItem(ref Object pOriObject, IEnumerable<KeyValuesObject> pOriEnumerable, XElement pElement)
        {
            //创建集合元素实例,根据增量内容内容向集合元素实例赋值。
            Object obj = new CombineCore(GenericArgumentType).Combine(null, pElement);

            //在集合中找到的待删除的元素实例。
            Object foundItem = null;

            if (pOriEnumerable != null)
            {
                //当当前集合是数组类型时
                if (this.GenericArgumentType.IsBasicType)
                {
                    //当时基础类型时，按照基础类型获取所有的方式查找索引。
                    foundItem = pOriEnumerable.FirstOrDefault(x => x.Equals(pElement.Value)).OriValue;
                }
                else
                {
                    int[] primaryKeys;
                    int[] primaryKeyValues;
                    pElement.TryGetPrimaryKeyValues(this.GenericArgumentType, out primaryKeys, out primaryKeyValues);
                    foundItem = pOriEnumerable.FirstOrDefault(x => x.EqualsByKeys(primaryKeys, primaryKeyValues)).OriValue;
                }
                CombineRemovedItem(ref pOriObject, foundItem);
            }
            else
            {
                //TODO:未实现
                throw new NotImplementedException();
            }
        }

        private void CombineRemovedItem(ref Object pOriObject, Object pFoundItem)
        {
            if (pFoundItem != null)
            {
                //原始集合对象的类型。
                Type listType = pOriObject.GetType();
                //当在集合中找到了待删除的元素实例时
                if (this.Type.IsArray)
                {
                    //当当前集合是数组类型时
                    //将原始对象转换为Array
                    Array oldList = (Array)pOriObject;
                    //创建长度比原始对象长度-1的新的Array
                    Array newList = Array.CreateInstance(GenericArgumentType.OriType, oldList.Length - 1);
                    //遍历原始集合中的所有元素
                    for (int i = 0, j = 0; i < oldList.Length; i++)
                    {
                        //当元素与被找到的待删除的元素不同时，将其赋值给新的Array
                        if (oldList.GetValue(i) != pFoundItem)
                        {
                            newList.SetValue(oldList.GetValue(i), j);
                            j++;
                        }
                    }
                    //使用新的集合对象替换原有的集合对象。
                    pOriObject = newList;
                }
                else if (listType.GetMethod(ConstValue.OPERATOR_REMOVE) != null)
                {
                    //非数组类型时，在当前集合类型上调用Remove方法
                    listType.InvokeMember(ConstValue.OPERATOR_REMOVE, BindingFlags.InvokeMethod, null, pOriObject, new object[] { pFoundItem }, CultureInfo.InvariantCulture);
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