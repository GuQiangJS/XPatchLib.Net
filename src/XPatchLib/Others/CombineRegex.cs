using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace XPatchLib.Others
{
    internal class CombineRegex: CombineOtherObjectBase
    {
        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.CombineBase" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        public CombineRegex(TypeExtend pType) : base(pType)
        {
        }

        /// <summary>
        /// 获取现有对象需要比较的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected override Dictionary<string, object> GetOriValues(object obj, IEqualityComparer<string> comparer)
        {
            Regex r = obj as Regex;
            Dictionary<string, object> result = new Dictionary<string, object>(comparer);
            result.Add(DivideAndCombineRegex.PatternName,
                r != null ? DivideAndCombineRegex.GetPattern(r) : string.Empty);

            result.Add(DivideAndCombineRegex.OptionsName,
                r != null ? DivideAndCombineRegex.GetOptions(r) : DivideAndCombineRegex.OptionsDefaultValue);

#if NET_45_UP || NETSTANDARD
            result.Add(DivideAndCombineRegex.MatchTimeoutName,
                r != null ? DivideAndCombineRegex.GetMatchTimeout(r) : DivideAndCombineRegex.MatchTimeoutDefaultValue);
#endif
            return result;
        }

        protected override object CreateInstance(Dictionary<string, object> values)
        {
            object pattern = values[DivideAndCombineRegex.PatternName];
            object option = values[DivideAndCombineRegex.OptionsName];
#if NET_45_UP || NETSTANDARD
            object timeout = values[DivideAndCombineRegex.MatchTimeoutName];
            return new Regex(pattern.ToString(), (RegexOptions)option, (TimeSpan)timeout);
#endif
            return new Regex(pattern.ToString(), (RegexOptions) option);
        }

        protected override object GetMemberValue(string proName, object pObj, ITextReader pReader)
        {
            if (string.Equals(proName, DivideAndCombineRegex.PatternName, StringComparison.OrdinalIgnoreCase))
                return new CombineBasic(
                        TypeExtendContainer.GetTypeExtend(pReader.Setting, typeof(string), null, null))
                    .Combine(pReader, pObj, proName).ToString();
            else if (string.Equals(proName, DivideAndCombineRegex.OptionsName, StringComparison.OrdinalIgnoreCase))
                return (int)new CombineBasic(
                        TypeExtendContainer.GetTypeExtend(pReader.Setting, typeof(int), null, null))
                    .Combine(pReader, pObj, proName);
#if NET_45_UP || NETSTANDARD
            else if (string.Equals(proName, DivideAndCombineRegex.MatchTimeoutName, StringComparison.OrdinalIgnoreCase))
                return (TimeSpan)new CombineBasic(
                        TypeExtendContainer.GetTypeExtend(pReader.Setting, typeof(TimeSpan), null, null))
                    .Combine(pReader, pObj, proName);
#endif
            return null;
        }
    }
}
