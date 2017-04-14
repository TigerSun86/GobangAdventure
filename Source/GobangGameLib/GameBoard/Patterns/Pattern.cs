using System.Collections.Generic;
using System.Linq;

namespace GobangGameLib.GameBoard.Patterns
{
    public class Pattern : IPattern
    {
        // To speed up Equals().
        private readonly int uniqueHashCode;

        public Pattern(PatternType patternType,
            PatternPositionType patternPositionType,
            PieceType player,
            IEnumerable<PieceType> pieces)
        {
            this.PatternType = patternType;
            this.PatternPositionType = patternPositionType;
            this.Player = player;
            this.Pieces = pieces.ToList();
            this.uniqueHashCode = GetUniqueHashCode();
        }

        public PatternType PatternType
        {
            get;
        }

        public PatternPositionType PatternPositionType
        {
            get;
        }

        public PieceType Player
        {
            get;
        }

        public IEnumerable<PieceType> Pieces
        {
            get;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            var item = obj as Pattern;
            if (item == null)
            {
                return false;
            }

            return this.uniqueHashCode == item.uniqueHashCode;
        }

        public override int GetHashCode()
        {
            return this.uniqueHashCode;
        }

        private int GetUniqueHashCode()
        {
            int shift = 0;
            int sum = 0;
            sum += (int)this.Player << shift;
            shift += 2;
            sum += (int)this.PatternPositionType << shift;
            shift += 2;
            sum += (int)this.PatternType << shift;
            shift += 4;
            foreach (PieceType piece in this.Pieces)
            {
                sum += (int)piece << shift;
                shift += 2;
            }

            return sum;
        }
    }
}
