using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    internal class DivideKeyValuePair : DivideBase
    {
        #region Internal Constructors

        /// <summary>
        /// 使用指定的类型初始化 <see cref="XPatchLib.DivideKeyValuePair" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <remarks>
        /// 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 
        /// </remarks>
        internal DivideKeyValuePair(TypeExtend pType)
            : this(pType, XmlDateTimeSerializationMode.RoundtripKind) { }

        /// <summary>
        /// 使用指定的类型及指定是否序列化默认值初始化 <see cref="XPatchLib.DivideKeyValuePair" /> 类的新实例。 
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
        internal DivideKeyValuePair(TypeExtend pType, Boolean pSerializeDefalutValue)
            : this(pType, XmlDateTimeSerializationMode.RoundtripKind, pSerializeDefalutValue) { }

        /// <summary>
        /// 使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.DivideKeyValuePair" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <param name="pMode">
        /// 指定在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        internal DivideKeyValuePair(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : this(pType, pMode, false) { }

        /// <summary>
        /// 使用指定的类型、指定是否序列化默认值和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.DivideKeyValuePair" /> 类的新实例。
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
        internal DivideKeyValuePair(TypeExtend pType, XmlDateTimeSerializationMode pMode, Boolean pSerializeDefalutValue)
            : base(pType, pMode, pSerializeDefalutValue)
        {
            //TODO:未判断是否为KeyValuePair类型
        }

        #endregion Internal Constructors

        #region Internal Methods

        /// <summary>
        /// 产生集合类型增量内容。 
        /// </summary>
        /// <param name="pName">
        /// 增量内容对象的名称。 
        /// </param>
        /// <param name="pOriObject">
        /// 原始对象。 
        /// </param>
        /// <param name="pRevObject">
        /// 更新后的对象。 
        /// </param>
        /// <returns>
        /// 返回 <paramref name="pOriObject" /> 与 <paramref name="pRevObject" /> 比较后需要记录的内容的
        /// <see cref="System.Xml.Linq.XElement" /> 的表现形式。
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <para> 当原始对象及更新后的对象的Key值均为Null时。 </para>
        /// <para> 或者 </para>
        /// <para> 当原始值不为Null同时更新后的值不为Null时，原始值的Key值就应该与更新后的值的Key值不相同时。 </para>
        /// </exception>
        internal XElement Divide(string pName, Object pOriObject, Object pRevObject)
        {
            Guard.ArgumentNotNullOrEmpty(pName, "pName");

            XElement result = null;

            //获取KeyValuePair类型对象的Key值与Value值的类型
            TypeExtend keyType = TypeExtendContainer.GetTypeExtend(this.Type.KeyArgumentType);
            TypeExtend valueType = TypeExtendContainer.GetTypeExtend(this.Type.KeyArgumentType);

            //分别获取原始值与修改后的值的Key值和Value值
            object oriKeyObj = null;
            object oriValueObj = null;
            object revKeyObj = null;
            object revValueObj = null;
            if (pOriObject != null)
            {
                oriKeyObj = pOriObject.GetType().GetProperty(ConstValue.KEY).GetValue(pOriObject, null);
                oriValueObj = pOriObject.GetType().GetProperty(ConstValue.VALUE).GetValue(pOriObject, null);
            }
            if (pRevObject != null)
            {
                revKeyObj = pRevObject.GetType().GetProperty(ConstValue.KEY).GetValue(pRevObject, null);
                revValueObj = pRevObject.GetType().GetProperty(ConstValue.VALUE).GetValue(pRevObject, null);
            }

            if (pOriObject != null && pRevObject != null && revKeyObj != oriKeyObj)
            {
                //当原始值不为Null同时更新后的值不为Null时，原始值的Key值就应该与更新后的值的Key值相同，否则不是同一个KeyValuePair对象
                //TODO
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "原始Key值:{0},更新后的Key值:{1}.", oriKeyObj, revKeyObj));
            }

            /*
             * 原始值可能为Null，因为是字典中新增的内容，
             * 变更后的值也能为Null，因为是字典中被移除的内容
             * 但是不可以两者同时为Null，那样字典中无法赋值，也不能进行增量内容附加
             */
            if (oriKeyObj == null && revKeyObj == null)
            {
                //当两者均为Null时，抛出异常
                //TODO:
                throw new ArgumentException("原始值与更新后的Key值均为Null时");
            }

            if (!object.Equals(oriKeyObj, revKeyObj))
            {
                if (oriKeyObj == null)
                {
                    //当原始值的Key值为Null时。因为原始值的Key值为Null，如果是字典对象，会在外围增加Action=Add，所以这里暂时标记Action=Edit，根据附加Action的方法中Edit不会被标记
                    XElement addEle;
                    if (this.TryDivideCore(keyType, valueType, revKeyObj, null, revValueObj, Action.Edit, out addEle))
                    {
                        result = addEle;
                    }
                }
                else if (revKeyObj == null)
                {
                    //当更新后的Key值为Null时，将Action设为Remove。
                    //因为只是标记该Key值对应的键值对被移除。标记为SetNull是表示该Key值对应的键值对的值被设为Null。
                    XElement removeEle;
                    if (this.TryDivideCore(keyType, valueType, oriKeyObj, null, revValueObj, Action.Remove, out removeEle))
                    {
                        result = removeEle;
                    }
                }
            }
            //两个KeyValuePair对象实例的Key值相同时
            else
            {
                if (revValueObj == null)
                {
                    XElement setNullEle;
                    if (this.TryDivideCore(keyType, valueType, revKeyObj, oriValueObj, revValueObj, Action.SetNull, out setNullEle))
                    {
                        result = setNullEle;
                    }
                }
                else
                {
                    XElement editEle;
                    if (this.TryDivideCore(keyType, valueType, revKeyObj, oriValueObj, revValueObj, Action.Edit, out editEle))
                    {
                        result = editEle;
                    }
                }
            }

            return result;
        }

        #endregion Internal Methods

        #region Private Methods

        private bool TryDivideCore(TypeExtend pKeyType, TypeExtend pValueType, object keyObject, object oriValueObject, object revValueObject, Action pAction, out XElement pDividedElement)
        {
            pDividedElement = new XElement(this.Type.TypeFriendlyName);

            XElement addedKeyResult = new DivideCore(pKeyType).Divide(ConstValue.KEY, null, keyObject);
            if (addedKeyResult != null && !addedKeyResult.IsEmpty)
            {
                pDividedElement.Add(addedKeyResult);
            }
            else
            {
                return false;
            }

            if (pAction != Action.Remove && pAction != Action.SetNull)
            {
                XElement addedValueResult = new DivideCore(pValueType).Divide(ConstValue.VALUE, oriValueObject, revValueObject);
                if (addedValueResult != null && !addedValueResult.IsEmpty)
                {
                    pDividedElement.Add(addedValueResult);
                }
                else
                {
                    return false;
                }
            }

            pDividedElement.AppendActionAttribute(pAction);

            return true;
        }

        #endregion Private Methods
    }
}