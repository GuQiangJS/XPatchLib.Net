// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace XPatchLib
{
    internal static class CombineInstanceContainer
    {
        private static readonly Dictionary<Type, ICombine> InnerDic =
            new Dictionary<Type, ICombine>();

        internal static void Clear()
        {
            lock (InnerDic)
            {
                InnerDic.Clear();
            }
        }

        internal static ICombine GetCombineInstance(TypeExtend pType)
        {
            ICombine result = null;
            lock (InnerDic)
            {
                if (!InnerDic.TryGetValue(pType.OriType, out result))
                {
                    if (!OtherConverterContainer.TryGetCombine(pType, out result))
                    {
                        if (pType.IsBasicType)
                            result = new ConverterBasic(pType);
                        else if (pType.IsIDictionary)
                            result = new ConverterIDictionary(pType);
                        else if (pType.IsIEnumerable)
                            result = new ConverterIEnumerable(pType);
                        else if (pType.IsGenericType &&
                                 pType.OriType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                            result = new ConverterKeyValuePair(pType);
#if NET || NETSTANDARD_2_0_UP
                        else if (pType.IsISerializable)
                            result = new ConverterISerializable(pType);
#endif
#if NET_40_UP || NETSTANDARD_2_0_UP
                    else if (pType.IsDynamicObject)
                        result = new ConverterDynamic(pType);
#endif
                        else
                            result = new ConverterObject(pType);
                    }

                    lock (InnerDic)
                    {
                        InnerDic.Add(pType.OriType, result);
                    }
                }
            }
            return result;
        }
    }
}