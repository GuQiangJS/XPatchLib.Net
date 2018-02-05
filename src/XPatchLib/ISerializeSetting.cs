// Copyright © 2013-2018 - GuQiang55
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;

namespace XPatchLib
{
    /// <summary>
    ///     序列化/反序列化时的设置。
    /// </summary>
    /// <seealso cref="SerializeSetting" />
    public interface ISerializeSetting : ICloneable
    {
        /// <summary>
        ///     获取或设置指示 <see cref="Serializer" /> 方法 <see cref="Serializer.Divide" /> 进行序列化的公共字段或公共读/写属性值。
        /// </summary>
        /// <remarks>
        ///     用于控制如何 <see cref="Serializer" /> 方法 <see cref="Serializer.Divide" /> 序列化对象。
        /// </remarks>
        /// <seealso cref="XmlSerializeSetting.IgnoreAttributeType" />
        Type IgnoreAttributeType { get; set; }

        /// <summary>
        ///     获取或设置在产生增量时类或结构中哪些类型的成员参与序列化。
        /// </summary>
        /// <seealso cref="SerializeSetting.MemberType"/>
        SerializeMemberType MemberType { get; set; }

        /// <summary>
        ///     获取或设置在产生增量时类或结构中哪些修饰符的成员参与序列化。
        /// </summary>
        /// <seealso cref="SerializeSetting.Modifier"/>
        SerializeMemberModifier Modifier { get; set; }

#if NET_40_UP || NETSTANDARD_2_0_UP 
        /// <summary>
        /// 获取或设置序列化/反序列化时，文本中标记 '<b>类型的程序集限定名称</b>' 的文本。
        /// </summary>
        string AssemblyQualifiedName { get; set; }
#endif

        /// <summary>
        ///     获取或设置序列化/反序列化时，文本中标记 '<b>动作</b>' 的文本。
        /// </summary>
        /// <seealso cref="SerializeSetting.ActionName"/>
        string ActionName { get; set; }

        /// <summary>
        ///     获取或设置在字符串与 <see cref="DateTime" /> 之间转换时，如何处理时间值。
        /// </summary>
        /// <seealso cref="SerializeSetting.Mode"/>
        DateTimeSerializationMode Mode { get; set; }

        /// <summary>
        ///     获取或设置是否序列化默认值。
        /// </summary>
        /// <seealso cref="SerializeSetting.SerializeDefalutValue"/>
        bool SerializeDefalutValue { get; set; }

#if NET || NETSTANDARD_2_0_UP
        /// <summary>
        ///     获取或设置序列化时是否支持 <see cref="System.Runtime.Serialization.OnSerializingAttribute" />。
        /// </summary>
        bool EnableOnSerializingAttribute { get; set; }

        /// <summary>
        ///     获取或设置序列化时是否支持 <see cref="System.Runtime.Serialization.OnSerializedAttribute" />。
        /// </summary>
        bool EnableOnSerializedAttribute { get; set; }

        /// <summary>
        ///     获取或设置反序列化时是否支持 <see cref="System.Runtime.Serialization.OnDeserializedAttribute" />。
        /// </summary>
        bool EnableOnDeserializedAttribute { get; set; }

        /// <summary>
        ///     获取或设置反序列化时是否支持 <see cref="System.Runtime.Serialization.OnDeserializingAttribute" />。
        /// </summary>
        bool EnableOnDeserializingAttribute { get; set; }
#endif
    }

#if NETSTANDARD && !NETSTANDARD_2_0_UP 
        /// <summary>
        /// 克隆的支持，这将类的新实例创建与现有实例相同的值。
        /// </summary>
        public interface ICloneable
        {
            /// <summary>
            /// 创建作为当前实例副本的新对象。
            /// </summary>
            /// <returns>作为此实例副本的新对象。</returns>
            object Clone();
        }
#endif
}