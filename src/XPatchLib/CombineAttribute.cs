// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace XPatchLib
{
    /// <summary>
    ///     数据合并时读取XmlReader临时使用的对象类型，用来记录当前XmlReader的Attributes
    /// </summary>
    internal class CombineAttribute
    {
        private string[] _keys;

        private object[] _values;

        private int[] _valuesHash;

        private int _count = 0;

        private int _index = 0;

        public int Count { get { return _index; } }

        public CombineAttribute(Action pAction, int capacity)
        {
            _action = pAction;
            _keys = new string[capacity];
            _values = new object[capacity];
            _valuesHash = new int[capacity];
            _count = capacity;
        }

        private Action _action;

        public string AssemblyQualifiedName { get; set; }

        public Action Action { get { return _action; }set { _action = value; } }

        public string[] Keys { get { return _keys; } }

        public int[] ValuesHash { get { return _valuesHash; } }

        public object[] Values { get { return _values; } }

        public void Set(string key, object value)
        {
            //if (_count == _index)
            //{
            //    _keys = CloneArray(_keys);
            //    _values = CloneArray(_values);
            //    _valuesHash = CloneArray(_valuesHash);
            //}

            _keys[_index] = key;
            _values[_index] = value;
            _valuesHash[_index] = value.GetHashCode();

            _index++;
            //_count++;
        }

        T[] CloneArray<T>(T[] source)
        {
            T[] temp = new T[_count * 2];
            Buffer.BlockCopy(source, 0, temp, 0, _count);
            return temp;
        }
    }
}
