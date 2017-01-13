using System;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    internal class CombineKeyValuePair : CombineBase
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
        internal CombineKeyValuePair(TypeExtend pType)
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
        internal CombineKeyValuePair(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : base(pType, pMode)
        {
            //TODO:未判断是否为KeyValuePair类型
        }

        #endregion Internal Constructors

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
        /// <exception cref="ArgumentException">
        /// 原始值的Key值与更新后的值的Key值不同时。 
        /// </exception>
        internal Object Combine(Object pOriObject, XElement pElement)
        {
            Guard.ArgumentNotNull(pElement, "pElement");

            //获取KeyValuePair类型对象的Key值与Value值的类型
            TypeExtend keyType = TypeExtendContainer.GetTypeExtend(this.Type.KeyArgumentType);
            TypeExtend valueType = TypeExtendContainer.GetTypeExtend(this.Type.ValueArgumentType);

            //获取原始值的Key值和Value值
            object oriKeyObj = null;
            object oriValueObj = null;

            object revKey = (pElement.Element(ConstValue.KEY) != null) ? new CombineCore(keyType, this.Mode).Combine(null, pElement.Element(ConstValue.KEY)) : null;
            object revValue = (pElement.Element(ConstValue.VALUE) != null) ? new CombineCore(valueType, this.Mode).Combine(oriValueObj, pElement.Element(ConstValue.VALUE)) : null;

            if (pOriObject == null)
            {
                //当原始对象为null时，先创建一个实例。并且赋予pElement转换的Key值和Value值
                pOriObject = this.Type.CreateInstance();
                pOriObject = this.Type.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[] { ((revKey != null) ? revKey : oriKeyObj), revValue }, CultureInfo.InvariantCulture);
            }
            else
            {
                //当原始值不为空时，先获取原始值中的Key值和Value值
                oriKeyObj = this.Type.GetMemberValue(pOriObject, ConstValue.KEY);
                oriValueObj = this.Type.GetMemberValue(pOriObject, ConstValue.VALUE);

                if (!oriKeyObj.Equals(revKey))
                {
                    //原始值的Key值与更新后的值的Key值不同时，抛出异常
                    //TODO:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "原始Key值:{0},更新后的Key值:{1}.", oriKeyObj, revKey));
                }

                Action action;
                if (pElement.TryGetActionAttribute(out action) && action == Action.SetNull)
                {
                    //如果在增量数据上找到了Action的Attribute，并且Action是SetNull时，表示增量对象的结果是null。
                    pOriObject = this.Type.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[] { ((revKey != null) ? revKey : oriKeyObj), revValue }, CultureInfo.InvariantCulture);
                }
                else if (action == Action.Remove)
                {
                    pOriObject = null;
                }
                else if (action == Action.Edit)
                {
                    pOriObject = this.Type.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[] { ((revKey != null) ? revKey : oriKeyObj), revValue }, CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return pOriObject;
        }

        #endregion Internal Methods
    }
}