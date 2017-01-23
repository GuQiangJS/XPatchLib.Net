using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    /// <summary>
    /// 复杂类型增量内容产生类。 
    /// </summary>
    internal class DivideObject : DivideBase
    {
        #region Internal Constructors

        /// <summary>
        /// 使用指定的类型初始化 <see cref="XPatchLib.DivideObject" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <remarks>
        /// 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 
        /// </remarks>
        internal DivideObject(TypeExtend pType)
            : base(pType) { }

        /// <summary>
        /// 使用指定的类型及指定是否序列化默认值初始化 <see cref="XPatchLib.DivideObject" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <param name="pSerializeDefalutValue">
        /// 指定是否序列化默认值。 
        /// </param>
        /// <exception cref="PrimaryKeyException">
        /// 当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。 
        /// </exception>
        /// <remarks>
        /// <para> 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 </para>
        /// </remarks>
        internal DivideObject(TypeExtend pType, Boolean pSerializeDefalutValue)
            : base(pType, pSerializeDefalutValue) { }

        /// <summary>
        /// 使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.DivideObject" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <param name="pMode">
        /// 指定在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <exception cref="PrimaryKeyException">
        /// 当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。 
        /// </exception>
        internal DivideObject(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : base(pType, pMode) { }

        /// <summary>
        /// 使用指定的类型、指定是否序列化默认值和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.DivideObject" /> 类的新实例。
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
        /// <exception cref="PrimaryKeyException">
        /// 当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。 
        /// </exception>
        internal DivideObject(TypeExtend pType, XmlDateTimeSerializationMode pMode, Boolean pSerializeDefalutValue)
            : base(pType, pMode, pSerializeDefalutValue) { }

        #endregion Internal Constructors

        #region Internal Methods

        /// <summary>
        /// 产生基础类型增量内容。 
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
        internal XElement Divide(string pName, Object pOriObject, Object pRevObject)
        {
            Guard.ArgumentNotNullOrEmpty(pName, "pName");

            XElement result = null;
            //当原始对象不为null，而更新对象为null时，设置为SetNull；
            if (pOriObject != null && pRevObject == null)
            {
                result = new XElement(pName);
                result.AppendActionAttribute(Action.SetNull);
                return result;
            }

            //遍历当前的类型上可以序列化的属性集合，逐个产生增量内容，之后再加入整个类型实例的增量结果result中
            IEnumerable<MemberWrapper> members = this.Type.FieldsToBeSerialized;
            foreach (MemberWrapper member in members)
            {
                object pOriMemberValue = this.Type.GetMemberValue(pOriObject, member.Name);
                object pRevMemberValue = this.Type.GetMemberValue(pRevObject, member.Name);

                Type memberType = member.MemberType;

                XElement memberElement = null;

                //基础类型 ReflectionUtils.IsBasicType
                if (member.IsBasicType)
                {
                    //如果是枚举类型，则先将枚举值转化为字符串
                    if (member.IsEnum)
                    {
                        pOriMemberValue = new EnumWrapper(memberType).TransToString(pOriMemberValue);
                        pRevMemberValue = new EnumWrapper(memberType).TransToString(pRevMemberValue);
                        memberType = typeof(string);
                    }
                    //如果是Color类型
                    if (member.IsColor)
                    {
                        pOriMemberValue = ColorHelper.TransToString(pOriMemberValue);
                        pRevMemberValue = ColorHelper.TransToString(pRevMemberValue);
                        memberType = typeof(string);
                    }

                    memberElement = new DivideBasic(TypeExtendContainer.GetTypeExtend(memberType), this.Mode, this.SerializeDefaultValue).Divide(member.Name, pOriMemberValue, pRevMemberValue);
                }
                //集合类型
                else if (member.IsIEnumerable)
                {
                    memberElement = new DivideIEnumerable(TypeExtendContainer.GetTypeExtend(memberType), this.Mode, this.SerializeDefaultValue).Divide(member.Name, pOriMemberValue, pRevMemberValue);
                }
                else
                {
                    memberElement = new DivideCore(TypeExtendContainer.GetTypeExtend(memberType), this.Mode, this.SerializeDefaultValue).Divide(member.Name, pOriMemberValue, pRevMemberValue);
                }

                if (memberElement != null)
                {
                    if (result == null)
                    {
                        result = new XElement(pName);
                    }
                    result.Add(memberElement);
                }
            }
            return result;
        }

        #endregion Internal Methods
    }
}