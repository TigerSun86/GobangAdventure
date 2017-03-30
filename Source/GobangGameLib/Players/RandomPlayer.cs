using GobangGameLib.GameBoard;
using GobangGameLib.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.Players
{
    public class RandomPlayer : IPlayer
    {
        private PieceType _player;
        private PositionManager _positions;

        public RandomPlayer(PositionManager positions)
        {
            _positions = positions;
        }

        public PieceType Player
        {
            set
            {
                _player = value;
            }
        }

        public Position MakeAMove(IBoard board)
        {
            var emptyPositions = _positions.GetEmptyPositions(board);
            return emptyPositions.ElementAt(new Random().Next(0, emptyPositions.Count()));
        }
    }
}
