// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;

namespace XPatchLib
{
    /// <summary>
    ///     产生增量的操作接口。
    /// </summary>
    internal interface IDivide
    {
        /// <summary>
        ///     获取或设置父级节点内容是否已经被写入。
        /// </summary>
        bool ParentElementWrited { get; set; }

        /// <summary>
        ///     产生增量内容。
        /// </summary>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。</returns>
        Boolean Divide(string pName, Object pOriObject, Object pRevObject, DivideAttachment pAttach);


        /// <summary>
        /// 从作为参数指定的增量产生器中复制设置。
        /// </summary>
        /// <param name="item">将其设置复制到当前对象。</param>
        void Assign(DivideBase item);
    }
}