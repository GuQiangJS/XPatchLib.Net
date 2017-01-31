// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace XPatchLib
{
    internal class ReflectionUtils
    {
        #region Internal Methods

        internal static Object GetDefaultValue(Type pType)
        {
            Object result = null;
            if (pType.IsValueType)
                result = Activator.CreateInstance(pType);
            return result;
        }

        /// <summary>
        ///     获取一个类型上需要序列化的属性或字段集合。
        /// </summary>
        /// <param name="pObjType">
        /// </param>
        /// <returns>
        /// </returns>
        internal static IOrderedEnumerable<MemberWrapper> GetFieldsToBeSerialized(Type pObjType)
        {
            IOrderedEnumerable<MemberWrapper> result;
            Queue<MemberWrapper> r = new Queue<MemberWrapper>();
            foreach (MemberInfo memberInfo in pObjType.GetMembers(BindingFlags.Instance | BindingFlags.Public))
                if (memberInfo.MemberType == MemberTypes.Property || memberInfo.MemberType == MemberTypes.Field)
                {
                    MemberWrapper wrapper = new MemberWrapper(memberInfo);
                    if (wrapper.XmlIgnore == null && wrapper.HasPublicGetter && wrapper.HasPublicSetter)
                        r.Enqueue(wrapper);
                }
            result = r.OrderBy(x => x.Name);
            return result;
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
            if (pType.IsGenericType)
            {
                int backqIndex = result.IndexOf('`');
                if (backqIndex == 0)
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Bad type name: {0}",
                        result));
                if (backqIndex > 0)
                    result = result.Substring(0, backqIndex);

                result += ConstValue.UNDERLINE;

                Array.ForEach(pType.GetGenericArguments(),
                    genType => result += GetTypeFriendlyName(genType) + ConstValue.UNDERLINE);

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

        internal static Boolean InvokeObjectEquals(object target, object[] args)
        {
            return
                (Boolean)
                Type.GetType("System.Object")
                    .InvokeMember("Equals", BindingFlags.InvokeMethod, null, target, args, CultureInfo.InvariantCulture);
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
            if (pType == typeof(string) || pType.IsPrimitive || pType.IsEnum || pType == typeof(DateTime) ||
                pType == typeof(decimal) ||
                pType == typeof(Guid) || pType == typeof(Color))
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

        internal static bool IsICollection(Type type)
        {
            // a direct reference to the interface itself is also OK. 
            if (type.IsInterface && type.GetGenericTypeDefinition() == typeof(ICollection<>))
                return true;

            foreach (Type interfaceType in type.GetInterfaces())
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    return true;

            return false;
        }

        internal static bool IsIDictionary(Type type)
        {
            Type keyType, valueType;
            return IsIDictionary(type, out keyType, out valueType);
        }

        internal static bool IsIDictionary(Type type, out Type keyType, out Type valueType)
        {
            keyType = typeof(object);
            valueType = typeof(object);

            // a direct reference to the interface itself is also OK. 
            if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                Type[] genArgs = type.GetGenericArguments();
                keyType = genArgs[0];
                valueType = genArgs[1];
                return true;
            }

            foreach (Type interfaceType in type.GetInterfaces())
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    Type[] genArgs = interfaceType.GetGenericArguments();
                    keyType = genArgs[0];
                    valueType = genArgs[1];
                    return true;
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
        internal static bool IsIEnumerable(Type pType)
        {
            Type seqType;
            return TryGetIEnumerableGenericArgument(pType, out seqType);
        }

        internal static bool IsIList(Type type)
        {
            // a direct reference to the interface itself is also OK. 
            if (type.IsInterface && type.GetGenericTypeDefinition() == typeof(IList<>))
                return true;

            foreach (Type interfaceType in type.GetInterfaces())
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                    return true;

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
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
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
            if (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(Nullable<>))
                return true;
            return false;
        }

        //internal static Boolean ObjectEquals<T>(T pOriObj, T pRevObj)
        //{
        //    object[] args = new object[] { pOriObj, pRevObj };
        //    object result = null;

        //    MethodInfo method = typeof(T).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);
        //    if (method != null)
        //    {
        //        result = method.Invoke(pOriObj, new object[] { pOriObj, pRevObj });
        //    }
        //    else
        //    {
        //        try
        //        {
        //            result = typeof(T).InvokeMember("Equals", BindingFlags.InvokeMethod, null, result, args, CultureInfo.InvariantCulture);
        //        }
        //        catch (MissingMethodException)
        //        {
        //            result = InvokeObjectEquals(result, args);
        //        }
        //        catch (AmbiguousMatchException)
        //        {
        //            result = InvokeObjectEquals(result, args);
        //        }
        //    }

        //    return (Boolean)result;
        //}

        internal static void RegisterTypes(IDictionary<Type, string[]> pTypes)
        {
            Guard.ArgumentNotNull(pTypes, "pTypes");

            foreach (KeyValuePair<Type, string[]> kv in pTypes)
            {
                TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(kv.Key);
                typeExtend.UpdatePrimaryKeyAttributes(new PrimaryKeyAttribute(kv.Value));
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
        internal static bool TryGetIEnumerableGenericArgument(Type pType, out Type pSeqType)
        {
            // detect arrays early 
            if (TryGetArrayElementType(pType, out pSeqType))
                return true;

            pSeqType = typeof(object);
            if (pType == typeof(IEnumerable))
                return true;

            bool isNongenericEnumerable = false;

            if (pType.IsInterface && pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                pSeqType = pType.GetGenericArguments()[0];
                return true;
            }

            foreach (Type interfaceType in pType.GetInterfaces())
                if (interfaceType.IsGenericType &&
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

            // the second case is a direct reference to IEnumerable 
            if (isNongenericEnumerable || pType == typeof(IEnumerable))
            {
                pSeqType = typeof(object);
                return true;
            }

            return false;
        }

        #endregion Internal Methods
    }
}