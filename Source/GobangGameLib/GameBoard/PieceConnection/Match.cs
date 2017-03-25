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
        public Match(PatternType type, List<Position> list)
        {
            PatternType = type;
            Positions = list;
        }

        public IEnumerable<Position> Positions
        {
            get; set;
        }

        public PatternType PatternType
        {
            get; set;
        }
    }
}
