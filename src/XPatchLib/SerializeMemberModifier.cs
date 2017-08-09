// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;

namespace XPatchLib
{
    /// <summary>
    ///     指定产生增量时类或结构中哪些修饰符的成员参与序列化。
    /// </summary>
    [Flags]
    public enum SerializeMemberModifier
    {
        /// <summary>
        ///     公用
        /// </summary>
        /// <summary>
        ///     https://docs.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/public
        /// </summary>
        Public = 1,

        /// <summary>
        ///     专用
        /// </summary>
        /// <summary>
        ///     https://docs.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/private
        /// </summary>
        Private = 2,

        /// <summary>
        ///     受保护
        /// </summary>
        /// <summary>
        ///     https://docs.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/protected
        /// </summary>
        Protected = 4,

        /// <summary>
        ///     内部
        /// </summary>
        /// <summary>
        ///     https://docs.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/internal
        /// </summary>
        Internal = 8,

        /// <summary>
        ///     非公用
        /// </summary>
        /// <summary>Private,Protected,Internal</summary>
        NonPublic = Private | Protected | Internal,

        /// <summary>
        ///     全部
        /// </summary>
        All = Public | NonPublic
    }
}