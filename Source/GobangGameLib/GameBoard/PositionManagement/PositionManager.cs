using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.PositionManagement
{
    public class PositionManager : IAllLineGroups
    {
        private readonly IDictionary<LineType, ILines> _lineGroups;
        private readonly BoardProperties _context;

        public PositionManager(BoardProperties context, IDictionary<LineType, ILines> lineGroups)
        {
            this._context = context;
            this._lineGroups = lineGroups;
        }

        public IDictionary<LineType, ILines> LineGroups
        {
            get
            {
                return _lineGroups;
            }
        }

        public IEnumerable<IPositions> Lines
        {
            get
            {
                return LineGroups.SelectMany(g => g.Value.Lines);
            }
        }

        /// <summary>
        /// Gets all positions in order of first row, second row, etc..
        /// </summary>
        public IEnumerable<Position> Positions
        {
            get
            {
                return LineGroups.First().Value.Lines.SelectMany(l => l.Positions);
            }
        }

        /// <summary>
        /// Yield every empty spot.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public IEnumerable<Position> GetEmptyPositions(IBoard board)
        {
            return GetPositionsByPieceType(board, PieceType.Empty);
        }

        public IEnumerable<Position> GetPositionsByPieceType(IBoard board, PieceType pieceType)
        {
            return Positions.Where(p => board.Get(p).Equals(pieceType));
        }

        public IEnumerable<Position> GetPlayerPositions(IBoard board)
        {
            return Positions.Where(p => !board.Get(p).Equals(PieceType.Empty));
        }

        public IEnumerable<IPositions> GetAllLinesOf(Position position)
        {
            return LineTypeExtensions.GetAll().Select(t => GetLineOf(position, t));
        }

        public IPositions GetLineOf(Position position, LineType type)
        {
            int lineIndex = GetLineIndex(position, type);
            return LineGroups[type].Lines[lineIndex];
        }

        public int GetLineIndex(Position position, LineType type)
        {
            if (type == LineType.Row)
            {
                return position.Row;
            }

            if (type == LineType.Column)
            {
                return position.Col;
            }

            if (type == LineType.DiagonalOne)
            {
                return GetDiagonalOneIndex(position);
            }

            if (type == LineType.DiagonalTwo)
            {
                return GetDiagonalTwoIndex(position);
            }

            throw new ArgumentException($"Unsupported LineType: {type}.");
        }

        /// <summary>
        /// The order of the diagonal one indexes is: bottom-left, top-left, top-right.
        /// 
        /// 3 4 5 6
        /// 2 \ \ \
        /// 1\ \ \ 
        /// 0 \ \ \ 
        /// </summary>
        public int GetDiagonalOneIndex(Position position)
        {
            return _context.RowSize - 1 - position.Row + position.Col;
        }

        /// <summary>
        /// The order of the diagonal two indexes is: top-left, bottom-left, bottom-right.
        /// 
        /// 0 / / /
        /// 1/ / /
        /// 2 / / /
        /// 3 4 5 6
        /// </summary>
        public int GetDiagonalTwoIndex(Position position)
        {
            return position.Row + position.Col;
        }
    }
}
