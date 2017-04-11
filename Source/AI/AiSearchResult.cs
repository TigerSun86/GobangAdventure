using System.Collections.Generic;

namespace AI
{
    internal class AiSearchResult
    {
        public AiSearchResult(double score)
        {
            this.Score = score;
            this.Moves = new Stack<PlayerAndMove>();
        }

        public double Score { get; set; }

        public Stack<PlayerAndMove> Moves { get; private set; }
    }
}
