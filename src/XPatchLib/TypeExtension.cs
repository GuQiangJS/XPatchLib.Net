﻿// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Collections.Generic;
#if (NET_35_UP || NETSTANDARD)
using System.Linq;
#else
using XPatchLib.NoLinq;
#endif

namespace XPatchLib
{
    internal static class TypeExtension
    {

#if (NETSTANDARD_1_0 || NETSTANDARD_1_1 || NETSTANDARD_1_2 || NETSTANDARD_1_3 || NETSTANDARD_1_4)
        public static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetGetMethod(false);
        }

        public static MethodInfo GetGetMethod(this PropertyInfo propertyInfo, bool nonPublic)
        {
            MethodInfo getMethod = propertyInfo.GetMethod;
            if (getMethod != null && (getMethod.IsPublic || nonPublic))
            {
                return getMethod;
            }

            return null;
        }
        public static MethodInfo GetSetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetSetMethod(false);
        }

        public static MethodInfo GetSetMethod(this PropertyInfo propertyInfo, bool nonPublic)
        {
            MethodInfo setMethod = propertyInfo.SetMethod;
            if (setMethod != null && (setMethod.IsPublic || nonPublic))
            {
                return setMethod;
            }

            return null;
        }
#endif

        internal static bool IsPublic(this PropertyInfo property)
        {
            if (property.GetGetMethod() != null && property.GetGetMethod().IsPublic)
                return true;
            if (property.GetSetMethod() != null && property.GetSetMethod().IsPublic)
                return true;

            return false;
        }

        public static bool IsAbstract(this Type type)
        {
#if (NET || NETSTANDARD_2_0_UP)
            return type.IsAbstract;
#else
            return type.GetTypeInfo().IsAbstract;
#endif
        }

        internal static bool IsEnum(this Type pType)
        {
#if (NET || NETSTANDARD_2_0_UP)
            return pType.IsEnum;
#else
            return pType.GetTypeInfo().IsEnum;
#endif
        }

        internal static Assembly Assembly(this Type pType)
        {
#if (NET || NETSTANDARD_2_0_UP)
            return pType.Assembly;
#else
            return pType.GetTypeInfo().Assembly;
#endif
        }

        internal static bool IsValueType(this Type pType)
        {
#if (NET || NETSTANDARD_2_0_UP)
            return pType.IsValueType;
#else
            return pType.GetTypeInfo().IsValueType;
#endif
        }

        internal static bool IsGenericType(this Type pType)
        {
#if (NET_20_UP || NETSTANDARD_2_0_UP)
            return pType.IsGenericType;
#else
            return pType.GetTypeInfo().IsGenericType;
#endif
        }

        internal static bool IsPrimitive(this Type type)
        {
#if (NET || NETSTANDARD_2_0_UP)
            return type.IsPrimitive;
#else
            return type.GetTypeInfo().IsPrimitive;
#endif
        }

        internal static bool IsInterface(this Type type)
        {
#if (NET || NETSTANDARD_2_0_UP)
            return type.IsInterface;
#else
            return type.GetTypeInfo().IsInterface;
#endif
        }

        public static MemberTypes MemberType(this MemberInfo memberInfo)
        {
#if (NET || NETSTANDARD_1_5_UP)
            return memberInfo.MemberType;
#else
            if (memberInfo is PropertyInfo)
            {
                return MemberTypes.Property;
            }
            else if (memberInfo is FieldInfo)
            {
                return MemberTypes.Field;
            }
            else if (memberInfo is EventInfo)
            {
                return MemberTypes.Event;
            }
            else if (memberInfo is MethodInfo)
            {
                return MemberTypes.Method;
            }
            else
            {
                throw new NotImplementedException();
            }
#endif
        }

#if !(NET || NETSTANDARD_2_0_UP)

        private static BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

        internal static MethodInfo GetMethod(this Type type, IList<Type> parameterTypes)
        {
            return type.GetMethod(null, parameterTypes);
        }

        internal static MethodInfo GetMethod(this Type type, string name, IList<Type> parameterTypes)
        {
            return type.GetMethod(name, DefaultFlags, null, parameterTypes, null);
        }

        internal static MethodInfo GetMethod(this Type type, string name, BindingFlags bindingFlags, object placeHolder1, IList<Type> parameterTypes, object placeHolder2)
        {
            return MethodBinder.SelectMethod(type.GetTypeInfo().DeclaredMethods.Where(m => (name == null || m.Name == name) && TestAccessibility(m, bindingFlags)), parameterTypes);
        }

        internal static IEnumerable<Type> GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces;
        }

        internal static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetProperty(name, DefaultFlags);
        }

        internal static PropertyInfo GetProperty(this Type type, string name, BindingFlags bindingFlags)
        {
            return type.GetTypeInfo().GetDeclaredProperty(name);
        }

        internal static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        internal static MemberInfo[] GetMembers(this Type type,BindingFlags bindingFlags)
        {
            return type.GetMembers(string.Empty, bindingFlags);
        }

        internal static MemberInfo[] GetMembers(this Type type, string member)
        {
            return type.GetMemberInternal(member, null, DefaultFlags);
        }

        internal static MemberInfo[] GetMembers(this Type type, string member, BindingFlags bindingFlags)
        {
            return type.GetMemberInternal(member, null, bindingFlags);
        }

        internal static MemberInfo[] GetMemberInternal(this Type type, string member, MemberTypes? memberType, BindingFlags bindingFlags)
        {
            return type.GetTypeInfo().GetMembersRecursive().Where(m =>
                ((!string.IsNullOrEmpty(member) && m.Name == member) || string.IsNullOrEmpty(member)) &&
                // test type before accessibility - accessibility doesn't support some types
                (memberType == null || m.MemberType() == memberType) &&
                TestAccessibility(m, bindingFlags)).ToArray();
        }

        private static IList<MemberInfo> GetMembersRecursive(this TypeInfo type)
        {
            TypeInfo t = type;
            IList<MemberInfo> members = new List<MemberInfo>();
            while (t != null)
            {
                foreach (MemberInfo member in t.DeclaredMembers)
                {
                    if (!members.Any(p => p.Name == member.Name))
                    {
                        members.Add(member);
                    }
                }
                t = (t.BaseType != null) ? t.BaseType.GetTypeInfo() : null;
            }

            return members;
        }


        internal static bool TestAccessibility(MemberInfo member, BindingFlags bindingFlags)
        {
            if (member is FieldInfo)
            {
                return TestAccessibility((FieldInfo)member, bindingFlags);
            }
            else if (member is MethodBase)
            {
                return TestAccessibility((MethodBase)member, bindingFlags);
            }
            else if (member is PropertyInfo)
            {
                return TestAccessibility((PropertyInfo)member, bindingFlags);
            }

            throw new Exception("Unexpected member type.");
        }

        internal static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetMethod(name, DefaultFlags);
        }

        internal static MethodInfo GetMethod(this Type type, string name, BindingFlags bindingFlags)
        {
            return type.GetTypeInfo().GetDeclaredMethod(name);
        }

        internal static IEnumerable<ConstructorInfo> GetConstructors(this Type type)
        {
            return type.GetConstructors(DefaultFlags);
        }

        internal static IEnumerable<ConstructorInfo> GetConstructors(this Type type, BindingFlags bindingFlags)
        {
            return type.GetTypeInfo().DeclaredConstructors.Where(c => TestAccessibility(c, bindingFlags));
        }

        internal static ConstructorInfo GetConstructor(this Type type, IList<Type> parameterTypes)
        {
            return type.GetConstructor(DefaultFlags, null, parameterTypes, null);
        }

        internal static ConstructorInfo GetConstructor(this Type type, BindingFlags bindingFlags, object placeholder1, IList<Type> parameterTypes, object placeholder2)
        {
            return MethodBinder.SelectMethod(type.GetConstructors(bindingFlags), parameterTypes);
        }
        internal static bool IsAssignableFrom(this Type type, Type c)
        {
            return type.GetTypeInfo().IsAssignableFrom(c.GetTypeInfo());
        }
#endif
    }
}