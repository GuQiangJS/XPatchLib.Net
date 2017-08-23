// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace XPatchLib
{
    /// <summary>
    ///     表示提供对数据进行快速、非缓存、只进访问的读取器。
    /// </summary>
    public interface ITextReader : IDisposable
    {
        /// <summary>
        ///     获取或设置读取器设置。
        /// </summary>
        ISerializeSetting Setting { get; set; }

        /// <summary>
        ///     获取读取器的状态。
        /// </summary>
        ReadState ReadState { get; }

        /// <summary>
        ///     获取一个值，该值指示此读取器是否定位在流的结尾。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "EOF")]
        bool EOF { get; }

        /// <summary>
        ///     获取当前节点的限定名。
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     获取当前节点的文本值。
        /// </summary>
        /// <returns>返回当前节点的文本值。</returns>
        string GetValue();

        /// <summary>
        ///     获取当前节点的类型。
        /// </summary>
        NodeType NodeType { get; }

        ///// <summary>
        ///// 获取当前解析的节点是否包含特性节点。
        ///// </summary>
        //Boolean HasAttribute { get; }

        /// <summary>
        ///     获取当前节点的特性名称与值的键值对数组。
        /// </summary>
        /// <param name="name">当前读取的节点的名称。</param>
        /// <param name="names">待读取的特性名称。</param>
        /// <returns>返回当前节点的特性名称与值的键值对数组。</returns>
        string[,] GetAttributes(string name,string[] names);

        /// <summary>
        ///     从流中读取下一个节点。
        /// </summary>
        /// <returns>如果成功读取了下一个节点，则为 <c>true</c>；如果没有其他节点可读取，则为 <c>false</c>。</returns>
        bool Read();
    }
}