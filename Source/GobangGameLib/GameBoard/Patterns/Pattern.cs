using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.Patterns
{
    public class Pattern : IPattern
    {
        public Pattern(PatternType patternType, PieceType player, IEnumerable<PieceType> pieces)
        {
            this.PatternType = patternType;
            this.Player = player;
            this.Pieces = pieces.ToList();
        }

        public PatternType PatternType
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
