// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace XPatchLib
{
    internal static class TypeExtendContainer
    {
        private static readonly Dictionary<int, TypeExtend> InnerDic = new Dictionary<int, TypeExtend>();

        internal static void Clear()
        {
            lock (InnerDic)
            {
                InnerDic.Clear();
            }
        }

        internal static TypeExtend GetTypeExtend(Type pType, Type pIgnoreAttributeType, TypeExtend pParentType = null)
        {
            TypeExtend result = null;
            lock (InnerDic)
            {
                if (!InnerDic.TryGetValue(pType.GetHashCode(), out result))
                {
                    result = new TypeExtend(pType, pIgnoreAttributeType, pParentType);
                    lock (InnerDic)
                    {
                        InnerDic.Add(pType.GetHashCode(), result);
                    }
                }
            }
            return result;
        }
    }
}