using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.Players
{
    public class NextAvailablePlayer : IPlayer
    {
        private PositionManager _positions;

        public NextAvailablePlayer(PositionManager positions)
        {
            _positions = positions;
        }

        public Position MakeAMove(IBoard board)
        {
            var emptyPositions = _positions.GetEmptyPositions(board);
            return emptyPositions.FirstOrDefault();
        }
    }
}
