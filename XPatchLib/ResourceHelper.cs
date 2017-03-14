using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using XPatchLib.Properties;

namespace XPatchLib
{
    internal static class ResourceHelper
    {
        private static ResourceManager resourceMan;
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    ResourceManager temp = new ResourceManager("XPatchLib.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        internal static string GetResourceString(string key, params object[] values)
        {
            string resourceString = GetResourceString(key);
            return string.Format(CultureInfo.CurrentCulture, resourceString, values);
        }

        internal static string GetResourceString(string key)
        {
            return GetResourceString(key,CultureInfo.CurrentCulture);
        }

        internal static string GetResourceString(string key, CultureInfo culture)
        {
            return ResourceManager.GetString(key, culture);
        }

    }
}
