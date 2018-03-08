// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if NET || NETSTANDARD_1_3_UP

using System.Collections.Generic;
using System.IO;

namespace XPatchLib
{
    internal class ConverterDirectoryInfo : ConverterFileSystemInfo
    {
        public ConverterDirectoryInfo(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        public ConverterDirectoryInfo(TypeExtend pType) : base(pType)
        {
        }


        protected override object CreateInstance(Dictionary<string, object> values)
        {
            return new DirectoryInfo(values[ORIGINALPATHNAME].ToString());
        }
    }
}
#endif