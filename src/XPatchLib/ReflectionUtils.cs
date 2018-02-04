// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
#if (NET || NETSTANDARD_2_0_UP)
using System.Drawing;
#endif
#if NET_40_UP || NETSTANDARD_2_0_UP
using System.Dynamic;
#endif
using System.Globalization;
#if (NET_35_UP || NETSTANDARD)
using System.Linq;
#endif
using System.Reflection;
using System.Runtime.CompilerServices;

namespace XPatchLib
{
    internal class ReflectionUtils
    {
        #region Internal Methods

        internal static Object GetDefaultValue(Type pType,MemberInfo memberInfo=null)
        {
            if (memberInfo != null)
            {
                DefaultValueAttribute attr =
                    memberInfo.GetCustomAttributes(false)
                        .FirstOrDefault(x =>
                            x.GetType() == typeof(DefaultValueAttribute)) as DefaultValueAttribute;
                if (attr != null)
                    return attr.Value;
            }
            Object result = null;
            if (pType.IsValueType())
                result = Activator.CreateInstance(pType);
            return result;
        }

        /// <summary>
        /// 获取一个类型上需要序列化的属性或字段集合。
        /// </summary>
        /// <param name="pSetting">序列化/反序列化时的设置。</param>
        /// <param name="pObjType">Type of the p object.</param>
        /// <param name="pIngoreAttributeType">不序列化的特性类型定义。</param>
        /// <returns></returns>
        internal static MemberWrapper[] GetFieldsToBeSerialized(ISerializeSetting pSetting,Type pObjType, Type pIngoreAttributeType)
        {
            List<MemberInfo> members = GetFieldsAndProperties(pObjType, pSetting);

            Queue<MemberWrapper> result = new Queue<MemberWrapper>(members.Count);

            foreach (MemberInfo memberInfo in members)
            {
                if (TestAccessibility(memberInfo, pSetting))
                {
                    MemberWrapper wrapper = new MemberWrapper(memberInfo);

                    //if (wrapper.IsProperty)
                    //{
                    //    //属性必须同时具有Get和Set
                    //    if(!(wrapper.HasPublicGetter && wrapper.HasPublicSetter))
                    //        continue;
                    //}

                    if (wrapper.GetIgnore(pIngoreAttributeType) != null)
                        continue;
                    result.Enqueue(wrapper);
                }
            }

            return result.OrderBy(x => x.Name).ToArray();
        }

        private static bool TestAccessibility(MemberInfo memberInfo, ISerializeSetting pSetting)
        {
            if (memberInfo.IsDefined(typeof(CompilerGeneratedAttribute), true))
                return false;

            if (memberInfo is FieldInfo)
            {
                return TestAccessibility((FieldInfo)memberInfo, pSetting);
            }
            else if (memberInfo is PropertyInfo)
            {
                return TestAccessibility((PropertyInfo)memberInfo, pSetting);
            }
            return false;
        }

        static bool TestAccessibility(FieldInfo fieldInfo, ISerializeSetting pSetting)
        {
            bool visibility = (fieldInfo.IsPublic && pSetting.Modifier.HasFlag(SerializeMemberModifier.Public)) ||
                              (fieldInfo.IsPrivate && pSetting.Modifier.HasFlag(SerializeMemberModifier.Private)) ||
                              (fieldInfo.IsAssembly && pSetting.Modifier.HasFlag(SerializeMemberModifier.Internal)) ||
                              (fieldInfo.IsFamily && pSetting.Modifier.HasFlag(SerializeMemberModifier.Protected));

            bool instance = true;
            //bool instance = (member.IsStatic && bindingFlags.HasFlag(BindingFlags.Static)) ||
            //                (!member.IsStatic && bindingFlags.HasFlag(BindingFlags.Instance));

            return visibility && instance;
        }

        private static bool TestAccessibility(PropertyInfo member, ISerializeSetting pSetting)
        {
            bool hasGetMethod = false;
            bool hasSetMethod = false;
            if (member.GetGetMethod() != null && TestAccessibility(member.GetGetMethod(), pSetting)
                ||
                member.GetGetMethod(true) != null && TestAccessibility(member.GetGetMethod(true), pSetting))
            {
                hasGetMethod = true;
            }

            if (member.GetSetMethod() != null && TestAccessibility(member.GetSetMethod(), pSetting)
                ||
                member.GetSetMethod(true) != null && TestAccessibility(member.GetSetMethod(true), pSetting))
            {
                hasSetMethod = true;
            }

            return hasGetMethod & hasSetMethod;
        }

        static bool TestAccessibility(MethodInfo methodInfo, ISerializeSetting pSetting)
        {
            bool visibility = (methodInfo.IsPublic && pSetting.Modifier.HasFlag(SerializeMemberModifier.Public)) ||
                              (methodInfo.IsPrivate && pSetting.Modifier.HasFlag(SerializeMemberModifier.Private)) ||
                              (methodInfo.IsAssembly && pSetting.Modifier.HasFlag(SerializeMemberModifier.Internal)) ||
                              (methodInfo.IsFamily && pSetting.Modifier.HasFlag(SerializeMemberModifier.Protected));

            bool instance = true;
            //bool instance = (member.IsStatic && bindingFlags.HasFlag(BindingFlags.Static)) ||
            //                (!member.IsStatic && bindingFlags.HasFlag(BindingFlags.Instance));

            return visibility && instance;
        }

        public static IEnumerable<MemberInfo> GetProperties(Type targetType, BindingFlags bindingAttr)
        {
            Guard.ArgumentNotNull(targetType, nameof(targetType));
            
            return targetType.GetProperties(bindingAttr);
        }

        private static List<MemberInfo> GetFieldsAndProperties(Type pObjType, ISerializeSetting pSetting)
        {
            List<MemberInfo> result = new List<MemberInfo>();
            BindingFlags bindingFlags = GetBindingFlags(pSetting);
            if(pSetting.MemberType.HasFlag(SerializeMemberType.Field))
                result.AddRange(GetFields(pObjType, bindingFlags));
            if (pSetting.MemberType.HasFlag(SerializeMemberType.Property))
                result.AddRange(GetProperties(pObjType, bindingFlags));

            return result;
        }

        private static IEnumerable<MemberInfo> GetFields(Type targetType, BindingFlags bindingAttr)
        {
            Guard.ArgumentNotNull(targetType, nameof(targetType));

            return targetType.GetFields(bindingAttr);
        }

        static BindingFlags GetBindingFlags(ISerializeSetting pSettings)
        {
            switch ((int)pSettings.Modifier)
            {
                case 1://public
                    return BindingFlags.Instance | BindingFlags.Public;
                case 2://private
                case 4://protected
                case 6://private | protected
                case 8://internal
                case 10://private | internal
                case 12://protected | internal
                case 14://private | protected | internal
                    return BindingFlags.Instance | BindingFlags.NonPublic;
                case 3://public | private
                case 5://public | protected
                case 7://public | private | protected
                case 9://public | internal
                case 11://public | private | internal
                case 13://public | protected | internal
                case 15://public | private | protected | internal 
                    return BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     获取Nullable类型的值类型。
        /// </summary>
        /// <param name="pType">
        /// </param>
        /// <returns>
        ///     <para> 当 <paramref name="pType" /> 是 <c> Nullable </c> 类型时，返回其中的值类型。 </para>
        ///     <para> 否则返回 <c> Null </c>。 </para>
        /// </returns>
        internal static Type GetNullableValueType(Type pType)
        {
            if (IsNullable(pType))
                return pType.GetGenericArguments()[0];
            return null;
        }

        /// <summary>
        ///     获取显示用的类型名称。
        /// </summary>
        /// <param name="pType">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="pType" /> 为 <c> null </c> 时。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="pType" /> 是泛型类型，但是未能找到泛型类型的类型 时。
        /// </exception>
        /// <remarks>
        ///     <para> 泛型类型时使用 <c> _ </c> 拼接各个泛型参数类型的名称。 </para>
        ///     <para> 数组类型时使用 Array（数组维数）Of（数组元素类型的名称） 拼接各个泛型参数类型的名称。 </para>
        /// </remarks>
        internal static string GetTypeFriendlyName(Type pType)
        {
            Guard.ArgumentNotNull(pType, "pType");

            string result = pType.Name;
            if (pType.IsGenericType())
            {
                int backqIndex = result.IndexOf('`');
                if (backqIndex == 0)
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Bad type name: {0}",
                        result));
                if (backqIndex > 0)
                    result = result.Substring(0, backqIndex);

                result += ConstValue.UNDERLINE;

                foreach(Type t in pType.GetGenericArguments())
                {
                    result += GetTypeFriendlyName(t) + ConstValue.UNDERLINE;
                }

                if (result.EndsWith(ConstValue.UNDERLINE, StringComparison.OrdinalIgnoreCase))
                    result = result.Remove(result.Length - 1);
            }
            else if (pType.IsArray)
            {
                Type t = pType.GetElementType();
                result = String.Format(CultureInfo.InvariantCulture, "Array{0}Of{1}", pType.GetArrayRank(),
                    GetTypeFriendlyName(t));
            }

            return result;
        }

        internal static bool IsArray(Type pType)
        {
            Type elementType;
            return TryGetArrayElementType(pType, out elementType);
        }

        /// <summary>
        ///     检测类型是否为基础类型。
        /// </summary>
        /// <param name="pType">
        /// </param>
        /// <returns>
        /// </returns>
        /// <remarks>
        ///     <para> Nullable类型中的值类型是基础类型时，此泛型也是基础类型。 </para>
        /// </remarks>
        internal static Boolean IsBasicType(Type pType)
        {
            bool result = false;
            if (pType == typeof(string) || pType.IsPrimitive() || pType.IsEnum() || pType == typeof(DateTime) ||
                pType == typeof(decimal) ||
                pType == typeof(Guid)
#if (NET || NETSTANDARD_2_0_UP)
                || pType == typeof(Color)
#endif
                )
            {
                result = true;
            }
            else
            {
                Type nullableValueType;
                if (IsNullable(pType, out nullableValueType))
                    result = IsBasicType(nullableValueType);
                else
                    result = false;
            }
            return result;
        }

        internal static bool IsICollection(Type type,Type[] interfaces)
        {
            // a direct reference to the interface itself is also OK. 
            if (type.IsInterface() && type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(ICollection<>))
                return true;

            if (interfaces != null)
            {
                foreach (Type interfaceType in interfaces)
                    if (interfaceType.IsGenericType() &&
                        interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>))
                        return true;
            }

            return false;
        }

        internal static bool IsIDictionary(Type type, Type[] interfaces)
        {
            Type keyType, valueType;
            return IsIDictionary(type, interfaces, out keyType, out valueType);
        }

#if NET_40_UP || NETSTANDARD_2_0_UP
        internal static bool IsDynamicObject(Type type)
        {
            return typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type);
        }
#endif

        internal static bool IsIDictionary(Type type, Type[] interfaces, out Type keyType, out Type valueType)
        {
            keyType = typeof(object);
            valueType = typeof(object);

            // a direct reference to the interface itself is also OK. 
            if (type.IsInterface() && type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                Type[] genArgs = type.GetGenericArguments();
                keyType = genArgs[0];
                valueType = genArgs[1];
                return true;
            }

            if (interfaces != null)
            {
                foreach (Type interfaceType in interfaces)
                    if (interfaceType.IsGenericType() &&
                        interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    {
                        Type[] genArgs = interfaceType.GetGenericArguments();
                        keyType = genArgs[0];
                        valueType = genArgs[1];
                        return true;
                    }
            }

            return false;
        }

        /// <summary>
        ///     检测类型是否为公开枚举数。
        /// </summary>
        /// <param name="pType">
        /// </param>
        /// <returns>
        /// </returns>
        internal static bool IsIEnumerable(Type pType, Type[] interfaces)
        {
            Type seqType;
            return TryGetIEnumerableGenericArgument(pType, interfaces, out seqType);
        }

        internal static bool IsIList(Type type,Type[] interfaces)
        {
            // a direct reference to the interface itself is also OK. 
            if (type.IsInterface() && type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(IList<>))
                return true;

            if (interfaces != null)
            {
                foreach (Type interfaceType in interfaces)
                    if (interfaceType.IsGenericType() &&
                        interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                        return true;
            }

            return false;
        }

        internal static bool IsKeyValuePair(Type type)
        {
            Type keyType, valueType;
            return IsKeyValuePair(type, out keyType, out valueType);
        }

        internal static bool IsKeyValuePair(Type type, out Type keyType, out Type valueType)
        {
            keyType = typeof(object);
            valueType = typeof(object);

            // a direct reference to the interface itself is also OK. 
            if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                Type[] genArgs = type.GetGenericArguments();
                keyType = genArgs[0];
                valueType = genArgs[1];
                return true;
            }

            return false;
        }

        internal static bool IsNullable(Type pType, out Type pValueType)
        {
            pValueType = GetNullableValueType(pType);
            if (pValueType == null)
                return false;
            return true;
        }

        /// <summary>
        ///     检测指定类型是否为 <c> Nullable </c>。
        /// </summary>
        /// <param name="pType">
        /// </param>
        /// <returns>
        /// </returns>
        internal static bool IsNullable(Type pType)
        {
            if (pType.IsGenericType() && pType.GetGenericTypeDefinition() == typeof(Nullable<>))
                return true;
            return false;
        }

        internal static void RegisterTypes(ISerializeSetting pSetting, IDictionary<Type, string[]> pTypes)
        {
            Guard.ArgumentNotNull(pTypes, "pTypes");

            foreach (KeyValuePair<Type, string[]> kv in pTypes)
            {
                TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(pSetting, kv.Key, null);
                PrimaryKeyAttribute attr = new PrimaryKeyAttribute(kv.Value);
                typeExtend.UpdatePrimaryKeyAttributes(attr);
                TypeExtendContainer.AddPrimaryKeyAttribute(kv.Key, attr);
            }
        }

        internal static bool TryGetArrayElementType(Type pType, out Type pElementType)
        {
            if (pType.IsArray)
            {
                pElementType = pType.GetElementType();
                return true;
            }
            if (pType == typeof(Array))
            {
                pElementType = typeof(object);
                return true;
            }
            pElementType = typeof(object);
            return false;
        }

        /// <summary>
        ///     检测类型是否为公开枚举数。
        /// </summary>
        /// <param name="pType">
        /// </param>
        /// <param name="pSeqType">
        ///     泛型类型。
        /// </param>
        /// <returns>
        /// </returns>
        internal static bool TryGetIEnumerableGenericArgument(Type pType, Type[] interfaces, out Type pSeqType)
        {
            // detect arrays early 
            if (TryGetArrayElementType(pType, out pSeqType))
                return true;

            pSeqType = typeof(object);
            if (pType == typeof(IEnumerable))
                return true;

            bool isNongenericEnumerable = false;

            if (pType.IsInterface() && pType.IsGenericType() &&
                pType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                pSeqType = pType.GetGenericArguments()[0];
                return true;
            }

            if (interfaces != null)
            {
                foreach (Type interfaceType in interfaces)
                    if (interfaceType.IsGenericType() &&
                        interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        Type[] genArgs = interfaceType.GetGenericArguments();
                        pSeqType = genArgs[0];
                        return true;
                    }
                    else if (interfaceType == typeof(IEnumerable))
                    {
                        isNongenericEnumerable = true;
                    }
            }
            // the second case is a direct reference to IEnumerable 
            if (isNongenericEnumerable || pType == typeof(IEnumerable))
            {
                pSeqType = typeof(object);
                return true;
            }

            return false;
        }

        internal static bool ShouldSkipDeserialized(Type t)
        {
            // ConcurrentDictionary throws an error in its OnDeserialized so ignore
            if (IsConcurrentDictionary(t) || IsConcurrentStack(t) || IsConcurrentQueue(t) || IsConcurrentBag(t))
            {
                return true;
            }

            return false;
        }
        internal static bool ShouldSkipSerializing(Type t)
        {
            if (IsConcurrentDictionary(t) || IsConcurrentStack(t) || IsConcurrentQueue(t) || IsConcurrentBag(t))
            {
                return true;
            }

            return false;
        }

        internal static bool IsQueue(Type t)
        {
            if (t.IsGenericType())
            {
                Type definition = t.GetGenericTypeDefinition();

                switch (definition.FullName)
                {
                    case "System.Collections.Generic.Queue`1":
                        return true;
                }
            }

            return false;
        }

        internal static bool IsStack(Type t)
        {
            if (t.IsGenericType())
            {
                Type definition = t.GetGenericTypeDefinition();

                switch (definition.FullName)
                {
                    case "System.Collections.Generic.Stack`1":
                        return true;
                }
            }

            return false;
        }

        internal static bool IsConcurrentDictionary(Type t, bool checkBase = false)
        {
            if (t.IsGenericType())
            {
                Type definition = t.GetGenericTypeDefinition();

                switch (definition.FullName)
                {
                    case "System.Collections.Concurrent.ConcurrentDictionary`2":
                        return true;
                }
            }

            if (checkBase && t.BaseType() != null && IsConcurrentDictionary(t.BaseType()))
            {
                return true;
            }

            return false;
        }

        internal static bool IsConcurrentStack(Type t,bool checkBase=false)
        {
            if (t.IsGenericType())
            {
                Type definition = t.GetGenericTypeDefinition();

                switch (definition.FullName)
                {
                    case "System.Collections.Concurrent.ConcurrentStack`1":
                        return true;
                }
            }

            if (checkBase && t.BaseType() != null && IsConcurrentStack(t.BaseType()))
            {
                return true;
            }

            return false;
        }

        internal static bool IsConcurrentQueue(Type t,bool checkBase=false)
        {
            if (t.IsGenericType())
            {
                Type definition = t.GetGenericTypeDefinition();

                switch (definition.FullName)
                {
                    case "System.Collections.Concurrent.ConcurrentQueue`1":
                        return true;
                }
            }

            if (checkBase && t.BaseType() != null && IsConcurrentQueue(t.BaseType()))
            {
                return true;
            }

            return false;
        }
        internal static bool IsConcurrentBag(Type t, bool checkBase = false)
        {
            if (t.IsGenericType())
            {
                Type definition = t.GetGenericTypeDefinition();

                switch (definition.FullName)
                {
                    case "System.Collections.Concurrent.ConcurrentBag`1":
                        return true;
                }
            }

            if (checkBase && t.BaseType() != null && IsConcurrentBag(t.BaseType()))
            {
                return true;
            }

            return false;
        }

        internal static bool IsObservableCollection(Type t)
        {
            if (t.IsGenericType())
            {
                Type definition = t.GetGenericTypeDefinition();

                switch (definition.FullName)
                {
                    case "System.Collections.ObjectModel.ObservableCollection`1":
                        return true;
                }
            }

            if (t.BaseType() != null && IsObservableCollection(t.BaseType()))
            {
                return true;
            }

            return false;
        }
        #endregion Internal Methods
    }
}