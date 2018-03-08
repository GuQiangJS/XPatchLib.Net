// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if NET ||NETSTANDARD_2_0_UP

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace XPatchLib
{
    internal class ConverterDriveInfo : OtherConverterBase
    {
        private const string NAMENAME = "Name";
        private static PropertyInfo proName;

        static ConverterDriveInfo()
        {
            proName = typeof(DriveInfo).GetProperty(NAMENAME,
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            _memberWrappers = new MemberWrapper[] {new MemberWrapper(proName),};
        }

        public ConverterDriveInfo(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        public ConverterDriveInfo(TypeExtend pType) : base(pType)
        {
        }

        private static MemberWrapper[] _memberWrappers;
        internal override MemberWrapper[] DivideMembers { get { return _memberWrappers; } }

        protected override object GetMemberValue(MemberWrapper member, object Obj)
        {
            return GetName(Obj);
        }

        string GetName(object obj)
        {
            DriveInfo info= obj as DriveInfo;
            if (info == null || string.IsNullOrEmpty(info.Name))
                return null;
            //if (info == null)
            //    throw new ArgumentNullException("info");
            //if(string.IsNullOrEmpty(info.Name))
            //    throw new ArgumentNullException("info.Name");
            return info.Name;
        }

        protected override Dictionary<string, object> GetOriValues(object obj, IEqualityComparer<string> comparer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>(comparer);
            result.Add(NAMENAME, GetName(obj));
            return result;
        }

        protected override object CreateInstance(Dictionary<string, object> values)
        {
            return new DriveInfo(values[NAMENAME].ToString());
        }

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
#endif