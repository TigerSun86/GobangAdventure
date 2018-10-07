using GobangGameLib.GameBoard;

namespace GobangBenchMark.Utilities
{
    public class NaiveBoard
    {
        public NaiveBoard(int rowSize, int colSize)
        {
            Data = new PieceType[rowSize, colSize];
        }

        public PieceType[,] Data { get; set; }
    }
}
