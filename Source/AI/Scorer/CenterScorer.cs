using System;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace AI.Scorer
{
    public class CenterScorer : IScorer
    {
        private readonly BoardProperties context;
        private readonly PositionManager positions;

        public CenterScorer(BoardProperties context, PositionManager positions)
        {
            this.context = context;
            this.positions = positions;
        }
        public double GetScore(IBoard board, PieceType player)
        {
            double sum = 0;
            foreach (Position p in this.positions.GetPlayerPositions(board))
            {
                double rowScore = -Math.Abs(p.Row - (this.context.RowSize / 2));
                double colScore = -Math.Abs(p.Col - (this.context.ColSize / 2));
                if (player == board.Get(p))
                {
                    sum += rowScore + colScore;
                }
                else
                {
                    sum -= rowScore + colScore;
                }
            }

            return sum;
        }
    }
}
