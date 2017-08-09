// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;

namespace XPatchLib
{
    /// <summary>
    ///     指定产生增量时类或结构中哪些类型的成员参与序列化。
    /// </summary>
    [Flags]
    public enum SerializeMemberType
    {
        /// <summary>
        ///     包含属性
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <c>对于属性来说，必须同时具有Get和Set属性才会参与序列化和反序列化。</c>
        ///     </para>
        ///     <para>https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/properties</para>
        /// </remarks>
        Property = 4,

        /// <summary>
        ///     只包含字段
        /// </summary>
        /// <remarks>
        ///     <para>https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/fields</para>
        /// </remarks>
        Field = 16,

        /// <summary>
        ///     属性及字段
        /// </summary>
        All = Field | Property
    }
}