using GobangGameLib.GameBoard;

namespace GobangGameLib.Game
{
    public interface IGame
    {
        void Start();
        void Run();
        IBoard Board { get; }
        GameStatus GameStatus { get; }
    }
}