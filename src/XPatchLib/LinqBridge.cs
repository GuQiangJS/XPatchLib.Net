// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if !HAVE_LINQ
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using XPatchLib.NoLinq;

namespace XPatchLib
{
    internal static class LinqBridge
    {
        internal static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            return source.FirstImpl(Futures<TSource>.Default);
        }

        internal static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return FirstOrDefault(source.Where(predicate));
        }

        internal static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(predicate, "predicate");

            return WhereYield(source, predicate);
        }

        internal static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            return first.Except(second, null);
        }


        internal static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return IntersectExceptImpl(first, second, comparer, false);
        }

        internal static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            return first.Intersect(second, null);
        }


        internal static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return IntersectExceptImpl(first, second, comparer, true);
        }

        internal static TSource Last<TSource>(this IEnumerable<TSource> source)
        {
            return source.LastImpl(Futures<TSource>.Undefined);
        }

        internal static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            Guard.ArgumentNotNull(source, "source");

            return new List<TSource>(source);
        }

        private static TSource LastImpl<TSource>(this IEnumerable<TSource> source, Func<TSource> empty)
        {
            Guard.ArgumentNotNull(source, "source");

            var list = source as IList<TSource>;
            if (list != null)
                return list.Count > 0 ? list[list.Count - 1] : empty();

            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    return empty();

                var last = e.Current;
                while (e.MoveNext())
                    last = e.Current;

                return last;
            }
        }


        internal static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return Last(source.Where(predicate));
        }

        internal static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.OrderBy(keySelector, /* comparer */ null);
        }

        /// <summary>
        ///     Sorts the elements of a sequence in ascending order by using a
        ///     specified comparer.
        /// </summary>
        internal static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(keySelector, "keySelector");

            return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, /* descending */ false);
        }

        internal static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(selector, "selector");

            return SelectYield(source, selector);
        }

        private static IEnumerable<TResult> SelectYield<TSource, TResult>(IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
        {
            foreach (var item in source)
                yield return selector(item);
        }

        /// <summary>
        ///     Projects each element of a sequence into a new form by
        ///     incorporating the element's index.
        /// </summary>
        internal static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector)
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(selector, "selector");

            return SelectYield(source, selector);
        }

        private static IEnumerable<TResult> SelectYield<TSource, TResult>(
            IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector)
        {
            var i = 0;
            foreach (var item in source)
                yield return selector(item, i++);
        }

        internal static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            IList<TSource> ilist = source as IList<TSource>;
            if (ilist != null)
            {
                TSource[] array = new TSource[ilist.Count];
                ilist.CopyTo(array, 0);
                return array;
            }

            return source.ToList().ToArray();
        }

        internal static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            return Union(first, second, /* comparer */ null);
        }

        /// <summary>
        ///     Produces the set union of two sequences by using a specified
        ///     <see cref="IEqualityComparer{T}" />.
        /// </summary>
        internal static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return first.Concat(second).Distinct(comparer);
        }

        internal static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            Guard.ArgumentNotNull(first, "first");
            Guard.ArgumentNotNull(second, "second");

            return ConcatYield(first, second);
        }

        internal static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
        {
            return Distinct(source, /* comparer */ null);
        }

        /// <summary>
        ///     Returns distinct elements from a sequence by using a specified
        ///     <see cref="IEqualityComparer{T}" /> to compare values.
        /// </summary>
        internal static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source,
            IEqualityComparer<TSource> comparer)
        {
            Guard.ArgumentNotNull(source, "source");

            return DistinctYield(source, comparer);
        }

        private static IEnumerable<TSource> DistinctYield<TSource>(IEnumerable<TSource> source,
            IEqualityComparer<TSource> comparer)
        {
            var set = new Dictionary<TSource, object>(comparer);
            var gotNull = false;

            foreach (var item in source)
            {
                if (item == null)
                {
                    if (gotNull)
                        continue;
                    gotNull = true;
                }
                else
                {
                    if (set.ContainsKey(item))
                        continue;
                    set.Add(item, null);
                }

                yield return item;
            }
        }

        private static IEnumerable<TSource> ConcatYield<TSource>(
            IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            foreach (var item in first)
                yield return item;

            foreach (var item in second)
                yield return item;
        }

        private static IEnumerable<TSource> WhereYield<TSource>(IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            foreach (var item in source)
                if (predicate(item))
                    yield return item;
        }

        private static IEnumerable<TSource> IntersectExceptImpl<TSource>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second, IEqualityComparer<TSource> comparer, bool flag)
        {
            Guard.ArgumentNotNull(first, "first");
            Guard.ArgumentNotNull(second, "second");

            var keys = new List<TSource>();
            var flags = new Dictionary<TSource, bool>(comparer);

            foreach (var item in first.Where(k => !flags.ContainsKey(k)))
            {
                flags.Add(item, !flag);
                keys.Add(item);
            }

            foreach (var item in second.Where(flags.ContainsKey))
                flags[item] = flag;

            return keys.Where(item => flags[item]);
        }

        private static TSource FirstImpl<TSource>(this IEnumerable<TSource> source, Func<TSource> empty)
        {
            Guard.ArgumentNotNull(source, "source");
            Debug.Assert(empty != null);

            var list = source as IList<TSource>;
            if (list != null)
                return list.Count > 0 ? list[0] : empty();

            using (var e = source.GetEnumerator())
            {
                return e.MoveNext() ? e.Current : empty();
            }
        }

        private static class Futures<T>
        {
            public static readonly Func<T> Default = () => default(T);
            public static readonly Func<T> Undefined = () => { throw new InvalidOperationException(); };
        }
    }

    internal interface IOrderedEnumerable<TElement> : IEnumerable<TElement>
    {
        /// <summary>
        ///     Performs a subsequent ordering on the elements of an
        ///     <see cref="IOrderedEnumerable{T}" /> according to a key.
        /// </summary>
        IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(
            Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
    }

    internal sealed class OrderedEnumerable<T, K> : IOrderedEnumerable<T>
    {
        private readonly List<Comparison<T>> _comparisons;
        private readonly IEnumerable<T> _source;

        public OrderedEnumerable(IEnumerable<T> source,
            Func<T, K> keySelector, IComparer<K> comparer, bool descending) :
            this(source, null, keySelector, comparer, descending) { }

        private OrderedEnumerable(IEnumerable<T> source, List<Comparison<T>> comparisons,
            Func<T, K> keySelector, IComparer<K> comparer, bool descending)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");

            _source = source;

            comparer = comparer ?? Comparer<K>.Default;

            if (comparisons == null)
                comparisons = new List<Comparison<T>>( /* capacity */ 4);

            comparisons.Add((x, y)
                => (descending ? -1 : 1) * comparer.Compare(keySelector(x), keySelector(y)));

            _comparisons = comparisons;
        }

        public IOrderedEnumerable<T> CreateOrderedEnumerable<KK>(
            Func<T, KK> keySelector, IComparer<KK> comparer, bool descending)
        {
            return new OrderedEnumerable<T, KK>(_source, _comparisons, keySelector, comparer, descending);
        }

        public IEnumerator<T> GetEnumerator()
        {
            //
            // We sort using List<T>.Sort, but docs say that it performs an 
            // unstable sort. LINQ, on the other hand, says OrderBy performs 
            // a stable sort. So convert the source sequence into a sequence 
            // of tuples where the second element tags the position of the 
            // element from the source sequence (First). The position is 
            // then used as a tie breaker when all keys compare equal,
            // thus making the sort stable.
            //

            var list = _source.Select(TagPosition).ToList();

            list.Sort((x, y) =>
            {
                //
                // Compare keys from left to right.
                //

                var comparisons = _comparisons;
                for (var i = 0; i < comparisons.Count; i++)
                {
                    var result = comparisons[i](x.First, y.First);
                    if (result != 0)
                        return result;
                }

                //
                // All keys compared equal so now break the tie by their
                // position in the original sequence, making the sort stable.
                //

                return x.Second.CompareTo(y.Second);
            });

            return list.Select(GetFirst).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <remarks>
        ///     See <a href="http://code.google.com/p/linqbridge/issues/detail?id=11">issue #11</a>
        ///     for why this method is needed and cannot be expressed as a
        ///     lambda at the call site.
        /// </remarks>
        private static Tuple<T, int> TagPosition(T e, int i)
        {
            return new Tuple<T, int>(e, i);
        }

        /// <remarks>
        ///     See <a href="http://code.google.com/p/linqbridge/issues/detail?id=11">issue #11</a>
        ///     for why this method is needed and cannot be expressed as a
        ///     lambda at the call site.
        /// </remarks>
        private static T GetFirst(Tuple<T, int> pv)
        {
            return pv.First;
        }
    }

    [Serializable]
    internal struct Tuple<TFirst, TSecond> : IEquatable<Tuple<TFirst, TSecond>>
    {
        public TFirst First { get; private set; }
        public TSecond Second { get; private set; }

        public Tuple(TFirst first, TSecond second)
            : this()
        {
            First = first;
            Second = second;
        }

        public override bool Equals(object obj)
        {
            return obj != null
                   && obj is Tuple<TFirst, TSecond>
                   && base.Equals((Tuple<TFirst, TSecond>) obj);
        }

        public bool Equals(Tuple<TFirst, TSecond> other)
        {
            return EqualityComparer<TFirst>.Default.Equals(other.First, First)
                   && EqualityComparer<TSecond>.Default.Equals(other.Second, Second);
        }

        public override int GetHashCode()
        {
            var num = 0x7a2f0b42;
            num = -1521134295 * num + EqualityComparer<TFirst>.Default.GetHashCode(First);
            return -1521134295 * num + EqualityComparer<TSecond>.Default.GetHashCode(Second);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, @"{{ First = {0}, Second = {1} }}", First, Second);
        }
    }
}

namespace System.Runtime.CompilerServices
{
    /// <remarks>
    ///     This attribute allows us to define extension methods without
    ///     requiring .NET Framework 3.5. For more information, see the section,
    ///     <a href="http://msdn.microsoft.com/en-us/magazine/cc163317.aspx#S7">Extension Methods in .NET Framework 2.0 Apps</a>
    ///     ,
    ///     of <a href="http://msdn.microsoft.com/en-us/magazine/cc163317.aspx">Basic Instincts: Extension Methods</a>
    ///     column in <a href="http://msdn.microsoft.com/msdnmag/">MSDN Magazine</a>,
    ///     issue <a href="http://msdn.microsoft.com/en-us/magazine/cc135410.aspx">Nov 2007</a>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    public class ExtensionAttribute : Attribute { }
}

namespace XPatchLib.NoLinq
{
    public delegate TResult Func<TResult>();

    public delegate TResult Func<T, TResult>(T a);

    public delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);

    public delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);

    public delegate TResult Func<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    public delegate void Action();

    public delegate void Action<T1, T2>(T1 arg1, T2 arg2);

    public delegate void Action<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

    public delegate void Action<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}
#endif