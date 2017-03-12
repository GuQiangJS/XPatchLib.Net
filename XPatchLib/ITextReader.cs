// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;

namespace XPatchLib
{
    /// <summary>
    ///     表示提供对数据进行快速、非缓存、只进访问的读取器。
    /// </summary>
    public interface ITextReader : IDisposable
    {
        /// <summary>
        /// 获取或设置在字符串与 <see cref="DateTime"/> 之间转换时，如何处理时间值。
        /// </summary>
        DateTimeSerializationMode Mode { get; set; }

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
        string Value { get; }

        /// <summary>
        ///     获取当前节点上的属性数。
        /// </summary>
        int AttributeCount { get; }

        /// <summary>
        ///     从流中读取下一个节点。
        /// </summary>
        bool Read();

        /// <summary>
        ///     将元素或文本节点的内容当做字符串读取。
        /// </summary>
        string ReadString();

        /// <summary>
        ///     移动到下一个属性。
        /// </summary>
        bool MoveToNextAttribute();

        /// <summary>
        ///     移动到包含当前属性节点的元素。
        /// </summary>
        /// <returns></returns>
        bool MoveToElement();

        /// <summary>
        /// 获取当前节点的类型。
        /// </summary>
        NodeType NodeType { get; }
    }
}