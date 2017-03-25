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

            var myPatterns = Enum.GetValues(typeof(PatternType)).Cast<PatternType>().Select(e=> Count(board, player, e)).ToList();
            //Debug.WriteLine("My patterns "+string.Join(",", myPatterns));

            var oPatterns = Enum.GetValues(typeof(PatternType)).Cast<PatternType>().Select(e => Count(board, player.GetOther(), e)).ToList();
            //Debug.WriteLine("O patterns " + string.Join(",", oPatterns));

            sum += myPatterns[0] * 10;
            sum += myPatterns[1] * 5;
            sum += myPatterns[2] * 3;
            sum += myPatterns[3] * 1;

            sum -= oPatterns[0] * 10;
            sum -= oPatterns[1] * 5;
            sum -= oPatterns[2] * 3;
            sum -= oPatterns[3] * 1;

            return sum;
        }

        private int Count(IBoard board, PieceType pieceType, PatternType patternType)
        {
            var matcher = new PatternMatcher();
            var patterns = PatternManager.Instance().PatternRepo[patternType].Patterns[pieceType];
            var matched = PositionManager.Instance()
                .Lines
                .SelectMany(l => matcher.MatchPatterns(board, l, patterns));
            return matched.Count();
        }
    }
}
