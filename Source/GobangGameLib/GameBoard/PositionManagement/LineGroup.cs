using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.PositionManagement
{
    public class LineGroup : ILines, IPositions
    {
        private List<IPositions> _lines;

        public LineGroup(LineType type)
        {
            _lines = GetAllLinesOfGroup(type).ToList();
        }

        public IEnumerable<IPositions> Lines
        {
            get
            {
                return _lines;
            }
        }

        public IEnumerable<Position> Positions
        {
            get
            {
                return Lines.SelectMany(l => l.Positions);
            }
        }

        private IEnumerable<IPositions> GetAllLinesOfGroup(LineType type)
        {
            return GetIndexes(type).Select(i => new Line(type, i.Item1, i.Item2));
        }

        private IEnumerable<Tuple<int, int>> GetIndexes(LineType type)
        {
            if (type.Equals(LineType.Row))
            {
                return BoardProperties.RowIndexes.Select(i => new Tuple<int, int>(i, -1));
            }
            else if (type.Equals(LineType.Column))
            {
                return BoardProperties.RowIndexes.Select(i => new Tuple<int, int>(-1, i));
            }
            else if (type.Equals(LineType.DiagonalOne))
            {
                return BoardProperties.GetDiagonalOneIndexes();
            }
            else if (type.Equals(LineType.DiagonalTwo))
            {
                return BoardProperties.GetDiagonalTwoIndexes();
            }
            else
            {
                throw new ArgumentOutOfRangeException(string.Format("Unconsidered subclass of Line: '{0}'.", type.ToString()));
            }
        }
    }
}
