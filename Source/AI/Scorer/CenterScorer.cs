using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.Game;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.Util;

namespace AI.Scorer
{
    public class CenterScorer : IScorer
    {
        public double GetScore(IBoard board, PieceType player)
        {
            double sum = 0;

            foreach (Position p in BoardHelper.GetPlayerPositions(board, player))
            {
                double rowScore = -Math.Abs(p.Row - (BoardProperties.RowSize / 2));
                double colScore = -Math.Abs(p.Col - (BoardProperties.ColSize / 2));
                sum += rowScore + colScore;
            }

            return sum;
        }
    }
}
