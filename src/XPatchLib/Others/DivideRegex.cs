// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
#if (NET_35_UP || NETSTANDARD)
using System.Linq;
#endif
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace XPatchLib.Others
{
    internal class DivideAndCombineRegex
    {
        internal const string PatternName = "Pattern";
        internal const string OptionsName = "Options";
        internal const string MatchTimeoutName = "MatchTimeout";

        internal static Type OptionsType = typeof(int);
        internal static Type PatternType = typeof(string);
        internal static Type MatchTimeoutType = typeof(TimeSpan);

        internal static int OptionsDefaultValue;

        internal static MemberWrapper[] Fields;

        private static readonly PropertyInfo proOptions;
        private static readonly FieldInfo filedPattern;


        static DivideAndCombineRegex()
        {
            Type t = typeof(Regex);
            Regex instance = new Regex(string.Empty);

            filedPattern = t.GetField(PatternName.ToLower(),
                BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

            proOptions = t.GetProperty(OptionsName,
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);

            OptionsDefaultValue = GetOptions(instance);

#if NET_45_UP || NETSTANDARD
            proMatchTimeout = t.GetProperty(MatchTimeoutName,
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            MatchTimeoutDefaultValue = GetMatchTimeout(instance);
#endif

            MemberWrapper[] members =
            {
                new MemberWrapper(filedPattern),
                new MemberWrapper(proOptions)
#if NET_45_UP || NETSTANDARD
                ,new MemberWrapper(proMatchTimeout)
#endif
            };
            Fields = members.OrderBy(x => x.Name).ToArray();
        }

        internal static string GetPattern(Regex obj)
        {
            if(obj==null)
                return string.Empty;
            return obj.ToString();
        }

        internal static int GetOptions(Regex obj)
        {
            if (obj == null)
                return OptionsDefaultValue;
            return (int)obj.Options;
        }

#if NET_45_UP || NETSTANDARD
        internal static TimeSpan GetMatchTimeout(Regex obj)
        {
            if (obj == null)
                return MatchTimeoutDefaultValue;
            return (TimeSpan) obj.MatchTimeout;
        }
#endif
#if NET_45_UP || NETSTANDARD
        private static readonly PropertyInfo proMatchTimeout;
        internal static TimeSpan MatchTimeoutDefaultValue;
#endif
    }
    

    internal class DivideRegex : DivideOtherObjectBase
    {
        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideBase" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pWriter" /> is null 时。</exception>
        public DivideRegex(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        protected override object GetMemberValue(MemberWrapper member, object Obj)
        {
            Regex re = Obj as Regex;
            
            if(string.Equals(member.Name,DivideAndCombineRegex.PatternName,StringComparison.OrdinalIgnoreCase))
                return DivideAndCombineRegex.GetPattern(re);
            if (string.Equals(member.Name, DivideAndCombineRegex.OptionsName, StringComparison.OrdinalIgnoreCase))
                return DivideAndCombineRegex.GetOptions(re);
#if NET_45_UP || NETSTANDARD
            if (string.Equals(member.Name, DivideAndCombineRegex.MatchTimeoutName, StringComparison.OrdinalIgnoreCase))
                return DivideAndCombineRegex.GetMatchTimeout(re);
#endif
            throw new NotImplementedException();
        }

        internal override MemberWrapper[] DivideMembers { get { return DivideAndCombineRegex.Fields; } }
    }
}