// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if NET ||NETSTANDARD_1_3_UP

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace XPatchLib
{
    internal abstract class ConverterFileSystemInfo : OtherConverterBase
    {
        protected const string ORIGINALPATHNAME = "OriginalPath";
        private static FieldInfo OriginalPathInfo;

        static ConverterFileSystemInfo()
        {
            OriginalPathInfo = typeof(FileSystemInfo).GetField(ORIGINALPATHNAME,
                BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
            _memberWrappers = new MemberWrapper[] {new MemberWrapper(OriginalPathInfo),};
        }

        public ConverterFileSystemInfo(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        public ConverterFileSystemInfo(TypeExtend pType) : base(pType)
        {
        }

        private static MemberWrapper[] _memberWrappers;
        internal override MemberWrapper[] DivideMembers { get { return _memberWrappers; } }

        protected override object GetMemberValue(MemberWrapper member, object Obj)
        {
            return GetOriginalPath(Obj);
        }

        string GetOriginalPath(object obj)
        {
            if (obj == null)
                return null;
            object result = OriginalPathInfo.GetValue(obj);
            return result != null ? result.ToString() : null;
        }

        protected override Dictionary<string, object> GetOriValues(object obj, IEqualityComparer<string> comparer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>(comparer);
            result.Add(ORIGINALPATHNAME, GetOriginalPath(obj));
            return result;
        }

        protected override object GetMemberValue(string proName, object pObj, ITextReader pReader)
        {
            if (string.Equals(proName, ORIGINALPATHNAME, StringComparison.OrdinalIgnoreCase))
                return new ConverterBasic(
                        TypeExtendContainer.GetTypeExtend(pReader.Setting, typeof(string), null, null))
                    .Combine(pReader, pObj, proName).ToString();
            return null;
        }
    }
}
#endif