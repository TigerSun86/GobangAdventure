using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternGroup : IPatternGroup
    {
        private IDictionary<PieceType, IEnumerable<IPattern>> _patterns;

        public PatternGroup(IDictionary<PieceType, IEnumerable<IPattern>> patterns)
        {
            _patterns = patterns;
        }

        public IDictionary<PieceType, IEnumerable<IPattern>> Patterns
        {
            get
            {
                return _patterns;
            }
        }
    }
}
