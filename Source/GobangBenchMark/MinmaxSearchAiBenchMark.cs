using AI;
using AI.Scorer;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;
using GoBangGameLibTest.Common;
using System.Collections.Generic;

namespace GobangBenchMark
{
    [MemoryDiagnoser]
    public class MinmaxSearchAiBenchMark
    {
        private List<IBoard> boards;
        private MinmaxSearchAi p1Ai;
        private MinmaxSearchAi p2Ai;

        [Params(1, 2)]
        public int SearchDepth { get; set; }

        public void Run()
        {
            var summary = BenchmarkRunner.Run<MinmaxSearchAiBenchMark>();
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
            foreach (var boardString in boardStrings)
            {
                this.boards.Add(Utils.ParseBoard(boardString, context, positions));
            }

            var centerScorer = new CenterScorer(context, positions);

            this.p1Ai = new MinmaxSearchAi(PieceType.P1, positions, SearchDepth, centerScorer);
            this.p2Ai = new MinmaxSearchAi(PieceType.P2, positions, SearchDepth, centerScorer);
        }

        [Benchmark]
        public int MinmaxSearchAi()
        {
            // To avoid dead code elimination
            int sum = 0;

            foreach (var board in boards)
            {
                var position1 = this.p1Ai.MakeAMove(board);
                board.Set(position1, PieceType.P1);
                var position2 = this.p2Ai.MakeAMove(board);

                // Cleanup.
                board.Set(position1, PieceType.Empty);

                sum += position1.Row + position1.Col + position2.Row + position2.Col;
            }

            return sum;
        }
    }
}
