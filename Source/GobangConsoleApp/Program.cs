using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AI;
using AI.Scorer;
using GobangGameLib.Game;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PieceConnection;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.GameJudge;

namespace GobangConsoleApp
{
    public class Program
    {
        private static readonly Dictionary<PieceType, string> PieceToDisplayChar =
            new Dictionary<PieceType, string>() {
                { PieceType.Empty," " },
                { PieceType.P1,"X" },
                { PieceType.P2,"O" },
            };

        public static void Main(string[] args)
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);
            var patterns = new PatternFactory().Create();

            IGame game = new GameFactory().CreateGame(context,
                //new HumanPlayer(),
                //new RandomPlayer(),
                new AbPruningAi(PieceType.P1, positions, 2, new PatternScorer(positions, patterns)),
                new AbPruningAi(PieceType.P2, positions, 2, new PatternScorer(positions, patterns)),
                new PatternJudge(positions, patterns)
                );
            game.Start();
            var board = game.Board;
            DisplayBoard(board, context);

            do
            {
                game.Run();
                DisplayBoard(board, context);

                DebugInfo(positions, board);

                if (game.GameStatus == GameStatus.P1Win)
                {
                    Console.WriteLine("Winner is player 1.");
                }
                else if (game.GameStatus == GameStatus.P2Win)
                {
                    Console.WriteLine("Winner is player 2.");
                }
                else if (game.GameStatus == GameStatus.Tie)
                {
                    Console.WriteLine("Game ties.");
                }
                System.Threading.Thread.Sleep(500);
                //Console.ReadLine();
            } while (game.GameStatus == GameStatus.NotEnd);
            Console.ReadLine();
        }

        private static void DisplayBoard(IBoard board, BoardProperties context)
        {
            Console.Write("   ");
            for (var i = 0; i < context.ColSize; i++)
            {
                Console.Write(i);
            }
            Console.WriteLine();
            for (var i = 0; i < context.RowSize; i++)
            {
                Console.Write(string.Format("{0,2:00} ", i));
                for (var j = 0; j < context.ColSize; j++)
                {
                    Console.Write(PieceToDisplayChar[board.Get(new Position(i, j))]);
                }
                Console.WriteLine();
            }
        }


        private static void DebugInfo(PositionManager positions, IBoard board)
        {
            Detailed(positions, board, PatternType.Five);
            Detailed(positions, board, PatternType.OpenFour);
            Detailed(positions, board, PatternType.OpenThree);
            Detailed(positions, board, PatternType.OpenTwo);
        }

        private static void Detailed(PositionManager positions, IBoard board, PatternType patternType)
        {
            var matcher = new PatternMatcher();
            var patternRepository = new PatternFactory().Create();
            var patterns = patternRepository.Patterns[patternType].Patterns.Values.SelectMany(x => x);
            var five = positions
                .Lines
                .SelectMany(l => matcher.MatchPatterns(board, l, patterns));
            var pos = string.Join(",", five.Select(l => $"({l.Positions.First().Row},{l.Positions.First().Col})"));
            if (!string.IsNullOrWhiteSpace(pos)) Debug.WriteLine($"Pattern {patternType} at {pos}.");
        }
    }
}
