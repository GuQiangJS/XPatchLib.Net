// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace XPatchLib
{
    //TODO:Not Complete
    /// <summary>
    ///     Json写入器。
    /// </summary>
    /// <seealso cref="XPatchLib.ITextWriter" />
    internal class JsonTextWriter : ITextWriter
    {
        private readonly TextWriter Writer;


        private State _State;

        /// <summary>
        ///     以指定的 <paramref name="pWriter" /> 实例创建 <see cref="JsonTextWriter" /> 类型实例。
        /// </summary>
        /// <param name="pWriter">指定的 Json 编写器。</param>
        public JsonTextWriter(TextWriter pWriter)
        {
            Guard.ArgumentNotNull(pWriter, "pWriter");
            Writer = pWriter;
            _State = State.Start;
        }

        /// <summary>
        ///     写入文档开始标记。
        /// </summary>
        public void WriteStartDocument()
        {
            Writer.Write("{");
#if DEBUG
            Debug.WriteLine("WriteStartDocument.");
#endif
            _State = State.StartDocument;
        }

        /// <summary>
        ///     写入文档结束标记。
        /// </summary>
        public void WriteEndDocument()
        {
            _State = State.EndDocument;
        }

        /// <summary>
        ///     将缓冲区中的所有内容刷新到基础流，并同时刷新基础流。
        /// </summary>
        public void Flush()
        {
            Writer.Flush();
        }

        /// <summary>
        ///     写入对象开始标记。
        /// </summary>
        /// <param name="pName">对象名称。</param>
        public void WriteStartObject(string pName)
        {
            WritePropertyDelimiter();
            Writer.Write("\"{0}\":{{", pName);
#if DEBUG
            Debug.WriteLine(string.Format("WriteStartObject '{0}'.", pName));
#endif
            _State = State.StartObject;
        }

        /// <summary>
        ///     写入对象结束标记。
        /// </summary>
        public void WriteEndObject()
        {
            Writer.Write("}");
#if DEBUG
            Debug.WriteLine("WriteEndObject.");
#endif
            _State = State.EndObject;
        }

        /// <summary>
        ///     写入特性。
        /// </summary>
        /// <param name="pName">特性名称。</param>
        /// <param name="pValue">特性值。</param>
        public void WriteAttribute(string pName, string pValue)
        {
            WritePropertyDelimiter();
            Writer.Write("\"{0}\":\"{1}\"", pName, pValue);
#if DEBUG
            Debug.WriteLine("WriteAttribute '{0}'='{1}'.", pName, pValue);
#endif
            _State = State.Property;
        }

        /// <summary>
        ///     写入属性。
        /// </summary>
        /// <param name="pName">属性名称。</param>
        /// <param name="pValue">属性值。</param>
        public void WriteProperty(string pName, string pValue)
        {
            WritePropertyDelimiter();
            Writer.Write("\"{0}\":\"{1}\"", pName, pValue);
#if DEBUG
            Debug.WriteLine("WriteProperty '{0}'='{1}'.", pName, pValue);
#endif
            _State = State.Property;
        }

        /// <summary>
        ///     写入列表类型对象开始标记。
        /// </summary>
        /// <param name="pName">列表类型对象实例名称</param>
        public void WriteStartArray(string pName)
        {
            WritePropertyDelimiter();
            Writer.Write("\"{0}\":[", pName);
#if DEBUG
            Debug.WriteLine(string.Format("WriteStartArray '{0}'.", pName));
#endif
            _State = State.StartArray;
        }

        /// <summary>
        ///     写入列表对象结束标记。
        /// </summary>
        public void WriteEndArray()
        {
            Writer.Write("]");
#if DEBUG
            Debug.WriteLine("WriteEndArray.");
#endif
            _State = State.EndArray;
        }

        /// <summary>
        ///     写入属性开始标记。
        /// </summary>
        /// <param name="pName">属性名称。</param>
        public void WriteStartProperty(string pName)
        {
            WritePropertyDelimiter();
            Writer.Write("\"{0}\":", pName);
#if DEBUG
            Debug.WriteLine(string.Format("WriteStartProperty '{0}'.", pName));
#endif
            _State = State.StartProperty;
        }

        /// <summary>
        ///     写入属性结束标记。
        /// </summary>
        public void WriteEndProperty()
        {
            //            if (Writer.State == State.Content || Writer.State == State.Element)
            //            {
            //                Writer.WriteEndElement();
            //#if DEBUG
            //                Debug.WriteLine("WriteEndProperty.");
            //#endif
            //            }
            _State = State.EndProperty;
        }

        /// <summary>
        ///     写入文本。
        /// </summary>
        /// <param name="pValue">待写入的文本。</param>
        public void WriteValue(string pValue)
        {
            Writer.Write("\"{0}\"", pValue);
#if DEBUG
            Debug.WriteLine(string.Format("WriteValue '{0}'.", pValue));
#endif
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        DateTimeSerializationMode _mode=DateTimeSerializationMode.RoundtripKind;

        /// <summary>
        ///     获取或设置在字符串与 <see cref="DateTime" /> 之间转换时，如何处理时间值。
        /// </summary>
        /// <value>默认值 <see cref="DateTimeSerializationMode.RoundtripKind" /> 。</value>
        public DateTimeSerializationMode Mode {
            get { return _mode; }
            set { _mode = value; }
        }

        /// <summary>
        ///     获取或设置是否序列化默认值。
        /// </summary>
        /// <value>默认值 <c>false</c> 。</value>
        public bool SerializeDefalutValue { get; set; }

        private Encoding _encoding = Encoding.UTF8;

        /// <summary>
        ///     获取或设置要使用的文本编码的类型。
        /// </summary>
        /// <value>默认值 <see cref="Encoding.UTF8" /> 。</value>
        public Encoding Encoding {
            get { return _encoding; }
            set { _encoding = value; }
        }

        private Formatting _formatting = Formatting.None;

        /// <summary>
        ///     获取或设置如何对输出进行格式设置。
        /// </summary>
        /// <value><see cref="Formatting" /> 值之一。 默认值是 <see cref="Formatting.None" /> （无特殊格式）。</value>
        public Formatting Formatting {
            get { return _formatting; }
            set { _formatting = value; }
        }

        private string _indentChars = " ";

        /// <summary>
        ///     获取或设置用于缩进时用于转换的字符 <see cref="Formatting" /> 设置为 <see cref="Formatting.Indented" />。
        /// </summary>
        /// <value>默认值是 空格 。</value>
        public string IndentChars
        {
            get { return _indentChars; }
            set { _indentChars = value; }
        }

        private void WritePropertyDelimiter()
        {
            if (_State == State.EndObject || _State == State.EndProperty || _State == State.EndArray)
            {
                Writer.Write(',');
#if DEBUG
                Debug.WriteLine("WritePropertyDelimiter");
#endif
            }
        }

        /// <summary>
        ///     执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                IDisposable d = Writer as IDisposable;
                if (d != null)
                    d.Dispose();
            }
        }

        private enum State
        {
            Start,
            StartDocument,
            EndDocument,
            StartObject,
            EndObject,
            Property,
            StartArray,
            EndArray,
            StartProperty,
            EndProperty
        }

        public ISerializeSetting Setting { get; set; }
        public Type IgnoreAttributeType { get { return typeof(object); }}
    }
}