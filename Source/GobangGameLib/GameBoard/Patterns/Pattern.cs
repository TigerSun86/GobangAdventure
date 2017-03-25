using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.Patterns
{
    public class Pattern : IPattern
    {
        private PatternType _patternType;
        private readonly List<PieceType> _pieces;

        public Pattern(PatternType patternType, IEnumerable<PieceType> pieces)
        {
            _patternType = patternType;
            _pieces = pieces.ToList();
        }

        public PatternType PatternType
        {
            get
            {
                return _patternType;
            }
        }

        public IEnumerable<PieceType> Pieces
        {
            get
            {
                return _pieces;
            }
        }

        public IPattern GetOther()
        {
            return new Pattern(PatternType, Pieces.Select(p => p.GetOther()));
        }
    }
}
