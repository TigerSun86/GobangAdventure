
using GobangGameLib.Game;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GobangGameLib.GameBoard
{
    public class Board : IBoard
    {
        private PieceType[,] _data;
        private int _pieceCount;

        public Board()
        {
            _data = new PieceType[BoardProperties.RowSize, BoardProperties.ColSize];
            _pieceCount = 0;
        }

        public PieceType Get(Position position)
        {
            return _data[position.Row, position.Col];
        }

        public void Set(Position position, PieceType piece)
        {
            // Remove a piece.
            if (piece.Equals(PieceType.Empty) && !_data[position.Row, position.Col].Equals(PieceType.Empty))
            {
                _pieceCount--;
            }
            // Place a piece.
            else if (!piece.Equals(PieceType.Empty) && _data[position.Row, position.Col].Equals(PieceType.Empty))
            {
                _pieceCount++;
            }

            _data[position.Row, position.Col] = piece;
        }

        public bool IsFull()
        {
            return _pieceCount == BoardProperties.RowSize * BoardProperties.ColSize;
        }

        public IBoard DeepClone()
        {
            Board result = new Board();
            result._data = (PieceType[,])_data.Clone();
            return result;
        }
    }
}
