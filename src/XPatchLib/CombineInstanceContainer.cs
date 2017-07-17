// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace XPatchLib
{
    internal static class CombineInstanceContainer
    {
        private static readonly Dictionary<TypeExtend, ICombineBase> InnerDic =
            new Dictionary<TypeExtend, ICombineBase>();

        internal static void Clear()
        {
            lock (InnerDic)
            {
                InnerDic.Clear();
            }
        }

        internal static ICombineBase GetCombineInstance(TypeExtend pType)
        {
            ICombineBase result = null;
            lock (InnerDic)
            {
                if (!InnerDic.TryGetValue(pType, out result))
                {
                    if (pType.IsBasicType)
                        result = new CombineBasic(pType);
                    else if (pType.IsIDictionary)
                        result = new CombineIDictionary(pType);
                    else if (pType.IsIEnumerable)
                        result = new CombineIEnumerable(pType);
                    else if (pType.IsGenericType && pType.OriType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                        result = new CombineKeyValuePair(pType);
#if NET || NETSTANDARD_2_0_UP
                    else if (pType.IsISerializable)
                        result=new CombineISerializable(pType);
#endif
                    else
                        result = new CombineObject(pType);

                    lock (InnerDic)
                    {
                        InnerDic.Add(pType, result);
                    }
                }
            }
            return result;
        }
    }
}