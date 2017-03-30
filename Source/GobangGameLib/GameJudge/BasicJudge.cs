using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.GameJudge
{
    public class BasicJudge : IJudge
    {
        private readonly BoardProperties _context;
        private readonly PositionManager _positions;

        public BasicJudge(BoardProperties context, PositionManager positions)
        {
            _context = context;
            _positions = positions;
        }

        public PieceType GetWinner(IBoard board)
        {
            var winner = _positions.Lines.Select(line => GetWinnerInOneLine(board, line))
                .Where(winnerPiece => !winnerPiece.Equals(PieceType.Empty))
                .FirstOrDefault();

            return winner;
        }

        private PieceType GetWinnerInOneLine(IBoard board, IPositions line)
        {
            var count = 0;
            var pre = PieceType.Empty;

            foreach (var position in line.Positions)
            {
                var piece = board.Get(position);
                if (!pre.Equals(PieceType.Empty) && pre.Equals(piece))
                {
                    count++;
                    if (count >= _context.NumOfPiecesToWin)
                    {
                        return piece;
                    }
                }
                else if (!pre.Equals(piece))
                {
                    count = 1;
                }
                pre = piece;
            }

            return PieceType.Empty;
        }
    }
}
