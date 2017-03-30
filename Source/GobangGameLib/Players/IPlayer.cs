using GobangGameLib.GameBoard;
using GobangGameLib.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.Players
{
    public interface IPlayer
    {
        Position MakeAMove(IBoard board);
    }
}
