// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Net.NetworkInformation;

namespace XPatchLib
{
    /// <summary>
    ///     表示提供对数据进行快速、非缓存、只进访问的读取器。
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ITextReader : IDisposable
    {
        /// <summary>
        ///     获取读取器的状态。
        /// </summary>
        ReadState ReadState { get; }

        /// <summary>
        ///     获取一个值，该值指示此读取器是否定位在流的结尾。
        /// </summary>
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

    /// <summary>
    /// 指定节点的类型。
    /// </summary>
    /// <seealso cref="System.Xml.XmlNodeType"/>
    public enum NodeType
    {
        /// <summary>
        /// 属性 (例如， id='123' )。
        /// </summary>
        Attribute,
        /// <summary>
        /// CDATA 节 (例如， &lt;![CDATA[my escaped text]]&gt; )。
        /// </summary>
        CDATA,
        /// <summary>
        /// 注释 (例如， &lt;!-- my comment --&gt; )。
        /// </summary>
        Comment,
        /// <summary>
        /// 文档提供的对象，作为文档树的根访问整个 XML 文档。
        /// </summary>
        Document,
        /// <summary>
        /// 将文档片段。
        /// </summary>
        DocumentFragment,
        /// <summary>
        /// 文档类型声明中，由以下标记 (例如， &lt;!DOCTYPE...&gt; )。
        /// </summary>
        DocumentType,
        /// <summary>
        /// 元素 (例如， &lt;item&gt; )。
        /// </summary>
        Element,
        /// <summary>
        /// 结束元素标记 (例如， &lt;/item&gt; )。
        /// </summary>
        EndElement,
        /// <summary>
        /// 返回当 XmlReader 到达实体替换为调用的结果末尾 ResolveEntity。
        /// </summary>
        EndEntity,
        /// <summary>
        /// 实体声明 (例如， &lt;!ENTITY...&gt; )。
        /// </summary>
        Entity,
        /// <summary>
        /// 对实体的引用 (例如， &num; )。
        /// </summary>
        EntityReference,
        /// <summary>
        /// 这由返回 XmlReader 如果 Read 不调用方法。
        /// </summary>
        None,
        /// <summary>
        /// 在文档类型声明中的表示法 (例如， &lt;!NOTATION...&gt; )。
        /// </summary>
        Notation,
        /// <summary>
        /// 处理指令 (例如， &lt;?pi test?&gt; )。
        /// </summary>
        ProcessingInstruction,
        /// <summary>
        /// 在混合内容模型或内的空格中标记之间空白区域 xml:space="preserve" 作用域。
        /// </summary>
        SignificantWhitespace,
        /// <summary>
        /// 节点的文本内容。
        /// </summary>
        Text,
        /// <summary>
        /// 标记之间的空白区域。
        /// </summary>
        Whitespace,
        /// <summary>
        /// XML 声明 (例如， &lt;?xml version='1.0'?&gt; )。
        /// </summary>
        XmlDeclaration,
    }

    /// <summary>
    ///     指定读取器的状态。
    /// </summary>
    public enum ReadState
    {
        /// <summary>
        ///     Close 调用方法。
        /// </summary>
        Closed,

        /// <summary>
        ///     已成功到达文件末尾。
        /// </summary>
        EndOfFile,

        /// <summary>
        ///     将出现错误，以防止读取的操作继续进行。
        /// </summary>
        Error,

        /// <summary>
        ///     Read 不调用方法。
        /// </summary>
        Initial,

        /// <summary>
        ///     Read 调用方法。 可以在读取器上调用其他方法。
        /// </summary>
        Interactive
    }
}