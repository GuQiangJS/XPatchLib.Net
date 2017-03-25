// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     指定如何处理字符串之间进行转换时的时间值和 <see cref="DateTime" />。
    /// </summary>
    /// <seealso cref="System.Xml.XmlDateTimeSerializationMode" />
    public enum DateTimeSerializationMode
    {
        /// <summary>
        ///     作为本地时间进行处理。 如果 <see cref="DateTime" /> 对象都表示协调世界时 (UTC)，它将转换为本地时间。
        /// </summary>
        Local,

        /// <summary>
        ///     将视为 UTC。 如果 <see cref="DateTime" /> 对象都表示本地时间，它将转换为 UTC。
        /// </summary>
        Utc,

        /// <summary>
        ///     如果将视为本地时间 <see cref="DateTime" /> 要转换为一个字符串。
        /// </summary>
        Unspecified,

        /// <summary>
        ///     在转换时，应保留时区信息。
        /// </summary>
        RoundtripKind
    }

    internal static class DateTimeSerializationModeHelper
    {
        internal static XmlDateTimeSerializationMode Convert(this DateTimeSerializationMode pMode)
        {
            switch (pMode)
            {
                case DateTimeSerializationMode.Local:
                    return XmlDateTimeSerializationMode.Local;
                case DateTimeSerializationMode.RoundtripKind:
                    return XmlDateTimeSerializationMode.RoundtripKind;
                case DateTimeSerializationMode.Unspecified:
                    return XmlDateTimeSerializationMode.Unspecified;
                case DateTimeSerializationMode.Utc:
                    return XmlDateTimeSerializationMode.Utc;
            }
            throw new NotImplementedException();
        }
    }
}