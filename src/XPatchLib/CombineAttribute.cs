// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace XPatchLib
{
    /// <summary>
    ///     数据合并时读取XmlReader临时使用的对象类型，用来记录当前XmlReader的Attributes
    /// </summary>
    internal class CombineAttribute
    {
        private readonly Queue<string> _keys;

        private readonly Queue<object> _values;

        private readonly Queue<int> _valuesHash;

        public int Count { get; private set; }

        public CombineAttribute(Action pAction, int capacity)
        {
            Action = pAction;
            _keys = new Queue<string>();
            _values = new Queue<object>();
            _valuesHash = new Queue<int>();
        }

        public Action Action { get; set; }

        public string[] Keys { get; private set; }

        public int[] ValuesHash { get; private set; }

        public object[] Values { get; private set; }

        public void Add(string key, object value)
        {
            _keys.Enqueue(key);
            _values.Enqueue(value);
            _valuesHash.Enqueue(value.GetHashCode());

            Count++;

            Keys = _keys.ToArray();
            Values = _values.ToArray();
            ValuesHash = _valuesHash.ToArray();
        }
    }
}