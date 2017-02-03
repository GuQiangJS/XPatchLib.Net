// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace XPatchLib
{
    internal class KeyValuesObject
    {
        private static readonly int EMPTY_STRING_HASHCODE = string.Empty.GetHashCode();

        public KeyValuesObject(Object pValue, Object pKey)
        {
            OriValue = pValue;
            SetKeyValues(pKey);
        }

        public KeyValuesObject(Object pValue)
        {
            OriValue = pValue;
            SetKeyValues(pValue);
        }

        public int[] KeysHash { get; private set; }

        public Object OriValue { get; private set; }

        //public Object[] KeyValues { get; private set; }
        public int[] ValuesHash { get; private set; }

        public static KeyValuesObject[] Translate(IEnumerable pValue)
        {
            if (pValue != null)
            {
                Queue<KeyValuesObject> result = new Queue<KeyValuesObject>();

                IEnumerator enumerator = pValue.GetEnumerator();
                if (enumerator != null)
                    while (enumerator.MoveNext())
                        result.Enqueue(new KeyValuesObject(enumerator.Current));
                return result.ToArray();
            }
            return null;
        }

        // override object.Equals 
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            KeyValuesObject o = obj as KeyValuesObject;
            if (o == null)
                return Equals(ValuesHash[0], obj.GetHashCode());

            return EqualsByKeys(o.KeysHash, o.ValuesHash);
        }

        //public string[] KeyNames { get; private set; }
        public bool EqualsByKeys(int[] pKeys, int[] pValues)
        {
            if (pKeys == null || pValues == null || KeysHash.Length != pKeys.Length ||
                ValuesHash.Length != pValues.Length)
                return false;

            for (int i = 0; i < KeysHash.Length; i++)
                if (KeysHash[i] != pKeys[i] || ValuesHash[i] != pValues[i])
                    return false;
            return true;
        }

        // override object.GetHashCode 
        public override int GetHashCode()
        {
            int result = 0;

            for (int i = 0; i < ValuesHash.Length; i++)
                result ^= ValuesHash[i];
            return result;
        }

        private void SetKeyValues(Object pValue)
        {
            if (pValue != null)
            {
                TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(pValue.GetType(), null);
                if (!typeExtend.IsBasicType)
                {
                    PrimaryKeyAttribute keyAttr = typeExtend.PrimaryKeyAttr;
                    string[] primaryKeys = keyAttr.GetPrimaryKeys();
                    if (primaryKeys.Length > 0)
                    {
                        KeysHash = new int[primaryKeys.Length];
                        ValuesHash = new int[primaryKeys.Length];
                        for (int i = 0; i < primaryKeys.Length; i++)
                        {
                            KeysHash[i] = primaryKeys[i].GetHashCode();
                            ValuesHash[i] = typeExtend.GetMemberValue(pValue, primaryKeys[i]).GetHashCode();
                        }
                    }
                    else
                    {
                        throw new AttributeMissException(pValue.GetType(), "PrimaryKeyAttribute");
                    }
                }
                else
                {
                    KeysHash = new int[1] {EMPTY_STRING_HASHCODE};
                    ValuesHash = new int[1] {pValue.ToString().GetHashCode()};
                }
            }
        }
    }

    internal class KeyValuesObjectEqualityComparer : IEqualityComparer<KeyValuesObject>
    {
        public bool Equals(KeyValuesObject x, KeyValuesObject y)
        {
            return KeyValuesObject.Equals(x, y);
        }

        public int GetHashCode(KeyValuesObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            return obj.GetHashCode();
        }
    }
}