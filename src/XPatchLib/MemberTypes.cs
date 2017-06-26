// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.


#if (NETSTANDARD_1_0 || NETSTANDARD_1_1 || NETSTANDARD_1_3)

using System;
using System.Collections.Generic;
using System.Text;

namespace XPatchLib
{
    [Flags]
    internal enum MemberTypes
    {
        All = 0xbf,
        Constructor = 1,
        Custom = 0x40,
        Event = 2,
        Field = 4,
        Method = 8,
        NestedType = 0x80,
        Property = 0x10,
        TypeInfo = 0x20
    }
}
#endif