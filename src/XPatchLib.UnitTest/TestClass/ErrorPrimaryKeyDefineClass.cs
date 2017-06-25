// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

namespace XPatchLib.UnitTest.TestClass
{
    /// <summary>
    ///     错误的主键定义类，用来验证定义了非基础类型的对象将无法被处理
    /// </summary>
    [PrimaryKey("Author")]
    internal class ErrorPrimaryKeyDefineClass
    {
        #region Public Properties

        public AuthorClass Author { get; set; }

        #endregion Public Properties
    }
}