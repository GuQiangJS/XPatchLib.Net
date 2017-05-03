using System;

namespace XPatchLib {
    /// <summary>
    ///     注册类型用的对象模型。
    /// </summary>
    public class RegistryKey {
        /// <summary>
        ///     获取或设置当前注册主键的类型。
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        ///     获取或设置当前注册主键的类型的父对象的类型。
        /// </summary>
        public Type ParentType { get; set; }

        /// <summary>
        ///     获取或设置当前注册主键的类型中需要被注册为主键的属性名集合。
        /// </summary>
        public string[] KeyPropertyNames { get; set; }
    }
}