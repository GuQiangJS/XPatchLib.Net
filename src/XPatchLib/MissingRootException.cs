// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;

namespace XPatchLib
{
    /// <summary>
    ///     增量文档缺少根节点异常。
    /// </summary>
    public class MissingRootException : Exception
    {
        /// <summary>
        ///     初始化 <see cref="XPatchLib.AttributeMissException" /> 类的新实例。
        /// </summary>
        public MissingRootException() : base(ResourceHelper.GetResourceString(LocalizationRes.Exp_String_MissingRoot))
        {
        }
    }
}