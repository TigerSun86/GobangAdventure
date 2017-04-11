using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace AI.Scorer
{
    public class AggregatedScorer : IScorer
    {
        private readonly IList<Tuple<IScorer, double>> scorersAndWeights;
        public AggregatedScorer(IList<Tuple<IScorer, double>> scorersAndWeights)
        {
            this.scorersAndWeights = scorersAndWeights;
        }

        public double GetScore(IBoard board, PieceType player)
        {
            double sum = 0;
            foreach (Tuple<IScorer, double> scorersAndWeight in this.scorersAndWeights)
            {
                double score = scorersAndWeight.Item1.GetScore(board, player);
                sum += score * scorersAndWeight.Item2;
            }

            return sum;
        }
    }
}
