using System.Collections.Generic;
using System.Linq;

namespace GobangGameLib.GameBoard.Patterns
{
    public class Pattern : IPattern
    {
        public Pattern(PatternType patternType,
            PatternPositionType patternPositionType,
            PieceType player,
            IEnumerable<PieceType> pieces)
        {
            this.PatternType = patternType;
            this.Player = player;
            this.Pieces = pieces.ToList();
            this.PatternPositionType = patternPositionType;
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
    }
}
