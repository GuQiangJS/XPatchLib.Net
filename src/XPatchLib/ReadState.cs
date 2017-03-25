// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

namespace XPatchLib
{
    /// <summary>
    ///     指定读取器的状态。
    /// </summary>
    public enum ReadState
    {
        /// <summary>
        ///     Close 调用方法。
        /// </summary>
        Closed,

        /// <summary>
        ///     已成功到达文件末尾。
        /// </summary>
        EndOfFile,

        /// <summary>
        ///     将出现错误，以防止读取的操作继续进行。
        /// </summary>
        Error,

        /// <summary>
        ///     Read 不调用方法。
        /// </summary>
        Initial,

        /// <summary>
        ///     Read 调用方法。 可以在读取器上调用其他方法。
        /// </summary>
        Interactive
    }
}