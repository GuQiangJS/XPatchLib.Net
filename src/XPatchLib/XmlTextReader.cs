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
        /// <returns>如果成功读取了下一个节点，则为 <c>true</c>；如果没有其他节点可读取，则为 <c>false</c>。</returns>
        public bool Read()
        {
            return _reader.Read();
        }

        /// <summary>
        ///     从流中读取下一个同级节点。
        /// </summary>
        /// <param name="nextElementName">下一个同级节点的名称。</param>
        /// <param name="parentElementName">父级节点的名称。</param>
        /// <returns>如果成功读取了下一个节点，则为 <c>true</c>；如果没有其他节点可读取，则为 <c>false</c>。</returns>
        public bool MoveToNextElement(string nextElementName, string parentElementName)
        {
            if (_reader.Name == parentElementName)
            {
                _reader.Read();
            }
            while (_reader.Name != nextElementName)
            {
                if (_reader.EOF)
                    break;
                if (_reader.Name == parentElementName && _reader.NodeType == XmlNodeType.EndElement)
                    break;
                _reader.Read();
            }
            if (_reader.Name == nextElementName)
                if (_reader.NodeType == XmlNodeType.EndElement)
                    _reader.Read();
                if(_reader.NodeType == XmlNodeType.Element)
                    return true;
            return false;
        }

        /// <summary>
        ///     移动到当前节点的结尾。
        /// </summary>
        /// <param name="currentElementName">当前节点的名称。</param>
        public void MoveToCurrentElementEnd(string currentElementName)
        {
            while (_reader.Name == currentElementName && _reader.NodeType != XmlNodeType.EndElement)
            {
                _reader.Read();
            }
        }

        /// <summary>
        ///     获取读取器的状态。
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        ///<para>当 内部读取器 <see cref="System.Xml.XmlReader"/> 的状态不包含在以下状态中时：</para>
        /// <para><see cref="System.Xml.ReadState.Closed"/></para>
        /// <para><see cref="System.Xml.ReadState.EndOfFile"/></para>
        /// <para><see cref="System.Xml.ReadState.Error"/></para>
        /// <para><see cref="System.Xml.ReadState.Initial"/></para>
        /// <para><see cref="System.Xml.ReadState.Interactive"/></para>
        /// </exception>
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
        /// <returns>该元素或文本节点的内容。如果读取器定位在元素或文本节点以外的位置，或者当前上下文中没有其他文本内容可返回，则这可以是空字符串。 
        /// <para>Note: 文本节点可以是元素或属性文本节点。</para></returns>
        public string ReadString()
        {
            return Read<string>();
        }

        /// <summary>
        ///     将元素或文本节点的内容当做 <typeparamref name="T"/> 读取。
        /// </summary>
        /// <typeparam name="T">读取节点内容的类型。</typeparam>
        /// <returns>该元素或文本节点的内容。如果读取器定位在元素或文本节点以外的位置，或者当前上下文中没有其他文本内容可返回，则这可以是空字符串。 
        /// <para>Note: 文本节点可以是元素或属性文本节点。</para></returns>
        public T Read<T>()
        {
            if(_reader.NodeType==XmlNodeType.Element)
                return (T)_reader.ReadElementContentAs(typeof(T), null);
            return (T) _reader.ReadContentAs(typeof(T), null);
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
        /// <returns>如果存在下一个属性，则为 <c>true</c>；如果没有其他属性，则为 <c>false</c>。</returns>
        public bool MoveToNextAttribute()
        {
            return _reader.MoveToNextAttribute();
        }

        /// <summary>
        ///     移动到包含当前属性节点的元素。
        /// </summary>
        /// <returns>如果读取器定位在属性上，则为 <c>true</c>（读取器移动到拥有该属性的元素）；如果读取器不是定位在属性上，则为 <c>false</c>（读取器的位置不改变）。</returns>
        public bool MoveToElement()
        {
            return _reader.MoveToElement();
        }

        /// <summary>
        ///     获取当前节点的类型。
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        ///<para>当 内部读取器 <see cref="System.Xml.XmlReader"/> 的读取的节点类型不包含在以下状态中时：</para>
        /// <para><see cref="System.Xml.XmlNodeType.None"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.Element"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.Attribute"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.Text"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.CDATA"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.EntityReference"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.Entity"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.ProcessingInstruction"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.Comment"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.Document"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.DocumentType"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.DocumentFragment"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.Notation"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.Whitespace"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.SignificantWhitespace"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.EndElement"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.EndEntity"/></para>
        /// <para><see cref="System.Xml.XmlNodeType.XmlDeclaration"/></para>
        /// </exception>
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