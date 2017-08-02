// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace XPatchLib
{
    /// <summary>
    ///     表示提供对 XML 数据进行快速、非缓存、只进访问的读取器。
    /// </summary>
    /// <seealso cref="XPatchLib.ITextReader" />
    public class XmlTextReader : ITextReader
    {
        private const int DefaultSize = 0x100000;
        private int _charPos = 0;
        private readonly TextReader _reader;

        ///// <summary>
        /////     是否认为 \r \n 是换行。
        ///// </summary>
        //private bool enableNewLine;

        //private ReadState readState;

        ///// <summary>
        /////     是否跳过空白字符
        ///// </summary>
        //private bool _skipWhiteSpace;

        internal XmlTextReader()
        {
            //enableNewLine = true;
            Name = string.Empty;
            Value = string.Empty;
            NodeType = NodeType.None;
            ReadState = ReadState.Initial;
        }

        /// <summary>
        /// 使用指定的流初始化 <see cref="XmlTextReader"/> 类的新实例。
        /// </summary>
        /// <param name="input">包含要读取的 XML 数据的流。</param>
        public XmlTextReader(Stream input) : this()
        {
            _reader = new StreamReader(input);
            ReadData();
        }

        /// <summary>
        /// 使用指定的 <see cref="TextReader"/> 初始化 <see cref="XmlTextReader"/> 类的新实例。
        /// </summary>
        /// <param name="input">包含要读取的 XML 数据的 <see cref="TextReader"/>。</param>
        public XmlTextReader(TextReader input) : this()
        {
            _reader = input;
            ReadData();
        }

        internal string Text { get; private set; }

        /// <summary>
        ///     获取当前节点的限定名。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     获取当前节点的文本值。
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        ///     获取当前节点的类型。
        /// </summary>
        public NodeType NodeType { get; private set; }

        /// <summary>
        ///     获取一个值，该值指示此读取器是否定位在流的结尾。
        /// </summary>
        public bool EOF { get; private set; }

        /// <summary>
        /// 获取当前解析的节点是否包含特性节点。
        /// </summary>
        public bool HasAttribute { get; private set; }

        private KeyValuePair<String, String>[] _attributes;

        private readonly KeyValuePair<String, String>[] _emptyAttributes = new KeyValuePair<string, string>[0];

        /// <summary>
        ///     获取当前节点的特性名称与值的键值对字典
        /// </summary>
        public KeyValuePair<String, String>[] GetAttributes()
        {
            if (!HasAttribute)
                return _emptyAttributes;
            return _attributes;
        }

        private ISerializeSetting _setting = new XmlSerializeSetting();
        private char[] _chars;

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
        public ReadState ReadState { get; private set; }

        /// <summary>
        ///     从流中读取下一个节点。
        /// </summary>
        /// <returns>如果成功读取了下一个节点，则为 <c>true</c>；如果没有其他节点可读取，则为 <c>false</c>。</returns>
        public bool Read()
        {
            ReadState = ReadState.Interactive;
            while (true)
            {
                int endIndex = -1;
                if (EOF)
                    return false;

                int pos = _charPos;

                try
                {
                    switch (_chars[pos])
                    {
                        case '<':
                            switch (_chars[pos + 1])
                            {
                                case '?':
                                    //XML描述标记
                                    NodeType = NodeType.XmlDeclaration;
                                    endIndex = FindFirstIndex(_chars, pos + 1, new[] {'?', '>'});
                                    if (endIndex >= 0)
                                    {
                                        SetText(_chars, pos, endIndex - pos + 2);
                                        _charPos = endIndex + 1;
                                        return true;
                                    }
                                    break;
                                case '/':
                                    //XML节点结束标志
                                    NodeType = NodeType.EndElement;
                                    //找到最近的 '>' 节点，成为结束标志
                                    endIndex = FindFirstIndex(_chars, pos + 1, new[] {'>'});
                                    if (endIndex >= 0)
                                    {
                                        SetText(_chars, pos, endIndex - pos + 1);
                                        _charPos = endIndex + 1;
                                        return true;
                                    }
                                    break;
                                default:
                                    endIndex = FindFirstEndTagIndex(_chars, pos);

                                    if (_chars[endIndex] == '/')
                                    {
                                        // '/>'
                                        NodeType = NodeType.FullElement;
                                        SetText(_chars, pos, endIndex - pos + 2);
                                        _charPos = endIndex + 1;
                                        return true;
                                    }
                                    else if (_chars[endIndex] == '>')
                                    {
                                        // '>'
                                        NodeType = NodeType.Element;
                                        SetText(_chars, pos, endIndex - pos + 1);
                                        _charPos = endIndex + 1;
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case '/':
                            if (_chars[pos + 1] == '>')
                            {
                                _charPos = pos + 1;
                                NodeType = NodeType.FullElement;
                                ParseAttribute();
                                ParseName();
                                ParseValue(1);
                            }
                            _charPos++;
                            break;
                        //换行
                        case '\r':
                        case '\n':
                            //if(enableNewLine)
                            _charPos++;
                            break;
                        case ' ':
                            //if (skipWhiteSpace)
                            _charPos++;
                            break;
                        default:
                            _charPos++;
                            break;
                    }
                }
                finally
                {
                    EOF = _chars.Length <= _charPos + 1;
                }
            }
        }

        /// <summary>
        ///     读取所有内容至 <see cref="_ps" /> 中。
        /// </summary>
        private void ReadData()
        {
            int readIndex = 0;

            while (true)
            {
                if (_chars == null)
                    _chars = new char[DefaultSize];

                int re = _reader.Read(_chars, readIndex, DefaultSize);

                if (re == 0)
                    break;
                readIndex = readIndex + re;
                char[] newchars = new char[_chars.Length + DefaultSize * 2];
                Array.Copy(_chars, newchars, _chars.Length);
                _chars = newchars;
            }

            char[] nChars = new char[readIndex];
            Array.Copy(_chars, nChars, readIndex);
            _chars = nChars;
        }

        private char[] _textChars;
        
        internal static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
        {
            Buffer.BlockCopy(src, srcOffset * sizeof(char), dst, dstOffset * sizeof(char), count * sizeof(char));
        }

        internal static void BlockCopy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        { 
            Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);

        }

        private void SetText(char[] chars, int startIndex, int len)
        {
            Text = string.Empty;
            Text = new string(chars, startIndex, len);

            _textChars = new char[len];

            BlockCopyChars(chars, startIndex, _textChars, 0, len);

            ParseAttribute();
            ParseName();
            ParseValue(len + 1);
        }

        private int FindFirstIndex(char[] chars, int startIndex, char[] findChars)
        {
            for (int i = 0;; i++)
            {
                int j = startIndex + i;
                if (chars[j] == findChars[0])
                {
                    var found = true;
                    for (int k = 1; i < findChars.Length; k++)
                        if (chars[j + k] != findChars[k])
                        {
                            found = false;
                            break;
                        }
                    if (found)
                        return j;
                }
            }
        }

        ///// <summary>
        /////     找到最先出现的 '>' 符号
        ///// </summary>
        ///// <returns>如果找到了就返回首先发现 '/' 字符的位置，否则返回-1。</returns>
        //private int FindFirstGreaterThanIndex(char[] chars, int startIndex)
        //{
        //    for (int i = 0;; i++)
        //    {
        //        int j = startIndex + i;
        //        if (chars[j] == '>')
        //            return j - startIndex;
        //    }
        //}

        ///// <summary>
        /////     找到最先出现的 '/>' 符号
        ///// </summary>
        ///// <returns>如果找到了就返回首先发现 '>' 字符的位置，否则返回-1。</returns>
        //private int FindFirstSelfClosingIndex(char[] chars, int startIndex)
        //{
        //    for (int i = 0;; i++)
        //    {
        //        int j = startIndex + i;
        //        if (chars[j] == '/' && chars[j + 1] == '>')
        //            return j - startIndex;
        //    }
        //}

        /// <summary>
        ///     找到最先出现的 '>' 或 '/>' 符号
        /// </summary>
        /// <returns>如果找到了就返回首先发现 '/' 或 '>' 字符的位置，否则返回-1。</returns>
        private int FindFirstEndTagIndex(char[] chars, int startIndex)
        {
            for (int i = 0;; i++)
            {
                int j = startIndex + i;
                if (chars[j] == '>' || (chars[j] == '/' && chars[j + 1] == '>'))
                    return j;
            }
        }

        private void ParseValue(int startLen)
        {
            Value = string.Empty;
            if (NodeType == NodeType.EndElement || NodeType == NodeType.FullElement)
                return;

            int pos = _charPos;

            if (_chars[pos + startLen - 1] == '<')
                return;
            int len = 0;

            if(pos + startLen + 1 < _chars.Length)
                while (true)
                {
                    if (_chars[pos + startLen + len] == '<')
                        break;
                    len++;
                }
            if (startLen + pos + len >= _chars.Length)
            {
                Value = string.Empty;
                return;
            }

            int s = startLen + pos - 1;
            Queue<char> cs = new Queue<char>(len + 1);
            for (int i = 0; i < len + 1;)
            {
                /*
                 * &(逻辑与)  &amp;        
                 * <(小于)    &lt;        
                 * >(大于)    &gt;        
                 * "(双引号)  &quot;      
                 * '(单引号)  &apos;
                 */
                if (_chars[s + i] == '&')
                {
                    if (_chars[s + i + 1] == 'a' && _chars[s + i + 2] == 'm' && _chars[s + i + 3] == 'p' &&
                        _chars[s + i + 4] == ';')
                    {
                        cs.Enqueue('&');
                        i = i + 5;
                        continue;
                    }
                    if (_chars[s + i + 1] == 'l' && _chars[s + i + 2] == 't' && _chars[s + i + 3] == ';')
                    {
                        cs.Enqueue('<');
                        i = i + 4;
                        continue;
                    }
                    if (_chars[s + i + 1] == 'g' && _chars[s + i + 2] == 't' && _chars[s + i + 3] == ';')
                    {
                        cs.Enqueue('>');
                        i = i + 4;
                        continue;
                    }
                    if (_chars[s + i + 1] == 'q' && _chars[s + i + 2] == 'u' && _chars[s + i + 3] == 'o' &&
                        _chars[s + i + 4] == 't' && _chars[s + i + 5] == ';')
                    {
                        cs.Enqueue('"');
                        i = i + 6;
                        continue;
                    }
                    if (_chars[s + i + 1] == 'a' && _chars[s + i + 2] == 'p' && _chars[s + i + 3] == 'o' &&
                        _chars[s + i + 4] == 's' && _chars[s + i + 5] == ';')
                    {
                        cs.Enqueue('\'');
                        i = i + 6;
                        continue;
                    }
                }
                cs.Enqueue(_chars[s + i]);
                i++;
            }
            Value = new string(cs.ToArray());
        }

        private void ParseName()
        {
            Name = string.Empty;
            if (NodeType == NodeType.XmlDeclaration) return;
            //char[] cs = Text.ToCharArray();
            int start = 0;
            int end = 0;
            int len = _textChars.Length;

            for (int i = 0; i < len; i++)
            {
                if (_textChars[i] == '<')
                {
                    start = i + 1;
                    continue;
                }
                if (_textChars[i] == '/' && i > 0 && _textChars[i - 1] == '<')
                {
                    start = i + 1;
                    continue;
                }
                if (_textChars[i] == ' ' && i > start)
                {
                    end = i - start;
                    break;
                }
                if (_textChars[i] == '>' || i + 1 != _textChars.Length && _textChars[i] == '/' && _textChars[i + 1] == '>')
                {
                    end = i - start;
                    break;
                }
            }
            Name = new string(_textChars, start, end).Trim();
        }

        private void ParseAttribute()
        {
            ResetAttributes();

            if (NodeType == NodeType.XmlDeclaration) return;
            
            int startName = 0;
            int endName = 0;
            int startValue = 0;
            int endValue = 0;
            int len = _textChars.Length;

            Queue<KeyValuePair<string, string>> kvs = new Queue<KeyValuePair<string, string>>(10);

            for (int i = 0; i < len; i++)
            {
                if (_textChars[i] == ' ')
                {
                    startName = i + 1;
                    continue;
                }
                if (_textChars[i] == '=')
                    endName = i - 1;
                if (_textChars[i] == '"')
                    if (startValue == 0)
                        startValue = i + 1;
                    else
                        endValue = i - 1;
                if (startName >= 0 && endName > 0 && startValue > 0 && endValue > 0)
                {
                    //KeyValuePair<String, String>[] c = new KeyValuePair<string, string>[_attributes.Length + 1];
                    //Array.Copy(_attributes, c, _attributes.Length);
                    //c[_attributes.Length] = new KeyValuePair<string, string>(
                    //    new string(cs, startName, endName - startName + 1),
                    //    new string(cs, startValue, endValue - startValue + 1));

                    kvs.Enqueue(new KeyValuePair<string, string>(
                        new string(_textChars, startName, endName - startName + 1),
                        new string(_textChars, startValue, endValue - startValue + 1)));

                    //_attributes = c;
                    HasAttribute = true;

                    startName = endName = startValue = endValue = 0;
                }
            }

            if (HasAttribute)
            {
                _attributes = kvs.ToArray();
            }
        }

        /// <summary>
        /// 将 <see cref="XmlTextReader.ReadState"/> 改为 Closed。
        /// </summary>
        public void Close()
        {
#if NET || NETSTANDARD_2_0_UP
            if (_reader != null)
                _reader.Close();
#endif
            //readState = ReadState.Closed;
            ResetAttributes();
        }

        private void ResetAttributes()
        {
            //_attributes = new KeyValuePair<String, String>[0];
            HasAttribute = false;
        }

#region Dispose

        /// <summary>执行与释放或重置非托管资源关联的应用程序定义的任务。</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// 释放 <see cref="XmlTextReader"/> 类的当前实例所使用的所有资源。
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            //the boolean flag may be used by subclasses to differentiate between disposing and finalizing
            //if (disposing && readState != ReadState.Closed)
            if (disposing)
                Close();
        }

#endregion
    }
}