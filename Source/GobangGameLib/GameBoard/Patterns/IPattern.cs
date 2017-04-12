using System.Collections.Generic;

namespace GobangGameLib.GameBoard.Patterns
{
    public interface IPattern
    {
        PatternType PatternType { get; }

        PatternPositionType PatternPositionType { get; }

        PieceType Player { get; }

        IEnumerable<PieceType> Pieces { get; }
    }
}
