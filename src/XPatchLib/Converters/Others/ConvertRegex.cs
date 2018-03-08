// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
#if (NET_35_UP || NETSTANDARD)
using System.Linq;
#endif

namespace XPatchLib
{
    internal sealed class ConvertRegex : OtherConverterBase
    {
        internal const string PatternName = "Pattern";
        internal const string OptionsName = "Options";
        internal const string MatchTimeoutName = "MatchTimeout";

        internal static Type OptionsType = typeof(int);
        internal static Type PatternType = typeof(string);
        internal static Type MatchTimeoutType = typeof(TimeSpan);

        internal static int OptionsDefaultValue;

        private static readonly PropertyInfo proOptions;
        private static readonly FieldInfo filedPattern;

        private static readonly MemberWrapper[] _members;

        static ConvertRegex()
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
                , new MemberWrapper(proMatchTimeout)
#endif
            };
            _members = members.OrderBy(x => x.Name).ToArray();
        }

        public ConvertRegex(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        public ConvertRegex(TypeExtend pType) : base(pType)
        {
        }

        internal override MemberWrapper[] DivideMembers
        {
            get { return _members; }
        }

        internal static string GetPattern(Regex obj)
        {
            if (obj == null)
                return string.Empty;
            return obj.ToString();
        }

        internal static int GetOptions(Regex obj)
        {
            if (obj == null)
                return OptionsDefaultValue;
            return (int) obj.Options;
        }

#if NET_45_UP || NETSTANDARD
        internal static TimeSpan GetMatchTimeout(Regex obj)
        {
            if (obj == null)
                return MatchTimeoutDefaultValue;
            return obj.MatchTimeout;
        }
#endif

        protected override object GetMemberValue(MemberWrapper member, object Obj)
        {
            Regex re = Obj as Regex;

            if (string.Equals(member.Name, PatternName, StringComparison.OrdinalIgnoreCase))
                return GetPattern(re);
            if (string.Equals(member.Name, OptionsName, StringComparison.OrdinalIgnoreCase))
                return GetOptions(re);
#if NET_45_UP || NETSTANDARD
            if (string.Equals(member.Name, MatchTimeoutName, StringComparison.OrdinalIgnoreCase))
                return GetMatchTimeout(re);
#endif
            throw new NotImplementedException();
        }

        protected override Dictionary<string, object> GetOriValues(object obj, IEqualityComparer<string> comparer)
        {
            Regex r = obj as Regex;
            Dictionary<string, object> result = new Dictionary<string, object>(comparer);
            result.Add(PatternName,
                r != null ? GetPattern(r) : string.Empty);

            result.Add(OptionsName,
                r != null ? GetOptions(r) : OptionsDefaultValue);

#if NET_45_UP || NETSTANDARD
            result.Add(MatchTimeoutName,
                r != null ? GetMatchTimeout(r) : MatchTimeoutDefaultValue);
#endif
            return result;
        }

        protected override object CreateInstance(Dictionary<string, object> values)
        {
            object pattern = values[PatternName];
            object option = values[OptionsName];
#if NET_45_UP || NETSTANDARD
            object timeout = values[MatchTimeoutName];
            return new Regex(pattern.ToString(), (RegexOptions) option, (TimeSpan) timeout);
#endif
            return new Regex(pattern.ToString(), (RegexOptions) option);
        }

        protected override object GetMemberValue(string proName, object pObj, ITextReader pReader)
        {
            if (string.Equals(proName, PatternName, StringComparison.OrdinalIgnoreCase))
                return new ConverterBasic(
                        TypeExtendContainer.GetTypeExtend(pReader.Setting, typeof(string), null, null))
                    .Combine(pReader, pObj, proName).ToString();
            if (string.Equals(proName, OptionsName, StringComparison.OrdinalIgnoreCase))
                return (int) new ConverterBasic(
                        TypeExtendContainer.GetTypeExtend(pReader.Setting, typeof(int), null, null))
                    .Combine(pReader, pObj, proName);
#if NET_45_UP || NETSTANDARD
            if (string.Equals(proName, MatchTimeoutName, StringComparison.OrdinalIgnoreCase))
                return (TimeSpan) new ConverterBasic(
                        TypeExtendContainer.GetTypeExtend(pReader.Setting, typeof(TimeSpan), null, null))
                    .Combine(pReader, pObj, proName);
#endif
            return null;
        }
#if NET_45_UP || NETSTANDARD
        private static readonly PropertyInfo proMatchTimeout;
        internal static TimeSpan MatchTimeoutDefaultValue;
#endif
    }
}