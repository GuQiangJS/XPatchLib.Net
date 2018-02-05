// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
#if (NET || NETSTANDARD_2_0_UP)
using System.Xml.Serialization;

#endif

namespace XPatchLib
{
    /// <summary>
    ///     XML类型写入器的默认设置。
    /// </summary>
    /// <seealso cref="XPatchLib.ISerializeSetting" />
    public class XmlSerializeSetting : SerializeSetting
    {
        /// <summary>创建作为当前实例副本的新对象。</summary>
        /// <returns>作为此实例副本的新对象。</returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            XmlSerializeSetting result = new XmlSerializeSetting();
            result.MemberType = MemberType;
#if NET || NETSTANDARD_2_0_UP
            result.EnableOnDeserializedAttribute = EnableOnDeserializedAttribute;
            result.EnableOnSerializedAttribute = EnableOnSerializedAttribute;
            result.EnableOnDeserializingAttribute = EnableOnDeserializingAttribute;
            result.EnableOnSerializingAttribute = EnableOnSerializingAttribute;
#endif
            result.ActionName = ActionName;
            result.Mode = Mode;
            result.Modifier = Modifier;
            result.SerializeDefalutValue = SerializeDefalutValue;
            result.IgnoreAttributeType = IgnoreAttributeType;
#if NET_40_UP || NETSTANDARD_2_0_UP
            result.AssemblyQualifiedName = this.AssemblyQualifiedName;
#endif
            return result;
        }

        private Type _ignoreAttributeType =
#if (NET || NETSTANDARD_2_0_UP)
            typeof(XmlIgnoreAttribute);
#else
        null;
#endif

#if NET || NETSTANDARD_2_0_UP
        /// <summary>
        ///     获取或设置指示<see cref="Serializer" /> 方法<see cref="Serializer.Divide" /> 进行序列化的公共字段或公共读 / 写属性值。
        /// </summary>
        /// <remarks>
        ///     用于控制如何<see cref="Serializer" /> 方法 <see cref="Serializer.Divide" /> 序列化对象。
        /// </remarks>
        /// <example>
        ///     <include file='docs/docs.xml'
        ///         path='Comments/examples/example[@class="XmlSerializeSetting" and @property="IgnoreAttributeType"]/*' />
        /// </example>
        /// <value>
        ///     默认值：
        ///     <see cref="System.Xml.Serialization.XmlIgnoreAttribute" />。
        /// </value>
#else
        /// <summary>
        /// 获取或设置指示<see cref="Serializer" /> 方法<see cref= "Serializer.Divide" /> 进行序列化的公共字段或公共读 / 写属性值。
        /// </summary>
        /// <remarks>
        /// 用于控制如何<see cref="Serializer" /> 方法 <see cref = "Serializer.Divide" /> 序列化对象。
        /// </remarks>
        /// <example>
        /// <include file='docs/docs.xml' path='Comments/examples/example[@class="XmlSerializeSetting" and @property="IgnoreAttributeType"]/*'/>
        /// </example>
        /// <value>
        /// 默认值：
        /// <c>null</c>。
        /// </value>
#endif
        public override Type IgnoreAttributeType
        {
            get { return _ignoreAttributeType; }
            set
            {
                if (_ignoreAttributeType != value)
                {
                    _ignoreAttributeType = value;
                    OnPropertyChanged(nameof(IgnoreAttributeType));
                }
            }
        }
    }
}