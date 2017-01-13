using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace XPatchLib
{
    internal static class TypeExtendContainer
    {
        private static Dictionary<int, TypeExtend> _innerDic = new Dictionary<int, TypeExtend>();

        internal static void Clear()
        {
            lock (_innerDic)
            {
                _innerDic.Clear();
            }
        }

        internal static TypeExtend GetTypeExtend(Type pType)
        {
            TypeExtend result = null;
            lock (_innerDic)
            {
                if (!_innerDic.TryGetValue(pType.GetHashCode(), out result))
                {
                    result = new TypeExtend(pType);
                    lock (_innerDic)
                    {
                        _innerDic.Add(pType.GetHashCode(), result);
                    }
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Type扩展 
    /// </summary>
    internal class TypeExtend
    {
        internal TypeExtend(Type pType)
        {
            this.OriType = pType;

            this.GetValueFuncs = new Dictionary<string, Func<object, object>>();
            this.SetValueFuncs = new Dictionary<string, Action<object, object>>();

            this.IsBasicType = ReflectionUtils.IsBasicType(pType);
            this.IsIDictionary = ReflectionUtils.IsIDictionary(pType);
            this.IsICollection = ReflectionUtils.IsICollection(pType);

            this.IsIEnumerable = ReflectionUtils.IsIEnumerable(pType);
            this.FieldsToBeSerialized = ReflectionUtils.GetFieldsToBeSerialized(pType);
            this.CustomAttributes = pType.GetCustomAttributes();
            this.PrimaryKeyAttribute = GetCustomAttribute<PrimaryKeyAttribute>();
            this.DefaultValue = ReflectionUtils.GetDefaultValue(pType);
            this.IsArray = ReflectionUtils.IsArray(pType);
            this.TypeCode = Type.GetTypeCode(pType);
            this.IsGenericType = this.OriType.IsGenericType;
            this.IsGuid = (pType == typeof(Guid));
            this.TypeFriendlyName = ReflectionUtils.GetTypeFriendlyName(pType);

            foreach (MemberWrapper member in this.FieldsToBeSerialized)
            {
                AddGetValueFunc(member);
                AddSetValueFunc(member);
            }

            if (this.IsIDictionary)
            {
                Type keyType = typeof(object);
                Type valueType = typeof(object);
                ReflectionUtils.IsIDictionary(pType, out keyType, out valueType);

                this.KeyArgumentType = keyType;
                this.ValueArgumentType = valueType;
            }

            this.IsKeyValuePair = ReflectionUtils.IsKeyValuePair(pType);
            if (this.IsKeyValuePair)
            {
                Type keyType = typeof(object);
                Type valueType = typeof(object);
                ReflectionUtils.IsKeyValuePair(pType, out keyType, out valueType);

                this.KeyArgumentType = keyType;
                this.ValueArgumentType = valueType;

                MemberWrapper[] members = new MemberWrapper[] {
                    new MemberWrapper(pType.GetProperty(ConstValue.KEY)),
                    new MemberWrapper(pType.GetProperty(ConstValue.VALUE))
                };
                this.FieldsToBeSerialized = members.OrderBy(x => x.Name);
            }
        }

        /// <summary>
        /// 获取该类型的自定义Attributes。 
        /// </summary>
        internal IEnumerable<Attribute> CustomAttributes { get; private set; }

        /// <summary>
        /// 获取该类型的默认值。 
        /// </summary>
        internal Object DefaultValue { get; private set; }

        /// <summary>
        /// 获取该类型下可以被序列化的字段。 
        /// </summary>
        internal IOrderedEnumerable<MemberWrapper> FieldsToBeSerialized { get; private set; }

        internal Boolean IsArray { get; private set; }

        /// <summary>
        /// 获取是否为基础类型。 
        /// </summary>
        internal Boolean IsBasicType { get; private set; }

        internal Boolean IsGenericType { get; private set; }

        internal Boolean IsGuid { get; private set; }

        internal Boolean IsICollection { get; private set; }

        internal Boolean IsIDictionary { get; private set; }

        internal Boolean IsIEnumerable { get; private set; }

        internal Boolean IsKeyValuePair { get; private set; }

        /// <summary>
        /// 只有当是字典类型或KeyValue类型时才会有值 
        /// </summary>
        internal Type KeyArgumentType { get; private set; }

        /// <summary>
        /// 获取原始类型定义。 
        /// </summary>
        internal Type OriType { get; private set; }

        internal PrimaryKeyAttribute PrimaryKeyAttribute { get; private set; }

        internal TypeCode TypeCode { get; private set; }

        internal String TypeFriendlyName { get; private set; }

        /// <summary>
        /// 只有当是字典类型或KeyValue类型时才会有值 
        /// </summary>
        internal Type ValueArgumentType { get; private set; }

        private Dictionary<String, Func<Object, Object>> GetValueFuncs { get; set; }

        private Dictionary<String, Action<Object, Object>> SetValueFuncs { get; set; }

        internal Object CreateInstance()
        {
            return this.OriType.CreateInstance();
        }

        internal T GetCustomAttribute<T>() where T : Attribute
        {
            T result = default(T);
            foreach (Attribute attr in this.CustomAttributes)
            {
                if (attr is T)
                {
                    result = (T)attr;
                    break;
                }
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
                    {
                        result = (isPro) ? ((PropertyInfo)memberInfo).GetValue(pObject, null) : ((FieldInfo)memberInfo).GetValue(pObject);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    result = getValueFunc(pObject);
                }
            }
            return result;
        }

        internal Object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, CultureInfo culture)
        {
            return this.OriType.InvokeMember(name, invokeAttr, binder, target, args, culture);
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
                    {
                        if (isPro)
                        {
                            ((PropertyInfo)memberInfo).SetValue(pObject, pValue, null);
                        }
                        else
                        {
                            ((FieldInfo)memberInfo).SetValue(pObject, pValue);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
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
            MemberWrapper member = this.FieldsToBeSerialized.FirstOrDefault(x => x.Name.Equals(pPropertyName, StringComparison.OrdinalIgnoreCase));

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
            if (this.TryGetMemberInfo(pPropertyName, out member, out isPro))
            {
                pType = (isPro) ? ((PropertyInfo)member).PropertyType : ((FieldInfo)member).FieldType;
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
            IEnumerable<Attribute> newAttrs = this.CustomAttributes.Where(x => x.GetType() != typeof(PrimaryKeyAttribute));
            this.CustomAttributes = newAttrs.Union(new Attribute[] { pAttribute });

            this.PrimaryKeyAttribute = pAttribute;
        }

        private void AddGetValueFunc(MemberWrapper pMember)
        {
            if (pMember.IsProperty)
            {
                Func<Object, Object> func = ((PropertyInfo)pMember.MemberInfo).GetValueFunc();
                if (func != null)
                {
                    GetValueFuncs.Add(pMember.Name, func);
                }
            }
            else
            {
                Func<Object, Object> func = ((FieldInfo)pMember.MemberInfo).GetValueFunc();
                if (func != null)
                {
                    GetValueFuncs.Add(pMember.Name, func);
                }
            }
        }

        private void AddSetValueFunc(MemberWrapper pMember)
        {
            if (pMember.IsProperty)
            {
                Action<Object, Object> act = ((PropertyInfo)pMember.MemberInfo).SetValueFunc();
                if (act != null)
                {
                    SetValueFuncs.Add(pMember.Name, act);
                }
            }
            else
            {
                Action<Object, Object> act = ((FieldInfo)pMember.MemberInfo).SetValueFunc();
                if (act != null)
                {
                    SetValueFuncs.Add(pMember.Name, act);
                }
            }
        }
    }
}