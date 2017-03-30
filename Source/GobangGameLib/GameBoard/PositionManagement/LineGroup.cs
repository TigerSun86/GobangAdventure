using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.PositionManagement
{
    public class LineGroup : ILines
    {
        private IList<IPositions> _lines;

        public LineGroup(LineType type, IList<IPositions> lines)
        {
            this._lines = lines;
        }

        public IList<IPositions> Lines
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
    }
}
