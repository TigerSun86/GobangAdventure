using System.Collections.Generic;
using System.Linq;
using GobangGameLib.GameBoard.Patterns;

namespace GobangGameLib.GameBoard.PieceConnection
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

            return (Pattern == item.Pattern) && (Positions.SequenceEqual(item.Positions));
        }

        public override int GetHashCode()
        {
            // To avoid conflict, assuming Position.GethashCode() be greater than 10 bits.
            return ((int)Pattern.PatternType << 20)
                ^ (Positions[0].GetHashCode() << 10)
                ^ Positions[Positions.Count - 1].GetHashCode();
        }
    }
}
