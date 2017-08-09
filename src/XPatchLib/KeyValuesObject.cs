// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace XPatchLib
{
    internal class KeyValuesObject
    {
        private static readonly string PRIMARY_KEY_MISS = typeof(PrimaryKeyAttribute).Name;

        public KeyValuesObject(Object pValue, Object pKey, ISerializeSetting pSetting)
        {
            OriValue = pValue;
            SetKeyValues(pKey, pSetting);
        }

        public KeyValuesObject(Object pValue, ISerializeSetting pSetting)
        {
            OriValue = pValue;
            SetKeyValues(pValue, pSetting);
        }

        public string[] Keys { get; private set; }

        public Object OriValue { get; private set; }

        //public Object[] KeyValues { get; private set; }
        public int[] ValuesHash { get; private set; }

        public static IEnumerable<KeyValuesObject> Translate(IEnumerable pValue,ISerializeSetting pSetting)
        {
            if (pValue != null)
            {
                Queue<KeyValuesObject> result = new Queue<KeyValuesObject>();
                foreach (var VARIABLE in pValue)
                    result.Enqueue(new KeyValuesObject(VARIABLE, pSetting));
                return result;
            }
            return null;
        }

        public static IEnumerator<KeyValuesObject> TranslateEnumerator(IEnumerable pValue, ISerializeSetting pSetting)
        {
            IEnumerable<KeyValuesObject> v = Translate(pValue, pSetting);
            if (v != null)
                return v.GetEnumerator();
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

            return EqualsByKeys(o.Keys, o.ValuesHash);
        }

        //public string[] KeyNames { get; private set; }
        public bool EqualsByKeys(string[] pKeys, int[] pValuesHash)
        {
            if (pKeys == null || ValuesHash == null || Keys.Length != pKeys.Length ||
                ValuesHash.Length != pValuesHash.Length)
                return false;

            for (int i = 0; i < Keys.Length; i++)
                if (!string.Equals(Keys[i], pKeys[i]) || ValuesHash[i] != pValuesHash[i])
                    return false;
            return true;
        }

        public bool EqualsByCombineAttr(CombineAttribute attrs)
        {
            if (attrs == null || ValuesHash == null || Keys.Length != attrs.Count ||
                ValuesHash.Length != attrs.Count)
                return false;

            for (int i = 0; i < attrs.Count; i++)
                if (!string.Equals(Keys[i], attrs.Keys[i]) || ValuesHash[i] != attrs.ValuesHash[i])
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

        private void SetKeyValues(Object pValue, ISerializeSetting pSetting)
        {
            if (pValue != null)
            {
                TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(pSetting, pValue.GetType(), null);
                if (!typeExtend.IsBasicType)
                {
                    PrimaryKeyAttribute keyAttr = typeExtend.PrimaryKeyAttr;
                    if (keyAttr == null)
                        throw new AttributeMissException(pValue.GetType(), PRIMARY_KEY_MISS);
                    string[] primaryKeys = keyAttr.GetPrimaryKeys();
                    if (primaryKeys.Length > 0)
                    {
                        Keys = new string[primaryKeys.Length];
                        ValuesHash = new int[primaryKeys.Length];
                        for (int i = 0; i < primaryKeys.Length; i++)
                        {
                            Keys[i] = primaryKeys[i];
                            ValuesHash[i] = typeExtend.GetMemberValue(pValue, primaryKeys[i]).GetHashCode();
                        }
                    }
                    else
                    {
                        throw new AttributeMissException(pValue.GetType(), PRIMARY_KEY_MISS);
                    }
                }
                else
                {
                    Keys = new[] {string.Empty};
                    ValuesHash = new[] {pValue.GetHashCode()};
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