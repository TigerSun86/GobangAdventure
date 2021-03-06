﻿using System.Collections.Generic;
using System.Linq;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.GameJudge
{
    public class PatternJudge : IJudge
    {
        private readonly PositionManager positions;
        private readonly PatternRepository patternRepository;
        private readonly PatternMatcher matcher;

        public PatternJudge(PositionManager positions, PatternRepository patternRepository, PatternMatcher matcher)
        {
            this.positions = positions;
            this.patternRepository = patternRepository;
            this.matcher = matcher;
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
            PatternBoard patternBoard = board as PatternBoard;
            if (patternBoard == null)
            {
                var patterns = this.patternRepository.Get(PatternType.Five);
                return this.matcher.MatchPatterns(board, this.positions.Lines, patterns).Select(m => m.Positions);
            }

            return  patternBoard.Matches.Get(PatternType.Five).Select(m => m.Positions);
        }
    }
}
