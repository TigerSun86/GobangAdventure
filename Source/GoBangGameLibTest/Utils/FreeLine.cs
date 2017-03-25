using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace GoBangGameLibTest.Utils
{
    public class FreeLine: IPositions
    {
        private readonly List<Position> _positions;

        public FreeLine(IEnumerable<Position> positions)
        {
            _positions = positions.ToList();
        }

        public IEnumerable<Position> Positions
        {
            get
            {
                return _positions;
            }
        }
    }
}
