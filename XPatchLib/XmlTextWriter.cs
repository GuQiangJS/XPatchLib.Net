// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     XML写入器。
    /// </summary>
    /// <seealso cref="XPatchLib.ITextWriter" />
    public class XmlTextWriter : ITextWriter
    {
        private readonly XmlWriter Writer;

        /// <summary>
        ///     以指定的 <paramref name="pWriter" /> 实例创建 <see cref="XmlTextWriter" /> 类型实例。
        /// </summary>
        /// <param name="pWriter">指定的 XML 编写器。</param>
        /// <remarks>
        ///     <para>
        ///         默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。
        ///     </para>
        ///     <para> 默认不序列化默认值。 </para>
        /// </remarks>
        public XmlTextWriter(XmlWriter pWriter)
        {
            Guard.ArgumentNotNull(pWriter, "pWriter");
            Writer = pWriter;
        }

        /// <summary>
        ///     写入文档开始标记。
        /// </summary>
        public void WriteStartDocument()
        {
            Writer.WriteStartDocument();
        }

        /// <summary>
        ///     写入文档结束标记。
        /// </summary>
        public void WriteEndDocument()
        {
            Writer.WriteEndDocument();
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
            Writer.WriteStartElement(pName);
#if DEBUG
            Debug.WriteLine(string.Format("WriteStartObject '{0}'.", pName));
#endif
        }

        /// <summary>
        ///     写入对象结束标记。
        /// </summary>
        public void WriteEndObject()
        {
            if (Writer.WriteState == WriteState.Content || Writer.WriteState == WriteState.Element)
            {
                Writer.WriteEndElement();
#if DEBUG
                Debug.WriteLine("WriteEndObject.");
#endif
            }
        }

        /// <summary>
        ///     写入特性。
        /// </summary>
        /// <param name="pName">特性名称。</param>
        /// <param name="pValue">特性值。</param>
        public void WriteAttribute(string pName, string pValue)
        {
            Writer.WriteAttributeString(pName, pValue);
#if DEBUG
            Debug.WriteLine("WriteAttribute '{0}'='{1}'.", pName, pValue);
#endif
        }

        /// <summary>
        ///     写入属性。
        /// </summary>
        /// <param name="pName">属性名称。</param>
        /// <param name="pValue">属性值。</param>
        public void WriteProperty(string pName, string pValue)
        {
            Writer.WriteElementString(pName, pValue);
#if DEBUG
            Debug.WriteLine("WriteProperty '{0}'='{1}'.", pName, pValue);
#endif
        }

        /// <summary>
        ///     写入列表类型对象开始标记。
        /// </summary>
        /// <param name="pName">列表类型对象实例名称</param>
        public void WriteStartArray(string pName)
        {
            Writer.WriteStartElement(pName);
#if DEBUG
            Debug.WriteLine(string.Format("WriteStartArray '{0}'.", pName));
#endif
        }

        /// <summary>
        ///     写入列表对象结束标记。
        /// </summary>
        public void WriteEndArray()
        {
            if (Writer.WriteState == WriteState.Content || Writer.WriteState == WriteState.Element)
            {
                Writer.WriteEndElement();
#if DEBUG
                Debug.WriteLine("WriteEndArray.");
#endif
            }
        }

        /// <summary>
        ///     写入属性开始标记。
        /// </summary>
        /// <param name="pName">属性名称。</param>
        public void WriteStartProperty(string pName)
        {
            Writer.WriteStartElement(pName);
#if DEBUG
            Debug.WriteLine(string.Format("WriteStartProperty '{0}'.", pName));
#endif
        }

        /// <summary>
        ///     写入属性结束标记。
        /// </summary>
        public void WriteEndProperty()
        {
            if (Writer.WriteState == WriteState.Content || Writer.WriteState == WriteState.Element)
            {
                Writer.WriteEndElement();
#if DEBUG
                Debug.WriteLine("WriteEndProperty.");
#endif
            }
        }

        /// <summary>
        ///     写入文本。
        /// </summary>
        /// <param name="pValue">待写入的文本。</param>
        public void WriteValue(string pValue)
        {
            Writer.WriteString(pValue);
#if DEBUG
            Debug.WriteLine(string.Format("WriteValue '{0}'.", pValue));
#endif
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     获取或设置在字符串与 <see cref="DateTime" /> 之间转换时，如何处理时间值。
        /// </summary>
        /// <value>默认为 <see cref="DateTimeSerializationMode.RoundtripKind" />。</value>
        public DateTimeSerializationMode Mode { get; set; } = DateTimeSerializationMode.RoundtripKind;


        /// <summary>
        ///     获取或设置是否序列化默认值。
        /// </summary>
        /// <value>默认为 <c>false</c>。</value>
        public bool SerializeDefalutValue { get; set; }

        /// <summary>
        ///     获取或设置如何对输出进行格式设置。
        /// </summary>
        /// <value><see cref="Formatting" /> 值之一。 默认值是 <see cref="Formatting.None" /> （无特殊格式）。</value>
        public Formatting Formatting
        {
            get { return Writer.Settings.Indent ? Formatting.Indented : Formatting.None; }
            set { Writer.Settings.Indent = value == Formatting.Indented; }
        }

        /// <summary>
        ///     获取或设置用于缩进时用于转换的字符 <see cref="Formatting" /> 设置为 <see cref="Formatting.Indented" />。
        /// </summary>
        public string IndentChars
        {
            get { return Writer.Settings.IndentChars; }
            set { Writer.Settings.IndentChars = value; }
        }

        /// <summary>
        ///     获取或设置要使用的文本编码的类型。
        /// </summary>
        public Encoding Encoding
        {
            get { return Writer.Settings.Encoding; }
            set { Writer.Settings.Encoding = value; }
        }

        /// <summary>
        ///     执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected void Dispose(bool disposing)
        {
            Writer.Flush();
            //if (disposing)
            //    ((IDisposable) Writer)?.Dispose();
        }
    }
}