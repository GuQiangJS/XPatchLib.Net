// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     XML写入器。
    /// </summary>
    /// <seealso cref="XPatchLib.ITextWriter" />
    internal class XmlTextWriter : ITextWriter
    {
        private readonly XmlWriter Writer;

        /// <summary>
        ///     以指定的 <paramref name="pWriter" /> 实例创建 <see cref="XmlTextWriter" /> 类型实例。
        /// </summary>
        /// <param name="pWriter">The p writer.</param>
        internal XmlTextWriter(XmlWriter pWriter)
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
        ///     执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((IDisposable) Writer)?.Dispose();
            }
        }
    }
}