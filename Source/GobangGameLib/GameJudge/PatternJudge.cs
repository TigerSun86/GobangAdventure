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
        private readonly PatternRepository _patternRepository;

        public PatternJudge(PositionManager positions, PatternRepository patternRepository)
        {
            _positions = positions;
            _patternRepository = patternRepository;
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
            var patterns = _patternRepository.Patterns[PatternType.Five].Patterns.Values.SelectMany(x => x);
            return _positions
                .Lines
                .SelectMany(l => matcher.MatchPatterns(board, l, patterns).Select(m => m.Positions));
        }
    }
}
