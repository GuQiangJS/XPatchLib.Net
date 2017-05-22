// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace XPatchLib
{
    internal static class TypeExtendContainer
    {
        /// <summary>
        ///     第一个int类型，表示当前Type的HashCode，
        ///     Value字典中的Key值，表示当前Type的ParetnType的HashCode
        ///     Value字典中的Value值，表示根据当前Type和ParentType找到的TypeExtend
        /// </summary>
        private static readonly Dictionary<int, Dictionary<int, TypeExtend>> InnerDic =
            new Dictionary<int, Dictionary<int, TypeExtend>>();

        /// <summary>
        ///     记录手工注册的类型主键特性
        /// </summary>
        private static readonly Dictionary<int, PrimaryKeyAttribute> InnerKeyAttributes =
            new Dictionary<int, PrimaryKeyAttribute>();

        internal static void ClearAll()
        {
            lock (InnerDic)
            {
                InnerDic.Clear();
            }
            lock (InnerKeyAttributes)
            {
                InnerKeyAttributes.Clear();
            }
        }

        internal static void ClearTypeExtends()
        {
            lock (InnerDic)
            {
                InnerDic.Clear();
            }
        }

        internal static void ClearKeyAttributes()
        {
            lock (InnerKeyAttributes)
            {
                InnerKeyAttributes.Clear();
            }
        }

        internal static PrimaryKeyAttribute GetPrimaryKeyAttribute(Type pType)
        {
            PrimaryKeyAttribute result = null;
            lock (InnerKeyAttributes)
            {
                int typeHash = pType.GetHashCode();
                InnerKeyAttributes.TryGetValue(typeHash, out result);
            }
            return result;
        }

        internal static void AddPrimaryKeyAttribute(Type pType, PrimaryKeyAttribute pAttribute)
        {
            lock (InnerKeyAttributes)
            {
                int typeHash = pType.GetHashCode();
                if (!InnerKeyAttributes.ContainsKey(typeHash))
                    lock (InnerKeyAttributes)
                    {
                        InnerKeyAttributes.Add(typeHash, pAttribute);
                    }
            }
        }

        internal static TypeExtend GetTypeExtend(Type pType, Type pIgnoreAttributeType, TypeExtend pParentType = null)
        {
            TypeExtend result = null;
            lock (InnerDic)
            {
                int typeHash = pType.GetHashCode();
                int parentTypeHash = pParentType != null ? pParentType.GetHashCode() : 0;

                Dictionary<int, TypeExtend> innerDictionary = null;

                if (!InnerDic.TryGetValue(typeHash, out innerDictionary))
                {
                    innerDictionary = new Dictionary<int, TypeExtend>();
                    lock (InnerDic)
                    {
                        InnerDic.Add(typeHash, innerDictionary);
                    }
                }
                if (!innerDictionary.TryGetValue(parentTypeHash, out result))
                {
                    result = new TypeExtend(pType, pIgnoreAttributeType, pParentType);
                    lock (InnerDic)
                    {
                        innerDictionary.Add(parentTypeHash, result);
                    }
                }
            }
            return result;
        }
    }
}