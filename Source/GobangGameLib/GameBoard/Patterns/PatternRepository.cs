using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternRepository
    {
        private IDictionary<PatternType, IPatternGroup> _patterns;

        public PatternRepository(IDictionary<PatternType, IPatternGroup> patterns)
        {
            _patterns = patterns;
        }

        public IDictionary<PatternType, IPatternGroup> Patterns
        {
            get
            {
                return _patterns;
            }
        }
    }
}
