using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.PositionManagement
{
    public class Line : IPositions
    {
        private readonly List<Position> _positions;

        public Line(LineType type, int row, int col)
        {
            if (type.Equals(LineType.Row))
            {
                _positions = GetRow(row).ToList();
            }
            else if (type.Equals(LineType.Column))
            {
                _positions = GetColumn(col).ToList();
            }
            else if (type.Equals(LineType.DiagonalOne))
            {
                _positions = GetDiagonalOne(row, col).ToList();
            }
            else if (type.Equals(LineType.DiagonalTwo))
            {
                _positions = GetDiagonalTwo(row, col).ToList();
            }
            else
            {
                throw new ArgumentException(type.ToString());
            }
        }

        public IEnumerable<Position> Positions
        {
            get
            {
                return _positions;
            }
        }

        private IEnumerable<Position> GetRow(int row)
        {
            return BoardProperties.ColIndexes.Select(i => new Position(row, i));
        }

        private IEnumerable<Position> GetColumn(int col)
        {
            return BoardProperties.RowIndexes.Select(i => new Position(i, col));
        }

        private IEnumerable<Position> GetDiagonalOne(int row, int col)
        {
            do
            {
                yield return new Position(row, col);
                row++;
                col++;
            } while (BoardProperties.IsWithinBoard(row, col));
        }

        private IEnumerable<Position> GetDiagonalTwo(int row, int col)
        {
            do
            {
                yield return new Position(row, col);
                row--;
                col++;
            } while (BoardProperties.IsWithinBoard(row, col));
        }
    }
}
