using System;
using System.Linq;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.Players
{
    public class RandomPlayer : IPlayer
    {
        private PositionManager _positions;

        public RandomPlayer(PositionManager positions)
        {
            _positions = positions;
        }

        public Position MakeAMove(IBoard board)
        {
            var emptyPositions = _positions.GetEmptyPositions(board);
            return emptyPositions.ElementAt(new Random().Next(0, emptyPositions.Count()));
        }
    }
}
