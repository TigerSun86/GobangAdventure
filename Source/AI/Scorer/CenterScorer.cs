using System;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace AI.Scorer
{
    public class CenterScorer : IScorer
    {
        private readonly BoardProperties _context;
        private readonly PositionManager _positions;

        public CenterScorer(BoardProperties context, PositionManager positions)
        {
            _context = context;
            _positions = positions;
        }
        public double GetScore(IBoard board, PieceType player)
        {
            double sum = 0;

            foreach (Position p in _positions.GetPlayerPositions(board, player))
            {
                double rowScore = -Math.Abs(p.Row - (_context.RowSize / 2));
                double colScore = -Math.Abs(p.Col - (_context.ColSize / 2));
                sum += rowScore + colScore;
            }

            return sum;
        }
    }
}
