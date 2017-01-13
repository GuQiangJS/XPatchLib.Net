using System;
using System.Collections.Generic;

namespace XPatchLib
{
    internal static class IEnumeratorHelper
    {
        public static int FindIndex<T>(this IEnumerator<T> items, Predicate<T> predicate)
        {
            int index = -1;
            items.Reset();
            while (items.MoveNext())
            {
                index++;
                if (predicate(items.Current))
                {
                    break;
                }
            }
            return index;
        }
    }
}