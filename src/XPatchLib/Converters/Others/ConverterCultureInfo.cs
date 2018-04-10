// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace XPatchLib
{
    internal class ConverterCultureInfo : OtherConverterBase
    {
        private const string NAMENAME = "Name";
        private static readonly MemberWrapper[] _memberWrappers;

        static ConverterCultureInfo()
        {
            PropertyInfo name = typeof(CultureInfo).GetProperty(NAMENAME,
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            _memberWrappers = new[] {new MemberWrapper(name)};
        }

        public ConverterCultureInfo(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        public ConverterCultureInfo(TypeExtend pType) : base(pType)
        {
        }

        /// <summary>
        /// 获取参与序列化的属性集合
        /// </summary>
        internal override MemberWrapper[] DivideMembers
        {
            get { return _memberWrappers; }
        }

        /// <summary>
        /// 获取 <paramref name="Obj"/> 中 <paramref name="member"/> 的值
        /// </summary>
        /// <param name="member"></param>
        /// <param name="Obj"></param>
        /// <returns></returns>
        protected override object GetMemberValue(MemberWrapper member, object Obj)
        {
            return GetName(Obj);
        }

        private string GetName(object obj)
        {
            CultureInfo culture = obj as CultureInfo;
            if (culture == null)
                return null;
            return culture.Name;
        }

        /// <summary>
        ///     获取现有对象需要比较的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected override Dictionary<string, object> GetOriValues(object obj, IEqualityComparer<string> comparer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>(comparer);
            result.Add(NAMENAME, GetName(obj));
            return result;
        }

        /// <summary>
        /// 根据字典创建当前对象类型的实例
        /// </summary>
        /// <param name="values">参数名称-值的字典</param>
        /// <returns></returns>
        protected override object CreateInstance(Dictionary<string, object> values)
        {
            return new CultureInfo(values[NAMENAME].ToString());
        }

        /// <summary>
        /// 读取原始对象 <paramref name="pObj"/> 指定属性 <paramref name="proName"/> 的值，并且合并 <paramref name="pReader"/> 中的增量
        /// </summary>
        /// <param name="proName">属性名称</param>
        /// <param name="pObj">原始对象</param>
        /// <param name="pReader"></param>
        /// <returns>返回合并后的结果</returns>
        protected override object GetMemberValue(string proName, object pObj, ITextReader pReader)
        {
            if (string.Equals(proName, NAMENAME, StringComparison.OrdinalIgnoreCase))
                return new ConverterBasic(
                        TypeExtendContainer.GetTypeExtend(pReader.Setting, typeof(string), null, null))
                    .Combine(pReader, pObj, proName).ToString();
            return null;
        }
    }
}