// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace XPatchLib
{
    /// <summary>
    ///     枚举对象包装器。
    /// </summary>
    internal class EnumWrapper
    {
        #region Private Fields

        //private const char SPLIT_CHAR = ',';

        ///// <summary>
        ///// 枚举成员
        ///// </summary>
        //private Dictionary<string, string> _Members;

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)] private readonly Type _type;

        #endregion Private Fields

        #region Internal Constructors

        /// <summary>
        ///     使用指定的枚举类型初始化 <see cref="XPatchLib.EnumWrapper" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的枚举类型。
        /// </param>
        /// <exception cref="ArgumentException">
        ///     当 <paramref name="pType" /> 不是枚举类型时。
        /// </exception>
        internal EnumWrapper(Type pType)
            : this()
        {
            if (!pType.IsEnum)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "类型 {0} 不是枚举类型。", pType.FullName), "pType");

            _type = pType;
        }

        #endregion Internal Constructors

        #region Private Constructors

        private EnumWrapper()
        {
            //_Members = new Dictionary<string, string>();
        }

        #endregion Private Constructors

        /// <summary>
        ///     当前包装器包装的枚举类型。
        /// </summary>
        /// <returns>
        /// </returns>
        internal new Type GetType()
        {
            return _type;
        }

        #region Internal Methods

        /// <summary>
        ///     将指定的枚举名称，转换为枚举对象。
        /// </summary>
        /// <param name="pEnumString">
        /// </param>
        /// <returns>
        /// </returns>
        internal Object TransFromString(String pEnumString)
        {
            return Enum.Parse(GetType(), pEnumString);
        }

        /// <summary>
        ///     获取枚举对象的值。
        /// </summary>
        /// <param name="pEnumObject">
        /// </param>
        /// <returns>
        ///     <para> 返回枚举对象的Name。 </para>
        ///     <para>
        ///         对于标记了 <see cref="System.FlagsAttribute" /> 特性的枚举对象，如果存在多个枚举值，则会在多个枚举值之间用 ', ' 进行分割。
        ///     </para>
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal Object TransToString(Object pEnumObject)
        {
            if (pEnumObject == null)
                return null;

            return pEnumObject.ToString();
        }

        #endregion Internal Methods
    }
}