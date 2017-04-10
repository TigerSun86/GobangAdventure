using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AI;
using AI.Moves;
using AI.Scorer;
using GobangGameLib.Game;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.GameJudge;
using GobangGameLib.Players;

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
            var matcher = new PatternMatcher();
            var boardFactory = new BoardFactory(context, positions);
            var patternBoardFactory = new PatternBoardFactory(context, positions, patterns, matcher);
            var patternScorer = new PatternScorer(positions, patterns, matcher);
            var judge = new PatternJudge(positions, patterns, matcher);
            var emptyMoveEnumerator = new EmptyPositionMoveEnumerator(positions);
            var scoredMoveEnumerator = new ScoredMoveEnumerator(positions, patternScorer);

            IGame game = new GameFactory().CreateGame(boardFactory,
                //new HumanPlayer(),
                //new RandomPlayer(positions),
                //new RandomPlayer(positions),
                new AbPruningAi(PieceType.P1, 3, patternScorer, scoredMoveEnumerator, patternBoardFactory, judge),
                new AbPruningAi(PieceType.P2, 3, patternScorer, scoredMoveEnumerator, patternBoardFactory, judge),
                judge
                );

            game.Start();
            var board = game.Board;
            DisplayBoard(board, context);

            do
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                game.Run();

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = $"{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
                Console.WriteLine($"Elapsed time {elapsedTime}.");
                Debug.WriteLine($"Elapsed time {elapsedTime}.");

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
                //System.Threading.Thread.Sleep(500);
                //Console.ReadLine();
            } while (game.GameStatus == GameStatus.NotEnd);
            Console.ReadLine();
        }

        private static void DisplayBoard(IBoard board, BoardProperties context)
        {
            Console.Write("   ");
            for (var i = 0; i < context.ColSize; i++)
            {
                Console.Write(string.Format(" {0}", i));
            }
            Console.WriteLine();
            for (var i = 0; i < context.RowSize; i++)
            {
                Console.Write(string.Format("{0,2:00} ", i));
                for (var j = 0; j < context.ColSize; j++)
                {
                    Console.Write(" ");
                    Console.Write(PieceToDisplayChar[board.Get(new Position(i, j))]);
                }
                Console.WriteLine();
            }
        }


        private static void DebugInfo(PositionManager positions, IBoard board)
        {
            Detailed(positions, board);
        }

        private static void Detailed(PositionManager positions, IBoard board)
        {
            var matches = GetMatches1(positions, board).ToList();
            PatternBoard pBoard = board as PatternBoard;
            if (pBoard != null)
            {
                var matches2 = GetMatches2(pBoard).ToList();
                var any = matches.Except(matches2).ToList();
                bool same = (matches.Count() == matches2.Count()) && !any.Any();
                Debug.Assert(same);
            }

            var m2 = matches.GroupBy(m => m.Pattern.PatternType, m => m);
            foreach (var m in m2)
            {
                var pos = string.Join(",", m.Select(l => $"({l.Positions.First().Row},{l.Positions.First().Col})"));
                if (!string.IsNullOrWhiteSpace(pos)) Debug.WriteLine($"Pattern {m.Key} at {pos}.");
            }
        }


        private static IEnumerable<IMatch> GetMatches1(PositionManager positions, IBoard board)
        {
            var matcher = new PatternMatcher();
            var patternRepository = new PatternFactory().Create();

            IEnumerable<IPattern> patterns = patternRepository.Get();

            var matches = matcher.MatchPatterns(board, positions.Lines, patterns);

            return matches;
        }

        private static IEnumerable<IMatch> GetMatches2(PatternBoard board)
        {
            return board.Matches.Get();
        }
    }
}
