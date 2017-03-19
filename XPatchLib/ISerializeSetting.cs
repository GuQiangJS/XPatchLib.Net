// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;

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
        ///     获取或设置在字符串与 <see cref="DateTime" /> 之间转换时，如何处理时间值。
        /// </summary>
        DateTimeSerializationMode Mode { get; set; }

        /// <summary>
        ///     获取或设置是否序列化默认值。
        /// </summary>
        bool SerializeDefalutValue { get; set; }
    }
}