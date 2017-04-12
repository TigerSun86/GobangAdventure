using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternMatcher
    {
        private readonly static int Base = PieceTypeExtensions.GetAll().Count();

        public IEnumerable<IMatch> MatchPatterns(IBoard board, IPositions line, IEnumerable<IPattern> patterns)
        {
            var patternsWithSameCount = patterns.GroupBy(p => p.Pieces.Count());
            return patternsWithSameCount.SelectMany(ps => MatchInternal(board, line, ps));
        }

        public IEnumerable<IMatch> MatchPatterns(IBoard board, IEnumerable<IPositions> lines, IEnumerable<IPattern> patterns)
        {
            return lines.SelectMany(l => this.MatchPatterns(board, l, patterns));
        }

        internal IEnumerable<IMatch> MatchInternal(IBoard board, IPositions line, IEnumerable<IPattern> patterns)
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
            Dictionary<int, IPattern> hashAndPattern = GetPatternHashes(patterns);
            for (int index = 0; index < line.Positions.Count; index++)
            {
                Position position = line.Positions[index];

                currentHash = Add(currentHash, board.Get(position));
                queue.Enqueue(position);

                Debug.Assert(queue.Count <= patternSize);

                if (queue.Count == patternSize)
                {
                    IPattern pattern;
                    if (hashAndPattern.TryGetValue(currentHash, out pattern))
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
            return sum - ((int)pieceType * highestBase);
        }

        private int Add(int sum, PieceType pieceType)
        {
            return (sum * Base) + (int)pieceType;
        }
    }
}
