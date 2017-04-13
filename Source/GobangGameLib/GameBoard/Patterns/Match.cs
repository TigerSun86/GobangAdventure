using System.Collections.Generic;
using System.Linq;

namespace GobangGameLib.GameBoard.Patterns
{
    public class Match : IMatch
    {
        public Match(IPattern pattern, IList<Position> positions)
        {
            this.Pattern = pattern;
            this.Positions = positions;
        }

        public IList<Position> Positions
        {
            get;
        }

        public IPattern Pattern
        {
            get;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            var item = obj as Match;
            if (item == null)
            {
                return false;
            }

            // To be fast, matches are equal when 1, types are the same and 2, start and end positions are equal.
            // Assuming 1, positions are in straight line.
            // 2, this equal method won't be used for comparing the same match with different players,
            // for example, match 1, empty, p1, empty; match 2, empty, p2, empty.
            return (Pattern.PatternType == item.Pattern.PatternType)
                && (Positions.Count == item.Positions.Count)
                && (Positions[0] == item.Positions[0])
                && (Positions[Positions.Count - 1] == item.Positions[Positions.Count - 1]);
        }

        public override int GetHashCode()
        {
            // To avoid conflict, assuming Position.GethashCode() be greater than 10 bits.
            return ((int)Pattern.PatternType << 20)
                ^ (Positions[0].GetHashCode() << 10)
                ^ Positions[Positions.Count - 1].GetHashCode();
        }

        public override string ToString()
        {
            return $"[{string.Join(",", Positions.Select(p => p.ToString()))}]";
        }
    }
}
