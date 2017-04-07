using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard.Patterns;

namespace GobangGameLib.GameBoard.PieceConnection
{
    public interface IMatch
    {
        IList<Position> Positions { get; }

        PatternType PatternType { get; }
    }
}
