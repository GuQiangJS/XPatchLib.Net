// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace XPatchLib
{
    /// <summary>
    ///     增量内容动作类型。
    /// </summary>
    internal enum Action
    {
        /// <summary>
        ///     编辑
        /// </summary>
        Edit = 0,

        /// <summary>
        ///     新增
        /// </summary>
        Add = 1,

        /// <summary>
        ///     删除
        /// </summary>
        Remove = 2,

        /// <summary>
        ///     设置为Null
        /// </summary>
        SetNull = 3
    }

    /// <summary>
    ///     <see cref="XPatchLib.Action" /> 帮助类。
    /// </summary>
    /// <remarks>
    ///     主要来用做由本文向<see cref="XPatchLib.Action" />对象实例转换。
    /// </remarks>
    internal static class ActionHelper
    {
        internal static String ToString(Action pAction)
        {
            switch(pAction)
            {
                case Action.Edit:
                    return "Edit";
                case Action.Add:
                    return "Add";
                case Action.Remove:
                    return "Remove";
                case Action.SetNull:
                    return "SetNull";
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     将一个枚举常数的名称的字符串表示转换成等效的枚举对象。用于指示转换是否成功的返回值。
        /// </summary>
        /// <param name="actionValue">要转换的枚举名称的字符串表示形式。</param>
        /// <param name="action">此方法在返回时包含一个类型为 <see cref="XPatchLib.Action" /> 的一个对象，其值由 value 表示。该参数未经初始化即被传递。</param>
        /// <returns>如果 <paramref name="actionValue" /> 参数成功转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
        internal static Boolean TryParse(string actionValue, out Action action)
        {
            action = Action.Edit;
            if (string.IsNullOrEmpty(actionValue))
                return false;
            switch (actionValue.ToUpper(CultureInfo.InvariantCulture))
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