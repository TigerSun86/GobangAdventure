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
        public Position MakeAMove(IBoard board)
        {
            throw new Exception("This player intends to throw exception.");
        }
    }
}
