using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.Patterns
{
    public interface IPatternGroup
    {
        IDictionary<PieceType, IEnumerable<IPattern>> Patterns { get; }
    }
}
