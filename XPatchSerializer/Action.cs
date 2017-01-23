using System;
using System.Globalization;
namespace XPatchLib
{
    /// <summary>
    /// 增量内容动作类型。 
    /// </summary>
    internal enum Action
    {
        /// <summary>
        /// 编辑 
        /// </summary>
        Edit = 0,

        /// <summary>
        /// 新增 
        /// </summary>
        Add = 1,

        /// <summary>
        /// 删除 
        /// </summary>
        Remove = 2,

        /// <summary>
        /// 设置为Null 
        /// </summary>
        SetNull = 3
    }

    internal class ActionHelper
    {
        internal static Boolean TryParse(string actionValue,out Action action)
        {
            action = default(Action);
            switch(actionValue.ToUpper(CultureInfo.InvariantCulture))
            {
                case "EDIT":
                    action = Action.Edit;
                    return true;
                case "ADD":
                    action = Action.Add;
                    return true;
                case "REMOVE":
                    action = Action.Remove;
                    return true;
                case "SETNULL":
                    action = Action.SetNull;
                    return true;
            }
            return false;
        }
    }
}