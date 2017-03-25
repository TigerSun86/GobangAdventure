using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard
{
    public static class BoardProperties
    {
        public const int RowSize = 11;
        public const int ColSize = 11;
        public const int NumOfPiecesToWin = 5;

        public readonly static IEnumerable<int> RowIndexes = Enumerable.Range(0, RowSize);
        public readonly static IEnumerable<int> ColIndexes = Enumerable.Range(0, ColSize);

        /// <summary>
        /// The order of the yielded indexes is: bottom-left, top-left, top-right.
        /// 
        /// 0 1 2 3
        /// 1 \ \ \
        /// 2\ \ \ 
        /// 3 \ \ \ 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Tuple<int, int>> GetDiagonalOneIndexes()
        {
            foreach (int row in RowIndexes.Reverse())
            {
                yield return new Tuple<int, int>(row, 0);
            }

            // Skip the col == 0, because it has been used when yield row indexes.
            foreach (int col in ColIndexes.Skip(1))
            {
                yield return new Tuple<int, int>(0, col);
            }
        }

        /// <summary>
        /// The order of the yielded indexes is: top-left, bottom-left, bottom-right.
        /// 
        /// 0 / / /
        /// 1/ / /
        /// 2 / / /
        /// 3 1 2 3
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Tuple<int, int>> GetDiagonalTwoIndexes()
        {
            foreach (int row in RowIndexes)
            {
                yield return new Tuple<int, int>(row, 0);
            }

            // Skip the col == 0, because it has been used when yield row indexes.
            foreach (int col in ColIndexes.Skip(1))
            {
                yield return new Tuple<int, int>(BoardProperties.RowSize - 1, col);
            }
        }

        public static bool IsWithinBoard(Position position)
        {
            return IsWithinBoard(position.Row, position.Col);
        }

        public static bool IsWithinBoard(int row, int col)
        {
            return IsWithinRow(row) && IsWithinCol(col);
        }


        public static bool IsWithinRow(int row)
        {
            return row >= 0 && row < RowSize;
        }

        public static bool IsWithinCol(int col)
        {
            return col >= 0 && col < ColSize;
        }
    }
}
