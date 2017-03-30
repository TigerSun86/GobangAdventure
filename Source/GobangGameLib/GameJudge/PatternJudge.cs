using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PieceConnection;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.GameJudge
{
    public class PatternJudge : IJudge
    {
        private readonly PositionManager _positions;

        public PatternJudge(PositionManager positions)
        {
            _positions = positions;
        }

        public PieceType GetWinner(IBoard board)
        {
            var line = GetWinLines(board).FirstOrDefault();
            if (line == null)
            {
                return PieceType.Empty;
            }

            return board.Get(line.First());
        }

        private IEnumerable<IEnumerable<Position>> GetWinLines(IBoard board)
        {
            var matcher = new PatternMatcher();
            var patterns = PatternManager.Instance().PatternRepo[PatternType.Five].Patterns.Values.SelectMany(x => x);
            return _positions
                .Lines
                .SelectMany(l => matcher.MatchPatterns(board, l, patterns).Select(m => m.Positions));
        }
    }
}
