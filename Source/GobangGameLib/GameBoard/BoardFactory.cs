using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PieceConnection;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.GameBoard
{
    public class BoardFactory : IBoardFactory
    {
        private readonly BoardProperties context;
        private readonly PositionManager positions;

        public BoardFactory(BoardProperties context, PositionManager positions)
        {
            this.context = context;
            this.positions = positions;
        }

        public IBoard Create()
        {
            return new Board(this.context);
        }

        public IBoard DeepCloneBoard(IBoard board)
        {
            if (typeof(Board) == board.GetType())
            {
                return board.DeepClone();
            }

            IBoard boardClone = this.Create();

            foreach (Position p in this.positions.Positions)
            {
                boardClone.Set(p, board.Get(p));
            }

            return boardClone;
        }
    }
}
