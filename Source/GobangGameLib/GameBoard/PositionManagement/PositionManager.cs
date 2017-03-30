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

        public PositionManager(IDictionary<LineType, ILines> lineGroups)
        {
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
    }
}
