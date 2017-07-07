using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OriBrainLearnerCore
{
    public static class ExtensionFunctions
    { 
        /// <summary>
        /// resize a double array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static T[,] ResizeArray<T>(T[,] original, int x, int y)
        {
            T[,] newArray = new T[x, y];
            int minX = Math.Min(original.GetLength(0), newArray.GetLength(0));
            int minY = Math.Min(original.GetLength(1), newArray.GetLength(1));

            for (int i = 0; i < minY; ++i)
                Array.Copy(original, i * original.GetLength(0), newArray, i * newArray.GetLength(0), minX);

            return newArray;
        }

        /// <summary>
        /// renive at and beginning for arrays.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        public static T[] RemoveAtBegining<T>(this T[] source, int amount)
        {
            T[] dest = new T[source.Length - amount];

            if (amount < source.Length - amount)
                Array.Copy(source, amount, dest, 0, dest.Length);

            return dest;
        }
    
        /// <summary>
        ///  zip extention function.
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="seqA"></param>
        /// <param name="seqB"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<T> Zip<A, B, T>(this IEnumerable<A> seqA, IEnumerable<B> seqB, Func<A, B, T> func)
        {
            if (seqA == null) throw new ArgumentNullException("seqA");
            if (seqB == null) throw new ArgumentNullException("seqB");

            return Zip35Deferred(seqA, seqB, func);
        }

        private static IEnumerable<T> Zip35Deferred<A, B, T>(this IEnumerable<A> seqA, IEnumerable<B> seqB, Func<A, B, T> func)
        {
            using (var iteratorA = seqA.GetEnumerator())
            using (var iteratorB = seqB.GetEnumerator())
            {
                while (iteratorA.MoveNext() && iteratorB.MoveNext())
                {
                    yield return func(iteratorA.Current, iteratorB.Current);
                }
            }
        }

        public static void test()
        {
            int[] integers1 = new int[] { 1, 2, 3, 4, 5 };
            int[] integers2 = new int[] { 10, 20, 30, 40, 50 };
            var sumsZip = integers1.Zip(integers2, (i, j) => i + j);
            // 11, 22, 33, 44, 55

            char[] characters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F' };
            var items = characters.Zip(integers1, (c, i) => string.Format("{0}{1}", c, i));
            // A1, B2, C3, D4, E5
        }
    }
}
