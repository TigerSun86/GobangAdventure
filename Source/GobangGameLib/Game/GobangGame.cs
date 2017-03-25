
using GobangGameLib.GameBoard;
using GobangGameLib.GameJudge;
using GobangGameLib.Players;
using System;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PositionManagement;
using System.Linq;
using GobangGameLib.GameBoard.PieceConnection;
using System.Diagnostics;

namespace GobangGameLib.Game
{
    public class GobangGame : IGame
    {
        private IPlayer _player1;
        private IPlayer _player2;
        private PieceType _curPiece;
        private IJudge _judge = new Judge();

        public GobangGame(IPlayer p1, IPlayer p2)
        {
            _player1 = p1;
            _player2 = p2;
        }

        public IBoard Board { get; private set; }
        public GameStatus GameStatus { get; private set; }

        public void Start() {
            _curPiece = PieceType.P1;

            Board = new Board();
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
            DebugInfo();

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

        private void DebugInfo()
        {
            Detailed(PatternType.Five);
            Detailed(PatternType.OpenFour);
            Detailed(PatternType.OpenThree);
            Detailed(PatternType.OpenTwo);
        }

        private void Detailed(PatternType patternType)
        {
            var matcher = new PatternMatcher();
            var patterns = PatternManager.Instance().PatternRepo[patternType].Patterns.Values.SelectMany(x => x);
            var five = PositionManager.Instance()
                .Lines
                .SelectMany(l => matcher.MatchPatterns(Board, l, patterns));
            var pos = string.Join(",", five.Select(l => $"({l.Positions.First().Row},{l.Positions.First().Col})"));
            if (!string.IsNullOrWhiteSpace(pos)) Debug.WriteLine($"Pattern {patternType} at {pos}.");
        }
    }
}