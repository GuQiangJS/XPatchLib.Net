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