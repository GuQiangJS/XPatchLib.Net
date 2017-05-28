// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#if SYSTEM_LINQ  //.NET 3.5
using System.Linq;
#endif

namespace XPatchLib
{
    /// <summary>
    ///     主键集合特性标记。
    /// </summary>
    /// <remarks>
    ///     用来标记一个对象由哪些属性组合成为主键，用以判断对象是否相等，并在生成增量文档时作为标记。
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class PrimaryKeyAttribute : Attribute
    {
        #region Private Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)] private readonly string[] PrimaryKeys;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///     使用指定的主键名称初始化 <see cref="XPatchLib.PrimaryKeyAttribute" /> 类的新实例。
        /// </summary>
        /// <param name="pPrimaryKeys">
        ///     指定的主键名称。
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="pPrimaryKeys" /> 为空时。
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="pPrimaryKeys" /> 的长度小于1时。
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1062:验证公共方法的参数", MessageId = "0")]
        public PrimaryKeyAttribute(params string[] pPrimaryKeys)
        {
            Guard.ArgumentNotNullOrEmpty(pPrimaryKeys, "pPrimaryKeys");

            PrimaryKeys = ((string[]) pPrimaryKeys.Clone()).OrderBy(x => x).ToArray();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///     获取主键类型名称。
        /// </summary>
        /// <returns>
        ///     返回主键名称数组。
        /// </returns>
        public string[] GetPrimaryKeys()
        {
            return PrimaryKeys;
        }

        #endregion Public Methods
    }
}