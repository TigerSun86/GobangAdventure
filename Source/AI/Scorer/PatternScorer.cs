using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PieceConnection;
using GobangGameLib.GameBoard.PositionManagement;

namespace AI.Scorer
{
    public class PatternScorer : IScorer
    {
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
            Dictionary<PatternType, int> myPatterns;
            Dictionary<PatternType, int> oPatterns;
            PatternBoard patternBoard = board as PatternBoard;
            if (patternBoard != null)
            {
                myPatterns = GetPatternCounts(patternBoard, player);
                oPatterns = GetPatternCounts(patternBoard, player.GetOther());
            }
            else
            {
                myPatterns = GetPatternCounts(board, player);
                oPatterns = GetPatternCounts(board, player.GetOther());
            }

            double myScore = 0;
            myScore += GetCountFromDictionary(myPatterns, PatternType.Five) * 100;
            myScore += GetCountFromDictionary(myPatterns, PatternType.OpenFour) * 20;
            myScore += GetCountFromDictionary(myPatterns, PatternType.OpenThree) * 3;
            myScore += GetCountFromDictionary(myPatterns, PatternType.OpenTwo) * 1;
            myScore += GetCountFromDictionary(myPatterns, PatternType.OpenOne) * 0.1;

            double oScore = 0;
            oScore += GetCountFromDictionary(oPatterns, PatternType.Five) * 100;
            oScore += GetCountFromDictionary(oPatterns, PatternType.OpenFour) * 20;
            oScore += GetCountFromDictionary(oPatterns, PatternType.OpenThree) * 3;
            oScore += GetCountFromDictionary(oPatterns, PatternType.OpenTwo) * 1;
            oScore += GetCountFromDictionary(oPatterns, PatternType.OpenOne) * 0.1;

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

        private void DebugInfo(Dictionary<PatternType, int> myPatterns, Dictionary<PatternType, int> oPatterns)
        {
            var my = string.Join(",", myPatterns);
            var o = string.Join(",", oPatterns);
            if (!string.IsNullOrWhiteSpace(my))
            {
                Debug.WriteLine("My patterns " + my);
            }
            if (!string.IsNullOrWhiteSpace(o))
            {
                Debug.WriteLine("O patterns " + o);
            }
        }

        private Dictionary<PatternType, int> GetPatternCounts(IBoard board, PieceType pieceType)
        {
            var patterns = patternRepository.Get(pieceType);
            var matches = this.matcher.MatchPatterns(board, positions.Lines, patterns);
            var counts = matches.GroupBy(m => m.Pattern.PatternType).ToDictionary(g => g.Key, g => g.Count());
            return counts;
        }

        private Dictionary<PatternType, int> GetPatternCounts(PatternBoard board, PieceType pieceType)
        {
            HashSet<IMatch> matches = board.Matches[pieceType];
            var counts = matches.GroupBy(m => m.Pattern.PatternType).ToDictionary(g => g.Key, g => g.Count());
            return counts;
        }

        private int GetCountFromDictionary(Dictionary<PatternType, int> dict, PatternType type)
        {
            int v;
            if (dict.TryGetValue(type, out v))
            {
                return v;
            }
            return 0;
        }
    }
}
