using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PositionManagement;

namespace AI.Scorer
{
    public class PatternScorer : IScorer
    {
        private readonly static Dictionary<PatternType, double> PatternAndScore = new Dictionary<PatternType, double>
        {
            { PatternType.Five, 100 },
            { PatternType.OpenFour, 20 },
            { PatternType.HalfFour, 10 },
            { PatternType.OpenThree, 3 },
            { PatternType.OpenTwo, 1 },
            { PatternType.OpenOne, 0.1 },
        };

        private readonly PositionManager positions;
        private readonly PatternRepository patternRepository;
        private readonly PatternMatcher matcher;

        public PatternScorer(PositionManager positions, PatternRepository patternRepository, PatternMatcher matcher)
        {
            this.positions = positions;
            this.patternRepository = patternRepository;
            this.matcher = matcher;
        }

        public double GetScore(IBoard board, PieceType player)
        {
            MatchRepository matchRepository;
            PatternBoard patternBoard = board as PatternBoard;
            if (patternBoard != null)
            {
                matchRepository = patternBoard.Matches;
            }
            else
            {
                var matches = this.matcher.MatchPatterns(board, positions.Lines);
                matchRepository = new MatchRepository();
                foreach (var match in matches)
                {
                    matchRepository.Add(match.Pattern.Player, match.Pattern.PatternType, match);
                }
            }

            double myScore = GetScore(player, matchRepository);
            double oScore = GetScore(player.GetOther(), matchRepository);

            var nextPlayer = board.Count % 2 == 0 ? PieceType.P1 : PieceType.P2;
            bool isMyTurn = player == nextPlayer;
            if (isMyTurn)
            {
                myScore *= 1.2;
            }
            else
            {
                oScore *= 1.2;
            }

            return myScore - oScore;
        }

        private double GetScore(PieceType player, MatchRepository matchRepository)
        {
            double score = 0;
            foreach (PatternType pattern in PatternTypeExtensions.GetAll())
            {
                int patternCount = matchRepository.Get(player, pattern).Count;
                score += patternCount * PatternScorer.PatternAndScore[pattern];
            }

            return score;
        }
    }
}
