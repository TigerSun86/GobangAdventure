using System;
using System.Collections.Generic;
using System.Linq;

namespace GobangGameLib.GameBoard.PositionManagement
{
    public class PositionFactory
    {
        public PositionManager Create(BoardProperties context)
        {
            var types = Enum.GetValues(typeof(LineType)).Cast<LineType>();
            var lineGroups = types.ToDictionary(t => t, t => (ILines)CreateLineGroup(context, t));
            var positionManager = new PositionManager(context, lineGroups);
            return positionManager;
        }

        private LineGroup CreateLineGroup(BoardProperties context, LineType type)
        {
            var lines = GetIndexes(context, type)
                .Select(i => (IPositions)CreateLine(context, type, i.Item1, i.Item2)).ToList();
            var lineGroup = new LineGroup(type, lines);
            return lineGroup;
        }

        private Line CreateLine(BoardProperties context, LineType type, int row, int col)
        {
            IEnumerable<Position> positions;
            if (type.Equals(LineType.Row))
            {
                positions = GetRow(context, row);
            }
            else if (type.Equals(LineType.Column))
            {
                positions = GetColumn(context, col);
            }
            else if (type.Equals(LineType.DiagonalOne))
            {
                positions = GetDiagonalOne(context, row, col);
            }
            else if (type.Equals(LineType.DiagonalTwo))
            {
                positions = GetDiagonalTwo(context, row, col);
            }
            else
            {
                throw new ArgumentException(type.ToString());
            }

            return new Line(type, positions.ToList());
        }

        private IEnumerable<Tuple<int, int>> GetIndexes(BoardProperties context, LineType type)
        {
            if (type.Equals(LineType.Row))
            {
                return context.RowIndexes.Select(i => new Tuple<int, int>(i, -1));
            }
            else if (type.Equals(LineType.Column))
            {
                return context.ColIndexes.Select(i => new Tuple<int, int>(-1, i));
            }
            else if (type.Equals(LineType.DiagonalOne))
            {
                return context.GetDiagonalOneIndexes();
            }
            else if (type.Equals(LineType.DiagonalTwo))
            {
                return context.GetDiagonalTwoIndexes();
            }
            else
            {
                throw new ArgumentOutOfRangeException(string.Format("Unconsidered subclass of Line: '{0}'.", type.ToString()));
            }
        }

        private IEnumerable<Position> GetRow(BoardProperties context, int row)
        {
            return context.ColIndexes.Select(i => new Position(row, i));
        }

        private IEnumerable<Position> GetColumn(BoardProperties context, int col)
        {
            return context.RowIndexes.Select(i => new Position(i, col));
        }

        private IEnumerable<Position> GetDiagonalOne(BoardProperties context, int row, int col)
        {
            do
            {
                yield return new Position(row, col);
                row++;
                col++;
            } while (context.IsWithinBoard(row, col));
        }

        private IEnumerable<Position> GetDiagonalTwo(BoardProperties context, int row, int col)
        {
            do
            {
                yield return new Position(row, col);
                row--;
                col++;
            } while (context.IsWithinBoard(row, col));
        }
    }
}
