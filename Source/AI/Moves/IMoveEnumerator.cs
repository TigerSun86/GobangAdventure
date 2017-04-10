using System.Collections.Generic;
using GobangGameLib.GameBoard;

namespace AI.Moves
{
    public interface IMoveEnumerator
    {
        IEnumerable<Position> GetMoves(IBoard board, PieceType player);
    }
}
