// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace XPatchLib
{
    internal static class TypeExtendContainer
    {
        internal class TypeExtendDic
        {
            private Dictionary<TypeExtend, TypeExtend> _innerDic = new Dictionary<TypeExtend, TypeExtend>();

            private TypeExtend _nullValue = null;
            public bool TryGetValue(TypeExtend parentTypeExtend,out TypeExtend typeExtend)
            {
                if (parentTypeExtend == null)
                {
                    typeExtend = _nullValue;
                    return typeExtend != null;
                }
                else
                {
                    return _innerDic.TryGetValue(parentTypeExtend, out typeExtend);
                }
            }

            public void Add(TypeExtend parentTypeExtend, TypeExtend typeExtend)
            {
                if (parentTypeExtend == null)
                {
                    _nullValue = typeExtend;
                    return;
                }
                _innerDic.Add(parentTypeExtend, typeExtend);
            }
        }

        /// <summary>
        ///     第一个int类型，表示当前Type的HashCode，
        ///     Value字典中的Key值，表示当前Type的ParetnType的HashCode
        ///     Value字典中的Value值，表示根据当前Type和ParentType找到的TypeExtend
        /// </summary>
        private static readonly Dictionary<Type, TypeExtendDic> InnerDic =
            new Dictionary<Type, TypeExtendDic>();

        /// <summary>
        ///     记录手工注册的类型主键特性
        /// </summary>
        private static readonly Dictionary<Type, PrimaryKeyAttribute> InnerKeyAttributes =
            new Dictionary<Type, PrimaryKeyAttribute>();

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
                InnerKeyAttributes.TryGetValue(pType, out result);
            }
            return result;
        }

        internal static void AddPrimaryKeyAttribute(Type pType, PrimaryKeyAttribute pAttribute)
        {
            lock (InnerKeyAttributes)
            {
                if (!InnerKeyAttributes.ContainsKey(pType))
                    lock (InnerKeyAttributes)
                    {
                        InnerKeyAttributes.Add(pType, pAttribute);
                    }
            }
        }

        internal static TypeExtend GetTypeExtend(Type pType, Type pIgnoreAttributeType, TypeExtend pParentType = null)
        {
            TypeExtend result = null;
            lock (InnerDic)
            {
                TypeExtendDic innerDictionary = null;

                if (!InnerDic.TryGetValue(pType, out innerDictionary))
                {
                    innerDictionary = new TypeExtendDic();
                    lock (InnerDic)
                    {
                        InnerDic.Add(pType, innerDictionary);
                    }
                }
                if (!innerDictionary.TryGetValue(pParentType, out result))
                {
                    result = new TypeExtend(pType, pIgnoreAttributeType, pParentType);
                    lock (InnerDic)
                    {
                        innerDictionary.Add(pParentType, result);
                    }
                }
            }
            return result;
        }
    }
}