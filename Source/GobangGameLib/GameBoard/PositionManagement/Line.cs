using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.PositionManagement
{
    public class Line : IPositions
    {
        private readonly LineType type;
        private readonly IList<Position> _positions;
        
        public Line(LineType type, IList<Position> positions)
        {
            this.type = type;
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
