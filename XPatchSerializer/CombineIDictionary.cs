using System;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    internal class CombineIDictionary : CombineBase
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
        /// <exception cref="ArgumentException">
        /// 待处理的类型不是字典类型时。 
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// 当 <paramref name="pType" /> 上无法获取元素类型时。 
        /// </exception>
        internal CombineIDictionary(TypeExtend pType)
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
        /// <exception cref="ArgumentException">
        /// 待处理的类型不是字典类型时。 
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// 当 <paramref name="pType" /> 上无法获取元素类型时。 
        /// </exception>
        internal CombineIDictionary(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : base(pType, pMode)
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

        /// <summary>
        /// 集合类型中元素的类型。 
        /// </summary>
        protected TypeExtend GenericArgumentType { get; private set; }

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

            if (pElement.HasElements)
            {
                //当元素类型是基础类型，但是增量内容下还有子节点时，说明增量内容当前的节点还不是基础类型数据的增量内容，还需要继续递归操作。
                foreach (XElement childEle in pElement.Elements())
                {
                    pOriObject = CombineCore(ref pOriObject, childEle);
                }
            }
            return pOriObject;
        }

        private void CombineAddedItem(ref object pOriObject, XElement pElement)
        {
            //当原始对象为null时，先创建一个实例。
            if (pOriObject == null)
            {
                pOriObject = this.Type.CreateInstance();
            }

            object newItem = new CombineKeyValuePair(this.GenericArgumentType, this.Mode).Combine(null, pElement);

            object newKey = GenericArgumentType.GetMemberValue(newItem, ConstValue.KEY);
            object newValue = GenericArgumentType.GetMemberValue(newItem, ConstValue.VALUE);

            this.Type.InvokeMember(ConstValue.OPERATOR_ADD, BindingFlags.InvokeMethod, null, pOriObject, new object[] { newKey, newValue }, CultureInfo.InvariantCulture);
        }

        private object CombineCore(ref object pOriObject, XElement pElement)
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
                    CombineEditItem(ref pOriObject, pElement);
                    break;

                case Action.Remove:
                    CombineRemovedItem(ref pOriObject, pElement);
                    break;

                case Action.SetNull:
                    CombineSetNullItem(ref pOriObject, pElement);
                    break;
            }
            return pOriObject;
        }

        private void CombineEditItem(ref object pOriObject, XElement pElement)
        {
            object editItem = new CombineKeyValuePair(this.GenericArgumentType, this.Mode).Combine(null, pElement);

            object newKey = GenericArgumentType.GetMemberValue(editItem, ConstValue.KEY);
            object newValue = GenericArgumentType.GetMemberValue(editItem, ConstValue.VALUE);

            this.Type.InvokeMember(ConstValue.OPERATOR_SET, BindingFlags.InvokeMethod, null, pOriObject, new object[] { newKey, newValue }, CultureInfo.InvariantCulture);
        }

        private void CombineRemovedItem(ref object pOriObject, XElement pElement)
        {
            object editItem = new CombineKeyValuePair(this.GenericArgumentType, this.Mode).Combine(null, pElement);

            object newKey = GenericArgumentType.GetMemberValue(editItem, ConstValue.KEY);

            this.Type.InvokeMember(ConstValue.OPERATOR_REMOVE, BindingFlags.InvokeMethod, null, pOriObject, new object[] { newKey }, CultureInfo.InvariantCulture);
        }

        private void CombineSetNullItem(ref object pOriObject, XElement pElement)
        {
            object editItem = new CombineKeyValuePair(this.GenericArgumentType, this.Mode).Combine(null, pElement);

            object newKey = GenericArgumentType.GetMemberValue(editItem, ConstValue.KEY);

            this.Type.InvokeMember(ConstValue.OPERATOR_SET, BindingFlags.InvokeMethod, null, pOriObject, new object[] { newKey, null }, CultureInfo.InvariantCulture);
        }
    }
}