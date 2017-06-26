// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.


#if (NETSTANDARD_1_0 || NETSTANDARD_1_1)

using System;
using System.Collections.Generic;
using System.Text;

namespace XPatchLib
{

    internal enum XmlDateTimeSerializationMode
    {
        Local = 0,
        RoundtripKind = 3,
        Unspecified = 2,
        Utc = 1
    }
}
#endif