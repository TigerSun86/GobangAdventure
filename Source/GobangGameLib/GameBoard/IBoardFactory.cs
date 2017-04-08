namespace GobangGameLib.GameBoard
{
    public interface IBoardFactory
    {
        IBoard Create();

        IBoard DeepCloneBoard(IBoard board);
    }
}
