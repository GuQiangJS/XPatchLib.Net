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

        public string[] Keys { get; private set; }

        public Object OriValue { get; private set; }

        //public Object[] KeyValues { get; private set; }
        public object[] Values { get; private set; }

        private IComparable[] comparableValues;

        public static IEnumerable<KeyValuesObject> Translate(IEnumerable pValue)
        {
            if (pValue != null)
            {
                Queue<KeyValuesObject> result = new Queue<KeyValuesObject>();
                foreach (var VARIABLE in pValue)
                {
                    result.Enqueue(new KeyValuesObject(VARIABLE));
                }
                return result;
            }
            return null;
        }

        public static IEnumerator<KeyValuesObject> TranslateEnumerator(IEnumerable pValue)
        {
            IEnumerable<KeyValuesObject> v = Translate(pValue);
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
                return Equals(Values[0], obj);

            return EqualsByKeys(o.Keys, o.Values);
        }

        //public string[] KeyNames { get; private set; }
        public bool EqualsByKeys(string[] pKeys, object[] pValues)
        {
            if (pKeys == null || pValues == null || Keys.Length != pKeys.Length ||
                Values.Length != pValues.Length)
                return false;

            for (int i = 0; i < Keys.Length; i++)
                if (Keys[i] == pKeys[i]) {
                    if (comparableValues[i] != null) {
                        if (comparableValues[i].CompareTo(pValues[i]) != 0)
                            return false;
                    }
                    else {
                        if (!Values[i].Equals(pValues[i]))
                            return false;
                    }
                }
                else {
                    return false;
                }
            return true;
        }

        // override object.GetHashCode 
        public override int GetHashCode()
        {
            int result = 0;

            for (int i = 0; i < Values.Length; i++)
                result ^= Values[i].GetHashCode();
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
                    if (keyAttr == null)
                    {
                        throw new AttributeMissException(pValue.GetType(), PRIMARY_KEY_MISS);
                    }
                    string[] primaryKeys = keyAttr.GetPrimaryKeys();
                    if (primaryKeys.Length > 0)
                    {
                        Keys = new string[primaryKeys.Length];
                        Values = new object[primaryKeys.Length];
                        comparableValues=new IComparable[Values.Length];
                        for (int i = 0; i < primaryKeys.Length; i++)
                        {
                            Keys[i] = primaryKeys[i];
                            Values[i] = typeExtend.GetMemberValue(pValue, primaryKeys[i]);
                            comparableValues[i] = Values[i] as IComparable;
                        }
                    }
                    else
                    {
                        throw new AttributeMissException(pValue.GetType(), PRIMARY_KEY_MISS);
                    }
                }
                else
                {
                    Keys = new [] {string.Empty};
                    Values = new[] {pValue};
                    comparableValues = new[] {pValue as IComparable};
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