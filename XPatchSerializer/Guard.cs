// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace XPatchLib
{
    /// <summary>
    ///     一个静态的辅助类，包括各种参数的检测程序。
    /// </summary>
    internal static class Guard
    {
        #region Public Methods

        /// <summary>
        ///     当参数 <paramref name="pArgumentValue" /> is null 时，抛出 <see cref="ArgumentNullException" /> 异常。
        /// </summary>
        /// <param name="pArgumentValue">待测试的参数实例。</param>
        /// <param name="pArgumentName">待测试的参数名称。</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pArgumentValue" /> is null 时。</exception>
        public static void ArgumentNotNull(object pArgumentValue, string pArgumentName)
        {
            if (pArgumentValue == null)
                throw new ArgumentNullException(pArgumentName);
        }

        /// <summary>
        ///     当参数 <paramref name="pArgumentValue" /> 长度为 0 时，抛出 <see cref="ArgumentException" /> 异常。
        /// </summary>
        /// <param name="pArgumentValue">待测试的参数实例。</param>
        /// <param name="pArgumentName">待测试的参数名称。</param>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pArgumentValue" /> is null 时。</exception>
        /// <exception cref="ArgumentException">当参数 <paramref name="pArgumentValue" /> 长度为 0 时。</exception>
        public static void ArgumentNotNullOrEmpty(ICollection pArgumentValue, string pArgumentName)
        {
            ArgumentNotNull(pArgumentValue, pArgumentName);

            if (pArgumentValue.Count == 0)
                throw new ArgumentException("", pArgumentName);
        }

        /// <summary>
        ///     当参数
        ///     <para>
        ///         <paramref name="pArgumentValue" /> 长度为 0 时，抛出 <see cref="ArgumentException" /> 异常。
        ///     </para>
        ///     <para>
        ///         <paramref name="pArgumentValue" /> is null 时，抛出 <see cref="ArgumentNullException" /> 异常。
        ///     </para>
        /// </summary>
        /// <param name="pArgumentValue">待测试的参数实例。</param>
        /// <param name="pArgumentName">待测试的参数名称。</param>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pArgumentValue" /> is null 时。</exception>
        /// <exception cref="ArgumentException">当参数 <paramref name="pArgumentValue" /> 长度为 0 时。</exception>
        public static void ArgumentNotNullOrEmpty(string pArgumentValue, string pArgumentName)
        {
            ArgumentNotNull(pArgumentValue, pArgumentName);

            if (pArgumentValue.Length == 0)
                throw new ArgumentException("", pArgumentName);
        }

        /// <summary>
        ///     当参数 <paramref name="pFileFullName" /> 标识的文件不存在时。抛出 <see cref="FileNotFoundException" /> 异常。
        /// </summary>
        /// <param name="pFileFullName">待检测的文件完整路径。</param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pFileFullName" /> is null 时。</exception>
        /// <exception cref="ArgumentException">当参数 <paramref name="pFileFullName" /> 长度为 0 时。</exception>
        /// <exception cref="FileNotFoundException">当参数 <paramref name="pFileFullName" /> 指定的文件不存在时。</exception>
        public static void FileNotFound(string pFileFullName)
        {
            ArgumentNotNullOrEmpty(pFileFullName, "pFileFullName");
            if (!File.Exists(pFileFullName))
                throw new FileNotFoundException(
                    string.Format(CultureInfo.InvariantCulture, "文件 {0} 不存在。", pFileFullName), pFileFullName);
        }

        #endregion Public Methods
    }
}