using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PieceConnection;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.Util;

namespace AI.Scorer
{
    public class PatternScorer : IScorer
    {
        public double GetScore(IBoard board, PieceType player)
        {
            double sum = 0;

            var myPatterns = GetPatternCounts(board, player);
            //Debug.WriteLine("My patterns "+string.Join(",", myPatterns));

            var oPatterns = GetPatternCounts(board, player.GetOther());
            //Debug.WriteLine("O patterns " + string.Join(",", oPatterns));

            sum += GetCountFromDictionary(myPatterns, PatternType.Five) * 10;
            sum += GetCountFromDictionary(myPatterns, PatternType.OpenFour) * 5;
            sum += GetCountFromDictionary(myPatterns, PatternType.OpenThree) * 3;
            sum += GetCountFromDictionary(myPatterns, PatternType.OpenTwo) * 1;

            sum -= GetCountFromDictionary(oPatterns, PatternType.Five) * 10;
            sum -= GetCountFromDictionary(oPatterns, PatternType.OpenFour) * 5;
            sum -= GetCountFromDictionary(oPatterns, PatternType.OpenThree) * 3;
            sum -= GetCountFromDictionary(oPatterns, PatternType.OpenTwo) * 1;

            return sum;
        }

        private Dictionary<PatternType, int> GetPatternCounts(IBoard board, PieceType pieceType)
        {
            var patternTypes = Enum.GetValues(typeof(PatternType)).Cast<PatternType>();
            var patterns = patternTypes.Select(p => PatternManager.Instance().PatternRepo[p].Patterns[pieceType]).SelectMany(p => p);
            var matcher = new PatternMatcher();
            var matches = PositionManager.Instance()
                .Lines
                .SelectMany(l => matcher.MatchPatterns(board, l, patterns));
            var counts = matches.GroupBy(m => m.PatternType).ToDictionary(g => g.Key, g => g.Count());
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
