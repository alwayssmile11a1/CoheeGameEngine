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

        private static void StableSortArray<T>(T[] list, T[] buffer, int index, int count, Comparison<T> comparison)
        {
            // This is a variant of merge sort that skips the split step and
            // instead merges back and forth between two arrays of equal size.
            // In this case, iteration is a better fit than recursion.
            int iterationCount = (int)Math.Ceiling(Math.Log(list.Length, 2));
            T[] source = list;
            T[] target = buffer;

            // In each iteration, process source segments in pairs of two
            // and merge them into one target segment each.
            for (int i = 0; i < iterationCount; i++)
            {
                // Determine how big a single segment will be, and how many target segments
                // we'll have to generate by merging from two source segments each.
                int segmentSize = 1 << (i + 1);
                int segmentCount = (int)Math.Ceiling((float)count / (float)segmentSize);
                for (int s = 0; s < segmentCount; s++)
                {
                    // First determine the target segment we'll work on
                    int segmentOffset = s * segmentSize;
                    int baseIndex = index + segmentOffset;
                    int baseCount = segmentSize;

                    if (s == segmentCount - 1)
                        baseCount = count - segmentOffset;

                    // Determine the two source segments we'll construct the
                    // target segment from. Note that this needs to match
                    // the previous iterations's target segments.
                    int leftCount = segmentSize / 2;
                    int rightCount = baseCount - leftCount;

                    // If we're only spanning a single previous source segment,
                    // skip merge and copy source to target to keep results.
                    if (leftCount <= 0 || rightCount <= 0)
                    {
                        Array.Copy(source, baseIndex, target, baseIndex, baseCount);
                        continue;
                    }

                    // Merge two segments from source into one segment of target
                    int leftIndex = 0;
                    int rightIndex = 0;
                    for (int k = 0; k < baseCount; k++)
                    {
                        // If we reach the end of one of the segments, copy the rest of the other
                        // as a single block without any further checks. This is an optimization.
                        if (rightIndex == rightCount)
                        {
                            Array.Copy(
                                source, baseIndex + leftIndex,
                                target, baseIndex + k, baseCount - k);
                            break;
                        }
                        else if (leftIndex == leftCount)
                        {
                            Array.Copy(
                                source, baseIndex + leftCount + rightIndex,
                                target, baseIndex + k, baseCount - k);
                            break;
                        }

                        // Copy the smaller element of the two source segments to target
                        if (comparison(source[baseIndex + leftIndex], source[baseIndex + leftCount + rightIndex]) <= 0)
                        {
                            target[baseIndex + k] = source[baseIndex + leftIndex];
                            leftIndex++;
                        }
                        else
                        {
                            target[baseIndex + k] = source[baseIndex + leftCount + rightIndex];
                            rightIndex++;
                        }
                    }
                }

                // Swap source and target
                MathF.Swap(ref source, ref target);
            }

            // If the last result ended up in the buffer, copy the results back to the original array
            if (source != list)
            {
                Array.Copy(source, index, list, index, count);
            }
        }

        private static void StableSortGeneric<T>(IList<T> list, IList<T> buffer, int index, int count, Comparison<T> comparison)
        {
            // This is a variant of merge sort that skips the split step and
            // instead merges back and forth between two arrays of equal size.
            // In this case, iteration is a better fit than recursion.
            int iterationCount = (int)Math.Ceiling(Math.Log(list.Count, 2));
            IList<T> source = list;
            IList<T> target = buffer;

            // In each iteration, process source segments in pairs of two
            // and merge them into one target segment each.
            for (int iteration = 0; iteration < iterationCount; iteration++)
            {
                // Determine how big a single segment will be, and how many target segments
                // we'll have to generate by merging from two source segments each.
                int segmentSize = 1 << (iteration + 1);
                int segmentCount = (int)Math.Ceiling((float)count / (float)segmentSize);
                for (int s = 0; s < segmentCount; s++)
                {
                    // First determine the target segment we'll work on
                    int segmentOffset = s * segmentSize;
                    int baseIndex = index + segmentOffset;
                    int baseCount = segmentSize;

                    if (s == segmentCount - 1)
                        baseCount = count - segmentOffset;

                    // Determine the two source segments we'll construct the
                    // target segment from. Note that this needs to match
                    // the previous iterations's target segments.
                    int leftCount = segmentSize / 2;
                    int rightCount = baseCount - leftCount;

                    // If we're only spanning a single previous source segment,
                    // skip merge and copy source to target to keep results.
                    if (leftCount <= 0 || rightCount <= 0)
                    {
                        ListCopy(source, baseIndex, target, baseIndex, baseCount);
                        continue;
                    }

                    // Merge two segments from source into one segment of target
                    int leftIndex = 0;
                    int rightIndex = 0;
                    for (int k = 0; k < baseCount; k++)
                    {
                        // If we reach the end of one of the segments, copy the rest of the other
                        // as a single block without any further checks. This is an optimization.
                        if (rightIndex == rightCount)
                        {
                            ListCopy(
                                source, baseIndex + leftIndex,
                                target, baseIndex + k, baseCount - k);
                            break;
                        }
                        else if (leftIndex == leftCount)
                        {
                            ListCopy(
                                source, baseIndex + leftCount + rightIndex,
                                target, baseIndex + k, baseCount - k);
                            break;
                        }

                        // Copy the smaller element of the two source segments to target
                        if (comparison(source[baseIndex + leftIndex], source[baseIndex + leftCount + rightIndex]) <= 0)
                        {
                            target[baseIndex + k] = source[baseIndex + leftIndex];
                            leftIndex++;
                        }
                        else
                        {
                            target[baseIndex + k] = source[baseIndex + leftCount + rightIndex];
                            rightIndex++;
                        }
                    }
                }

                // Swap source and target
                MathF.Swap(ref source, ref target);
            }

            // If the last result ended up in the buffer, copy the results back to the original array
            if (source != list)
            {
                ListCopy(source, index, list, index, count);
            }
        }

        private static void ListCopy<T>(IList<T> source, int sourceIndex, IList<T> target, int targetIndex, int count)
        {
            for (int i = 0; i < count; i++)
            {
                target[targetIndex + i] = source[sourceIndex + i];
            }
        }
    }
}
