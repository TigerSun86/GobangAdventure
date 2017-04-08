using GobangGameLib.GameBoard;

namespace GobangGameLib.Game
{
    public interface IGame
    {
        void Start();

        void Run();

        IBoard Board { get; }

        PieceType CurPiece { get; }

        GameStatus GameStatus { get; }
    }
}