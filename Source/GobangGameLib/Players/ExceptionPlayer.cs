using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;

namespace GobangGameLib.Players
{
    public class ExceptionPlayer : IPlayer
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
            throw new Exception("This player intends to throw exception.");
        }
    }
}
