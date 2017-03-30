using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.PositionManagement
{
    public class PositionManager : IAllLineGroups, ILines, IPositions
    {
        private readonly List<ILines> _lineGroups;

        public PositionManager(List<ILines> lineGroups)
        {
            this._lineGroups = lineGroups;
        }

        public IEnumerable<ILines> LineGroups
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
                return LineGroups.SelectMany(g => g.Lines);
            }
        }

        /// <summary>
        /// Gets all positions in order of first row, second row, etc..
        /// </summary>
        public IEnumerable<Position> Positions
        {
            get
            {
                return LineGroups.First().Lines.SelectMany(l => l.Positions);
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
