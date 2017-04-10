using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace AI.Moves
{
    public class EmptyPositionMoveEnumerator : IMoveEnumerator
    {
        private readonly PositionManager positions;

        public EmptyPositionMoveEnumerator(PositionManager positions)
        {
            this.positions = positions;
        }

        public IEnumerable<Position> GetMoves(IBoard board, PieceType player)
        {
            return positions.GetEmptyPositions(board);
        }
    }
}
