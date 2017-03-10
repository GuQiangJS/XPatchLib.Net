using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPatchLib
{
    public enum Formatting
    {
        /// <summary>
        /// 可能导致子元素根据缩进显示 Indentation 和 IndentChar 设置。
        /// </summary>
        Indented,

        /// <summary>
        /// 尚无特殊格式设置将应用。 这是默认设置。
        /// </summary>
        None
    }
}
