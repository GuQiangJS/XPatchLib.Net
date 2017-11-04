// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if NET_40_UP || NETSTANDARD_2_0_UP
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
#if NET_40_UP
using Microsoft.CSharp.RuntimeBinder;
#else
using System.Reflection;
#endif

namespace XPatchLib
{
    internal static class DynamicObjectHelper
    {
        internal static List<string> GetMembers(this DynamicObject[] o)
        {
            return o.GetMembers(new string[] { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="excepts">排除的名称</param>
        /// <returns></returns>
        internal static List<string> GetMembers(this DynamicObject[] o, string[] excepts)
        {
            List<string> result = new List<string>();
            foreach (DynamicObject dynamicObject in o)
            {
                if (dynamicObject == null) continue;
                IEnumerable<string> s = dynamicObject.GetDynamicMemberNames();
                foreach (string s1 in s)
                {
                    if (!result.Contains(s1) && !excepts.Contains(s1))
                        result.Add(s1);
                }
            }
            return result;
        }

#if NETSTANDARD_2_0_UP
        private static bool _init;
        private const string BinderTypeName = "Microsoft.CSharp.RuntimeBinder.Binder, " + CSharpAssemblyName;
        public const string CSharpAssemblyName = "Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
        private static object _getCSharpArgumentInfoArray;
        private const string CSharpArgumentInfoTypeName = "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo, " + CSharpAssemblyName;
        private const string CSharpArgumentInfoFlagsTypeName = "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags, " + CSharpAssemblyName;
        private const string CSharpBinderFlagsTypeName = "Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags, " + CSharpAssemblyName;
        private static ClrHelper.MethodCall<object, object> _getMemberCall;
        private static ClrHelper.MethodCall<object, object> _setMemberCall;
        private static object _setCSharpArgumentInfoArray;

        private static void CreateMemberCalls()
        {
            Type csharpArgumentInfoType = Type.GetType(CSharpArgumentInfoTypeName, true);
            Type csharpBinderFlagsType = Type.GetType(CSharpBinderFlagsTypeName, true);
            Type binderType = Type.GetType(BinderTypeName, true);

            Type csharpArgumentInfoTypeEnumerableType = typeof(IEnumerable<>).MakeGenericType(csharpArgumentInfoType);

            MethodInfo getMemberMethod = binderType.GetMethod("GetMember", new[] { csharpBinderFlagsType, typeof(string), typeof(Type), csharpArgumentInfoTypeEnumerableType });
            _getMemberCall = csharpBinderFlagsType.CreateMethodCall<object>(getMemberMethod);

            MethodInfo setMemberMethod = binderType.GetMethod("SetMember", new[] { csharpBinderFlagsType, typeof(string), typeof(Type), csharpArgumentInfoTypeEnumerableType });
            _setMemberCall = csharpBinderFlagsType.CreateMethodCall<object>(setMemberMethod);
        }

        private static object CreateSharpArgumentInfoArray(params int[] values)
        {
            Type csharpArgumentInfoType = Type.GetType(CSharpArgumentInfoTypeName);
            Type csharpArgumentInfoFlags = Type.GetType(CSharpArgumentInfoFlagsTypeName);

            Array a = Array.CreateInstance(csharpArgumentInfoType, values.Length);

            for (int i = 0; i < values.Length; i++)
            {
                MethodInfo createArgumentInfoMethod = csharpArgumentInfoType.GetMethod("Create", new[] { csharpArgumentInfoFlags, typeof(string) });
                object arg = createArgumentInfoMethod.Invoke(null, new object[] { 0, null });
                a.SetValue(arg, i);
            }

            return a;
        }

        private static void Init()
        {
            if (!_init)
            {
                Type binderType = Type.GetType(BinderTypeName, false);
                if (binderType == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Could not resolve type '{0}'. You may need to add a reference to Microsoft.CSharp.dll to work with dynamic types.",
                            CultureInfo.InvariantCulture, BinderTypeName));
                }

                // None
                _getCSharpArgumentInfoArray = CreateSharpArgumentInfoArray(0);
                // None, Constant | UseCompileTimeType
                _setCSharpArgumentInfoArray = CreateSharpArgumentInfoArray(0, 3);
                CreateMemberCalls();

                _init = true;
            }
        }
#endif

        internal static object GetMemberValue(this DynamicObject o, string memberName)
        {
            if (o == null) return null;
#if NET_40_UP
            var binder = Binder.GetMember(CSharpBinderFlags.None, memberName, o.GetType(),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
#else
            Init();
            var binder = (CallSiteBinder)_getMemberCall(null, 0, memberName, o.GetType(), _getCSharpArgumentInfoArray);
#endif
            var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
            return callsite.Target(callsite, o);
        }

        internal static void SetMemberValue(this DynamicObject o, string memberName, object value)
        {
            if (o == null)
                return;
#if NET_40_UP
            var binder = Binder.SetMember(CSharpBinderFlags.None, memberName,
            typeof(object),
            new List<CSharpArgumentInfo>{
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
            });
#else
            Init();
            var binder = (CallSiteBinder)_setMemberCall(null, 0, memberName, o.GetType(), _setCSharpArgumentInfoArray);
#endif
            var callsite = CallSite<Func<CallSite, object, object, object>>.Create(binder);
            callsite.Target(callsite, o, value);
        }
    }
}
#endif