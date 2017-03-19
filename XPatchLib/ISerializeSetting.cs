// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Text;

namespace XPatchLib
{
    /// <summary>
    ///     序列化/反序列化时的设置。
    /// </summary>
    /// <seealso cref="XmlSerializeSetting" />
    public interface ISerializeSetting
    {
        /// <summary>
        ///     获取或设置序列化/反序列化时，文本中标记 '<b>动作</b>' 的文本。
        /// </summary>
        string ActionName { get; set; }

        /// <summary>
        ///     获取或设置要使用的文本编码的类型。
        /// </summary>
        Encoding Encoding { get; set; }

        /// <summary>
        ///     获取或设置在字符串与 <see cref="DateTime" /> 之间转换时，如何处理时间值。
        /// </summary>
        DateTimeSerializationMode Mode { get; set; }

        /// <summary>
        ///     获取或设置如何对输出进行格式设置。
        /// </summary>
        Formatting Formatting { get; set; }

        /// <summary>
        ///     获取或设置用于缩进时用于转换的字符 <see cref="Formatting" /> 设置为 <see cref="Formatting.Indented" />。
        /// </summary>
        string IndentChars { get; set; }

        /// <summary>
        ///     获取或设置是否序列化默认值。
        /// </summary>
        bool SerializeDefalutValue { get; set; }
    }
}