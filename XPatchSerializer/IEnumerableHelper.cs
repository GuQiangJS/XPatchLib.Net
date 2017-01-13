using System;
using System.Collections.Generic;

namespace XPatchLib
{
    internal static class IEnumerableHelper
    {
        public static int FindIndex<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            return items.GetEnumerator().FindIndex(predicate);
        }
    }
}