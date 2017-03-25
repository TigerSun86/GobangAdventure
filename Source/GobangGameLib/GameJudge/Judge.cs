using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PieceConnection;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangGameLib.GameJudge
{
    public class Judge : IJudge
    {
        public PieceType GetWinner2(IBoard board)
        {
            var winner = PositionManager.Instance().LineGroups
                .SelectMany(lineGroup => lineGroup.Lines.Select(line => GetWinnerInOneLine(board, line)))
                .Where(winnerPiece => !winnerPiece.Equals(PieceType.Empty))
                .FirstOrDefault();

            return winner;
        }

        public PieceType GetWinner(IBoard board)
        {
            var line = GetWinLines(board).FirstOrDefault();
            if (line == null)
            {
                return PieceType.Empty;
            }

            return board.Get(line.First());
        }

        private IEnumerable<IEnumerable<Position>> GetWinLines(IBoard board)
        {
            var matcher = new PatternMatcher();
            var patterns = PatternManager.Instance().PatternRepo[PatternType.Five].Patterns.Values.SelectMany(x => x);
            return PositionManager.Instance()
                .Lines
                .SelectMany(l => matcher.MatchPatterns(board, l, patterns));
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
                    if (count >= BoardProperties.NumOfPiecesToWin)
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
