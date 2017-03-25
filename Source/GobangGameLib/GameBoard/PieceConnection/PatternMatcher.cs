using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.GameBoard.PieceConnection
{
    public class PatternMatcher
    {
        private readonly static int Base = Enum.GetNames(typeof(PieceType)).Length;

        public IEnumerable<Match> MatchPatterns(IBoard board, IPositions line, IEnumerable<IPattern> patterns)
        {
            var patternsWithSameCount = patterns.GroupBy(p => p.Pieces.Count());
            return patternsWithSameCount.SelectMany(ps => Match(board, line, ps));
        }

        internal IEnumerable<Match> Match(IBoard board, IPositions line, IEnumerable<IPattern> patterns)
        {
            int num = patterns.First().Pieces.Count();
            int highestBase = (int)Math.Pow(Base, num - 1);
            int currentHash = 0;
            Queue<Position> queue = new Queue<Position>();
            Dictionary<int, PatternType> patternHashes = GetPatternHashes(patterns);
            foreach (Position p in line.Positions)
            {
                currentHash = Add(currentHash, board.Get(p));
                queue.Enqueue(p);

                Debug.Assert(queue.Count <= num);

                if (queue.Count == num)
                {
                    PatternType type;
                    if (patternHashes.TryGetValue(currentHash, out type))
                    {
                        yield return new Match(type, queue.ToList());
                    }

                    Position headPosition = queue.Dequeue();
                    currentHash = Remove(currentHash, highestBase, board.Get(headPosition));
                }
            }
        }

        private Dictionary<int, PatternType> GetPatternHashes(IEnumerable<IPattern> patterns)
        {
            Dictionary<int, PatternType> hashes = new Dictionary<int, PatternType>();
            foreach (IPattern pattern in patterns)
            {
                var hash = pattern.Pieces.Aggregate(0, (sum, piece) => Add(sum, piece));
                Debug.Assert(!hashes.ContainsKey(hash));
                hashes[hash] = pattern.PatternType;
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
