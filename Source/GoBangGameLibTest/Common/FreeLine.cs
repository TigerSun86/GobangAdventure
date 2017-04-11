using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace GoBangGameLibTest.Common
{
    public class FreeLine: IPositions
    {
        private readonly List<Position> _positions;

        public FreeLine(IEnumerable<Position> positions)
        {
            _positions = positions.ToList();
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
