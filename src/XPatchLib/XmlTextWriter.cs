// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.Text;
#if (NET || NETSTANDARD_2_0_UP)
using System.Xml.Serialization;

#endif

namespace XPatchLib
{
    /// <summary>
    ///     表示提供快速、非缓存、只进方法的写入器，该方法生成包含 XML 数据（这些数据符合 W3C 可扩展标记语言 (XML) 1.0 和“XML 命名空间”建议）的流或文件。
    /// </summary>
    /// <seealso cref="XPatchLib.ITextWriter" />
    public class XmlTextWriter : ITextWriter
    {
        private static readonly State[] StateTableDocument =
        {
            // State.Start      State.Prolog     State.PostDTD    State.Element    State.Attribute  State.Content   State.AttrOnly   State.Epilog
            //
            /* Token.PI             */ State.Error, State.Prolog, State.PostDtd, State.Content, State.Content,
            State.Content, State.Error, State.Epilog,
            /* Token.Doctype        */ State.Error, State.PostDtd, State.Error, State.Error, State.Error, State.Error,
            State.Error, State.Error,
            /* Token.Comment        */ State.Error, State.Prolog, State.PostDtd, State.Content, State.Content,
            State.Content, State.Error, State.Epilog,
            /* Token.CData          */ State.Error, State.Error, State.Error, State.Content, State.Content,
            State.Content, State.Error, State.Error,
            /* Token.StartElement   */ State.Error, State.Element, State.Element, State.Element, State.Element,
            State.Element, State.Error, State.Error,
            /* Token.EndElement     */ State.Error, State.Error, State.Error, State.Content, State.Content,
            State.Content, State.Error, State.Error,
            /* Token.LongEndElement */ State.Error, State.Error, State.Error, State.Content, State.Content,
            State.Content, State.Error, State.Error,
            /* Token.StartAttribute */ State.Error, State.Error, State.Error, State.Attribute, State.Attribute,
            State.Error, State.Error, State.Error,
            /* Token.EndAttribute   */ State.Error, State.Error, State.Error, State.Error, State.Element, State.Error,
            State.Error, State.Error,
            /* Token.Content        */ State.Error, State.Error, State.Error, State.Content, State.Attribute,
            State.Content, State.Error, State.Error,
            /* Token.Base64         */ State.Error, State.Error, State.Error, State.Content, State.Attribute,
            State.Content, State.Error, State.Error,
            /* Token.RawData        */ State.Error, State.Prolog, State.PostDtd, State.Content, State.Attribute,
            State.Content, State.Error, State.Epilog,
            /* Token.Whitespace     */ State.Error, State.Prolog, State.PostDtd, State.Content, State.Attribute,
            State.Content, State.Error, State.Epilog
        };

        private static readonly State[] StateTableDefault =
        {
            // State.Start      State.Prolog     State.PostDTD    State.Element    State.Attribute  State.Content   State.AttrOnly   State.Epilog
            //
            /* Token.PI             */ State.Prolog, State.Prolog, State.PostDtd, State.Content, State.Content,
            State.Content, State.Error, State.Epilog,
            /* Token.Doctype        */ State.PostDtd, State.PostDtd, State.Error, State.Error, State.Error, State.Error,
            State.Error, State.Error,
            /* Token.Comment        */ State.Prolog, State.Prolog, State.PostDtd, State.Content, State.Content,
            State.Content, State.Error, State.Epilog,
            /* Token.CData          */ State.Content, State.Content, State.Error, State.Content, State.Content,
            State.Content, State.Error, State.Epilog,
            /* Token.StartElement   */ State.Element, State.Element, State.Element, State.Element, State.Element,
            State.Element, State.Error, State.Element,
            /* Token.EndElement     */ State.Error, State.Error, State.Error, State.Content, State.Content,
            State.Content, State.Error, State.Error,
            /* Token.LongEndElement */ State.Error, State.Error, State.Error, State.Content, State.Content,
            State.Content, State.Error, State.Error,
            /* Token.StartAttribute */ State.AttrOnly, State.Error, State.Error, State.Attribute, State.Attribute,
            State.Error, State.Error, State.Error,
            /* Token.EndAttribute   */ State.Error, State.Error, State.Error, State.Error, State.Element, State.Error,
            State.Epilog, State.Error,
            /* Token.Content        */ State.Content, State.Content, State.Error, State.Content, State.Attribute,
            State.Content, State.Attribute, State.Epilog,
            /* Token.Base64         */ State.Content, State.Content, State.Error, State.Content, State.Attribute,
            State.Content, State.Attribute, State.Epilog,
            /* Token.RawData        */ State.Prolog, State.Prolog, State.PostDtd, State.Content, State.Attribute,
            State.Content, State.Attribute, State.Epilog,
            /* Token.Whitespace     */ State.Prolog, State.Prolog, State.PostDtd, State.Content, State.Attribute,
            State.Content, State.Attribute, State.Epilog
        };
        private readonly Encoding _encoding;

        private readonly TextWriter _textWriter;

        private char _curQuoteChar;

        private State _currentState;
        private Formatting _formatting;
        private int _indentation;
        private bool _indented; // perf - faster to check a boolean.
        private Token _lastToken;
        private char _quoteChar;

        private TagInfo[] _stack;
        private State[] _stateTable;
        private int _top;

        /// <summary>
        ///     创建 <see cref="XmlTextWriter" /> 类型实例。
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。
        ///     </para>
        ///     <para> 默认不序列化默认值。 </para>
        ///     <para> 默认 <see cref="Formatting.Indented" />。 </para>
        /// </remarks>
        internal XmlTextWriter()
        {
            _formatting = Formatting.Indented;
            _indented = true;
            _indentation = 2;
            _indentChar = ' ';
            // element stack
            _stack = new TagInfo[10];
            _top = 0; // 0 is an empty sentanial element
            _stack[_top].Init();
            _quoteChar = '"';
            _encoding = new UTF8Encoding(false);

            _stateTable = StateTableDefault;
            _currentState = State.Start;
            _lastToken = Token.Empty;

#if (NET || NETSTANDARD_2_0_UP)
            IgnoreAttributeType = typeof(XmlIgnoreAttribute);
#endif
        }

        /// <summary>
        ///     使用指定的流和编码方式创建 <see cref="XmlTextWriter" /> 类的实例。
        /// </summary>
        /// <param name="pStream">要写入的流。</param>
        /// <param name="pEncoding">要生成的编码方式。如果编码方式为 空引用，则它以 UTF-8 的形式写出流。</param>
        /// <remarks>
        ///     <para>
        ///         默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。
        ///     </para>
        ///     <para> 默认不序列化默认值。 </para>
        ///     <para> 默认 <see cref="Formatting.Indented" />。 </para>
        /// </remarks>
        public XmlTextWriter(Stream pStream, Encoding pEncoding) : this()
        {
            _encoding = pEncoding;
            StreamWriter w = pEncoding != null ? new StreamWriter(pStream, pEncoding) : new StreamWriter(pStream);
            w.AutoFlush = false;
            _textWriter = w;
        }

        /// <summary>
        ///     使用指定的文件创建 <see cref="XmlTextWriter" /> 类的实例。
        /// </summary>
        /// <param name="pFilename">要写入的文件名。如果该文件存在，它将截断该文件并用新内容对其进行改写。</param>
        /// <param name="pEncoding">要生成的编码方式。如果编码方式为 空引用，则它以 UTF-8 的形式写出流。</param>
        public XmlTextWriter(String pFilename, Encoding pEncoding)
            : this(new FileStream(pFilename, FileMode.Create,
                FileAccess.Write, FileShare.Read), pEncoding)
        {
        }

        /// <summary>
        ///     以指定的 <paramref name="pWriter" /> 实例创建 <see cref="XmlTextWriter" /> 类型实例。
        /// </summary>
        /// <param name="pWriter">指定的有序字符系列的编写器。</param>
        /// <remarks>
        ///     <para>
        ///         默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。
        ///     </para>
        ///     <para> 默认不序列化默认值。 </para>
        ///     <para> 默认 <see cref="Formatting.Indented" />。 </para>
        /// </remarks>
        public XmlTextWriter(TextWriter pWriter) : this()
        {
            _textWriter = pWriter;

            _encoding = pWriter.Encoding;
        }


        /// <summary>
        ///     指示如何对输出进行格式设置。
        /// </summary>
        /// <value><see cref="ITextWriter.Formatting" /> 值之一。默认值为 <c>Formatting.Indented</c>（缩进显示）。</value>
        /// <remarks>
        ///     如果设置了 <c>Formatting.Indented</c> 选项，则使用 <see cref="ITextWriter.Indentation" /> 和
        ///     <see cref="ITextWriter.IndentChar" /> 属性对子元素进行缩进。
        /// </remarks>
        public Formatting Formatting
        {
            get { return _formatting; }
            set
            {
                _formatting = value;
                _indented = value == Formatting.Indented;
            }
        }

        /// <summary>
        ///     获取或设置当 <see cref="ITextWriter.Formatting" /> 设置为 <c>Formatting.Indented</c> 时将为层次结构中的每个级别书写多少
        ///     <see cref="ITextWriter.IndentChar" />。
        /// </summary>
        /// <value>每个级别的 <see cref="ITextWriter.IndentChar" /> 的数目。默认值为 2。</value>
        public int Indentation
        {
            get { return _indentation; }
            set
            {
                if (value < 0)
                    throw new ArgumentException( /*Res.GetString(Res.Xml_InvalidIndentation)*/);
                _indentation = value;
            }
        }

        /// <summary>
        ///     获取或设置当 <see cref="ITextWriter.Formatting" /> 设置为 <c>Formatting.Indented</c> 时哪个字符用于缩进。
        /// </summary>
        /// <value>用于缩进的字符。默认为空格。</value>
        public char IndentChar
        {
            get { return _indentChar; }
            set
            {
                if (value != _indentChar)
                    _indentChar = value;
            }
        }

        /// <summary>
        ///     获取或设置哪个字符用于将属性值引起来。
        /// </summary>
        /// <value>用于将属性值引起来的字符。这必须是单引号 (&#39;) 或双引号 (&#34;)。默认为双引号。</value>
        public char QuoteChar
        {
            get { return _quoteChar; }
            set
            {
                if (value != '"' && value != '\'')
                    throw new ArgumentException( /*Res.GetString(Res.Xml_InvalidQuote)*/);
                _quoteChar = value;
            }
        }

        private void AutoCompleteAll()
        {
            while (_top > 0)
                InternalWriteEndElement(false);
        }

        private void Indent(bool beforeEndElement)
        {
            // pretty printing.
            if (_top == 0)
            {
                _textWriter.WriteLine();
            }
            else if (!_stack[_top].Mixed)
            {
                _textWriter.WriteLine();
                int i = beforeEndElement ? _top - 1 : _top;
                _textWriter.Write(new string(_indentChar, i * _indentation));
            }
        }

        private void WriteEndAttributeQuote()
        {
            _textWriter.Write(_curQuoteChar);
        }

        private void WriteEndStartTag(bool empty)
        {
            if (empty)
                _textWriter.Write(" /");
            _textWriter.Write('>');
        }

        private void AutoComplete(Token token)
        {
            if (_currentState == State.Closed)
                throw new InvalidOperationException( /*Res.GetString(Res.Xml_Closed)*/);
            if (_currentState == State.Error)
                throw new InvalidOperationException(
                    /*Res.GetString(Res.Xml_WrongToken, tokenName[(int)token], stateName[(int)State.Error])*/);

            State newState = _stateTable[(int) token * 8 + (int) _currentState];
            if (newState == State.Error)
                throw new InvalidOperationException(
                    /*Res.GetString(Res.Xml_WrongToken, tokenName[(int)token], stateName[(int)this.currentState])*/);

            switch (token)
            {
                case Token.Doctype:
                    if (_indented && _currentState != State.Start)
                        Indent(false);
                    break;

                case Token.StartElement:
                case Token.Comment:
                case Token.Pi:
                case Token.CData:
                    if (_currentState == State.Attribute)
                    {
                        WriteEndAttributeQuote();
                        WriteEndStartTag(false);
                    }
                    else if (_currentState == State.Element)
                    {
                        WriteEndStartTag(false);
                    }
                    if (token == Token.CData)
                        _stack[_top].Mixed = true;
                    else if (_indented && _currentState != State.Start)
                        Indent(false);
                    break;

                case Token.EndElement:
                case Token.LongEndElement:
                    if (_currentState == State.Attribute)
                        WriteEndAttributeQuote();
                    if (_currentState == State.Content)
                        token = Token.LongEndElement;
                    else
                        WriteEndStartTag(token == Token.EndElement);
                    if (StateTableDocument == _stateTable && _top == 1)
                        newState = State.Epilog;
                    break;

                case Token.StartAttribute:
                    if (_currentState == State.Attribute)
                    {
                        WriteEndAttributeQuote();
                        _textWriter.Write(' ');
                    }
                    else if (_currentState == State.Element)
                    {
                        _textWriter.Write(' ');
                    }
                    break;

                case Token.EndAttribute:
                    WriteEndAttributeQuote();
                    break;

                case Token.Whitespace:
                case Token.Content:
                case Token.RawData:
                case Token.Base64:

                    if (_currentState == State.Element && _lastToken != Token.Content)
                        WriteEndStartTag(false);
                    if (newState == State.Content)
                        _stack[_top].Mixed = true;
                    break;

                default:
                    throw new InvalidOperationException( /*Res.GetString(Res.Xml_InvalidOperation)*/);
            }
            _currentState = newState;
            _lastToken = token;
        }

        private void InternalWriteEndElement(bool longFormat)
        {
            try
            {
                if (_top <= 0)
                    throw new InvalidOperationException( /*Res.GetString(Res.Xml_NoStartTag)*/);
                // if we are in the element, we need to close it.
                AutoComplete(longFormat ? Token.LongEndElement : Token.EndElement);
                if (_lastToken == Token.LongEndElement)
                {
                    if (_indented)
                        Indent(true);
                    _textWriter.Write('<');
                    _textWriter.Write('/');
                    _textWriter.Write(_stack[_top].Name);
                    _textWriter.Write('>');
                }

                _top--;
            }
            catch
            {
                _currentState = State.Error;
                throw;
            }
        }

        private void InternalWriteStartDocument()
        {
            try
            {
                if (_currentState != State.Start)
                    throw new InvalidOperationException( /*Res.GetString(Res.Xml_NotTheFirst)*/);
                _stateTable = StateTableDocument;
                _currentState = State.Prolog;

                StringBuilder bufBld = new StringBuilder(128);
                bufBld.Append("version=" + _quoteChar + "1.0" + _quoteChar);
                if (_encoding != null)
                {
                    bufBld.Append(" encoding=");
                    bufBld.Append(_quoteChar);
                    bufBld.Append(_encoding.WebName);
                    bufBld.Append(_quoteChar);
                }
                InternalWriteProcessingInstruction("xml", bufBld.ToString());
            }
            catch
            {
                _currentState = State.Error;
                throw;
            }
        }

        private void InternalWriteEndDocument()
        {
            try
            {
                AutoCompleteAll();
                if (_currentState != State.Epilog)
                {
                    if (_currentState == State.Closed)
                        throw new ArgumentException( /*Res.GetString(Res.Xml_ClosedOrError)*/);
                    throw new ArgumentException( /*Res.GetString(Res.Xml_NoRoot)*/);
                }
                _stateTable = StateTableDefault;
                _currentState = State.Start;
                _lastToken = Token.Empty;
            }
            catch
            {
                _currentState = State.Error;
                throw;
            }
        }

        private void InternalWriteProcessingInstruction(string name, string text)
        {
            _textWriter.Write("<?");
            //ValidateName(name, false);
            _textWriter.Write(name);
            _textWriter.Write(' ');
            if (null != text)
                _textWriter.Write(text);
            _textWriter.Write("?>");
        }

        private void InternalWriteStartAttribute(string localName)
        {
            try
            {
                AutoComplete(Token.StartAttribute);

                _textWriter.Write(localName);
                _textWriter.Write('=');
                if (_curQuoteChar != _quoteChar)
                    _curQuoteChar = _quoteChar;
                _textWriter.Write(_curQuoteChar);
            }
            catch
            {
                _currentState = State.Error;
                throw;
            }
        }

        // Closes the attribute opened by WriteStartAttribute.
        private void InternalWriteEndAttribute()
        {
            try
            {
                AutoComplete(Token.EndAttribute);
            }
            catch
            {
                _currentState = State.Error;
                throw;
            }
        }

        private void PushStack()
        {
            if (_top == _stack.Length - 1)
            {
                TagInfo[] na = new TagInfo[_stack.Length + 10];
                if (_top > 0) Array.Copy(_stack, na, _top + 1);
                _stack = na;
            }

            _top++; // Move up stack
        }

        private void InternalWriteElementString(string localName, String value)
        {
            InternalWriteStartElement(localName);
            if (null != value && 0 != value.Length)
                InternalWriteString(value);
            InternalWriteEndElement(false);
        }


        private void InternalWriteStartElement(string localName)
        {
            try
            {
                AutoComplete(Token.StartElement);
                PushStack();
                _textWriter.Write('<');

                _stack[_top].Name = localName;
                _stack[_top].Mixed = false;
                _textWriter.Write(localName);
            }
            catch
            {
                _currentState = State.Error;
                throw;
            }
        }

        private void InternalWriteString(string text)
        {
            try
            {
                if (null != text && text.Length != 0)
                {
                    AutoComplete(Token.Content);
                    Write(text);
                }
            }
            catch
            {
                _currentState = State.Error;
                throw;
            }
        }

        private void WriteEntityRefImpl(string name)
        {
            _textWriter.Write('&');
            _textWriter.Write(name);
            _textWriter.Write(';');
        }

        private enum State
        {
            Start,
            Prolog,
            PostDtd,
            Element,
            Attribute,
            Content,
            AttrOnly,
            Epilog,
            Error,
            Closed
        }

        private enum Token
        {
            Pi,
            Doctype,
            Comment,
            CData,
            StartElement,
            EndElement,
            LongEndElement,
            StartAttribute,
            EndAttribute,
            Content,
            Base64,
            RawData,
            Whitespace,
            Empty
        }

        private struct TagInfo
        {
            internal string Name;
            internal bool Mixed; // whether to pretty print the contents of this element.

            internal void Init()
            {
                Name = null;
                Mixed = false;
            }
        }

        #region 来源：XmlTextEncoder

        private void Write(string text)
        {
            if (text == null)
                return;

            // scan through the string to see if there are any characters to be escaped
            int len = text.Length;

            int i = 0;
            char ch = (char) 0;
            for (;;)
            {
                while (i < len && (text[i] < 128 && !SpecChar.SpecCharsFlags[text[i]] || text[i] >= 128))
                    i++;
                if (i == len)
                {
                    _textWriter.Write(text);
                    return;
                }
                if (inAttribute)
                {
                    //https://ostermiller.org/calc/ascii.html
                    if (ch == 0x9)
                    {
                        i++;
                        continue;
                    }
                }
                else
                {
                    if (ch == 0x9 || ch == 0xA || ch == 0xD || ch == '"' || ch == '\'')
                    {
                        i++;
                        continue;
                    }
                }
                // some character that needs to be escaped is found:
                break;
            }

            if (i == len)
            {
                // reached the end of the string -> write it whole out
                _textWriter.Write(text);
                return;
            }
            int startPos = 0;
            char[] helperBuffer = new char[256];
            for (;;)
            {
                if (startPos < i)
                    WriteStringFragment(text, startPos, i - startPos, helperBuffer);
                if (i == len)
                    break;
                switch (text[i])
                {
                    case '<':
                        WriteEntityRefImpl("lt");
                        break;
                    case '>':
                        WriteEntityRefImpl("gt");
                        break;
                    case '&':
                        WriteEntityRefImpl("amp");
                        break;
                    case '\'':
                        if (inAttribute && _quoteChar == text[i])
                            WriteEntityRefImpl("apos");
                        else
                            _textWriter.Write('\'');
                        break;
                    case '"':
                        if (inAttribute && _quoteChar == text[i])
                            WriteEntityRefImpl("quot");
                        else
                            _textWriter.Write('"');
                        break;
                    default:
                        WriteCharEntityImpl(ch);
                        break;
                }
                i++;
                startPos = i;
                while (i < len && (text[i] < 128 && !SpecChar.SpecCharsFlags[text[i]] || text[i] >= 128))
                    i++;
            }
        }

        private void WriteStringFragment(string str, int offset, int count, char[] helperBuffer)
        {
            int bufferSize = helperBuffer.Length;
            while (count > 0)
            {
                int copyCount = count;
                if (copyCount > bufferSize)
                    copyCount = bufferSize;

                str.CopyTo(offset, helperBuffer, 0, copyCount);
                _textWriter.Write(helperBuffer, 0, copyCount);
                offset += copyCount;
                count -= copyCount;
            }
        }

        private void WriteCharEntityImpl(char ch)
        {
            WriteCharEntityImpl(((int) ch).ToString("X", NumberFormatInfo.InvariantInfo));
        }

        private void WriteCharEntityImpl(string strVal)
        {
            _textWriter.Write("&#x");
            _textWriter.Write(strVal);
            _textWriter.Write(';');
        }

        #endregion

        #region

#if NET || NETSTANDARD_2_0_UP
        /// <summary>
        /// 获取或设置指示<see cref="Serializer" /> 方法<see cref= "Serializer.Divide" /> 进行序列化的公共字段或公共读 / 写属性值。
        /// </summary>
        /// <remarks>
        /// 用于控制如何<see cref="Serializer" /> 方法 <see cref = "Serializer.Divide" /> 序列化对象。
        /// </remarks>
        /// <example>
        /// <include file='docs/docs.xml' path='Comments/examples/example[@class="XmlTextWriter" and @property="IgnoreAttributeType"]/*'/>
        /// </example>
        /// <value>
        /// 默认值：
        /// <see cref = "System.Xml.Serialization.XmlIgnoreAttribute" />。
        /// </value>
#else
        /// <summary>
        /// 获取或设置指示<see cref="Serializer" /> 方法<see cref= "Serializer.Divide" /> 进行序列化的公共字段或公共读 / 写属性值。
        /// </summary>
        /// <remarks>
        /// 用于控制如何<see cref="Serializer" /> 方法 <see cref = "Serializer.Divide" /> 序列化对象。
        /// </remarks>
        /// <example>
        /// <include file='docs/docs.xml' path='Comments/examples/example[@class="XmlTextWriter" and @property="IgnoreAttributeType"]/*'/>
        /// </example>
        /// <value>
        /// 默认值：
        /// <c>null</c>。
        /// </value>
#endif
        public Type IgnoreAttributeType { get; set; }

        private ISerializeSetting _setting = new XmlSerializeSetting();

        /// <summary>
        ///     获取或设置写入器设置。
        /// </summary>
        public ISerializeSetting Setting
        {
            get { return _setting; }
            set
            {
                if (value != null) _setting = value;
            }
        }

        /// <summary>执行与释放或重置非托管资源关联的应用程序定义的任务。</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                AutoCompleteAll();
                Flush();
            }
            catch
            {
                // never fail
            }
            finally
            {
                _currentState = State.Closed;
#if NET || NETSTANDARD_2_0_UP
                _textWriter.Close();
#endif
            }
        }

        /// <summary>
        ///     写入文档开始标记。
        /// </summary>
        public void WriteStartDocument()
        {
            InternalWriteStartDocument();
        }

        /// <summary>
        ///     写入文档结束标记。
        /// </summary>
        public void WriteEndDocument()
        {
            InternalWriteEndDocument();
        }

        /// <summary>
        ///     将缓冲区中的所有内容刷新到基础流，并同时刷新基础流。
        /// </summary>
        public void Flush()
        {
            _textWriter.Flush();
        }

        /// <summary>
        ///     写入对象开始标记。
        /// </summary>
        /// <param name="pName">对象名称。</param>
        public void WriteStartObject(string pName)
        {
            InternalWriteStartElement(pName);
        }

        /// <summary>
        ///     写入对象结束标记。
        /// </summary>
        public void WriteEndObject()
        {
            InternalWriteEndElement(false);
        }

        /// <summary>
        ///     是否正在写Attribute
        /// </summary>
        private bool inAttribute;

        /// <summary>
        ///     用于缩进的字符
        /// </summary>
        private char _indentChar;

        /// <summary>
        ///     写入特性。
        /// </summary>
        /// <param name="pName">特性名称。</param>
        /// <param name="pValue">特性值。</param>
        public void WriteAttribute(string pName, string pValue)
        {
            inAttribute = true;
            InternalWriteStartAttribute(pName);
            InternalWriteString(pValue);
            InternalWriteEndAttribute();
            inAttribute = false;
        }

        /// <summary>
        ///     写入属性。
        /// </summary>
        /// <param name="pName">属性名称。</param>
        /// <param name="pValue">属性值。</param>
        public void WriteProperty(string pName, string pValue)
        {
            InternalWriteElementString(pName, pValue);
        }

        /// <summary>
        ///     写入属性开始标记。
        /// </summary>
        /// <param name="pName">属性名称。</param>
        public void WriteStartProperty(string pName)
        {
            InternalWriteStartElement(pName);
        }

        /// <summary>
        ///     写入属性结束标记。
        /// </summary>
        public void WriteEndProperty()
        {
            InternalWriteEndElement(false);
        }

        /// <summary>
        ///     写入列表类型对象开始标记。
        /// </summary>
        /// <param name="pName">列表类型对象实例名称。</param>
        public void WriteStartArray(string pName)
        {
            InternalWriteStartElement(pName);
        }

        /// <summary>
        ///     写入列表元素对象开始标记。
        /// </summary>
        /// <param name="pName">列表元素对象实例名称。</param>
        public void WriteStartArrayItem(string pName)
        {
            InternalWriteStartElement(pName);
        }

        /// <summary>
        ///     写入列表元素结束标记。
        /// </summary>
        public void WriteEndArrayItem()
        {
            InternalWriteEndElement(false);
        }

        /// <summary>
        ///     写入列表对象结束标记。
        /// </summary>
        public void WriteEndArray()
        {
            InternalWriteEndElement(false);
        }

        /// <summary>
        ///     写入文本。
        /// </summary>
        /// <param name="pValue">待写入的文本。</param>
        public void WriteValue(string pValue)
        {
            InternalWriteString(pValue);
        }

        #endregion
    }
}