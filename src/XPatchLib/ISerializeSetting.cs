// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;

namespace XPatchLib
{
    /// <summary>
    ///     序列化/反序列化时的设置。
    /// </summary>
    /// <seealso cref="SerializeSetting" />
    public interface ISerializeSetting:ICloneable
    {
        /// <summary>
        ///     获取或设置在产生增量时类或结构中哪些类型的成员参与序列化。
        /// </summary>
        SerializeMemberType MemberType { get; set; }

        /// <summary>
        ///     获取或设置在产生增量时类或结构中哪些修饰符的成员参与序列化。
        /// </summary>
        SerializeMemberModifier Modifier { get; set; }

        /// <summary>
        ///     获取或设置序列化/反序列化时，文本中标记 '<b>动作</b>' 的文本。
        /// </summary>
        string ActionName { get; set; }

        /// <summary>
        ///     获取或设置在字符串与 <see cref="DateTime" /> 之间转换时，如何处理时间值。
        /// </summary>
        DateTimeSerializationMode Mode { get; set; }

        /// <summary>
        ///     获取或设置是否序列化默认值。
        /// </summary>
        bool SerializeDefalutValue { get; set; }

#if NET || NETSTANDARD_2_0_UP
        /// <summary>
        /// 获取或设置序列化时是否支持 <see cref="System.Runtime.Serialization.OnSerializingAttribute"/>。
        /// </summary>
        bool EnableOnSerializingAttribute { get; set; }
        
        /// <summary>
        /// 获取或设置序列化时是否支持 <see cref="System.Runtime.Serialization.OnSerializedAttribute"/>。
        /// </summary>
        bool EnableOnSerializedAttribute { get; set; }

        /// <summary>
        /// 获取或设置反序列化时是否支持 <see cref="System.Runtime.Serialization.OnDeserializedAttribute"/>。
        /// </summary>
        bool EnableOnDeserializedAttribute { get; set; }

        /// <summary>
        /// 获取或设置反序列化时是否支持 <see cref="System.Runtime.Serialization.OnDeserializingAttribute"/>。
        /// </summary>
        bool EnableOnDeserializingAttribute { get; set; }
#endif
    }

#if NETSTANDARD && !NETSTANDARD_2_0_UP
    
    //
    // 摘要:
    //     支持克隆，即用与现有实例相同的值创建类的新实例。
    public interface ICloneable
    {
        //
        // 摘要:
        //     创建作为当前实例副本的新对象。
        //
        // 返回结果:
        //     作为此实例副本的新对象。
        object Clone();
    }
#endif
}