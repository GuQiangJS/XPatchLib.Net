using System;
using System.Globalization;
using System.Reflection;

namespace XPatchLib
{
    /// <summary>
    /// <see cref="System.Type" /> 类型扩展。 
    /// </summary>
    internal static class TypeHelper
    {
        #region Internal Methods

        ///// <summary>
        ///// 检测类型上的PrimaryKeyAttribute特性是否符合要求。
        ///// </summary>
        ///// <param name="pType">
        ///// 待检测的类型。
        ///// </param>
        ///// <param name="pCheckAttributeExists">
        ///// 是否强制要求类型必须设定PrimaryKeyAttribute特性。
        ///// </param>
        //internal static Boolean CheckPrimaryKeyAttribute(this Type pType, bool pCheckAttributeExists)
        //{
        //    string errorPrimaryKeyName = string.Empty;
        //    return CheckPrimaryKeyAttribute(pType, pCheckAttributeExists, out errorPrimaryKeyName);
        //}


        ///// <summary>
        ///// 创建类型实例。 
        ///// </summary>
        ///// <param name="pType">
        ///// </param>
        ///// <returns>
        ///// </returns>
        //internal static Object CreateInstance(this Type pType)
        //{
        //    if (ReflectionUtils.IsBasicType(pType))
        //    {
        //        if (pType.IsValueType)
        //        {
        //            return pType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[0], CultureInfo.InvariantCulture);
        //        }
        //        else if (pType == typeof(string))
        //        {
        //            return string.Empty;
        //        }
        //    }
        //    else
        //    {
        //        if (ReflectionUtils.IsArray(pType))
        //        {
        //            Type elementType;
        //            if (ReflectionUtils.TryGetArrayElementType(pType, out elementType))
        //            {
        //                return Array.CreateInstance(elementType, 0);
        //            }
        //            throw new NotImplementedException();
        //        }
        //        else
        //        {
        //            try
        //            {
        //                return pType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[0], CultureInfo.InvariantCulture);
        //            }
        //            catch (MissingMethodException)
        //            {
        //                return Activator.CreateInstance(pType, true);
        //            }
        //        }
        //    }
        //    return null;
        //}

        #endregion Internal Methods
    }
}