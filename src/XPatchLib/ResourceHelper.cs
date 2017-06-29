// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Reflection;
using System.Resources;

namespace XPatchLib
{
    internal static class ResourceHelper
    {
        private static ResourceManager resourceMan;

        internal static ResourceManager ResourceManager
        {
            get
            {
                if (ReferenceEquals(resourceMan, null))
                {
                    ResourceManager temp = new ResourceManager("XPatchLib.Properties.LocalizationRes",
                        typeof(LocalizationRes).Assembly());
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "XPatchLib.ResourceHelper.GetResourceString(System.String)")]
        internal static string GetResourceString(string key, params object[] values)
        {
            string resourceString = GetResourceString(key);
            return string.Format(CultureInfo.CurrentUICulture, resourceString, values);
        }

        internal static string GetResourceString(string key)
        {
            return GetResourceString(key, CultureInfo.CurrentUICulture);
        }

        internal static string GetResourceString(string key, CultureInfo culture)
        {
            return ResourceManager.GetString(key, culture);
        }
    }
}