using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.Util
{
    public class BoardHelper
    {
        /// <summary>
        /// Yield every empty spot.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static IEnumerable<Position> GetEmptyPositions(IBoard board)
        {
            return GetPlayerPositions(board, PieceType.Empty);
        }

        public static IEnumerable<Position> GetPlayerPositions(IBoard board, PieceType player)
        {
            return PositionManager.Instance().Positions.Where(p => board.Get(p).Equals(player));
        }
    }
}
