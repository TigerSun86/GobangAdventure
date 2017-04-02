using GobangGameLib.GameBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard
{
    public interface IBoard
    {
        PieceType Get(Position position);

        void Set(Position position, PieceType piece);

        bool IsFull();

        int Count { get; }

        IBoard DeepClone();
    }
}
