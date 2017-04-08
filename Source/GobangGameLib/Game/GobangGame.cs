
using System;
using System.Diagnostics;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.GameJudge;
using GobangGameLib.Players;

namespace GobangGameLib.Game
{
    public class GobangGame : IGame
    {
        private readonly IBoardFactory boardFactory;
        private readonly IPlayer player1;
        private readonly IPlayer player2;
        private readonly IJudge judge;

        public GobangGame(IBoardFactory boardFactory, IPlayer p1, IPlayer p2, IJudge judge)
        {
            this.boardFactory = boardFactory;
            this.player1 = p1;
            this.player2 = p2;
            this.judge = judge;
        }

        public IBoard Board { get; private set; }

        public PieceType CurPiece { get; private set; }

        public GameStatus GameStatus { get; private set; }

        public void Start()
        {
            this.Board = this.boardFactory.Create();
            this.CurPiece = PieceType.P1;
            this.GameStatus = GameStatus.NotEnd;
        }

        public void Run()
        {
            if (this.GameStatus != GameStatus.NotEnd)
            {
                throw new InvalidOperationException("Failed to run the game after it's over.");
            }

            IPlayer curPlayer = this.GetPlayer(CurPiece);
            Position move = curPlayer.MakeAMove(Board);
            this.Board.Set(move, this.CurPiece);

            Debug.WriteLine($"{CurPiece} moved at ({move.Row},{move.Col}).");

            this.CurPiece = this.CurPiece.GetOther();

            var winner = this.judge.GetWinner(Board);
            if (winner == PieceType.P1)
            {
                this.GameStatus = GameStatus.P1Win;
            }
            else if (winner == PieceType.P2)
            {
                this.GameStatus = GameStatus.P2Win;
            }
            else if (Board.IsFull())
            {
                this.GameStatus = GameStatus.Tie;
            }
        }

        private IPlayer GetPlayer(PieceType p)
        {
            if (p == PieceType.P1)
            {
                return player1;
            }

            if (p == PieceType.P2)
            {
                return player2;
            }

            throw new ArgumentException($"Unsupported player: {p.ToString()}.");
        }
    }
}