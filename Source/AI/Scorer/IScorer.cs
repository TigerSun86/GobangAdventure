using GobangGameLib.GameBoard;
using GobangGameLib.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Scorer
{
    public interface IScorer
    {
        double GetScore(IBoard board, PieceType player);
    }
}
