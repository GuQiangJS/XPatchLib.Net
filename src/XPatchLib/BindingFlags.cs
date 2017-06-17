// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.


#if (NETSTANDARD_1_0 || NETSTANDARD_1_1 || NETSTANDARD_1_2 || NETSTANDARD_1_3 || NETSTANDARD_1_4)

using System;
using System.Collections.Generic;
using System.Text;

namespace XPatchLib
{
    [Flags]
    internal enum BindingFlags
    {
        CreateInstance = 0x200,
        DeclaredOnly = 2,
        Default = 0,
        ExactBinding = 0x10000,
        FlattenHierarchy = 0x40,
        GetField = 0x400,
        GetProperty = 0x1000,
        IgnoreCase = 1,
        IgnoreReturn = 0x1000000,
        Instance = 4,
        InvokeMethod = 0x100,
        NonPublic = 0x20,
        OptionalParamBinding = 0x40000,
        Public = 0x10,
        PutDispProperty = 0x4000,
        PutRefDispProperty = 0x8000,
        SetField = 0x800,
        SetProperty = 0x2000,
        Static = 8,
        SuppressChangeType = 0x20000
    }
}
#endif