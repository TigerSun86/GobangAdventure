
using GobangGameLib.Game;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GobangGameLib.GameBoard
{
    public class Board : IBoard
    {
        private BoardProperties _context;
        private PieceType[,] _data;
        private int _pieceCount;

        public Board(BoardProperties context)
        {
            _context = context;
            _data = new PieceType[_context.RowSize, _context.ColSize];
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
            return _pieceCount == _context.RowSize * _context.ColSize;
        }

        public int Count
        {
            get
            {
                return _pieceCount;
            }
        }

        public IBoard DeepClone()
        {
            Board result = new Board(_context);
            result._data = (PieceType[,])_data.Clone();
            result._pieceCount = _pieceCount;
            return result;
        }
    }
}
