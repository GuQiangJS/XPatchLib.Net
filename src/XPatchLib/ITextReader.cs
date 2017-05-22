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
        string Value { get; }

        /// <summary>
        ///     获取当前节点上的属性数。
        /// </summary>
        int AttributeCount { get; }

        /// <summary>
        ///     从流中读取下一个节点。
        /// </summary>
        /// <returns>如果成功读取了下一个节点，则为 <c>true</c>；如果没有其他节点可读取，则为 <c>false</c>。</returns>
        bool Read();

        /// <summary>
        ///     将元素或文本节点的内容当做字符串读取。
        /// </summary>
        /// <returns>该元素或文本节点的内容。如果读取器定位在元素或文本节点以外的位置，或者当前上下文中没有其他文本内容可返回，则这可以是空字符串。 
        /// <para>Note: 文本节点可以是元素或属性文本节点。</para></returns>
        string ReadString();

        /// <summary>
        ///     移动到下一个属性。
        /// </summary>
        /// <returns>如果存在下一个属性，则为 <c>true</c>；如果没有其他属性，则为 <c>false</c>。</returns>
        bool MoveToNextAttribute();

        /// <summary>
        ///     移动到包含当前属性节点的元素。
        /// </summary>
        /// <returns>如果读取器定位在属性上，则为 <c>true</c>（读取器移动到拥有该属性的元素）；如果读取器不是定位在属性上，则为 <c>false</c>（读取器的位置不改变）。</returns>
        bool MoveToElement();

        /// <summary>
        /// 获取当前节点的类型。
        /// </summary>
        NodeType NodeType { get; }
    }
}