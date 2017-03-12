// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

namespace XPatchLib
{
    /// <summary>
    ///     指定节点的类型。
    /// </summary>
    /// <seealso cref="System.Xml.XmlNodeType" />
    public enum NodeType
    {
        /// <summary>
        ///     属性 (例如， id='123' )。
        /// </summary>
        Attribute,

        /// <summary>
        ///     CDATA 节 (例如， &lt;![CDATA[my escaped text]]&gt; )。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CDATA")]
        CDATA,

        /// <summary>
        ///     注释 (例如， &lt;!-- my comment --&gt; )。
        /// </summary>
        Comment,

        /// <summary>
        ///     文档提供的对象，作为文档树的根访问整个 XML 文档。
        /// </summary>
        Document,

        /// <summary>
        ///     将文档片段。
        /// </summary>
        DocumentFragment,

        /// <summary>
        ///     文档类型声明中，由以下标记 (例如， &lt;!DOCTYPE...&gt; )。
        /// </summary>
        DocumentType,

        /// <summary>
        ///     元素 (例如， &lt;item&gt; )。
        /// </summary>
        Element,

        /// <summary>
        ///     结束元素标记 (例如， &lt;/item&gt; )。
        /// </summary>
        EndElement,

        /// <summary>
        ///     返回当 XmlReader 到达实体替换为调用的结果末尾 ResolveEntity。
        /// </summary>
        EndEntity,

        /// <summary>
        ///     实体声明 (例如， &lt;!ENTITY...&gt; )。
        /// </summary>
        Entity,

        /// <summary>
        ///     对实体的引用 (例如， &amp;num; )。
        /// </summary>
        EntityReference,

        /// <summary>
        ///     这由返回 XmlReader 如果 Read 不调用方法。
        /// </summary>
        None,

        /// <summary>
        ///     在文档类型声明中的表示法 (例如， &lt;!NOTATION...&gt; )。
        /// </summary>
        Notation,

        /// <summary>
        ///     处理指令 (例如， &lt;?pi test?&gt; )。
        /// </summary>
        ProcessingInstruction,

        /// <summary>
        ///     在混合内容模型或内的空格中标记之间空白区域 xml:space="preserve" 作用域。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace")]
        SignificantWhitespace,

        /// <summary>
        ///     节点的文本内容。
        /// </summary>
        Text,

        /// <summary>
        ///     标记之间的空白区域。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace")]
        Whitespace,

        /// <summary>
        ///     XML 声明 (例如， &lt;?xml version='1.0'?&gt; )。
        /// </summary>
        XmlDeclaration
    }
}