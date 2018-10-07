using GobangGameLib.GameBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangBenchMark.Utilities
{
    public class NaiveCenterScorer
    {
        public double GetScore(NaiveBoard board, PieceType player)
        {
            double sum = 0;
            for (int r = 0; r < board.RowSize; r++)
            {
                double rowScore = -Math.Abs(r - (board.RowSize / 2));

                for (int c = 0; c < board.ColSize; c++)
                {
                    double colScore = -Math.Abs(c - (board.ColSize / 2));

                    if (player == board.Data[r, c])
                    {
                        sum += rowScore + colScore;
                    }
                    else if (player.GetOther() == board.Data[r, c])
                    {
                        sum -= rowScore + colScore;
                    }
                }
            }

            return sum;
        }

        public double GetScoreWithAction(NaiveBoard board, PieceType player)
        {
            double sum = 0;
            board.Traversal((r, c) =>
            {
                double rowScore = -Math.Abs(r - (board.RowSize / 2));
                double colScore = -Math.Abs(c - (board.ColSize / 2));

                if (player == board.Data[r, c])
                {
                    sum += rowScore + colScore;
                }
                else if (player.GetOther() == board.Data[r, c])
                {
                    sum -= rowScore + colScore;
                }
            });

            return sum;
        }
    }
}
