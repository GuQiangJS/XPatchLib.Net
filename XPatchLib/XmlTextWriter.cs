// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace XPatchLib
{
    /// <summary>
    ///     表示提供快速、非缓存、只进方法的写入器，该方法生成包含 XML 数据（这些数据符合 W3C 可扩展标记语言 (XML) 1.0 和“XML 命名空间”建议）的流或文件。
    /// </summary>
    /// <seealso cref="XPatchLib.ITextWriter" />
    public class XmlTextWriter : ITextWriter
    {
        private readonly XmlWriter _writer;

        private ISerializeSetting _setting = new XmlSerializeSetting();

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
            IgnoreAttributeType = typeof(XmlIgnoreAttribute);
            _writer = pWriter;
        }

        /// <summary>
        ///     写入文档开始标记。
        /// </summary>
        public void WriteStartDocument()
        {
            _writer.WriteStartDocument();
        }

        /// <summary>
        ///     写入文档结束标记。
        /// </summary>
        public void WriteEndDocument()
        {
            _writer.WriteEndDocument();
        }

        /// <summary>
        ///     将缓冲区中的所有内容刷新到基础流，并同时刷新基础流。
        /// </summary>
        public void Flush()
        {
            _writer.Flush();
        }

        /// <summary>
        ///     写入对象开始标记。
        /// </summary>
        /// <param name="pName">对象名称。</param>
        public void WriteStartObject(string pName)
        {
            _writer.WriteStartElement(pName);
#if DEBUG
            Debug.WriteLine(string.Format("WriteStartObject '{0}'.", pName));
#endif
        }

        /// <summary>
        ///     写入对象结束标记。
        /// </summary>
        public void WriteEndObject()
        {
            if (_writer.WriteState == WriteState.Content || _writer.WriteState == WriteState.Element)
            {
                _writer.WriteEndElement();
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
            _writer.WriteAttributeString(pName, pValue);
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
            _writer.WriteElementString(pName, pValue);
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
            _writer.WriteStartElement(pName);
#if DEBUG
            Debug.WriteLine(string.Format("WriteStartArray '{0}'.", pName));
#endif
        }

        /// <summary>
        ///     写入列表对象结束标记。
        /// </summary>
        public void WriteEndArray()
        {
            if (_writer.WriteState == WriteState.Content || _writer.WriteState == WriteState.Element)
            {
                _writer.WriteEndElement();
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
            _writer.WriteStartElement(pName);
#if DEBUG
            Debug.WriteLine(string.Format("WriteStartProperty '{0}'.", pName));
#endif
        }

        /// <summary>
        ///     写入属性结束标记。
        /// </summary>
        public void WriteEndProperty()
        {
            if (_writer.WriteState == WriteState.Content || _writer.WriteState == WriteState.Element)
            {
                _writer.WriteEndElement();
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
            _writer.WriteString(pValue);
#if DEBUG
            Debug.WriteLine(string.Format("WriteValue '{0}'.", pValue));
#endif
        }

        /// <summary>
        ///     执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     获取或设置写入器设置。
        /// </summary>
        /// <value>默认返回以构造函数中传入的<see cref="XmlWriter" />作为参数创建的<see cref="XmlSerializeSetting" />实例。</value>
        public ISerializeSetting Setting
        {
            get { return _setting; }
            set
            {
                if (value != null) _setting = value;
            }
        }

        /// <summary>
        ///     获取指示 <see cref="Serializer" /> 方法 <see cref="Serializer.Divide" /> 进行序列化的公共字段或公共读/写属性值。
        /// </summary>
        /// <remarks>
        ///     用于控制如何 <see cref="Serializer" /> 方法 <see cref="Serializer.Divide" /> 序列化对象。
        /// </remarks>
        /// <value>
        ///     <see cref="XmlIgnoreAttribute" />
        /// </value>
        public Type IgnoreAttributeType { get; internal set; }

        /// <summary>
        ///     执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                _writer.Flush();
            }
            catch (ObjectDisposedException)
            {
            }
            //if (disposing)
            //    ((IDisposable) Writer)?.Dispose();
        }
    }
}