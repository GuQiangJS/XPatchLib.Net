// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
#if HAVE_LINQ
using System.Linq;
#else
using XPatchLib.NoLinq;
#endif
using System.Reflection;

namespace XPatchLib
{
    /// <summary>
    ///     Type扩展
    /// </summary>
    internal class TypeExtend
    {
        private readonly Func<Object> CreateInstanceFuncs;

        private readonly Dictionary<String, Func<Object, Object>> GetValueFuncs;

        private readonly Dictionary<String, Action<Object, Object>> SetValueFuncs;

        internal TypeExtend(Type pType,Type pIgnoreAttributeType, TypeExtend pParentType = null)
        {
            ParentType = pParentType;

            OriType = pType;
            CustomAttributes = pType.GetCustomAttributes();
            PrimaryKeyAttr = GetCustomAttribute<PrimaryKeyAttribute>();

            if (PrimaryKeyAttr == null) {
                PrimaryKeyAttr = TypeExtendContainer.GetPrimaryKeyAttribute(pType);
            }

            GetValueFuncs = new Dictionary<string, Func<object, object>>();
            SetValueFuncs = new Dictionary<string, Action<object, object>>();

            IsBasicType = ReflectionUtils.IsBasicType(pType);
            IsIDictionary = ReflectionUtils.IsIDictionary(pType);
            IsICollection = ReflectionUtils.IsICollection(pType);

            IsIEnumerable = ReflectionUtils.IsIEnumerable(pType);
            FieldsToBeSerialized = ReflectionUtils.GetFieldsToBeSerialized(pType, pIgnoreAttributeType);
            DefaultValue = ReflectionUtils.GetDefaultValue(pType);
            IsArray = ReflectionUtils.IsArray(pType);
            TypeCode = Type.GetTypeCode(pType);
            IsGenericType = OriType.IsGenericType;
            IsGuid = pType == typeof(Guid);
            TypeFriendlyName = ReflectionUtils.GetTypeFriendlyName(pType);

            foreach (MemberWrapper member in FieldsToBeSerialized)
            {
                AddGetValueFunc(member);
                AddSetValueFunc(member);
            }

            if (IsIDictionary)
            {
                Type keyType = typeof(object);
                Type valueType = typeof(object);
                ReflectionUtils.IsIDictionary(pType, out keyType, out valueType);

                KeyArgumentType = keyType;
                ValueArgumentType = valueType;
            }

            IsKeyValuePair = ReflectionUtils.IsKeyValuePair(pType);
            if (IsKeyValuePair)
            {
                Type keyType = typeof(object);
                Type valueType = typeof(object);
                ReflectionUtils.IsKeyValuePair(pType, out keyType, out valueType);

                KeyArgumentType = keyType;
                ValueArgumentType = valueType;

                MemberWrapper[] members =
                {
                    new MemberWrapper(pType.GetProperty(ConstValue.KEY)),
                    new MemberWrapper(pType.GetProperty(ConstValue.VALUE))
                };
                FieldsToBeSerialized = members.OrderBy(x => x.Name).ToArray();
            }

            string errorPrimaryKeyName = string.Empty;
            if (!CheckPrimaryKeyAttribute(false, out errorPrimaryKeyName))
                throw new PrimaryKeyException(pType, errorPrimaryKeyName);

            CreateInstanceFuncs = ClrHelper.CreateInstanceFunc(pType);
        }

        /// <summary>
        ///     获取父级类型定义。
        /// </summary>
        internal TypeExtend ParentType { get; private set; }

        /// <summary>
        ///     获取该类型的自定义Attributes。
        /// </summary>
        internal IEnumerable<Attribute> CustomAttributes { get; private set; }

        /// <summary>
        ///     获取该类型的默认值。
        /// </summary>
        internal Object DefaultValue { get; private set; }

        /// <summary>
        ///     获取该类型下可以被序列化的字段。
        /// </summary>
        internal MemberWrapper[] FieldsToBeSerialized { get; private set; }

        internal Boolean IsArray { get; private set; }

        /// <summary>
        ///     获取是否为基础类型。
        /// </summary>
        internal Boolean IsBasicType { get; private set; }

        internal Boolean IsGenericType { get; private set; }

        internal Boolean IsGuid { get; private set; }

        internal Boolean IsICollection { get; private set; }

        internal Boolean IsIDictionary { get; private set; }

        internal Boolean IsIEnumerable { get; private set; }

        internal Boolean IsKeyValuePair { get; private set; }

        /// <summary>
        ///     只有当是字典类型或KeyValue类型时才会有值
        /// </summary>
        internal Type KeyArgumentType { get; private set; }

        /// <summary>
        ///     获取原始类型定义。
        /// </summary>
        internal Type OriType { get; private set; }

        internal PrimaryKeyAttribute PrimaryKeyAttr { get; private set; }

        internal TypeCode TypeCode { get; private set; }

        internal String TypeFriendlyName { get; private set; }

        /// <summary>
        ///     只有当是字典类型或KeyValue类型时才会有值
        /// </summary>
        internal Type ValueArgumentType { get; private set; }

        /// <summary>
        ///     检测类型上的PrimaryKeyAttribute特性是否符合要求。
        /// </summary>
        /// <param name="pCheckAttributeExists">是否强制要求类型必须设定PrimaryKeyAttribute特性。</param>
        /// <param name="pErrorPrimaryKeyName">有问题的主键属性名称。</param>
        /// <returns></returns>
        internal Boolean CheckPrimaryKeyAttribute(bool pCheckAttributeExists, out string pErrorPrimaryKeyName)
        {
            Boolean result = true;
            pErrorPrimaryKeyName = string.Empty;

            if (PrimaryKeyAttr == null)
            {
                if (pCheckAttributeExists)
                    result = false;
            }
            else
            {
                foreach (string primaryKey in PrimaryKeyAttr.GetPrimaryKeys())
                {
                    Type primaryKeyType;
                    TryGetMemberType(primaryKey, out primaryKeyType);
                    if (!ReflectionUtils.IsBasicType(primaryKeyType))
                    {
                        result = false;
                        pErrorPrimaryKeyName = primaryKey;
                        break;
                    }
                }
            }

            return result;
        }

        internal Object CreateInstance()
        {
            if (CreateInstanceFuncs != null)
                return CreateInstanceFuncs();

            if (IsBasicType)
            {
                if (OriType.IsValueType)
                    return CreateInstanceFuncs();
                if (OriType == typeof(string))
                    return string.Empty;
            }
            else
            {
                if (IsArray)
                {
                    Type elementType;
                    if (ReflectionUtils.TryGetArrayElementType(OriType, out elementType))
                        return Array.CreateInstance(elementType, 0);
                    throw new NotImplementedException();
                }
                try
                {
                    return OriType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[0],
                        CultureInfo.InvariantCulture);
                }
                catch (MissingMethodException)
                {
                    return Activator.CreateInstance(OriType, true);
                }
            }
            return null;
        }

        internal T GetCustomAttribute<T>() where T : Attribute
        {
            T result = default(T);
            foreach (Attribute attr in CustomAttributes)
                if (attr is T)
                {
                    result = (T) attr;
                    break;
                }
            return result;
        }

        internal Object GetMemberValue(Object pObject, String pMemberName)
        {
            Object result = null;
            if (pObject != null)
            {
                Func<Object, Object> getValueFunc;
                if (!TryGetGetValueFunc(pMemberName, out getValueFunc))
                {
                    MemberInfo memberInfo;
                    Boolean isPro;
                    if (TryGetMemberInfo(pMemberName, out memberInfo, out isPro))
                        result = isPro
                            ? ((PropertyInfo) memberInfo).GetValue(pObject, null)
                            : ((FieldInfo) memberInfo).GetValue(pObject);
                    else
                        throw new NotImplementedException();
                }
                else
                {
                    result = getValueFunc(pObject);
                }
            }
            return result;
        }

        internal static Boolean Equals(object objA, object objB)
        {
            if (objA == null && objB == null)
                return true;
            if (objA == null || objB == null)
                return false;
            return objA.Equals(objB);
        }

        internal static Boolean IsDefaultValue(object defaultValue, object value)
        {
            if (defaultValue == null && value == null)
                return true;
            else if (defaultValue != null)
                return defaultValue.Equals(value);
            return value.Equals(defaultValue);
        }

        internal static Boolean NeedSerialize(object defaultValue, object objA, object objB, bool serializeDefaultValue)
        {
            bool result = Equals(objA, objB);
            if (result && IsDefaultValue(defaultValue, objA) && serializeDefaultValue)
                return true;
            return !result;
        }

        internal Object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args,
            CultureInfo culture)
        {
            return OriType.InvokeMember(name, invokeAttr, binder, target, args, culture);
        }

        internal void SetMemberValue(Object pObject, String pMemberName, Object pValue)
        {
            if (pObject != null)
            {
                Action<Object, Object> setValueFunc;
                if (!TryGetSetValueFunc(pMemberName, out setValueFunc))
                {
                    MemberInfo memberInfo;
                    Boolean isPro;
                    if (TryGetMemberInfo(pMemberName, out memberInfo, out isPro))
                        if (isPro)
                            ((PropertyInfo) memberInfo).SetValue(pObject, pValue, null);
                        else
                            ((FieldInfo) memberInfo).SetValue(pObject, pValue);
                    else
                        throw new NotImplementedException();
                }
                else
                {
                    setValueFunc(pObject, pValue);
                }
            }
        }

        internal Boolean TryGetGetValueFunc(String pPropertyName, out Func<Object, Object> pFunc)
        {
            return GetValueFuncs.TryGetValue(pPropertyName, out pFunc);
        }

        internal Boolean TryGetMemberInfo(String pPropertyName, out MemberInfo pProInfo, out Boolean pIsProperty)
        {
            pProInfo = null;
            pIsProperty = false;
            MemberWrapper member = null;

            for (int i = 0; i < FieldsToBeSerialized.Length; i++)
                if (FieldsToBeSerialized[i].Name.Equals(pPropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    member = FieldsToBeSerialized[i];
                    break;
                }

            if (member != null)
            {
                pProInfo = member.MemberInfo;
                pIsProperty = member.IsProperty;
                return true;
            }
            return false;
        }

        internal Boolean TryGetMemberType(String pPropertyName, out Type pType)
        {
            MemberInfo member;
            Boolean isPro;
            pType = null;
            if (TryGetMemberInfo(pPropertyName, out member, out isPro))
            {
                pType = isPro ? ((PropertyInfo) member).PropertyType : ((FieldInfo) member).FieldType;
                return true;
            }
            return false;
        }

        internal Boolean TryGetSetValueFunc(String pPropertyName, out Action<Object, Object> pFunc)
        {
            return SetValueFuncs.TryGetValue(pPropertyName, out pFunc);
        }

        internal void UpdatePrimaryKeyAttributes(PrimaryKeyAttribute pAttribute)
        {
            IEnumerable<Attribute> newAttrs = CustomAttributes.Where(x => x.GetType() != typeof(PrimaryKeyAttribute));
            CustomAttributes = newAttrs.Union(new Attribute[] {pAttribute});

            PrimaryKeyAttr = pAttribute;
        }

        private void AddGetValueFunc(MemberWrapper pMember)
        {
            if (pMember.IsProperty)
            {
                Func<Object, Object> func = ((PropertyInfo) pMember.MemberInfo).GetValueFunc();
                if (func != null)
                    GetValueFuncs.Add(pMember.Name, func);
            }
            else
            {
                Func<Object, Object> func = ((FieldInfo) pMember.MemberInfo).GetValueFunc();
                if (func != null)
                    GetValueFuncs.Add(pMember.Name, func);
            }
        }

        private void AddSetValueFunc(MemberWrapper pMember)
        {
            if (pMember.IsProperty)
            {
                Action<Object, Object> act = ((PropertyInfo) pMember.MemberInfo).SetValueFunc();
                if (act != null)
                    SetValueFuncs.Add(pMember.Name, act);
            }
            else
            {
                Action<Object, Object> act = ((FieldInfo) pMember.MemberInfo).SetValueFunc();
                if (act != null)
                    SetValueFuncs.Add(pMember.Name, act);
            }
        }
    }
}