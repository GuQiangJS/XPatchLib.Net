// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     XML类型写入器的默认设置。
    /// </summary>
    /// <seealso cref="XPatchLib.ISerializeSetting" />
    public class XmlSerializeSetting : ISerializeSetting
    {
        private readonly XmlWriter _writer;
        private string _actionName = "Action";

        private Encoding _encoding = Encoding.UTF8;

        private Formatting _formatting = Formatting.None;

        private string _indentChars = string.Empty;

        /// <summary>
        ///     创建 <see cref="XmlSerializeSetting" /> 类型实例。
        /// </summary>
        /// <param name="pWriter">指定的 <see cref="System.Xml.XmlWriter" /> 写入器。</param>
        public XmlSerializeSetting(XmlWriter pWriter)
        {
            Guard.ArgumentNotNull(pWriter, "pWriter");
            _writer = pWriter;
        }

        /// <summary>
        ///     创建 <see cref="XmlSerializeSetting" /> 类型实例。
        /// </summary>
        public XmlSerializeSetting()
        {
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
        /// <value>
        ///     <para><see cref="Formatting" /> 值之一。</para>
        ///     <para>
        ///         如果创建实例时传入了指定的<see cref="XmlWriter" />实例，那么会根据指定实例的<see cref="XmlWriter" />.
        ///         <see cref="XmlWriterSettings.Indent" />的设置返回内容。
        ///         当指定实例的<see cref="XmlWriter" />.<see cref="XmlWriterSettings.Indent" /> 为 <b>true</b> 时，返回
        ///         <see cref="Formatting.Indented" /> ，否则返回
        ///         <see cref="Formatting.None" /> ,
        ///         同时，设置值时会根据设置值将对应的值赋值至指定实例的<see cref="XmlWriter" />.<see cref="XmlWriterSettings.Indent" />属性。
        ///     </para>
        ///     <para>默认值是 <see cref="Formatting.None" /> （当创建实例时没有指定<see cref="XmlWriter" />实例时）。</para>
        /// </value>
        public Formatting Formatting
        {
            get
            {
                if (_writer.Settings != null)
                    return _writer.Settings.Indent ? Formatting.Indented : Formatting.None;
                return _formatting;
            }
            set
            {
                if (_writer.Settings != null) _writer.Settings.Indent = value == Formatting.Indented;
                _formatting = value;
            }
        }

        /// <summary>
        ///     获取或设置用于缩进时用于转换的字符 <see cref="Formatting" /> 设置为 <see cref="Formatting.Indented" />。
        /// </summary>
        /// <value>
        ///     <para>
        ///         如果创建实例时传入了指定的<see cref="XmlWriter" />实例，那么会返回指定实例的<see cref="XmlWriter" />.
        ///         <see cref="XmlWriterSettings.IndentChars" />的设置值,
        ///         同时，设置值时会将值赋值至指定实例的<see cref="XmlWriter" />.<see cref="XmlWriterSettings.IndentChars" />属性。
        ///     </para>
        ///     <para>默认值是 <see cref="string.Empty" /> （当创建实例时没有指定<see cref="XmlWriter" />实例时）。</para>
        /// </value>
        public string IndentChars
        {
            get
            {
                if (_writer.Settings != null) return _writer.Settings.IndentChars;
                return _indentChars;
            }
            set
            {
                if (_writer.Settings != null) _writer.Settings.IndentChars = value;
                else _indentChars = value;
            }
        }

        /// <summary>
        ///     获取或设置要使用的文本编码的类型。
        /// </summary>
        /// <value>
        ///     <para>
        ///         如果创建实例时传入了指定的<see cref="XmlWriter" />实例，那么会返回指定实例的<see cref="XmlWriter" />.
        ///         <see cref="XmlWriterSettings.Encoding" />的设置值,
        ///         同时，设置值时会将值赋值至指定实例的<see cref="XmlWriter" />.<see cref="XmlWriterSettings.Encoding" />属性。
        ///     </para>
        ///     <para>默认值是 <see cref="Encoding.UTF8" /> （当创建实例时没有指定<see cref="XmlWriter" />实例时）。</para>
        /// </value>
        public Encoding Encoding
        {
            get
            {
                if (_writer.Settings != null) return _writer.Settings.Encoding;
                return _encoding;
            }
            set
            {
                if (_writer.Settings != null) _writer.Settings.Encoding = value;
                else _encoding = value;
            }
        }

        /// <summary>
        ///     获取或设置序列化/反序列化时，文本中标记 '<b>动作</b>' 的文本。
        /// </summary>
        /// <value>
        ///     默认值是 "<b>Action</b>" 。
        /// </value>
        /// <exception cref="ArgumentNullException">当设置值是传入 <b>null</b> 时。</exception>
        /// <exception cref="ArgumentException">当设置值为空时。</exception>
        public string ActionName
        {
            get { return _actionName; }
            set
            {
                Guard.ArgumentNotNullOrEmpty(value, "value");
                _actionName = value;
            }
        }
    }
}