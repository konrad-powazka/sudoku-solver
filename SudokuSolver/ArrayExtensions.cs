using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    public static class ArrayExtensions
    {
        public static IEnumerable<T> Flatten<T>(this T[,] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            return
                Enumerable.Range(0, array.GetLength(1))
                    .SelectMany(i => Enumerable.Range(0, array.GetLength(0)),
                        (i1, i0) => new {Index0 = i0, Index1 = i1})
                    .Select(i => array[i.Index0, i.Index1]);
        }

        public static IEnumerable<IEnumerable<T>> GetRows<T>(this T[,] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            return
                Enumerable.Range(0, array.GetLength(1))
                    .Select(i1 => Enumerable.Range(0, array.GetLength(0)).Select(i0 => array[i0, i1]));
        }

        public static IEnumerable<IEnumerable<T>> GetColumns<T>(this T[,] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            return
                Enumerable.Range(0, array.GetLength(0))
                    .Select(i0 => Enumerable.Range(0, array.GetLength(1)).Select(i1 => array[i0, i1]));
        }
    }
}