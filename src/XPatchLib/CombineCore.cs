// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     增量内容合并入口类。
    /// </summary>
    /// <seealso cref="XPatchLib.CombineBase" />
    /// <remarks>
    ///     此类是增量内容合并的入口类，由此类区分待产生增量内容的对象类型，调用不同的增量内容产生类。
    /// </remarks>
    internal class CombineCore : CombineBase
    {
        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        protected override Object CombineAction(ITextReader pReader, Object pOriObject, String pName)
        {
            if (pOriObject == null && Type.IsISerializable)
            {
                pOriObject = Type.CreateInstance();
            }

            return CombineInstanceContainer.GetCombineInstance(Type).Combine(pReader, pOriObject, pName);
        }

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.CombineCore" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        internal CombineCore(TypeExtend pType)
            : base(pType)
        {
        }

        #endregion Internal Constructors
    }
}