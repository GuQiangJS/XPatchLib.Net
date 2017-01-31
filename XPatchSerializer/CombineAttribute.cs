// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace XPatchLib
{
    /// <summary>
    ///     数据合并时读取XmlReader临时使用的对象类型，用来记录当前XmlReader的Attributes
    /// </summary>
    internal struct CombineAttribute
    {
        public CombineAttribute(Action pAction)
        {
            Action = pAction;
            KeysValuePairs = new Dictionary<string, string>();
        }

        public Action Action { get; set; }

        public IDictionary<string, string> KeysValuePairs { get; }

        public int[] Keys
        {
            get
            {
                Queue<int> result = new Queue<int>(KeysValuePairs.Count);
                foreach (var variable in KeysValuePairs.Keys)
                    result.Enqueue(variable.GetHashCode());
                return result.ToArray();
            }
        }

        public int[] Values
        {
            get
            {
                Queue<int> result = new Queue<int>(KeysValuePairs.Count);
                foreach (var variable in KeysValuePairs.Values)
                    result.Enqueue(variable.GetHashCode());
                return result.ToArray();
            }
        }
    }
}