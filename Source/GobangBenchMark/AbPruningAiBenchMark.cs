using AI;
using AI.Moves;
using AI.Scorer;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using GobangBenchMark.Utilities;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.GameJudge;
using GoBangGameLibTest.Common;
using System;
using System.Collections.Generic;

namespace GobangBenchMark
{
    [MemoryDiagnoser]
    public class AbPruningAiBenchMark
    {
        private List<IBoard> boards;
        private List<NaiveBoard> naiveBoards;
        private MinmaxSearchAi p1MinmaxSearchAi;
        private MinmaxSearchAi p2MinmaxSearchAi;
        private AbPruningAi p1AbPruningAi;
        private AbPruningAi p2AbPruningAi;
        private NaiveMinmaxSearchAi p1NaiveAi;
        private NaiveMinmaxSearchAi p2NaiveAi;

        [Params(1,2,3)]
        public int SearchDepth { get; set; }

        [Params(1)]
        public int NumOfMovesToMake { get; set; }

        public void Run()
        {
            var summary = BenchmarkRunner.Run<AbPruningAiBenchMark>();
        }

        [Setup]
        public void Setup()
        {
            var boardStrings = new List<string[]>
            {
                new[]
                {
                    // 2345678
                    "         ", // 0
                    "         ", // 1
                    "         ", // 2
                    "   XO    ", // 3
                    "   XXO   ", // 4
                    "     OO  ", // 5
                    "     X   ", // 6
                    "         ", // 7
                    "         ", // 8
                },
                new[]
                {
                    // 2345678
                    "         ", // 0
                    "         ", // 1
                    "         ", // 2
                    "   XO    ", // 3
                    "   XXO   ", // 4
                    "    O O  ", // 5
                    "     X   ", // 6
                    "         ", // 7
                    "         ", // 8
                }
            };

            var context = new BoardProperties(boardStrings[0].Length, boardStrings[0][0].Length);
            var positions = new PositionFactory().Create(context);
            this.boards = new List<IBoard>();
            this.naiveBoards = new List<NaiveBoard>();
            foreach (var boardString in boardStrings)
            {
                var board = Utils.ParseBoard(boardString, context, positions);
                this.boards.Add(board);

                var naiveBoard = new NaiveBoard(context.RowSize, context.ColSize);
                foreach (var p in positions.GetPlayerPositions(board))
                {
                    naiveBoard.Data[p.Row, p.Col] = board.Get(p);
                }

                this.naiveBoards.Add(naiveBoard);
            }

            var patterns = new PatternFactory().Create();
            var matcher = new PatternMatcher(patterns);
            var patternBoardFactory = new PatternBoardFactory(context, positions, matcher);
            var patternScorer = new PatternScorer(positions, patterns, matcher);
            var centerScorer = new CenterScorer(context, positions);
            var aggregatedScorer = new AggregatedScorer(new[]
            {
                new Tuple<IScorer, double>(patternScorer, 1),
                new Tuple<IScorer, double>(centerScorer, 0.01)
            });
            var judge = new BasicJudge(context, positions);
            var emptyMoveEnumerator = new EmptyPositionMoveEnumerator(positions);
            var scoredMoveEnumerator = new ScoredMoveEnumerator(positions, aggregatedScorer);

            this.p1AbPruningAi = new AbPruningAi(PieceType.P1, SearchDepth, aggregatedScorer, scoredMoveEnumerator, patternBoardFactory, judge);
            this.p2AbPruningAi = new AbPruningAi(PieceType.P2, SearchDepth, aggregatedScorer, scoredMoveEnumerator, patternBoardFactory, judge);

            this.p1MinmaxSearchAi = new MinmaxSearchAi(PieceType.P1, positions, SearchDepth, aggregatedScorer);
            this.p2MinmaxSearchAi = new MinmaxSearchAi(PieceType.P2, positions, SearchDepth, aggregatedScorer);

            this.p1NaiveAi = new NaiveMinmaxSearchAi(PieceType.P1, SearchDepth);
            this.p2NaiveAi = new NaiveMinmaxSearchAi(PieceType.P2, SearchDepth);
        }

        [Benchmark]
        public int MinmaxSearchAi()
        {
            // To avoid dead code elimination
            int sum = 0;

            foreach (var board in boards)
            {
                var boardCopy = board.DeepClone();
                for (int i = 0; i < NumOfMovesToMake; i++)
                {
                    var p = this.p1MinmaxSearchAi.MakeAMove(boardCopy);
                    boardCopy.Set(p, PieceType.P1);
                    sum += p.Row + p.Col;

                    p = this.p1MinmaxSearchAi.MakeAMove(boardCopy);
                    boardCopy.Set(p, PieceType.P2);
                    sum += p.Row + p.Col;
                }
            }

            return sum;
        }

        [Benchmark]
        public int AbPruningAi()
        {
            // To avoid dead code elimination
            int sum = 0;

            foreach (var board in boards)
            {
                var boardCopy = board.DeepClone();
                for (int i = 0; i < NumOfMovesToMake; i++)
                {
                    var p = this.p1AbPruningAi.MakeAMove(boardCopy);
                    boardCopy.Set(p, PieceType.P1);
                    sum += p.Row + p.Col;

                    p = this.p2AbPruningAi.MakeAMove(boardCopy);
                    boardCopy.Set(p, PieceType.P2);
                    sum += p.Row + p.Col;
                }
            }

            return sum;
        }

        [Benchmark]
        public int NaiveMinmaxSearchAi()
        {
            // To avoid dead code elimination
            int sum = 0;

            foreach (var board in naiveBoards)
            {
                var boardCopy = board.DeepClone();
                for (int i = 0; i < NumOfMovesToMake; i++)
                {
                    var p = this.p1NaiveAi.MakeAMove(boardCopy);
                    boardCopy.Data[p.Row, p.Col] = PieceType.P1;
                    sum += p.Row + p.Col;

                    p = this.p2NaiveAi.MakeAMove(boardCopy);
                    boardCopy.Data[p.Row, p.Col] = PieceType.P2;
                    sum += p.Row + p.Col;
                }
            }

            return sum;
        }

        [Benchmark]
        public int NaiveMinmaxSearchAiWithAction()
        {
            // To avoid dead code elimination
            int sum = 0;

            foreach (var board in naiveBoards)
            {
                var boardCopy = board.DeepClone();
                for (int i = 0; i < NumOfMovesToMake; i++)
                {
                    var p = this.p1NaiveAi.MakeAMoveWithAction(boardCopy);
                    boardCopy.Data[p.Row, p.Col] = PieceType.P1;
                    sum += p.Row + p.Col;

                    p = this.p2NaiveAi.MakeAMoveWithAction(boardCopy);
                    boardCopy.Data[p.Row, p.Col] = PieceType.P2;
                    sum += p.Row + p.Col;
                }
            }

            return sum;
        }
    }
}
