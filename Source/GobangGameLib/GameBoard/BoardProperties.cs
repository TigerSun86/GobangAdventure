using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard
{
    public class BoardProperties
    {
        private int _rowSize;
        private int _colSize;
        private int _numOfPiecesToWin;

        public BoardProperties(int rowSize = 11, int colSize = 11, int numOfPiecesToWin = 5)
        {
            _rowSize = rowSize;
            _colSize = colSize;
            _numOfPiecesToWin = numOfPiecesToWin;
        }

        public int RowSize
        {
            get
            {
                return _rowSize;
            }
        }
        public int ColSize
        {
            get
            {
                return _colSize;
            }
        }

        public int NumOfPiecesToWin
        {
            get
            {
                return _numOfPiecesToWin;
            }
        }

        public IEnumerable<int> RowIndexes
        {
            get
            {
                return Enumerable.Range(0, RowSize);
            }
        }

        public IEnumerable<int> ColIndexes
        {
            get
            {
                return Enumerable.Range(0, ColSize);
            }
        }

        /// <summary>
        /// The order of the yielded indexes is: bottom-left, top-left, top-right.
        /// 
        /// 0 1 2 3
        /// 1 \ \ \
        /// 2\ \ \ 
        /// 3 \ \ \ 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<int, int>> GetDiagonalOneIndexes()
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
        public IEnumerable<Tuple<int, int>> GetDiagonalTwoIndexes()
        {
            foreach (int row in RowIndexes)
            {
                yield return new Tuple<int, int>(row, 0);
            }

            // Skip the col == 0, because it has been used when yield row indexes.
            foreach (int col in ColIndexes.Skip(1))
            {
                yield return new Tuple<int, int>(RowSize - 1, col);
            }
        }

        public bool IsWithinBoard(Position position)
        {
            return IsWithinBoard(position.Row, position.Col);
        }

        public bool IsWithinBoard(int row, int col)
        {
            return IsWithinRow(row) && IsWithinCol(col);
        }


        public bool IsWithinRow(int row)
        {
            return row >= 0 && row < RowSize;
        }

        public bool IsWithinCol(int col)
        {
            return col >= 0 && col < ColSize;
        }
    }
}
