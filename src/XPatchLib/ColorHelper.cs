// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using System.Globalization;
using XPatchLib.Properties;

namespace XPatchLib
{
    /// <summary>
    ///     ARGB 颜色 操作帮助类。
    /// </summary>
    internal static class ColorHelper
    {
        #region Internal Methods

        /// <summary>
        ///     将指定字符串转换为 <see cref="System.Drawing.Color" /> 的实例。
        /// </summary>
        /// <param name="pColorString">
        ///     指定字符串。
        /// </param>
        /// <returns>
        ///     正常被转换后的 <see cref="System.Drawing.Color" /> 的实例。
        /// </returns>
        /// <exception cref="System.FormatException">
        ///     <paramref name="pColorString" /> 不能被转换成 Color 对象时。
        /// </exception>
        internal static Color TransFromString(String pColorString)
        {
            Color result;
            if (!TryTransFromString(pColorString, out result))
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, Resources.Exp_String_IsNotColor,
                    pColorString));
            return result;
        }

        /// <summary>
        ///     将一个 ARGB 颜色 的实例转换为字符串。
        /// </summary>
        /// <param name="pColorObject">
        ///     ARGB 颜色 的实例。
        /// </param>
        /// <returns>
        ///     <para> 当 <paramref name="pColorObject" /> </para>
        ///     <para> is null 时 ， 返回 <see cref="string.Empty" /> 。 </para>
        ///     <para>
        ///         <see cref="Color.IsNamedColor" />== <b> true </b> 时 ， 返回 <see cref="Color.Name" /> 。
        ///     </para>
        ///     <para> 否则 </para>
        ///     <para> 返回 <paramref name="pColorObject" /> 的32位ARGB值，并转换为16进制。 </para>
        /// </returns>
        /// <example>
        ///     <code>
        ///  using System;
        /// 
        ///  class ColorHelperTestClass
        ///  {
        ///      public static void Main()
        ///      {
        ///          Console.WriteLine(ColorHelper.TransToString(Color.Aqua));
        ///          Console.WriteLine(ColorHelper.TransToString(Color.FromArgb(255, 255, 255)));
        ///      }
        ///  }
        ///  /*
        ///  This code example produces the following results:
        /// 
        ///  Aqua
        ///  #FFFFFFFF
        ///  */
        ///  </code>
        /// </example>
        internal static String TransToString(Object pColorObject)
        {
            if (pColorObject == null)
                return string.Empty;
            Color c = (Color) pColorObject;
            if (c.IsNamedColor)
                return c.Name;
            return String.Format(CultureInfo.InvariantCulture, ConstValue.COLOR_FORMAT, c.ToArgb());
        }

        /// <summary>
        ///     尝试将指定字符串转换为 <see cref="System.Drawing.Color" /> 的实例。
        /// </summary>
        /// <param name="pColorString">
        ///     指定字符串。
        /// </param>
        /// <param name="pColor">
        ///     转换后的 <see cref="System.Drawing.Color" /> 的实例。
        /// </param>
        /// <returns>
        ///     <para> 当转换成功时，返回 true，否则返回 false。 </para>
        /// </returns>
        /// <example>
        ///     <code>
        ///  using System;
        /// 
        ///  class ColorHelperTestClass
        ///  {
        ///      public static void Main()
        ///      {
        ///          DoTrans("AliceBlue");
        ///          Console.WriteLine("");
        ///          DoTrans("#FFFFFFFF");
        ///      }
        /// 
        ///      private static void DoTrans(string pString)
        ///      {
        ///          Color result;
        ///          ColorHelper.TryTransFromString(pString, out result);
        /// 
        ///          Console.WriteLineWriteLine(result.Name);
        ///          Console.WriteLine(result.A);
        ///          Console.WriteLine(result.R);
        ///          Console.WriteLine(result.G);
        ///          Console.WriteLine(result.B);
        ///      }
        ///  }
        ///  /*
        ///  This code example produces the following results:
        /// 
        ///  AliceBlue
        ///  255
        ///  240
        ///  248
        ///  255
        /// 
        ///  ffffffff
        ///  255
        ///  255
        ///  255
        ///  255
        ///  */
        ///  </code>
        /// </example>
        internal static Boolean TryTransFromString(String pColorString, out Color pColor)
        {
            pColor = Color.Black;

#if NET40
            if (string.IsNullOrWhiteSpace(pColorString))
                return false;
#else
            if (String.IsNullOrEmpty(pColorString) || pColorString.Trim().Length == 0)
                return false;
#endif

            pColorString = pColorString.Trim();
            if (pColorString.StartsWith(ConstValue.COLOR_STARTCHAR, StringComparison.OrdinalIgnoreCase))
            {
                pColorString = pColorString.Substring(1);
                int n;
                if (Int32.TryParse(pColorString, NumberStyles.HexNumber, null, out n))
                {
                    pColor = Color.FromArgb(n);
                    return true;
                }
                return false;
            }
            pColor = Color.FromName(pColorString);
            return true;
        }

        #endregion Internal Methods
    }
}