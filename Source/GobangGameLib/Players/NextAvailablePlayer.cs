using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;
using GobangGameLib.Util;

namespace GobangGameLib.Players
{
    public class NextAvailablePlayer : IPlayer
    {
        private PieceType _player;

        public PieceType Player
        {
            set
            {
                _player = value;
            }
        }

        public Position MakeAMove(IBoard board)
        {
            var emptyPositions = BoardHelper.GetEmptyPositions(board);
            return emptyPositions.FirstOrDefault();
        }
    }
}
