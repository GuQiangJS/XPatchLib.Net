using System;
using System.Collections.Generic;
using System.Text;

namespace XPatchLib
{
    internal class SpecChar
    {
        static SpecChar()
        {
            for (int i = 0; i < SpecChars.Length; i++)
                SpecCharsFlags[SpecChars[i]] = true;
        }

        internal static readonly char[] SpecChars = { '<', '>', '&', '\'', '"' };

        internal static readonly bool[] SpecCharsFlags = new bool[128];
    }
}
