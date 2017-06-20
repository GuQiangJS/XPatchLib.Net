// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;

namespace XPatchLib
{
    /// <summary>
    ///     表示提供快速、非缓存、只进方法的写入器，该方法生成包含指定数据结构的流或文件。
    /// </summary>
    /// <seealso cref="XmlTextWriter"/>
    public interface ITextWriter : IDisposable
    {
        /// <summary>
        ///     获取或设置指示 <see cref="Serializer" /> 方法 <see cref="Serializer.Divide" /> 进行序列化的公共字段或公共读/写属性值。
        /// </summary>
        /// <remarks>
        ///     用于控制如何 <see cref="Serializer" /> 方法 <see cref="Serializer.Divide" /> 序列化对象。
        /// </remarks>
        /// <seealso cref="XmlTextWriter.IgnoreAttributeType"/>
        Type IgnoreAttributeType { get; set; }

        /// <summary>
        ///     获取或设置写入器设置。
        /// </summary>
        ISerializeSetting Setting { get; set; }

        /// <summary>
        ///     写入文档开始标记。
        /// </summary>
        void WriteStartDocument();

        /// <summary>
        ///     写入文档结束标记。
        /// </summary>
        void WriteEndDocument();

        /// <summary>
        ///     将缓冲区中的所有内容刷新到基础流，并同时刷新基础流。
        /// </summary>
        void Flush();

        /// <summary>
        ///     写入对象开始标记。
        /// </summary>
        /// <param name="pName">对象名称。</param>
        void WriteStartObject(string pName);

        /// <summary>
        ///     写入对象结束标记。
        /// </summary>
        void WriteEndObject();

        /// <summary>
        ///     写入特性。
        /// </summary>
        /// <param name="pName">特性名称。</param>
        /// <param name="pValue">特性值。</param>
        void WriteAttribute(string pName, string pValue);

        /// <summary>
        ///     写入属性。
        /// </summary>
        /// <param name="pName">属性名称。</param>
        /// <param name="pValue">属性值。</param>
        void WriteProperty(string pName, string pValue);

        /// <summary>
        ///     写入属性开始标记。
        /// </summary>
        /// <param name="pName">属性名称。</param>
        void WriteStartProperty(string pName);

        /// <summary>
        ///     写入属性结束标记。
        /// </summary>
        void WriteEndProperty();

        /// <summary>
        ///     写入列表类型对象开始标记。
        /// </summary>
        ///     <param name="pName">列表类型对象实例名称。</param>
        void WriteStartArray(string pName);

        /// <summary>
        /// 写入列表元素对象开始标记。
        /// </summary>
        /// <param name="pName">列表元素对象实例名称。</param>
        void WriteStartArrayItem(string pName);

        /// <summary>
        /// 写入列表元素结束标记。
        /// </summary>
        void WriteEndArrayItem();

        /// <summary>
        ///     写入列表对象结束标记。
        /// </summary>
        void WriteEndArray();

        /// <summary>
        ///     写入文本。
        /// </summary>
        /// <param name="pValue">待写入的文本。</param>
        void WriteValue(string pValue);
    }
}