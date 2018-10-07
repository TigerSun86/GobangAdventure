using GobangGameLib.GameBoard;
using System;
using System.Collections.Generic;

namespace GobangBenchMark.Utilities
{
    public class NaiveBoard
    {
        public NaiveBoard(int rowSize, int colSize)
        {
            Data = new PieceType[rowSize, colSize];
            RowSize = rowSize;
            ColSize = colSize;
        }

        public PieceType[,] Data { get; set; }

        public int RowSize { get; private set; }

        public int ColSize { get; private set; }

        public NaiveBoard DeepClone()
        {
            NaiveBoard result = new NaiveBoard(this.Data.GetLength(0), this.Data.GetLength(1));
            result.Data = (PieceType[,])Data.Clone();
            return result;
        }

        public int Compress(int r, int c)
        {
            return (r << 8) + c;
        }
    }
}
