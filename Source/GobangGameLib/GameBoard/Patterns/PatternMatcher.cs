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
                yield break;
            }

            int num = patterns.First().Pieces.Count();
            int highestBase = (int)Math.Pow(Base, num - 1);
            int currentHash = 0;
            Queue<Position> queue = new Queue<Position>();
            Dictionary<int, IPattern> patternHashes = GetPatternHashes(patterns);
            foreach (Position p in line.Positions)
            {
                currentHash = Add(currentHash, board.Get(p));
                queue.Enqueue(p);

                Debug.Assert(queue.Count <= num);

                if (queue.Count == num)
                {
                    IPattern pattern;
                    if (patternHashes.TryGetValue(currentHash, out pattern))
                    {
                        yield return new Match(pattern, queue.ToList());
                    }

                    Position headPosition = queue.Dequeue();
                    currentHash = Remove(currentHash, highestBase, board.Get(headPosition));
                }
            }
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
