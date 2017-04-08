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
            return GetPlayerPositions(board, PieceType.Empty);
        }

        public IEnumerable<Position> GetPlayerPositions(IBoard board, PieceType player)
        {
            return Positions.Where(p => board.Get(p).Equals(player));
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
            else if (type == LineType.Column)
            {
                return position.Col;
            }
            else if (type == LineType.DiagonalOne)
            {
                return GetDiagonalOneIndex(position);
            }
            else if (type == LineType.DiagonalTwo)
            {
                return GetDiagonalTwoIndex(position);
            }
            else
            {
                throw new ArgumentException($"Unsupported LineType: {type}.");
            }
        }

        public int GetDiagonalOneIndex(Position position)
        {
            return _context.RowSize - 1 - position.Row + position.Col;
        }

        public int GetDiagonalTwoIndex(Position position)
        {
            return position.Row + position.Col;
        }
    }
}
