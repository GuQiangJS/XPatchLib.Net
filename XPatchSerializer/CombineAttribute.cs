// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace XPatchLib
{
    /// <summary>
    ///     数据合并时读取XmlReader临时使用的对象类型，用来记录当前XmlReader的Attributes
    /// </summary>
    internal class CombineAttribute
    {
        public CombineAttribute(Action pAction, int capacity)
        {
            Action = pAction;
            _keysValuePairs = new KeyValuePairs<String, Object>(capacity);
            _keysValuePairs.OnAdd += _keysValuePairs_OnAdd;
            _keys = new Queue<int>();
            _values = new Queue<int>();
        }

        void _keysValuePairs_OnAdd(object sender, AddEventArgs<String, Object> e)
        {
            _keys.Enqueue(e.Item.Key.GetHashCode());
            _values.Enqueue(e.Item.Value.GetHashCode());

            //TODO:其他可能未处理
        }

        Queue<int> _keys;
        Queue<int> _values;

        public Action Action { get; set; }

        private KeyValuePairs<String, Object> _keysValuePairs;

        public KeyValuePairs<String, Object> KeysValuePairs
        {
            get
            {
                return _keysValuePairs;
            }
        }

        public int[] Keys
        {
            get
            {
                return _keys.ToArray();
            }
        }

        public int[] Values
        {
            get
            {
                return _values.ToArray();
            }
        }
    }

    internal class AddEventArgs<TKey, TValue> : EventArgs
    {
        public AddEventArgs(TKey key, TValue value)
        {
            Item = new KeyValuePair<TKey, TValue>(key, value);
        }
        public KeyValuePair<TKey, TValue> Item { get; private set; }
    }

    internal class KeyValuePairs<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        List<KeyValuePair<TKey, TValue>> _innerCol;
        public KeyValuePairs() : this(0) { }

        public KeyValuePairs(int capacity)
        {
            _innerCol = new List<KeyValuePair<TKey, TValue>>(capacity);
        }

        public event EventHandler<AddEventArgs<TKey,TValue>> OnAdd;

        public void Add(TKey key, TValue value)
        {
            _innerCol.Add(new KeyValuePair<TKey, TValue>(key, value));
            if (OnAdd != null)
                OnAdd(this, new AddEventArgs<TKey, TValue>(key, value));
        }

        #region IEnumerable<KeyValuePair<TKey,TValue>> 成员

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _innerCol.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerCol.GetEnumerator();
        }

        #endregion
    }
}