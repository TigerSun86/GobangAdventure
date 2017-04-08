using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternBoardFactory : IBoardFactory
    {
        private readonly BoardProperties context;
        private readonly PositionManager positions;
        private readonly PatternRepository patternRepository;
        private readonly PatternMatcher matcher;

        public PatternBoardFactory(BoardProperties context, PositionManager positions, PatternRepository patternRepository, PatternMatcher matcher)
        {
            this.context = context;
            this.positions = positions;
            this.patternRepository = patternRepository;
            this.matcher = matcher;
        }

        public IBoard Create()
        {
            return new PatternBoard(new Board(this.context), this.positions, this.patternRepository, this.matcher);
        }

        public IBoard DeepCloneBoard(IBoard board)
        {
            if (typeof(PatternBoard) == board.GetType())
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
