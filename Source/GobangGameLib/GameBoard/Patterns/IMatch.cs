using System.Collections.Generic;

namespace GobangGameLib.GameBoard.Patterns
{
    public interface IMatch
    {
        IList<Position> Positions { get; }

        IPattern Pattern { get; }
    }
}
