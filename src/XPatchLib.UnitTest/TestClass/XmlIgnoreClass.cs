// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Xml.Serialization;

namespace XPatchLib.UnitTest.TestClass
{
    public class XmlIgnoreClass
    {
#if (NET || NETSTANDARD_2_0_UP)
        [XmlIgnore]
#endif
        public string A { get; set; }

        public string B { get; set; }

        public override bool Equals(object obj)
        {
            XmlIgnoreClass c = obj as XmlIgnoreClass;
            if (c == null) return false;
            return string.Equals(A, c.A) && string.Equals(B, c.B);
        }
    }
}