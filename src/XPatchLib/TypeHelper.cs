// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace XPatchLib
{
    /// <summary>
    ///     <see cref="System.Type" /> 类型扩展。
    /// </summary>
    internal static class TypeHelper
    {
        internal static Object CreateInstance(Type t, params object[] args)
        {
            ConstructorInfo constructorInfo = null;
            Type[] ts = args != null ? new Type[args.Length] : new Type[0];
            if (args != null && args.Length > 0)
                for (int i = 0; i < args.Length; i++)
                    ts[i] = args[i] != null ? args[i].GetType() : typeof(object);
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.CreateInstance |
                                 BindingFlags.Instance | BindingFlags.Public;
            constructorInfo = t.GetConstructor(flags, null, ts, null);
#if (NET || NETSTANDARD_2_0_UP)
            if (constructorInfo != null)
                return constructorInfo.Invoke(args);
            return Activator.CreateInstance(t, args);
#else
                if (constructorInfo != null)
                {
                    ClrHelper.MethodCall<object, object> call = t.CreateMethodCall<object>(constructorInfo);
                    return call.Invoke(null, args);
                }
                return Activator.CreateInstance(t, args);
#endif
        }
    }
}