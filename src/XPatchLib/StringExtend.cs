// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if !(NET && NETSTANDARD_2_0_UP)

using System;
using System.Globalization;

namespace XPatchLib
{
    internal static class StringExtend
    {
        internal static String ToUpper(this string value, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException("culture");
            return culture.TextInfo.ToUpper(value);
        }
    }
}

#endif