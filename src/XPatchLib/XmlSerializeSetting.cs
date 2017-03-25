// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;

namespace XPatchLib
{
    /// <summary>
    ///     XML类型写入器的默认设置。
    /// </summary>
    /// <seealso cref="XPatchLib.ISerializeSetting" />
    public class XmlSerializeSetting : ISerializeSetting, INotifyPropertyChanged
    {
        private string _actionName = "Action";

        private DateTimeSerializationMode _mode = DateTimeSerializationMode.RoundtripKind;

        private bool _serializeDefalutValue;

        /// <summary>
        ///     在更改属性值时发生。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     获取或设置在字符串与 <see cref="DateTime" /> 之间转换时，如何处理时间值。
        /// </summary>
        /// <value>默认为 <see cref="DateTimeSerializationMode.RoundtripKind" />。</value>
        public DateTimeSerializationMode Mode
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    OnPropertyChanged(nameof(Mode));
                }
            }
        }

        /// <summary>
        ///     获取或设置是否序列化默认值。
        /// </summary>
        /// <value>默认为 <c>false</c>。</value>
        public bool SerializeDefalutValue
        {
            get { return _serializeDefalutValue; }
            set
            {
                if (_serializeDefalutValue != value)
                {
                    _serializeDefalutValue = value;
                    OnPropertyChanged(nameof(SerializeDefalutValue));
                }
            }
        }

        /// <summary>
        ///     获取或设置序列化/反序列化时，文本中标记 '<b>动作</b>' 的文本。
        /// </summary>
        /// <value>
        ///     默认值是 "<b>Action</b>" 。
        /// </value>
        /// <exception cref="ArgumentNullException">当设置值是传入 <b>null</b> 时。</exception>
        /// <exception cref="ArgumentException">当设置值为空时。</exception>
        public string ActionName
        {
            get { return _actionName; }
            set
            {
                Guard.ArgumentNotNullOrEmpty(value, "value");
                if (_actionName != value)
                {
                    _actionName = value;
                    OnPropertyChanged(nameof(ActionName));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}