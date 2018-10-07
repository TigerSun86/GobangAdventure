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
            for (int r = 0; r < board.Data.GetLength(0); r++)
            {
                double rowScore = -Math.Abs(r - (board.Data.GetLength(0) / 2));

                for (int c = 0; c < board.Data.GetLength(1); c++)
                {
                    double colScore = -Math.Abs(c - (board.Data.GetLength(1) / 2));

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
    }
}
