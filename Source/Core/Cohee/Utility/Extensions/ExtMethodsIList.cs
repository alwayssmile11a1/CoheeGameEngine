using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cohee
{
    public static class ExtMethodsIList
    {
        /// <summary>
		/// Returns the combined hash code of the specified list.
        /// For more details, see Fowler–Noll–Vo hash function
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static int GetCombinedHashCode<T>(this IList<T> list, int firstIndex = 0, int length = -1)
        {
            if (length == -1) length = list.Count;
            int endIndex = length < 0 ? list.Count - firstIndex : firstIndex + length;
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = firstIndex; i < endIndex; i++)
                    hash = (hash ^ list[i].GetHashCode()) * p;

                return hash;
            }
        }

        /// <summary>
		/// Performs a stable sort.
		/// </summary>
		/// <typeparam name="T">The lists object type.</typeparam>
		/// <param name="list">List to perform the sort operation on.</param>
		/// <param name="comparison">The comparison to use.</param>
		public static void StableSort<T>(this IList<T> list, Comparison<T> comparison)
        {
            StableSort<T>(list, 0, list.Count, comparison);
        }

        /// <summary>
        /// Performs a stable sort.
        /// </summary>
        /// <typeparam name="T">The lists object type.</typeparam>
        /// <param name="list">List to perform the sort operation on.</param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="comparison">The comparison to use.</param>
        public static void StableSort<T>(this IList<T> list, int index, int count, Comparison<T> comparison)
        {
            T[] buffer = new T[list.Count];
            StableSort<T>(list, buffer, index, count, comparison);
        }

        public static void StableSort<T>(this IList<T> list, IList<T> buffer, int index, int count, Comparison<T> comparison)
        {
            if (list == null) throw new ArgumentNullException("array");
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (buffer.Count < list.Count) throw new ArgumentException("Zero-alloc stable sort requires a buffer of at least the sorted arrays size.", "buffer");
            if (index < 0) throw new ArgumentOutOfRangeException("index");
            if (index + count > list.Count) throw new ArgumentOutOfRangeException("count");

            // Fall back to default comparison when null
            comparison = comparison ?? Comparer<T>.Default.Compare;

            // Use an optimized array-based version when possible
            if (list is T[] && buffer is T[])
            {
                StableSortArray<T>(
                    list as T[],
                    buffer as T[],
                    index, count, comparison);
            }
            else if (list is RawList<T> && buffer is RawList<T>)
            {
                StableSortArray<T>(
                    (list as RawList<T>).Data,
                    (buffer as RawList<T>).Data,
                    index, count, comparison);
            }
            else
            {
                StableSortGeneric<T>(list, buffer, index, count, comparison);
            }
        }

    }
}
