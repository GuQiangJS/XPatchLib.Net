using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XPatchLib
{
    internal class KeyValuesObject
    {
        private static int EMPTY_STRING_HASHCODE = string.Empty.GetHashCode();

        public KeyValuesObject(Object pValue, Object pKey)
        {
            this.OriValue = pValue;
            SetKeyValues(pKey);
        }

        public KeyValuesObject(Object pValue)
        {
            this.OriValue = pValue;
            SetKeyValues(pValue);
        }

        public int[] KeysHash { get; private set; }

        public Object OriValue { get; private set; }

        //public Object[] KeyValues { get; private set; }
        public int[] ValuesHash { get; private set; }

        public static IEnumerable<KeyValuesObject> Translate(IEnumerable pValue)
        {
            if (pValue != null)
            {
                Queue<KeyValuesObject> result = new Queue<KeyValuesObject>();

                IEnumerator enumerator = pValue.GetEnumerator();
                if (enumerator != null)
                {
                    while (enumerator.MoveNext())
                    {
                        result.Enqueue(new KeyValuesObject(enumerator.Current));
                    }
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        // override object.Equals 
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            KeyValuesObject o = obj as KeyValuesObject;
            if (o == null)
            {
                return Object.Equals(ValuesHash[0], obj.GetHashCode());
            }

            return EqualsByKeys(o.KeysHash, o.ValuesHash);
        }

        //public string[] KeyNames { get; private set; }
        public bool EqualsByKeys(int[] pKeys, int[] pValues)
        {
            if (pKeys == null || pValues == null || this.KeysHash.Length != pKeys.Length || this.ValuesHash.Length != pValues.Length)
            {
                return false;
            }

            for (int i = 0; i < this.KeysHash.Length; i++)
            {
                if (this.KeysHash[i] != pKeys[i] || this.ValuesHash[i] != pValues[i])
                {
                    return false;
                }
            }
            return true;
        }

        // override object.GetHashCode 
        public override int GetHashCode()
        {
            int result = 0;

            for (int i = 0; i < ValuesHash.Length; i++)
            {
                result ^= ValuesHash[i];
            }
            return result;
        }

        private void SetKeyValues(Object pValue)
        {
            if (pValue != null)
            {
                TypeExtend typeExtend = TypeExtendContainer.GetTypeExtend(pValue.GetType());
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
                    KeysHash = new int[1] { EMPTY_STRING_HASHCODE };
                    ValuesHash = new int[1] { pValue.GetHashCode() };
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
            {
                throw new ArgumentNullException("obj");
            }
            return obj.GetHashCode();
        }
    }
}