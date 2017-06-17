// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
#if (NET || NETSTANDARD_2_0_UP)
using System.Drawing;
#endif
using System.Globalization;
using System.Reflection;
#if (NET || NETSTANDARD_1_3_UP)
using System.Xml.Serialization;
#endif
using XPatchLib.Properties;

namespace XPatchLib
{
    /// <summary>
    ///     成员属性包装器。
    /// </summary>
    internal class MemberWrapper
    {
#region Internal Constructors

        /// <summary>
        ///     使用指定的 <see cref="System.Reflection.MemberInfo" /> 初始化
        ///     <see cref="XPatchLib.MemberWrapper" /> 类的新实例。
        /// </summary>
        /// <param name="pMemberInfo">
        ///     指定的成员属性的信息。
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="pMemberInfo" /> 为空时。
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="pMemberInfo" />.MemberType 既不是 <see cref="MemberTypes.Property" /> 也不是
        ///     <see cref="MemberTypes.Field" /> 时。
        /// </exception>
        internal MemberWrapper(MemberInfo pMemberInfo)
        {
            Guard.ArgumentNotNull(pMemberInfo, "pMemberInfo");

            if (pMemberInfo.MemberType() != MemberTypes.Property && pMemberInfo.MemberType() != MemberTypes.Field)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, Resources.Exp_String_MemberType, pMemberInfo,
                        pMemberInfo.MemberType())
                    , "pMemberInfo");

            MemberInfo = pMemberInfo;

            Name = MemberInfo.Name;

            InitType();
            InitDefaultValue();
        }

#endregion Internal Constructors

        /// <summary>
        ///     获取类型默认值。
        /// </summary>
        internal Object DefaultValue { get; private set; }

        /// <summary>
        ///     获取Getter是否为Public。
        /// </summary>
        internal Boolean HasPublicGetter { get; private set; }

        /// <summary>
        ///     获取Setter是否为Public。
        /// </summary>
        internal Boolean HasPublicSetter { get; private set; }

        /// <summary>
        ///     是否为基础类型。
        /// </summary>
        /// <remarks>
        ///     参见 <see cref="ReflectionUtils.IsBasicType" />
        /// </remarks>
        internal Boolean IsBasicType { get; private set; }

#if (NET || NETSTANDARD_2_0_UP)
        /// <summary>
        ///     获取是否为 Color 类型。
        /// </summary>
        internal Boolean IsColor
        {
            get { return Type == typeof(Color); }
        }
#endif

        /// <summary>
        ///     获取是否为枚举类型。
        /// </summary>
        internal Boolean IsEnum { get; private set; }

        /*
        /// <summary>
        /// 是否为泛型集合类型。 
        /// </summary>
        /// <remarks>
        /// 参见 <see cref="ReflectionUtils.IsICollection" /> 
        /// </remarks>
        public Boolean IsICollection { get; private set; }

        /// <summary>
        /// 是否为字典类型。 
        /// </summary>
        /// <remarks>
        /// 参见 <see cref="ReflectionUtils.IsIDictionary" /> 
        /// </remarks>
        public Boolean IsIDictionary { get; private set; }

        /// <summary>
        /// 是否为泛型List类型。 
        /// </summary>
        /// <remarks>
        /// 参见 <see cref="ReflectionUtils.IsIList" /> 
        /// </remarks>
        public Boolean IsIList { get; private set; }
        */

        /// <summary>
        ///     是否为集合类型。
        /// </summary>
        /// <remarks>
        ///     参见 <see cref="ReflectionUtils.IsArray" />
        /// </remarks>
        internal Boolean IsIEnumerable { get; private set; }

        /// <summary>
        ///     是不是属性类型。
        /// </summary>
        /// <remarks>
        ///     如果不是就是字段（Field）类型。
        /// </remarks>
        internal Boolean IsProperty { get; private set; }

        /// <summary>
        ///     获取当前成员属性实例。
        /// </summary>
        internal MemberInfo MemberInfo { get; private set; }

        /// <summary>
        ///     获取当前成员的名称。
        /// </summary>
        internal String Name { get; private set; }

        /// <summary>
        ///     获取类型。
        /// </summary>
        internal Type Type { get; private set; }

        /// <summary>
        ///     获取当前成员的 <b>跳过序列化</b> 特性标记。
        /// </summary>
        internal Attribute GetIgnore(Type pIngoreAttributeType)
        {
            if (pIngoreAttributeType == null)
                return null;
#if (NET || NETSTANDARD_2_0_UP)
            return Attribute.GetCustomAttribute(MemberInfo, pIngoreAttributeType);
#else
            return MemberInfo.GetCustomAttribute(pIngoreAttributeType);
#endif
        }

#region Private Methods

        /// <summary>
        ///     初始化当前成员属性的默认值。
        /// </summary>
        private void InitDefaultValue()
        {
            if (Type.IsValueType())
                DefaultValue = Activator.CreateInstance(Type);
            else
                DefaultValue = null;
        }

        /// <summary>
        ///     获取类型。
        /// </summary>
        /// <remarks>实际数据类型，可能是NullableValueType。</remarks>
        internal Type MemberType { get; private set; }

        /// <summary>
        ///     初始化当前成员属性的类型信息。
        /// </summary>
        private void InitType()
        {
            IsProperty = MemberInfo.MemberType() == MemberTypes.Property;

            HasPublicGetter = true;
            HasPublicSetter = true;
            if (IsProperty)
            {
                HasPublicSetter = ((PropertyInfo) MemberInfo).GetSetMethod() != null;
                HasPublicGetter = ((PropertyInfo) MemberInfo).GetGetMethod() != null;
            }

            Type = IsProperty ? ((PropertyInfo) MemberInfo).PropertyType : ((FieldInfo) MemberInfo).FieldType;

            MemberType = ReflectionUtils.IsNullable(Type) ? ReflectionUtils.GetNullableValueType(Type) : Type;

            IsBasicType = ReflectionUtils.IsBasicType(Type);

            IsIEnumerable = ReflectionUtils.IsIEnumerable(Type);

            IsEnum = Type.IsEnum();

            //IsICollection = ReflectionUtils.IsICollection(Type);

            //IsIDictionary = ReflectionUtils.IsIDictionary(Type);

            //IsIList = ReflectionUtils.IsIList(Type);
        }

#endregion Private Methods
    }
}