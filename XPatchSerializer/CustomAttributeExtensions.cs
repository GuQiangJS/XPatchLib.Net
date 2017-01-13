using System;
using System.Collections.Generic;
using System.Reflection;

namespace XPatchLib
{
    internal static class CustomAttributeExtensions
    {
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element)
        {
            return Attribute.GetCustomAttributes(element);
        }

        public static T GetCustomAttribute<T>(this MemberInfo element) where T : Attribute
        {
            return (T)GetCustomAttribute(element, typeof(T));
        }

        public static Attribute GetCustomAttribute(this MemberInfo element, Type attributeType)
        {
            return Attribute.GetCustomAttribute(element, attributeType);
        }
    }
}
