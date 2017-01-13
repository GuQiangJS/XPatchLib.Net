using System;

namespace XPatchLib
{
    internal class ConstValue
    {
        internal static String ACTION_NAME { get { return "Action"; } }

        /// <summary>
        /// 格式化字符串。 
        /// </summary>
        internal static String COLOR_FORMAT { get { return "#{0:X}"; } }

        internal static String COLOR_STARTCHAR { get { return "#"; } }
        internal static String KEY { get { return "Key"; } }

        /// <summary>
        /// 集合类型 增加 操作方法名 
        /// </summary>
        internal static String OPERATOR_ADD { get { return "Add"; } }

        /// <summary>
        /// 集合类型 获取值 操作方法名 
        /// </summary>
        internal static String OPERATOR_GET { get { return "get_Item"; } }

        /// <summary>
        /// 集合类型 删除 操作方法名 
        /// </summary>
        internal static String OPERATOR_REMOVE { get { return "Remove"; } }

        /// <summary>
        /// 集合类型 更改值 操作方法名 
        /// </summary>
        internal static String OPERATOR_SET { get { return "set_Item"; } }

        internal static String UNDERLINE { get { return "_"; } }
        internal static String VALUE { get { return "Value"; } }
    }
}