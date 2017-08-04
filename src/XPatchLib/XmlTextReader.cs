// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using BufferBuilder = System.Text.StringBuilder;

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

        private string _value;

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
            NodeType = NodeType.None;
            _readState = ReadState.Initial;
            _stringBuilder = new BufferBuilder();
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

        private string _text;

        internal string Text { get { return _text; } }

        /// <summary>
        ///     获取当前节点的限定名。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     获取当前节点的文本值。
        /// </summary>
        public string GetValue()
        {
            if (NodeType == NodeType.Element)
            {
                if (_value == null)
                {
                    int e = FindFirstIndex(_chars, _charPos, new[] {'<'});
                    _value = ParseValue(_chars, _charPos, e);
                }
                return _value;
            }
            return string.Empty;
        }

        /// <summary>
        ///     获取当前节点的类型。
        /// </summary>
        public NodeType NodeType { get; private set; }

        private bool _eof;

        /// <summary>
        ///     获取一个值，该值指示此读取器是否定位在流的结尾。
        /// </summary>
        public bool EOF { get { return _eof; } }

        private bool _hasAttribute;

        /// <summary>
        /// 获取当前解析的节点是否包含特性节点。
        /// </summary>
        public bool HasAttribute { get { return _hasAttribute; } }

        private KeyValuePair<String, String>[] _attributes;

        private readonly KeyValuePair<String, String>[] _emptyAttributes = new KeyValuePair<string, string>[0];

        /// <summary>
        ///     获取当前节点的特性名称与值的键值对字典
        /// </summary>
        public KeyValuePair<String, String>[] GetAttributes()
        {
            if (!HasAttribute || NodeType == NodeType.XmlDeclaration)
            {
                return _emptyAttributes;
            }

            if (_attributes == null)
            {
                int startName = 0;
                int endName = 0;
                int startValue = 0;
                int endValue = 0;

                Queue<KeyValuePair<string, string>> kvs = new Queue<KeyValuePair<string, string>>(10);

                for (int i = 0; i < _textLen; i++)
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
                        //HasAttribute = true;

                        startName = endName = startValue = endValue = 0;
                    }
                }

                if (kvs.Count > 0)
                {
                    _attributes = kvs.ToArray();
                }
            }
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

        private ReadState _readState;

        /// <summary>
        ///     获取读取器的状态。
        /// </summary>
        public ReadState ReadState {
            get { return _readState; }
        }

        /// <summary>
        ///     从流中读取下一个节点。
        /// </summary>
        /// <returns>如果成功读取了下一个节点，则为 <c>true</c>；如果没有其他节点可读取，则为 <c>false</c>。</returns>
        public bool Read()
        {
            if (_readState == ReadState.Initial)
                _readState = ReadState.Interactive;
            while (true)
            {
                int endIndex = -1;
                if (_eof)
                    return false;

                int pos = _charPos;

                ResetAll();

                try
                {
                    char ch = _chars[pos];
                    switch (ch)
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
                                        ParseName();
                                        _charPos = endIndex + 1;
                                        return true;
                                    }
                                    break;
                                default:
                                    endIndex = FindFirstEndTagIndex(_chars, pos);

                                    if (_chars[endIndex] == '/' && _chars[endIndex + 1] == '>')
                                    {
                                        // '/>'
                                        NodeType = NodeType.FullElement;
                                        SetText(_chars, pos, endIndex - pos + 2);
                                        ParseName();
                                        ParseAttribute();
                                        _charPos = endIndex + 1;
                                        return true;
                                    }
                                    else if (_chars[endIndex] == '>')
                                    {
                                        // '>'
                                        NodeType = NodeType.Element;
                                        SetText(_chars, pos, endIndex - pos + 1);
                                        ParseName();
                                        ParseValue(_chars, pos, endIndex);
                                        ParseAttribute();
                                        _charPos = endIndex + 1;
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        //case '/':
                        //    if (_chars[pos + 1] == '>')
                        //    {
                        //        _charPos = pos + 1;
                        //        NodeType = NodeType.FullElement;
                        //        ResetAttributes();
                        //        ParseAttribute();
                        //        ParseName();
                        //        ResetValue();
                        //        //ParseValue(1);
                        //    }
                        //    _charPos++;
                        //    break;
                        //换行
                        case '\r':
                        case '\n':
                        case ' ':
                        default:
                            //if(enableNewLine)
                            _charPos++;
                            break;
                    }
                }
                finally
                {
                    _eof = _chars.Length <= _charPos + 1;
                }
            }
        }

        private void ResetAll()
        {
            _text = null;
            _textChars = null;
            _textLen = -1;
            _hasAttribute = false;
            _attributes = null;
            _value = null;
        }

        /// <summary>
        ///     读取所有内容至 <see cref="_chars" /> 中。
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
        private int _textLen;

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
            _text = new string(chars, startIndex, len);
            _textLen = len;
            _textChars = new char[len];

            BlockCopyChars(chars, startIndex, _textChars, 0, len);

            //ResetAttributes();
            //ResetValue();

            //ParseAttribute();
            //ParseName();
            //ParseValue(len + 1);
        }

        private void ResetValue()
        {
            _value = null;
        }

        private int FindFirstIndex(char[] chars, int startIndex, char[] findChars)
        {
            int targetLength = findChars.Length;
            int count = 0;
            char currentCharToSearch = findChars[0];
            int len = chars.Length;
            for (; startIndex < len; startIndex++)
            {
                if (chars[startIndex] == currentCharToSearch)
                {
                    count++;
                    if (count == targetLength)
                    {
                        return startIndex;
                    }
                    else
                    {
                        currentCharToSearch = findChars[count];
                    }
                }
                else
                {
                    count = 0;
                    currentCharToSearch = findChars[0];
                }
            }
            return 0;
        }

        private bool FindFirstIndexCore(char[] chars, int startIndex, char[] findChars)
        {
            int len = findChars.Length;
            for (int i = 0; i < len; i++)
            {
                if (chars[i + startIndex] != findChars[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        ///     找到最先出现的 '>' 或 '/>' 符号
        /// </summary>
        /// <returns>如果找到了就返回首先发现 '/' 或 '>' 字符的位置，否则返回-1。</returns>
        private int FindFirstEndTagIndex(char[] chars, int startIndex)
        {
            for (;; startIndex++)
            {
                char ch = chars[startIndex];
                if (ch == '>' || (ch == '/' && chars[startIndex + 1] == '>'))
                    return startIndex;
            }
        }

        BufferBuilder _stringBuilder;

        private string ParseValue(char[] chars,int startIndex,int endIndex)
        {
            if (endIndex <= 0 || chars.Length < endIndex)
                return string.Empty;

            string result;
            while (true)
            {
                int end = FindFirstIndex(chars, startIndex, new[] {'&'});
                if (end > 0 && endIndex > end)
                {
                    if (_chars[end + 1] == 'a' && _chars[end + 2] == 'm' && _chars[end + 3] == 'p' &&
                        _chars[end + 4] == ';')
                    {
                        _stringBuilder.Append('&');
                        startIndex = startIndex + 5;
                        continue;
                    }
                    if (_chars[end + 1] == 'l' && _chars[end + 2] == 't' && _chars[end + 3] == ';')
                    {
                        _stringBuilder.Append('<');
                        startIndex = startIndex + 4;
                        continue;
                    }
                    if (_chars[end + 1] == 'g' && _chars[end + 2] == 't' && _chars[end + 3] == ';')
                    {
                        _stringBuilder.Append('>');
                        startIndex = startIndex + 4;
                        continue;
                    }
                    if (_chars[end + 1] == 'q' && _chars[end + 2] == 'u' && _chars[end + 3] == 'o' &&
                        _chars[end + 4] == 't' && _chars[end + 5] == ';')
                    {
                        _stringBuilder.Append('"');
                        startIndex = startIndex + 6;
                        continue;
                    }
                    if (_chars[end + 1] == 'a' && _chars[end + 2] == 'p' && _chars[end + 3] == 'o' &&
                        _chars[end + 4] == 's' && _chars[end + 5] == ';')
                    {
                        _stringBuilder.Append('\'');
                        startIndex = startIndex + 6;
                        continue;
                    }
                }
                else
                {
                    break;
                }
            }
            if (endIndex > startIndex)
            {
                _stringBuilder.Append(chars, startIndex, endIndex - startIndex);
            }
            result = _stringBuilder.ToString();
            _stringBuilder.Length = 0;
            return result;
        }

        private void ParseName()
        {
            Name = string.Empty;
            if (NodeType == NodeType.XmlDeclaration) return;
            //char[] cs = Text.ToCharArray();
            int start = 0;
            int end = 0;

            for (int i = 0; i < _textLen; i++)
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
                if (_textChars[i] == '>' || i + 1 != _textLen && _textChars[i] == '/' && _textChars[i + 1] == '>')
                {
                    end = i - start;
                    break;
                }
            }
            Name = new string(_textChars, start, end).Trim();
        }

        /// <summary>
        /// 解析当前Text是否包含Attribute
        /// </summary>
        private void ParseAttribute()
        {
            //ResetAttributes();

            if (NodeType == NodeType.XmlDeclaration) return;
            
            for (int i = 0; i < _textLen; i++)
            {
                if (_textChars[i] == ' ')
                {
                    for (int j = i; j < _textLen; j++)
                    {
                        if (_textChars[j] == '=')
                        {
                            _hasAttribute = true;
                            return;
                        }
                    }
                }
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
            _hasAttribute = false;
            _attributes = null;
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