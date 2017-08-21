// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if NET_40_UP || NETSTANDARD_2_0_UP
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

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

        internal static object GetMemberValue(this DynamicObject o, string memberName)
        {
            if (o == null) return null;
            var binder = Binder.GetMember(CSharpBinderFlags.None, memberName, o.GetType(),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
            return callsite.Target(callsite, o);
        }

        internal static void SetMemberValue(this DynamicObject o, string memberName, object value)
        {
            if (o == null) return;
            var binder = Binder.SetMember(CSharpBinderFlags.None, memberName,
            typeof(object),
            new List<CSharpArgumentInfo>{
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
            });
            var callsite = CallSite<Func<CallSite, object, object, object>>.Create(binder);
            callsite.Target(callsite, o, value);
        }
    }
}
#endif