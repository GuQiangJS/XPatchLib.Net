// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
#if NET_20

#endif

namespace XPatchLib
{
    /// <summary>
    ///     表示提供对 XML 数据进行快速、非缓存、只进访问的读取器。
    /// </summary>
    /// <seealso cref="XPatchLib.ITextReader" />
    public class XmlTextReader : ITextReader
    {
        private static readonly Regex Regex = new Regex(@"\r\n|\n\r|\n|\r", RegexOptions.CultureInvariant);
        private readonly string[,] _emptyAttributeDic = new string[0, 0];
        private readonly XmlReaderSettings _settings;
        private readonly XmlReader _xmlReader;

        private string[,] _attributes;

        private ISerializeSetting _setting = new XmlSerializeSetting();

        private string _value;

        /// <summary>
        ///     初始化 <see cref="XmlTextReader" /> 的新实例。
        /// </summary>
        protected XmlTextReader()
        {
            _settings = new XmlReaderSettings();
            _settings.IgnoreComments = true;
            _settings.IgnoreProcessingInstructions = true;
            _settings.IgnoreWhitespace = true;
        }

        /// <summary>
        ///     使用指定的流初始化 <see cref="XmlTextReader" /> 类的新实例。
        /// </summary>
        /// <param name="input">包含要读取的 XML 数据的流。</param>
        public XmlTextReader(Stream input) : this()
        {
            _xmlReader = XmlReader.Create(input, _settings);
            SetXmlReaderNormalization();
        }

        /// <summary>
        ///     使用指定的 <see cref="TextReader" /> 初始化 <see cref="XmlTextReader" /> 的新实例。
        /// </summary>
        /// <param name="input">包含要读取的 XML 数据的 TextReader。</param>
        public XmlTextReader(TextReader input) : this()
        {
            _xmlReader = XmlReader.Create(input, _settings);
            SetXmlReaderNormalization();
        }

        /// <summary>
        ///     用指定的文件初始化 <see cref="XmlTextReader" /> 的新实例。
        /// </summary>
        /// <param name="input">包含 XML 数据的文件的 URL。</param>
        public XmlTextReader(string input) : this()
        {
            _xmlReader = XmlReader.Create(input, _settings);
            SetXmlReaderNormalization();
        }

        /// <summary>
        ///     获取或设置读取器设置。
        /// </summary>
        public ISerializeSetting Setting
        {
            get { return _setting; }
            set
            {
                if (value != null) _setting = value;
            }
        }

        /// <summary>
        ///     获取读取器的状态。
        /// </summary>
        public ReadState ReadState
        {
            get
            {
                switch (_xmlReader.ReadState)
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
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        ///     获取一个值，该值指示此读取器是否定位在流的结尾。
        /// </summary>
        public bool EOF
        {
            get { return _xmlReader.EOF; }
        }

        private string _name;
        /// <summary>
        ///     获取当前节点的限定名。
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        ///     获取当前节点的文本值。
        /// </summary>
        public string GetValue()
        {
            if (_value == null)
                Read();
            return _value;
        }

        private NodeType _nodeType;

        /// <summary>
        ///     获取当前节点的类型。
        /// </summary>
        public NodeType NodeType { get { return _nodeType; } }

        /// <summary>
        ///     获取当前解析的节点是否包含特性节点。
        /// </summary>
        public bool HasAttribute
        {
            get { return _xmlReader.HasAttributes; }
        }

        /// <summary>
        ///     获取当前节点的特性名称与值的键值对字典
        /// </summary>
        public string[,] GetAttributes()
        {
            return _attributes;
        }

        /// <summary>
        ///     从流中读取下一个节点。
        /// </summary>
        /// <returns>如果成功读取了下一个节点，则为 <c>true</c>；如果没有其他节点可读取，则为 <c>false</c>。</returns>
        public bool Read()
        {
            while (true)
            {
                if (_xmlReader.EOF)
                    return false;
                _xmlReader.Read();
                switch (_xmlReader.NodeType)
                {
                    case XmlNodeType.Text:
#if NET
                        _value = _xmlReader.Value;
#elif NETSTANDARD
                        _value = Normalize(_xmlReader.Value);
#endif
                        return true;
                    case XmlNodeType.Element:
                        _nodeType = _xmlReader.IsEmptyElement ? NodeType.FullElement : NodeType.Element;
                        _name = _xmlReader.LocalName;
                        ParseAttributes();
                        _value = _xmlReader.IsEmptyElement ? string.Empty : null;
                        return true;
                    case XmlNodeType.EndElement:
                        _nodeType = NodeType.EndElement;
                        _attributes = _emptyAttributeDic;
                        _value = null;
                        _name = _xmlReader.LocalName;
                        return true;
                    case XmlNodeType.XmlDeclaration:
                        _nodeType = NodeType.XmlDeclaration;
                        _value = null;
                        _attributes = _emptyAttributeDic;
                        _name = null;
                        return true;
                }
            }
        }

        private void SetXmlReaderNormalization()
        {
#if NET
            PropertyInfo pi = _xmlReader.GetType().GetProperty("Normalization",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (pi != null)
                pi.SetValue(_xmlReader, false, null);
            //#elif NETSTANDARD
            //            IEnumerable<FieldInfo> fis = _xmlReader.GetType().GetRuntimeFields();
            //            foreach (FieldInfo fieldInfo in fis)
            //            {
            //                if (fieldInfo.Name == "_normalize")
            //                {
            //                    fieldInfo.SetValue(_xmlReader, false);
            //                }
            //            }
#endif
        }

        internal static string Normalize(string s)
        {
            if (s != null)
                s = Regex.Replace(s, "\r\n");

            return s;
        }

        //private void ParseValue()
        //{
        //    if (_xmlReader.MoveToContent() == XmlNodeType.Element || _xmlReader.IsStartElement())
        //    {
        //        _value = _xmlReader.Value;
        //        return;
        //    }
        //    _value = null;
        //}

        private void ParseAttributes()
        {
            if (_xmlReader.HasAttributes)
            {
                _attributes = new string[_xmlReader.AttributeCount, 2];
                _xmlReader.MoveToFirstAttribute();
                _attributes[0, 0] = _xmlReader.Name;
                _attributes[0, 1] = _xmlReader.Value;
                int index = 1;
                while (_xmlReader.MoveToNextAttribute())
                {
                    _attributes[index, 0] = _xmlReader.Name;
                    _attributes[index, 1] = _xmlReader.Value;
                    index++;
                }
            }
        }

        #region Dispose

        /// <summary>执行与释放或重置非托管资源关联的应用程序定义的任务。</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     释放 <see cref="XmlTextReader" /> 类的当前实例所使用的所有资源。
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            //the boolean flag may be used by subclasses to differentiate between disposing and finalizing
            //if (disposing && readState != ReadState.Closed)
            if (disposing)
#if NET
                if (_xmlReader.ReadState != System.Xml.ReadState.Closed)
                    _xmlReader.Close();
#elif NETSTANDARD
                            _xmlReader.Dispose();
#endif
        }

        #endregion
    }
}