// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;

namespace XPatchLib
{
    /// <summary>
    ///     写入器的默认设置。
    /// </summary>
    public abstract class SerializeSetting : ISerializeSetting, INotifyPropertyChanged
    {
        private SerializeMemberType _memberType = SerializeMemberType.All;

        /// <summary>
        ///     获取或设置在产生增量时类或结构中哪些成员参与序列化。
        /// </summary>
        /// <value>默认为 <see cref="SerializeMemberType.All" />。</value>
        [Obsolete("暂时无效，暂时会获取所有Property和Field，但是必须同时存在Get,Set")]
        public virtual SerializeMemberType MemberType
        {
            get { return _memberType; }
            set
            {
                if (_memberType != value)
                {
                    _memberType = value;
                    OnPropertyChanged(nameof(MemberType));
                }
            }
        }

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
        public virtual DateTimeSerializationMode Mode
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
        /// <example>
        ///     <include file='docs/docs.xml'
        ///         path='Comments/examples/example[@class="XmlSerializeSetting" and @property="SerializeDefalutValue"]/*' />
        /// </example>
        public virtual bool SerializeDefalutValue
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
        ///     默认值是 <c>Action</c> 。
        /// </value>
        /// <exception cref="ArgumentNullException">当设置值是传入 <b>null</b> 时。</exception>
        /// <exception cref="ArgumentException">当设置值为空时。</exception>
        /// <example>
        ///     <include file='docs/docs.xml'
        ///         path='Comments/examples/example[@class="XmlSerializeSetting" and @property="ActionName"]/*' />
        /// </example>
        public virtual string ActionName
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

        /// <summary>
        ///     播发属性变更后事件。
        /// </summary>
        /// <param name="propertyName">被变更的属性名称。</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 指定产生增量时类或结构中哪些成员参与序列化。
    /// </summary>
    public enum SerializeMemberType
    {
        /// <summary>
        /// 只包含属性
        /// </summary>
        /// <summary>https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/properties</summary>
        PropertyOnly,
        /// <summary>
        /// 只包含字段
        /// </summary>
        /// <summary>https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/fields</summary>
        FieldOnly,
        /// <summary>
        /// 属性及字段
        /// </summary>
        All= FieldOnly | PropertyOnly
    }

}