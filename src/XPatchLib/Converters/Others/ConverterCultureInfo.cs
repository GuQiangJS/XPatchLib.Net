using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace XPatchLib
{
    internal class ConverterCultureInfo:OtherConverterBase
    {
        private static MemberWrapper[] _memberWrappers;
        private const string NAMENAME = "Name";

        public ConverterCultureInfo(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        public ConverterCultureInfo(TypeExtend pType) : base(pType)
        {
        }
        
        static ConverterCultureInfo()
        {
            PropertyInfo name = typeof(CultureInfo).GetProperty(NAMENAME,
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            _memberWrappers = new MemberWrapper[] { new MemberWrapper(name), };
        }

        internal override MemberWrapper[] DivideMembers { get { return _memberWrappers; } }
        protected override object GetMemberValue(MemberWrapper member, object Obj)
        {
            return GetName(Obj);
        }
        string GetName(object obj)
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
        /// <returns></returns>
        protected override Dictionary<string, object> GetOriValues(object obj, IEqualityComparer<string> comparer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>(comparer);
            result.Add(NAMENAME, GetName(obj));
            return result;
        }

        protected override object CreateInstance(Dictionary<string, object> values)
        {
            return new CultureInfo(values[NAMENAME].ToString());
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
