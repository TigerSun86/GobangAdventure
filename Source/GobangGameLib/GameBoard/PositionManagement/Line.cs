using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.PositionManagement
{
    public class Line : IPositions
    {
        private readonly LineType _type;
        private readonly IList<Position> _positions;
        
        public Line(LineType type, IList<Position> positions)
        {
            this._type = type;
            this._positions = positions;
        }

        public IList<Position> Positions
        {
            get
            {
                return _positions;
            }
        }
    }
}
