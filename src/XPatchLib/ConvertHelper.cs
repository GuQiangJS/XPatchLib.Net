// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Text.RegularExpressions;

namespace XPatchLib
{
    internal static class ConvertHelper
    {
        internal static Guid ConvertGuidFromString(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Regex format = new Regex("^[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}$");
            Match match = format.Match(value);
            if (match.Success)
                return new Guid(value);

            return Guid.Empty;
        }
    }
}