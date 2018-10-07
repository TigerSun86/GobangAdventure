using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternMatcher
    {
        // Add 1 to avoid PieceType.Empty value is 0.
        private readonly static int Base = PieceTypeExtensions.GetAll().Count() + 1;

        private readonly List<HashSet<IPattern>> allPatterns;
        private readonly Dictionary<int, IPattern> hashAndPattern;

        public PatternMatcher(PatternRepository patternRepository)
        {
            IEnumerable<IPattern> patterns = patternRepository.Get();

            this.allPatterns = patterns.GroupBy(p => p.Pieces.Count()).Select(g => new HashSet<IPattern>(g)).ToList();
            this.hashAndPattern = GetPatternHashes(patterns);
        }

        public IEnumerable<IMatch> MatchPatterns(IBoard board, IEnumerable<IPositions> lines)
        {
            return lines.SelectMany(line =>
                this.allPatterns.SelectMany(patterns => MatchInternal(board, line, patterns)));
        }

        public IEnumerable<IMatch> MatchPatterns(IBoard board, IEnumerable<IPositions> lines, IEnumerable<IPattern> patterns)
        {
            var patternsWithSameCount = patterns.GroupBy(p => p.Pieces.Count());

            return lines.SelectMany(line =>
                patternsWithSameCount.SelectMany(ps => MatchInternal(board, line, new HashSet<IPattern>(ps))));
        }

        internal IEnumerable<IMatch> MatchInternal(IBoard board, IPositions line, HashSet<IPattern> patterns)
        {
            if (board.Count == 0)
            {
                // Assuming there is no pattern when no piece on the board.
                yield break;
            }

            // Assuming all patterns are of the same size.
            int patternSize = patterns.First().Pieces.Count();
            int highestBase = (int)Math.Pow(Base, patternSize - 1);
            int currentHash = 0;
            Queue<Position> queue = new Queue<Position>();
            for (int index = 0; index < line.Positions.Count; index++)
            {
                Position position = line.Positions[index];

                currentHash = Add(currentHash, board.Get(position));
                queue.Enqueue(position);

                Debug.Assert(queue.Count <= patternSize);

                if (queue.Count == patternSize)
                {
                    IPattern pattern;
                    if (hashAndPattern.TryGetValue(currentHash, out pattern) && patterns.Contains(pattern))
                    {
                        if (IsPatternInValidPosition(pattern.PatternPositionType, index, patternSize, line.Positions.Count))
                        {
                            yield return new Match(pattern, queue.ToList());
                        }
                    }

                    Position headPosition = queue.Dequeue();
                    currentHash = Remove(currentHash, highestBase, board.Get(headPosition));
                }
            }
        }

        private bool IsPatternInValidPosition(PatternPositionType type, int index, int patternSize, int lineSize)
        {
            return (type == PatternPositionType.Any) ||
                ((type == PatternPositionType.Head) && (index + 1 == patternSize)) ||
                ((type == PatternPositionType.Tail) && (index + 1 == lineSize));
        }

        private Dictionary<int, IPattern> GetPatternHashes(IEnumerable<IPattern> patterns)
        {
            Dictionary<int, IPattern> hashes = new Dictionary<int, IPattern>();
            foreach (IPattern pattern in patterns)
            {
                var hash = pattern.Pieces.Aggregate(0, (sum, piece) => Add(sum, piece));
                Debug.Assert(!hashes.ContainsKey(hash));
                hashes[hash] = pattern;
            }

            return hashes;
        }

        private int Remove(int sum, int highestBase, PieceType pieceType)
        {
            // Add 1 to avoid PieceType.Empty value is 0.
            return sum - (((int)pieceType + 1) * highestBase);
        }

        private int Add(int sum, PieceType pieceType)
        {
            // Add 1 to avoid PieceType.Empty value is 0.
            return (sum * Base) + (int)pieceType + 1;
        }
    }
}
