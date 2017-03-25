using GobangGameLib.GameBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameJudge
{
    interface IJudge
    {
        PieceType GetWinner(IBoard board);
    }
}
