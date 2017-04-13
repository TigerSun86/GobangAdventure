using System.Collections.Generic;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternBoard : IBoard
    {
        private readonly IBoard board;
        private readonly PositionManager positions;
        private readonly PatternMatcher matcher;
        private readonly MatchRepository matches;

        public PatternBoard(IBoard board, PositionManager positions, PatternMatcher matcher)
        {
            this.board = board;
            this.positions = positions;
            this.matcher = matcher;
            this.matches = new MatchRepository();
            foreach (var match in GetAllMatches())
            {
                matches.Add(match.Pattern.Player, match.Pattern.PatternType, match);
            }
        }

        public MatchRepository Matches
        {
            get
            {
                return this.matches;
            }
        }

        public PieceType Get(Position position)
        {
            return this.board.Get(position);
        }

        public void Set(Position position, PieceType piece)
        {
            IEnumerable<IPositions> relatedLines = this.positions.GetAllLinesOf(position);

            IEnumerable<IMatch> oldMatches = this.matcher.MatchPatterns(board, relatedLines);

            foreach (IMatch match in oldMatches)
            {
                this.matches.Remove(match.Pattern.Player, match.Pattern.PatternType, match);
            }

            this.board.Set(position, piece);

            IEnumerable<IMatch> newMatches = this.matcher.MatchPatterns(board, relatedLines);

            foreach (IMatch match in newMatches)
            {
                this.matches.Add(match.Pattern.Player, match.Pattern.PatternType, match);
            }
        }

        public int Count
        {
            get
            {
                return this.board.Count;
            }
        }

        public bool IsFull()
        {
            return this.board.IsFull();
        }

        public IBoard DeepClone()
        {
            return new PatternBoard(this.board, this.positions, this.matcher);
        }

        private IEnumerable<IMatch> GetAllMatches()
        {
            return this.matcher.MatchPatterns(this.board, this.positions.Lines);
        }
    }
}
