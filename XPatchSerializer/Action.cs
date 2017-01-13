namespace XPatchLib
{
    /// <summary>
    /// 增量内容动作类型。 
    /// </summary>
    internal enum Action
    {
        /// <summary>
        /// 编辑 
        /// </summary>
        Edit = 0,

        /// <summary>
        /// 新增 
        /// </summary>
        Add = 1,

        /// <summary>
        /// 删除 
        /// </summary>
        Remove = 2,

        /// <summary>
        /// 设置为Null 
        /// </summary>
        SetNull = 3
    }
}