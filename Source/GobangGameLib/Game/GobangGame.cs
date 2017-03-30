
using System;
using System.Diagnostics;
using GobangGameLib.GameBoard;
using GobangGameLib.GameJudge;
using GobangGameLib.Players;

namespace GobangGameLib.Game
{
    public class GobangGame : IGame
    {
        private BoardProperties _context;
        private IPlayer _player1;
        private IPlayer _player2;
        private IJudge _judge;
        private PieceType _curPiece;

        public GobangGame(BoardProperties context, IPlayer p1, IPlayer p2, IJudge judge)
        {
            _context = context;
            _player1 = p1;
            _player2 = p2;
            _judge = judge;
        }

        public IBoard Board { get; private set; }
        public GameStatus GameStatus { get; private set; }

        public void Start()
        {
            _curPiece = PieceType.P1;

            Board = new Board(_context);
            GameStatus = GameStatus.NotEnd;
        }

        public void Run()
        {
            if (GameStatus != GameStatus.NotEnd)
            {
                throw new InvalidOperationException("Failed to run the game after it's over.");
            }

            IPlayer curPlayer = GetPlayer(_curPiece);
            Position move = curPlayer.MakeAMove(Board);
            Board.Set(move, _curPiece);

            Debug.WriteLine($"{_curPiece} moved at ({move.Row},{move.Col}).");

            _curPiece = _curPiece.GetOther();

            var winner = _judge.GetWinner(Board);
            if (winner == PieceType.P1)
            {
                GameStatus = GameStatus.P1Win;
            }
            else if (winner == PieceType.P2)
            {
                GameStatus = GameStatus.P2Win;
            }
            else if (Board.IsFull())
            {
                GameStatus = GameStatus.Tie;
            }
        }

        private IPlayer GetPlayer(PieceType p)
        {
            if (p == PieceType.P1)
            {
                return _player1;
            }
            else if (p == PieceType.P2)
            {
                return _player2;
            }
            else
            {
                throw new ArgumentException("Player should be p1 or p2.");
            }
        }
    }
}