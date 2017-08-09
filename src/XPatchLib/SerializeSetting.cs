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
        private string _actionName = "Action";
        private SerializeMemberType _memberType = SerializeMemberType.Property;

        private DateTimeSerializationMode _mode = DateTimeSerializationMode.RoundtripKind;
        private SerializeMemberModifier _modifier = SerializeMemberModifier.Public;

#if NET || NETSTANDARD_2_0_UP
        /// <summary>
        /// 获取或设置序列化时是否支持 <see cref="System.Runtime.Serialization.OnSerializingAttribute"/>。
        /// </summary>
        /// <value>默认值：<c>true</c> 。</value>
        public virtual bool EnableOnSerializingAttribute
        {
            get { return _enableOnSerializingAttribute; }
            set
            {
                if (_enableOnSerializingAttribute != value)
                {
                    _enableOnSerializingAttribute = value;
                    OnPropertyChanged(nameof(EnableOnSerializingAttribute));
                }
            }
        }

        /// <summary>
        /// 获取或设置序列化时是否支持 <see cref="System.Runtime.Serialization.OnSerializedAttribute"/>。
        /// </summary>
        /// <value>默认值：<c>true</c> 。</value>
        public virtual bool EnableOnSerializedAttribute
        {
            get { return _enableOnSerializedAttribute; }
            set
            {
                if (_enableOnSerializedAttribute != value)
                {
                    _enableOnSerializedAttribute = value;
                    OnPropertyChanged(nameof(EnableOnSerializedAttribute));
                }
            }
        }

        /// <summary>
        /// 获取或设置反序列化时是否支持 <see cref="System.Runtime.Serialization.OnDeserializedAttribute"/>。
        /// </summary>
        /// <value>默认值：<c>true</c> 。</value>
        public virtual bool EnableOnDeserializedAttribute
        {
            get { return _enableOnDeserializedAttribute; }
            set
            {
                if (_enableOnDeserializedAttribute != value)
                {
                    _enableOnDeserializedAttribute = value;
                    OnPropertyChanged(nameof(EnableOnDeserializedAttribute));
                }
            }
        }

        /// <summary>
        /// 获取或设置反序列化时是否支持 <see cref="System.Runtime.Serialization.OnDeserializingAttribute"/>。
        /// </summary>
        /// <value>默认值：<c>true</c> 。</value>
        public virtual bool EnableOnDeserializingAttribute
        {
            get { return _enableOnDeserializingAttribute; }
            set
            {
                if (_enableOnDeserializingAttribute != value)
                {
                    _enableOnDeserializingAttribute = value;
                    OnPropertyChanged(nameof(EnableOnDeserializingAttribute));
                }
            }
        }
#endif


        private bool _serializeDefalutValue;
        private bool _enableOnSerializingAttribute = true;
        private bool _enableOnSerializedAttribute = true;
        private bool _enableOnDeserializedAttribute = true;
        private bool _enableOnDeserializingAttribute = true;

        /// <summary>
        ///     在更改属性值时发生。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     获取或设置在产生增量时类或结构中哪些类型的成员参与序列化。
        /// </summary>
        /// <value>默认为 <see cref="SerializeMemberType.Property" />。</value>
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

        /// <summary>
        ///     获取或设置在产生增量时类或结构中哪些修饰符的成员参与序列化。
        /// </summary>
        /// <value>默认为 <see cref="SerializeMemberModifier.Public" />。</value>
        public virtual SerializeMemberModifier Modifier
        {
            get { return _modifier; }
            set
            {
                if (_modifier != value)
                {
                    _modifier = value;
                    OnPropertyChanged(nameof(Modifier));
                }
            }
        }

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

        /// <summary>创建作为当前实例副本的新对象。</summary>
        /// <returns>作为此实例副本的新对象。</returns>
        /// <filterpriority>2</filterpriority>
        public abstract object Clone();
    }
}