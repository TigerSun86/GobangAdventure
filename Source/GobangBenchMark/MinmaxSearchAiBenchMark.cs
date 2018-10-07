using AI;
using AI.Scorer;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using GobangBenchMark.Utilities;
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
        private List<NaiveBoard> naiveBoards;
        private MinmaxSearchAi p1Ai;
        private MinmaxSearchAi p2Ai;
        private NaiveMinmaxSearchAi p1NaiveAi;
        private NaiveMinmaxSearchAi p2NaiveAi;

        [Params(1)]
        public int SearchDepth { get; set; }

        [Params(1)]
        public int NumOfMovesToMake { get; set; }

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

            var centerScorer = new CenterScorer(context, positions);

            this.p1Ai = new MinmaxSearchAi(PieceType.P1, positions, SearchDepth, centerScorer);
            this.p2Ai = new MinmaxSearchAi(PieceType.P2, positions, SearchDepth, centerScorer);

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
                    var p = this.p1Ai.MakeAMove(boardCopy);
                    boardCopy.Set(p, PieceType.P1);
                    sum += p.Row + p.Col;

                    p = this.p1Ai.MakeAMove(boardCopy);
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
                    boardCopy.Data[p.Row, p.Col] =  PieceType.P2;
                    sum += p.Row + p.Col ;
                }
            }

            return sum;
        }
    }
}
