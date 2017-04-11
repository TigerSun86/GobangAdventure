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
        private readonly LineType _type;
        private readonly IList<IPositions> _lines;

        public LineGroup(LineType type, IList<IPositions> lines)
        {
            this._type = type;
            this._lines = lines;
        }

        public IList<IPositions> Lines
        {
            get
            {
                return _lines;
            }
        }
    }
}
