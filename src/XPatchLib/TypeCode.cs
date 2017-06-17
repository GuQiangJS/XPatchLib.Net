// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.


#if (NETSTANDARD_1_0 || NETSTANDARD_1_1 || NETSTANDARD_1_2)

using System;
using System.Collections.Generic;
using System.Text;

namespace XPatchLib
{

    [Flags]
    internal enum TypeCode
    {
        Boolean = 3,
        Byte = 6,
        Char = 4,
        DateTime = 0x10,
        DBNull = 2,
        Decimal = 15,
        Double = 14,
        Empty = 0,
        Int16 = 7,
        Int32 = 9,
        Int64 = 11,
        Object = 1,
        SByte = 5,
        Single = 13,
        String = 0x12,
        UInt16 = 8,
        UInt32 = 10,
        UInt64 = 12

    }
}
#endif