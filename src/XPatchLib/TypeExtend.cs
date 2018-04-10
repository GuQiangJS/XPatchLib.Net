// Copyright © 2013-2018 - GuQiang55
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if (NET_40_UP || NETSTANDARD_2_0_UP)
using System.Dynamic;
#endif
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
#if (NET_35_UP || NETSTANDARD)
using System.Linq;

#else
using XPatchLib.NoLinq;
#endif
#if NET || NETSTANDARD_2_0_UP
using System.Runtime.Serialization;
#endif

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
#if (NET || NETSTANDARD_2_0_UP)
        private readonly Dictionary<String, MethodInfo> Methods;
#else
        private readonly Dictionary<String, ClrHelper.MethodCall<object, object>> Methods;
#endif
        private readonly bool _isDynamicObject;

        public Type[] InterfaceTypes { get; }

        public Boolean IsStack { get; }

        public Boolean IsQueue { get; }

        public Boolean IsConcurrentStack { get; private set; }

        public Boolean IsConcurrentBag { get; private set; }

        public Boolean IsConcurrentQueue { get; private set; }

        public Boolean IsConcurrentDictionary { get; private set; }

        public Boolean IsNullable { get; }

#if NET || NETSTANDARD_1_3_UP
        public Boolean IsFileSystemInfo { get; private set; }
#endif

#if NET || NETSTANDARD_2_0_UP
        public Boolean IsDriveInfo { get; private set; }
#endif

        private readonly Type NullableType;


#if NET || NETSTANDARD_2_0_UP

        /// <summary>
        ///     是否为 <see cref="ISerializable" /> 接口的实现
        /// </summary>
        /// <remarks>
        ///     https://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.iserializable(v=vs.110).aspx
        /// </remarks>
        public Boolean IsISerializable { get; }
#endif

        /// <summary>
        ///     获取序列化/反序列化时的设置。
        /// </summary>
        public ISerializeSetting Setting { get; }

        /// <summary>
        ///     获取当前类型可能产生的特性名称集合。
        /// </summary>
        public string[] AttributeNames { get; private set; }

        internal TypeExtend(ISerializeSetting pSetting, Type pType, Type pIgnoreAttributeType,
            TypeExtend pParentType = null, MemberInfo memberInfo = null)
        {
            ParentType = pParentType;
            Setting = pSetting;
            OriType = pType;

            Type actType;
            IsNullable = ReflectionUtils.IsNullable(pType, out actType);
            if (IsNullable)
                NullableType = pType = actType;

#if (NET_20_UP || NETSTANDARD_2_0_UP)
            CustomAttributes = Attribute.GetCustomAttributes(pType);
#else
            CustomAttributes = pType.GetTypeInfo().GetCustomAttributes().ToArray();
#endif
            PrimaryKeyAttr = GetCustomAttribute<PrimaryKeyAttribute>();

            InterfaceTypes = pType.GetInterfaces();

            if (PrimaryKeyAttr == null) PrimaryKeyAttr = TypeExtendContainer.GetPrimaryKeyAttribute(pType);

            //InitAttributeNames(pSetting,_primaryKeyAttr);

            GetValueFuncs = new Dictionary<string, Func<object, object>>();
            SetValueFuncs = new Dictionary<string, Action<object, object>>();
#if (NET || NETSTANDARD_2_0_UP)
            Methods = new Dictionary<string, MethodInfo>();
#else
            Methods = new Dictionary<string, ClrHelper.MethodCall<object, object>>();
#endif

            IsBasicType = ReflectionUtils.IsBasicType(pType);
            if (!IsBasicType)
            {
                IsIDictionary = ReflectionUtils.IsIDictionary(pType, InterfaceTypes);
                IsICollection = ReflectionUtils.IsICollection(pType, InterfaceTypes);
                IsIEnumerable = ReflectionUtils.IsIEnumerable(pType, InterfaceTypes);
                IsArray = ReflectionUtils.IsArray(pType);
            }

            DefaultValue = ReflectionUtils.GetDefaultValue(OriType, memberInfo);
            IsArrayItem = ParentType != null &&
                          (ParentType.IsArray || ParentType.IsICollection ||
                           ParentType.IsIEnumerable);
            TypeCode = ConvertHelper.GetTypeCode(pType);
            IsGenericType = OriType.IsGenericType();
            IsGuid = pType == typeof(Guid);
            TypeFriendlyName = ReflectionUtils.GetTypeFriendlyName(pType);
#if NET_40_UP || NETSTANDARD_2_0_UP
            _isDynamicObject = ReflectionUtils.IsDynamicObject(pType);
            IsConcurrentDictionary = ReflectionUtils.IsConcurrentDictionary(pType, true);
            IsConcurrentQueue = ReflectionUtils.IsConcurrentQueue(pType,true);
            IsConcurrentStack = ReflectionUtils.IsConcurrentStack(pType);
            IsConcurrentBag = ReflectionUtils.IsConcurrentBag(pType);
#endif
            IsQueue = ReflectionUtils.IsQueue(pType);
            IsStack = ReflectionUtils.IsStack(pType);

            FieldsToBeSerialized = new MemberWrapper[] { };
            if (!(IsIEnumerable || IsIDictionary || IsICollection || IsBasicType || IsArray))
            {
                FieldsToBeSerialized =
                    ReflectionUtils.GetFieldsToBeSerialized(pSetting, pType, pIgnoreAttributeType);
                foreach (MemberWrapper member in FieldsToBeSerialized)
                {
                    AddGetValueFunc(member);
                    AddSetValueFunc(member);
                }
            }

            if (IsIDictionary)
            {
                Type keyType = typeof(object);
                Type valueType = typeof(object);
                ReflectionUtils.IsIDictionary(pType, InterfaceTypes, out keyType, out valueType);

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

#if NET || NETSTANDARD_1_3_UP
            IsFileSystemInfo = ReflectionUtils.IsFileSystemInfo(pType);
#endif

#if NET || NETSTANDARD_2_0_UP
            IsDriveInfo = ReflectionUtils.IsDriveInfo(pType);
#endif

            string errorPrimaryKeyName = string.Empty;
            if (!CheckPrimaryKeyAttribute(false, out errorPrimaryKeyName))
                throw new PrimaryKeyException(pType, errorPrimaryKeyName);

            if (!IsBasicType)
                CreateInstanceFuncs = ClrHelper.CreateInstanceFunc<Object>(pType);
#if NET || NETSTANDARD_2_0_UP
            ResolveCallbackMethods(OriType);
#endif

#if NET || NETSTANDARD_2_0_UP
            IsISerializable = typeof(ISerializable).IsAssignableFrom(OriType);
#endif
        }

        private void InitAttributeNames(ISerializeSetting pSetting, PrimaryKeyAttribute primaryKeyAttr)
        {
            string[] app =
            {
                pSetting.ActionName
            };
#if NET_40_UP || NETSTANDARD_2_0_UP
            if (ParentType != null && ParentType.IsDynamicObject)
            {
                app = new []
                {
                    pSetting.ActionName
                    ,pSetting.AssemblyQualifiedName
                };
            }
#endif

            int attrLen = 0;
            if (primaryKeyAttr != null)
            {
                string[] attrs = primaryKeyAttr.GetPrimaryKeys();
                if (attrs != null && attrs.Length > 0)
                {
                    attrLen = attrs.Length;
                    AttributeNames = new string[attrLen + app.Length];
                    Array.Copy(attrs, 0, AttributeNames, 0, attrLen);
                }
            }
            if (AttributeNames == null)
                AttributeNames = new string[attrLen + app.Length];
            Array.Copy(app, 0, AttributeNames, attrLen, app.Length);
        }

        /// <summary>
        ///     获取父级类型定义。
        /// </summary>
        internal TypeExtend ParentType { get; }

        /// <summary>
        ///     获取该类型的自定义Attributes。
        /// </summary>
        internal IEnumerable<Attribute> CustomAttributes { get; private set; }

        /// <summary>
        ///     获取该类型的默认值。
        /// </summary>
        internal Object DefaultValue { get; }

        /// <summary>
        ///     获取该类型下可以被序列化的字段。
        /// </summary>
        internal MemberWrapper[] FieldsToBeSerialized { get; }

        internal Boolean IsArrayItem { get; }

        internal Boolean IsArray { get; }

        /// <summary>
        ///     获取是否为基础类型。
        /// </summary>
        internal Boolean IsBasicType { get; }

        internal Boolean IsGenericType { get; }

        internal Boolean IsGuid { get; }

        internal Boolean IsICollection { get; }

#if NET_40_UP || NETSTANDARD_2_0_UP
        internal Boolean IsDynamicObject
        {
            get { return _isDynamicObject; }
        }
#endif

        internal Boolean IsIDictionary { get; }

        internal Boolean IsIEnumerable { get; }

        internal Boolean IsKeyValuePair { get; }

        /// <summary>
        ///     只有当是字典类型或KeyValue类型时才会有值
        /// </summary>
        internal Type KeyArgumentType { get; }

        /// <summary>
        ///     获取原始类型定义。
        /// </summary>
        internal Type OriType { get; }

        internal PrimaryKeyAttribute PrimaryKeyAttr { get; private set; }

        internal TypeCode TypeCode { get; }

        internal String TypeFriendlyName { get; }

        /// <summary>
        ///     只有当是字典类型或KeyValue类型时才会有值
        /// </summary>
        internal Type ValueArgumentType { get; }

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

        internal Object CreateInstance(params object[] args)
        {
            IConverter converter;
            if (OtherConverterContainer.TryGetConverter(this, out converter))
                return converter.CreateInstance();

            if (CreateInstanceFuncs != null && (args == null || args.Length <= 0))
                return CreateInstanceFuncs();

            if (IsBasicType)
            {
                if ((IsNullable ? NullableType : OriType).IsValueType())
                    if (CreateInstanceFuncs == null)
                        return DefaultValue;
                    else
                        return CreateInstanceFuncs();
                if ((IsNullable ? NullableType : OriType) == typeof(string))
                    return string.Empty;
            }
            else
            {
                if (IsArray)
                {
                    Type elementType;
                    if (ReflectionUtils.TryGetArrayElementType(IsNullable ? NullableType : OriType, out elementType))
                        return Array.CreateInstance(elementType, 0);
                    throw new NotImplementedException();
                }
                return TypeHelper.CreateInstance(IsNullable ? NullableType : OriType, args);
            }
            return null;
        }


        internal T GetCustomAttribute<T>() where T : Attribute
        {
            T result = default(T);
            if (CustomAttributes == null)
                return result;
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

        internal static Boolean IsDefaultValue(object defaultValue, object value)
        {
            if (defaultValue == null && value == null)
                return true;
            if (defaultValue != null)
                return defaultValue.Equals(value);
            return value.Equals(defaultValue);
        }

        internal static Boolean NeedSerialize(object defaultValue, object objA, object objB, bool serializeDefaultValue)
        {
            bool result = Equals(objA, objB);
            if (result && IsDefaultValue(defaultValue, objB) && serializeDefaultValue)
                return true;
            return !result;
        }

        internal Object InvokeMember(string name, BindingFlags invokeAttr, object target, object[] args,
            CultureInfo culture)
        {
#if (NET || NETSTANDARD_2_0_UP)
            MethodInfo methodInfo = null;
            if (!TryGetMethod(name, out methodInfo))
            {
#else
            ClrHelper.MethodCall<Object, Object> methodCall = null;
            if (!TryGetMethod(name, out methodCall))
            {
                MethodInfo
#endif
                methodInfo = OriType.GetMethod(name, invokeAttr);
                Type t = OriType;
                if (methodInfo == null)
                {
                    //当前OriType可能是接口类型（如IList<T>，调用 Add 方法时，无法在当前类型上找到方法，所以还可以根据实例的类型来找方法）
                    t = target.GetType();
#if NETSTANDARD_2_0_UP // .NET Core 2.0 开始，字典类型Remove 方法增加了一个可以同时传入 Key 值和 Value 值的方法。
                    if (args != null)
                    {
                        Type[] ts = new Type[args.Length];
                        for (int i = 0; i < args.Length; i++)
                        {
                            if (args[i] == null)
                            {
                                methodInfo = t.GetMethod(name);
                                ts = null;
                                break;
                            }
                            ts[i] = args[i].GetType();
                        }
                        if(ts!=null)
                            methodInfo = t.GetMethod(name, ts);
                    }
                    else
#endif
                    methodInfo = t.GetMethod(name);

                    if (methodInfo == null)
                        return null;
                }
#if (NET || NETSTANDARD_2_0_UP)
                Methods.Add(name, methodInfo);
#else
                methodCall = t.CreateMethodCall<object>(methodInfo);
                Methods.Add(name, methodCall);
#endif
            }
#if (NET || NETSTANDARD_2_0_UP)
            return methodInfo.Invoke(target, args);
#else
            return methodCall.Invoke(target, args);
#endif
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

#if NET || NETSTANDARD_2_0_UP
        internal Boolean TryGetMethod(String pMethodName, out MethodInfo pMethod)
        {
            return Methods.TryGetValue(pMethodName, out pMethod);
        }
#else
        internal Boolean TryGetMethod(String pMethodName, out ClrHelper.MethodCall<Object, Object> pMethod)
        {
            return Methods.TryGetValue(pMethodName, out pMethod);
        }
#endif

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
                Func<Object, Object> func = ((PropertyInfo) pMember.MemberInfo).GetValueFunc<Object>();
                if (func != null)
                    GetValueFuncs.Add(pMember.Name, func);
            }
            else
            {
                Func<Object, Object> func = ((FieldInfo) pMember.MemberInfo).GetValueFunc<Object>();
                if (func != null)
                    GetValueFuncs.Add(pMember.Name, func);
            }
        }

        private void AddSetValueFunc(MemberWrapper pMember)
        {
            if (pMember.IsProperty)
            {
                Action<Object, Object> act = ((PropertyInfo) pMember.MemberInfo).SetValueFunc<Object>();
                if (act != null)
                    SetValueFuncs.Add(pMember.Name, act);
            }
            else
            {
                Action<Object, Object> act = ((FieldInfo) pMember.MemberInfo).SetValueFunc<Object>();
                if (act != null)
                    SetValueFuncs.Add(pMember.Name, act);
            }
        }

#if NET || NETSTANDARD_2_0_UP
        internal void InvokeOnSerializing(object o, StreamingContext context)
        {
            if (_onSerializingCallbacks != null)
                foreach (SerializationCallback callback in _onSerializingCallbacks)
                    callback(o, context);
        }

        internal void InvokeOnSerialized(object o, StreamingContext context)
        {
            if (_onSerializedCallbacks != null)
                foreach (SerializationCallback callback in _onSerializedCallbacks)
                    callback(o, context);
        }

        internal void InvokeOnDeserializing(object o, StreamingContext context)
        {
            if (_onDeserializingCallbacks != null)
                foreach (SerializationCallback callback in _onDeserializingCallbacks)
                    callback(o, context);
        }

        internal void InvokeOnDeserialized(object o, StreamingContext context)
        {
            if (_onDeserializedCallbacks != null)
                foreach (SerializationCallback callback in _onDeserializedCallbacks)
                    callback(o, context);
        }

        private List<SerializationCallback> _onDeserializedCallbacks;

        private List<SerializationCallback> _onDeserializingCallbacks;

        private List<SerializationCallback> _onSerializedCallbacks;

        private List<SerializationCallback> _onSerializingCallbacks;

        /// <summary>
        ///     Gets or sets all methods called immediately after deserialization of the object.
        /// </summary>
        /// <value>The methods called immediately after deserialization of the object.</value>
        public IList<SerializationCallback> OnDeserializedCallbacks
        {
            get
            {
                if (_onDeserializedCallbacks == null)
                    _onDeserializedCallbacks = new List<SerializationCallback>();

                return _onDeserializedCallbacks;
            }
        }

        /// <summary>
        ///     Gets or sets all methods called during deserialization of the object.
        /// </summary>
        /// <value>The methods called during deserialization of the object.</value>
        public IList<SerializationCallback> OnDeserializingCallbacks
        {
            get
            {
                if (_onDeserializingCallbacks == null)
                    _onDeserializingCallbacks = new List<SerializationCallback>();

                return _onDeserializingCallbacks;
            }
        }

        /// <summary>
        ///     Gets or sets all methods called after serialization of the object graph.
        /// </summary>
        /// <value>The methods called after serialization of the object graph.</value>
        public IList<SerializationCallback> OnSerializedCallbacks
        {
            get
            {
                if (_onSerializedCallbacks == null)
                    _onSerializedCallbacks = new List<SerializationCallback>();

                return _onSerializedCallbacks;
            }
        }

        /// <summary>
        ///     Gets or sets all methods called before serialization of the object.
        /// </summary>
        /// <value>The methods called before serialization of the object.</value>
        public IList<SerializationCallback> OnSerializingCallbacks
        {
            get
            {
                if (_onSerializingCallbacks == null)
                    _onSerializingCallbacks = new List<SerializationCallback>();

                return _onSerializingCallbacks;
            }
        }

        private void ResolveCallbackMethods(Type t)
        {
            List<SerializationCallback> onSerializing;
            List<SerializationCallback> onSerialized;
            List<SerializationCallback> onDeserializing;
            List<SerializationCallback> onDeserialized;

            GetCallbackMethodsForType(t, out onSerializing, out onSerialized, out onDeserializing, out onDeserialized);

            if (onSerializing != null)
                OnSerializingCallbacks.AddRange(onSerializing);

            if (onSerialized != null)
                OnSerializedCallbacks.AddRange(onSerialized);

            if (onDeserializing != null)
                OnDeserializingCallbacks.AddRange(onDeserializing);

            if (onDeserialized != null)
                OnDeserializedCallbacks.AddRange(onDeserialized);
        }

        private void GetCallbackMethodsForType(Type type, out List<SerializationCallback> onSerializing,
            out List<SerializationCallback> onSerialized, out List<SerializationCallback> onDeserializing,
            out List<SerializationCallback> onDeserialized)
        {
            onSerializing = null;
            onSerialized = null;
            onDeserializing = null;
            onDeserialized = null;

            foreach (Type baseType in GetClassHierarchyForType(type))
            {
                // while we allow more than one OnSerialized total, only one can be defined per class
                MethodInfo currentOnSerializing = null;
                MethodInfo currentOnSerialized = null;
                MethodInfo currentOnDeserializing = null;
                MethodInfo currentOnDeserialized = null;

                foreach (MethodInfo method in baseType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public |
                                                                  BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    // compact framework errors when getting parameters for a generic method
                    // lame, but generic methods should not be callbacks anyway
                    if (method.ContainsGenericParameters)
                        continue;

                    Type prevAttributeType = null;
                    ParameterInfo[] parameters = method.GetParameters();

                    if (IsValidCallback(method, parameters, typeof(OnSerializingAttribute), currentOnSerializing,
                            ref prevAttributeType) && !ReflectionUtils.ShouldSkipSerializing(method.DeclaringType))
                    {
                        onSerializing = onSerializing ?? new List<SerializationCallback>();
                        onSerializing.Add(CreateSerializationCallback(method));
                        currentOnSerializing = method;
                    }
                    if (IsValidCallback(method, parameters, typeof(OnSerializedAttribute), currentOnSerialized,
                        ref prevAttributeType))
                    {
                        onSerialized = onSerialized ?? new List<SerializationCallback>();
                        onSerialized.Add(CreateSerializationCallback(method));
                        currentOnSerialized = method;
                    }
                    if (IsValidCallback(method, parameters, typeof(OnDeserializingAttribute), currentOnDeserializing,
                        ref prevAttributeType))
                    {
                        onDeserializing = onDeserializing ?? new List<SerializationCallback>();
                        onDeserializing.Add(CreateSerializationCallback(method));
                        currentOnDeserializing = method;
                    }
                    if (IsValidCallback(method, parameters, typeof(OnDeserializedAttribute), currentOnDeserialized,
                            ref prevAttributeType) && !ReflectionUtils.ShouldSkipDeserialized(method.DeclaringType))
                    {
                        onDeserialized = onDeserialized ?? new List<SerializationCallback>();
                        onDeserialized.Add(CreateSerializationCallback(method));
                        currentOnDeserialized = method;
                    }
                }
            }
        }

        internal static SerializationCallback CreateSerializationCallback(MethodInfo callbackMethodInfo)
        {
            return (o, context) => callbackMethodInfo.Invoke(o, new object[] {context});
        }

        private static bool IsValidCallback(MethodInfo method, ParameterInfo[] parameters, Type attributeType,
            MethodInfo currentCallback, ref Type prevAttributeType)
        {
            if (!method.IsDefined(attributeType, false))
                return false;
            if (currentCallback != null || prevAttributeType != null || method.IsVirtual ||
                method.ReturnType != typeof(void))
                return false;
            if (parameters == null || parameters.Length != 1 || parameters[0].ParameterType != typeof(StreamingContext))
                return false;

            prevAttributeType = attributeType;

            return true;
        }

        private List<Type> GetClassHierarchyForType(Type type)
        {
            List<Type> ret = new List<Type>();

            Type current = type;
            while (current != null && current != typeof(object))
            {
                ret.Add(current);
                current = current.BaseType();
            }

            // Return the class list in order of simple => complex
            ret.Reverse();
            return ret;
        }
#endif
    }

#if NET || NETSTANDARD_2_0_UP
    internal delegate void SerializationCallback(object o, StreamingContext context);
#endif
}