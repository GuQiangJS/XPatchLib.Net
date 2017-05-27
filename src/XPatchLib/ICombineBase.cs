// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;

namespace XPatchLib
{
    internal interface ICombineBase
    {
        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">增量内容读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pReader" /> is null 时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pReader" /> is null 时。</exception>
        /// <exception cref="ArgumentException">当参数 <paramref name="pName" /> 长度为 0 时。</exception>
        object Combine(ITextReader pReader, Object pOriObject, string pName);
    }
}