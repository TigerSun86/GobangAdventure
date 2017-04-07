using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard.Patterns;

namespace GobangGameLib.GameBoard.PieceConnection
{
    public class Match : IMatch
    {
        public Match(PatternType type, IList<Position> positions)
        {
            this.PatternType = type;
            this.Positions = positions;
        }

        public IList<Position> Positions
        {
            get;
        }

        public PatternType PatternType
        {
            get;
        }

        public override bool Equals(object obj)
        {
            var item = obj as Match;
            if (item == null)
            {
                return false;
            }

            return (PatternType == item.PatternType) && (Positions.SequenceEqual(item.Positions));
        }

        public override int GetHashCode()
        {
            // To avoid conflict, assuming Position.GethashCode() be greater than 10 bits.
            return ((int)PatternType << 20)
                ^ (Positions[0].GetHashCode() << 10)
                ^ Positions[Positions.Count - 1].GetHashCode();
        }
    }
}
