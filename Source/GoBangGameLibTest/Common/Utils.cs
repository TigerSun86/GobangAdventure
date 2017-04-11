using System;
using System.Collections.Generic;
using System.Linq;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace GoBangGameLibTest.Common
{
    public class Utils
    {
        private static readonly Dictionary<char, PieceType> CharToPiece =
            new Dictionary<char, PieceType>() {
                { ' ' , PieceType.Empty },
                { 'X' , PieceType.P1 },
                { 'O' , PieceType.P2 },
            };

        public static IBoard ParseBoard(string[] boardString, BoardProperties context, PositionManager positions)
        {
            if (context.RowSize != boardString.Length || boardString.Any(s => s.Length != context.ColSize))
            {
                throw new ArgumentException("The board string does not consist with the board sizes in context.");
            }

            IBoardFactory boardFactory = new BoardFactory(context, positions);
            IBoard board = boardFactory.Create();
            foreach (Position p in positions.Positions)
            {
                char pieceChar = boardString[p.Row][p.Col];
                PieceType piece = CharToPiece[pieceChar];
                board.Set(p, piece);
            }

            return board;
        }
    }
}
