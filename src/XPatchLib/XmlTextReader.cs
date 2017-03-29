// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     表示提供对 XML 数据进行快速、非缓存、只进访问的读取器。
    /// </summary>
    /// <seealso cref="XPatchLib.ITextReader" />
    public class XmlTextReader : ITextReader
    {
        private readonly XmlReader _reader;

        /// <summary>
        ///     以指定的 <paramref name="pReader" /> 实例创建 <see cref="XmlTextReader" /> 类型实例。
        /// </summary>
        /// <param name="pReader">指定的 XML 读取器。</param>
        public XmlTextReader(XmlReader pReader)
        {
            Guard.ArgumentNotNull(pReader, "pReader");
            _reader = pReader;
            Setting = new XmlSerializeSetting();
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     从流中读取下一个节点。
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            return _reader.Read();
        }

        /// <summary>
        ///     获取读取器的状态。
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public ReadState ReadState
        {
            get
            {
                switch (_reader.ReadState)
                {
                    case System.Xml.ReadState.Closed:
                        return ReadState.Closed;
                    case System.Xml.ReadState.EndOfFile:
                        return ReadState.EndOfFile;
                    case System.Xml.ReadState.Error:
                        return ReadState.Error;
                    case System.Xml.ReadState.Initial:
                        return ReadState.Initial;
                    case System.Xml.ReadState.Interactive:
                        return ReadState.Interactive;
                    default:
                        throw new NotSupportedException(_reader.ReadState.ToString());
                }
            }
        }

        /// <summary>
        ///     将元素或文本节点的内容当做字符串读取。
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            return _reader.ReadString();
        }

        /// <summary>
        ///     获取一个值，该值指示此读取器是否定位在流的结尾。
        /// </summary>
        public bool EOF
        {
            get { return _reader.EOF; }
        }

        /// <summary>
        ///     获取当前节点的限定名。
        /// </summary>
        public string Name
        {
            get { return _reader.Name; }
        }

        /// <summary>
        ///     获取当前节点的文本值。
        /// </summary>
        public string Value
        {
            get { return _reader.Value; }
        }

        /// <summary>
        ///     获取当前节点上的属性数。
        /// </summary>
        public int AttributeCount
        {
            get { return _reader.AttributeCount; }
        }

        /// <summary>
        ///     移动到下一个属性。
        /// </summary>
        /// <returns></returns>
        public bool MoveToNextAttribute()
        {
            return _reader.MoveToNextAttribute();
        }

        /// <summary>
        ///     移动到包含当前属性节点的元素。
        /// </summary>
        /// <returns></returns>
        public bool MoveToElement()
        {
            return _reader.MoveToElement();
        }

        /// <summary>
        ///     获取当前节点的类型。
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public NodeType NodeType
        {
            get
            {
                switch (_reader.NodeType)
                {
                    case XmlNodeType.None:
                        return NodeType.None;
                    case XmlNodeType.Element:
                        return NodeType.Element;
                    case XmlNodeType.Attribute:
                        return NodeType.Attribute;
                    case XmlNodeType.Text:
                        return NodeType.Text;
                    case XmlNodeType.CDATA:
                        return NodeType.CDATA;
                    case XmlNodeType.EntityReference:
                        return NodeType.EntityReference;
                    case XmlNodeType.Entity:
                        return NodeType.Entity;
                    case XmlNodeType.ProcessingInstruction:
                        return NodeType.ProcessingInstruction;
                    case XmlNodeType.Comment:
                        return NodeType.Comment;
                    case XmlNodeType.Document:
                        return NodeType.Document;
                    case XmlNodeType.DocumentType:
                        return NodeType.DocumentType;
                    case XmlNodeType.DocumentFragment:
                        return NodeType.DocumentFragment;
                    case XmlNodeType.Notation:
                        return NodeType.Notation;
                    case XmlNodeType.Whitespace:
                        return NodeType.Whitespace;
                    case XmlNodeType.SignificantWhitespace:
                        return NodeType.SignificantWhitespace;
                    case XmlNodeType.EndElement:
                        return NodeType.EndElement;
                    case XmlNodeType.EndEntity:
                        return NodeType.EndEntity;
                    case XmlNodeType.XmlDeclaration:
                        return NodeType.XmlDeclaration;
                    default:
                        throw new NotSupportedException(_reader.NodeType.ToString());
                }
            }
        }

        /// <summary>
        ///     执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                IDisposable d = _reader as IDisposable;
                if (d != null)
                    d.Dispose();
            }
        }

        /// <summary>
        /// 获取或设置读取器设置。
        /// </summary>
        /// <value>默认返回以无参数构造函数创建的<see cref="XmlSerializeSetting"/>实例。</value>
        public ISerializeSetting Setting { get; set; }
    }
}