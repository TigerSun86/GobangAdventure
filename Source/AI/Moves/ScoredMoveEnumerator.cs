using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.Scorer;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace AI.Moves
{
    public class ScoredMoveEnumerator : IMoveEnumerator
    {
        private readonly PositionManager positions;
        private readonly IScorer scorer;

        public ScoredMoveEnumerator(PositionManager positions, IScorer scorer)
        {
            this.positions = positions;
            this.scorer = scorer;
        }

        public IEnumerable<Position> GetMoves(IBoard board, PieceType player)
        {
            IBoard boardCopy = board.DeepClone();
            return GetAllMoveAndScore(boardCopy, player).OrderByDescending(t => t.Item2).Select(t => t.Item1);
        }

        private IEnumerable<Tuple<Position, double>> GetAllMoveAndScore(IBoard board, PieceType player)
        {
            foreach (var move in this.positions.GetEmptyPositions(board))
            {
                board.Set(move, player);
                double score = scorer.GetScore(board, player);
                board.Set(move, PieceType.Empty);
                yield return new Tuple<Position, double>(move, score);
            }
        }
    }
}
