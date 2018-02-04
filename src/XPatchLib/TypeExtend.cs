// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
#if (NET_40_UP || NETSTANDARD_2_0_UP)
using System.Dynamic;
#endif
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
        private readonly Boolean _isNullable;
        private readonly bool _isGenericType;
        private readonly bool _isGuid;
        private readonly bool _isICollection;
        private readonly bool _isIDictionary;
        private readonly bool _isIEnumerable;
        private readonly bool _isKeyValuePair;
        private readonly Type _keyArgumentType;
        private readonly Type _oriType;
        private PrimaryKeyAttribute _primaryKeyAttr;
        private readonly TypeCode _typeCode;
        private readonly string _typeFriendlyName;
        private readonly Type _valueArgumentType;
        private readonly object _defaultValue;
        private IEnumerable<Attribute> _customAttributes;
        private readonly TypeExtend _parentType;
        private readonly MemberWrapper[] _fieldsToBeSerialized;
        private readonly bool _isArrayItem;
        private readonly bool _isArray;
        private readonly bool _isDynamicObject;
        private readonly Type[] _interfaceTypes;
        private readonly bool _isConcurrentDictionary;
        private readonly bool _isConcurrentQueue;
        private readonly bool _isConcurrentStack;
        private readonly bool _isConcurrentBag;
        private readonly bool _isQueue;
        private readonly bool _isStack;

        public Type[] InterfaceTypes
        {
            get { return _interfaceTypes; }
        }

        public Boolean IsStack
        {
            get { return _isStack; }
        }

        public Boolean IsQueue
        {
            get { return _isQueue; }
        }

        public Boolean IsConcurrentStack
        {
            get { return _isConcurrentStack; }
        }

        public Boolean IsConcurrentBag
        {
            get { return _isConcurrentBag; }
        }

        public Boolean IsConcurrentQueue
        {
            get { return _isConcurrentQueue; }
        }

        public Boolean IsConcurrentDictionary
        {
            get { return _isConcurrentDictionary; }
        }

        public Boolean IsNullable
        {
            get { return _isNullable; }
        }

        private readonly Type NullableType;


#if NET || NETSTANDARD_2_0_UP
        private readonly Boolean _isISerializable;

        /// <summary>
        ///     是否为 <see cref="ISerializable" /> 接口的实现
        /// </summary>
        /// <remarks>
        ///     https://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.iserializable(v=vs.110).aspx
        /// </remarks>
        public Boolean IsISerializable
        {
            get { return _isISerializable; }
        }
#endif

        private ISerializeSetting _setting;

        /// <summary>
        ///     获取序列化/反序列化时的设置。
        /// </summary>
        public ISerializeSetting Setting
        {
            get { return _setting; }
        }

        /// <summary>
        /// 获取当前类型可能产生的特性名称集合。
        /// </summary>
        public string[] AttributeNames { get { return _attributeNames; } }

        private string[] _attributeNames;

        internal TypeExtend(ISerializeSetting pSetting,Type pType, Type pIgnoreAttributeType, TypeExtend pParentType = null)
        {
            _parentType = pParentType;
            _setting = pSetting;
            _oriType = pType;

            Type actType;
            _isNullable = ReflectionUtils.IsNullable(pType, out actType);
            if (_isNullable)
                NullableType = pType = actType;

#if (NET_20_UP || NETSTANDARD_2_0_UP)
            CustomAttributes = Attribute.GetCustomAttributes(pType);
#else
            CustomAttributes = pType.GetTypeInfo().GetCustomAttributes().ToArray();
#endif
            _primaryKeyAttr = GetCustomAttribute<PrimaryKeyAttribute>();

            _interfaceTypes = pType.GetInterfaces();

            if (_primaryKeyAttr == null) _primaryKeyAttr = TypeExtendContainer.GetPrimaryKeyAttribute(pType);

            //InitAttributeNames(pSetting,_primaryKeyAttr);

            GetValueFuncs = new Dictionary<string, Func<object, object>>();
            SetValueFuncs = new Dictionary<string, Action<object, object>>();
#if (NET || NETSTANDARD_2_0_UP)
            Methods = new Dictionary<string, MethodInfo>();
#else
            Methods = new Dictionary<string, ClrHelper.MethodCall<object, object>>();
#endif

            _isBasicType = ReflectionUtils.IsBasicType(pType);
            if (!_isBasicType)
            {
                _isIDictionary = ReflectionUtils.IsIDictionary(pType, _interfaceTypes);
                _isICollection = ReflectionUtils.IsICollection(pType, _interfaceTypes);
                _isIEnumerable = ReflectionUtils.IsIEnumerable(pType, _interfaceTypes);
                _isArray = ReflectionUtils.IsArray(pType);
            }
            _defaultValue = ReflectionUtils.GetDefaultValue(pType);
            _isArrayItem = ParentType != null &&
                           (ParentType.IsArray || ParentType.IsICollection ||
                            ParentType.IsIEnumerable);
            _typeCode = ConvertHelper.GetTypeCode(pType);
            _isGenericType = OriType.IsGenericType();
            _isGuid = pType == typeof(Guid);
            _typeFriendlyName = ReflectionUtils.GetTypeFriendlyName(pType);
#if NET_40_UP || NETSTANDARD_2_0_UP
            _isDynamicObject = ReflectionUtils.IsDynamicObject(pType);
            _isConcurrentDictionary = ReflectionUtils.IsConcurrentDictionary(pType, true);
            _isConcurrentQueue=ReflectionUtils.IsConcurrentQueue(pType,true);
            _isConcurrentStack=ReflectionUtils.IsConcurrentStack(pType);
            _isConcurrentBag = ReflectionUtils.IsConcurrentBag(pType);
#endif
            _isQueue = ReflectionUtils.IsQueue(pType);
            _isStack = ReflectionUtils.IsStack(pType);

            _fieldsToBeSerialized = new MemberWrapper[] { };
            if (!(_isIEnumerable || _isIDictionary || _isICollection || _isBasicType || _isArray))
            {
                _fieldsToBeSerialized =
                    ReflectionUtils.GetFieldsToBeSerialized(pSetting, pType, pIgnoreAttributeType);
                foreach (MemberWrapper member in _fieldsToBeSerialized)
                {
                    AddGetValueFunc(member);
                    AddSetValueFunc(member);
                }
            }

            if (_isIDictionary)
            {
                Type keyType = typeof(object);
                Type valueType = typeof(object);
                ReflectionUtils.IsIDictionary(pType, _interfaceTypes, out keyType, out valueType);

                _keyArgumentType = keyType;
                _valueArgumentType = valueType;
            }

            _isKeyValuePair = ReflectionUtils.IsKeyValuePair(pType);
            if (_isKeyValuePair)
            {
                Type keyType = typeof(object);
                Type valueType = typeof(object);
                ReflectionUtils.IsKeyValuePair(pType, out keyType, out valueType);

                _keyArgumentType = keyType;
                _valueArgumentType = valueType;

                MemberWrapper[] members =
                {
                    new MemberWrapper(pType.GetProperty(ConstValue.KEY)),
                    new MemberWrapper(pType.GetProperty(ConstValue.VALUE))
                };
                _fieldsToBeSerialized = members.OrderBy(x => x.Name).ToArray();
            }

            string errorPrimaryKeyName = string.Empty;
            if (!CheckPrimaryKeyAttribute(false, out errorPrimaryKeyName))
                throw new PrimaryKeyException(pType, errorPrimaryKeyName);

            if (!_isBasicType)
                CreateInstanceFuncs = ClrHelper.CreateInstanceFunc<Object>(pType);
#if NET || NETSTANDARD_2_0_UP
            ResolveCallbackMethods(OriType);
#endif

#if NET || NETSTANDARD_2_0_UP
            _isISerializable = typeof(ISerializable).IsAssignableFrom(OriType);
#endif
        }

        private void InitAttributeNames(ISerializeSetting pSetting, PrimaryKeyAttribute primaryKeyAttr)
        {
            string[] app = new[]
            {
                pSetting.ActionName
            };
#if NET_40_UP || NETSTANDARD_2_0_UP
            if (ParentType != null && ParentType.IsDynamicObject)
            {
                app =new []
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
                    _attributeNames = new string[attrLen + app.Length];
                    Array.Copy(attrs, 0, _attributeNames, 0, attrLen);
                }
            }
            if (_attributeNames == null)
            {
                _attributeNames = new string[attrLen + app.Length];
            }
            Array.Copy(app, 0, _attributeNames, attrLen, app.Length);
        }

        /// <summary>
        ///     获取父级类型定义。
        /// </summary>
        internal TypeExtend ParentType
        {
            get { return _parentType; }
        }

        /// <summary>
        ///     获取该类型的自定义Attributes。
        /// </summary>
        internal IEnumerable<Attribute> CustomAttributes
        {
            get { return _customAttributes; }
            private set { _customAttributes = value; }
        }

        /// <summary>
        ///     获取该类型的默认值。
        /// </summary>
        internal Object DefaultValue
        {
            get { return _defaultValue; }
        }

        /// <summary>
        ///     获取该类型下可以被序列化的字段。
        /// </summary>
        internal MemberWrapper[] FieldsToBeSerialized
        {
            get { return _fieldsToBeSerialized; }
        }

        internal Boolean IsArrayItem
        {
            get { return _isArrayItem; }
        }

        internal Boolean IsArray
        {
            get { return _isArray; }
        }

        private readonly bool _isBasicType;

        /// <summary>
        ///     获取是否为基础类型。
        /// </summary>
        internal Boolean IsBasicType
        {
            get { return _isBasicType; }
        }

        internal Boolean IsGenericType
        {
            get { return _isGenericType; }
        }

        internal Boolean IsGuid
        {
            get { return _isGuid; }
        }

        internal Boolean IsICollection
        {
            get { return _isICollection; }
        }

#if NET_40_UP || NETSTANDARD_2_0_UP
        internal Boolean IsDynamicObject
        {
            get { return _isDynamicObject; }
        }
#endif

        internal Boolean IsIDictionary
        {
            get { return _isIDictionary; }
        }

        internal Boolean IsIEnumerable
        {
            get { return _isIEnumerable; }
        }

        internal Boolean IsKeyValuePair
        {
            get { return _isKeyValuePair; }
        }

        /// <summary>
        ///     只有当是字典类型或KeyValue类型时才会有值
        /// </summary>
        internal Type KeyArgumentType
        {
            get { return _keyArgumentType; }
        }

        /// <summary>
        ///     获取原始类型定义。
        /// </summary>
        internal Type OriType
        {
            get { return _oriType; }
        }

        internal PrimaryKeyAttribute PrimaryKeyAttr
        {
            get { return _primaryKeyAttr; }
        }

        internal TypeCode TypeCode
        {
            get { return _typeCode; }
        }

        internal String TypeFriendlyName
        {
            get { return _typeFriendlyName; }
        }

        /// <summary>
        ///     只有当是字典类型或KeyValue类型时才会有值
        /// </summary>
        internal Type ValueArgumentType
        {
            get { return _valueArgumentType; }
        }

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
            if (CreateInstanceFuncs != null && (args == null || args.Length <= 0))
                return CreateInstanceFuncs();

            if (_isBasicType)
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
                if (_isArray)
                {
                    Type elementType;
                    if (ReflectionUtils.TryGetArrayElementType(IsNullable ? NullableType : OriType, out elementType))
                        return Array.CreateInstance(elementType, 0);
                    throw new NotImplementedException();
                }
                ConstructorInfo constructorInfo = null;
                Type[] ts = args != null ? new Type[args.Length] : new Type[0];
                if (args != null && args.Length > 0)
                    for (int i = 0; i < args.Length; i++)
                        ts[i] = args[i] != null ? args[i].GetType() : typeof(object);
                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.CreateInstance |
                                     BindingFlags.Instance | BindingFlags.Public;
                constructorInfo = (IsNullable ? NullableType : OriType).GetConstructor(flags, null, ts, null);
#if (NET || NETSTANDARD_2_0_UP)
                if (constructorInfo != null)
                    return constructorInfo.Invoke(args);
                return Activator.CreateInstance(IsNullable ? NullableType : OriType, args);
#else
                if (constructorInfo != null)
                {
                    ClrHelper.MethodCall<object, object> call =
                        (IsNullable ? NullableType : OriType).CreateMethodCall<object>(constructorInfo);
                    return call.Invoke(null, args);
                }
                return Activator.CreateInstance(IsNullable ? NullableType : OriType, args);
#endif
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
            if (result && IsDefaultValue(defaultValue, objA) && serializeDefaultValue)
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
#if NETSTANDARD_2_0_UP
                    // .NET Core 2.0 开始，字典类型Remove 方法增加了一个可以同时传入 Key 值和 Value 值的方法。
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

            for (int i = 0; i < _fieldsToBeSerialized.Length; i++)
                if (_fieldsToBeSerialized[i].Name.Equals(pPropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    member = _fieldsToBeSerialized[i];
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

            _primaryKeyAttr = pAttribute;
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