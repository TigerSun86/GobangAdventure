using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.Patterns
{
    public class Pattern : IPattern
    {
        private readonly List<PieceType> _pieces;

        public Pattern(IEnumerable<PieceType> pieces)
        {
            _pieces = pieces.ToList();
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
            return new Pattern(Pieces.Select(p => p.GetOther()));
        }
    }
}
