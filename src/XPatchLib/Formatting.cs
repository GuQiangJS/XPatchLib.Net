// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

namespace XPatchLib
{
    /// <summary>
    /// 指示如何对输出进行格式设置。
    /// </summary>
    public enum Formatting
    {
        /// <summary>
        ///     可能导致子元素根据缩进显示 Indentation 和 IndentChar 设置。
        /// </summary>
        Indented = 1,

        /// <summary>
        ///     尚无特殊格式设置将应用。 这是默认设置。
        /// </summary>
        None = 0
    }
}