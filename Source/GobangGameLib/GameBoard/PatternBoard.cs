using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PieceConnection;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.GameBoard
{
    public class PatternBoard : IBoard
    {
        private readonly IBoard board;
        private readonly PositionManager positions;
        private readonly PatternRepository patternRepository;
        private readonly PatternMatcher matcher;
        private readonly IDictionary<PieceType, HashSet<IMatch>> matches;

        public PatternBoard(IBoard board, PositionManager positions,
            PatternRepository patternRepository, PatternMatcher matcher)
        {
            this.board = board;
            this.positions = positions;
            this.patternRepository = patternRepository;
            this.matcher = matcher;
            this.matches = new Dictionary<PieceType, HashSet<IMatch>>
            {
                { PieceType.P1, new HashSet<IMatch>(GetPatternCounts(PieceType.P1))},
                { PieceType.P2, new HashSet<IMatch>(GetPatternCounts(PieceType.P2))}
            };
        }

        private IEnumerable<IMatch> GetPatternCounts(PieceType pieceType)
        {
            if (this.board.Count == 0)
            {
                return Enumerable.Empty<IMatch>();
            }

            var patternTypes = Enum.GetValues(typeof(PatternType)).Cast<PatternType>();
            var patterns = patternTypes
                .Select(p => this.patternRepository.Patterns[p].Patterns[pieceType])
                .SelectMany(p => p);
            var matcher = new PatternMatcher();
            var matches = this.positions.Lines.SelectMany(l => matcher.MatchPatterns(this.board, l, patterns));
            return matches;
        }

        public IDictionary<PieceType, HashSet<IMatch>> Matches
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

            IEnumerable<PatternType> patternTypes = Enum.GetValues(typeof(PatternType)).Cast<PatternType>();

            IEnumerable<IPattern> patterns = patternTypes
                .Select(p => this.patternRepository.Patterns[p].Patterns.Values)
                .SelectMany(p => p)
                .SelectMany(p => p);

            IList<Match> oldMatches = relatedLines
                .SelectMany(l => this.matcher.MatchPatterns(board, l, patterns))
                .ToList();

            foreach (Match match in oldMatches)
            {
                this.matches[match.Pattern.Player].Remove(match);
            }

            this.board.Set(position, piece);

            IList<Match> newMatches = relatedLines
                .SelectMany(l => this.matcher.MatchPatterns(board, l, patterns))
                .ToList();

            foreach (Match match in newMatches)
            {
                this.matches[match.Pattern.Player].Add(match);
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
            return new PatternBoard(this.board, this.positions, this.patternRepository, this.matcher);
        }
    }
}
