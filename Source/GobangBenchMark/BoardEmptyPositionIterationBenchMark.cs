using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Running;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GobangBenchMark
{
    [MemoryDiagnoser]
    public class BoardEmptyPositionIterationBenchMark
    {
        private BoardProperties context;
        private PositionManager positions;
        private IBoard boardEmpty;
        private IBoard boardFull;
        private IBoard boardHalfFull;

        /// <summary>
        /// 10, 100, 10000 shows similar ratio of result in Mean. But 1 result is not similar with them.
        /// </summary>
        [Params(10)]
        public int PositionCountToCheck { get; set; }

        public void Run()
        {
            var summary = BenchmarkRunner.Run<BoardEmptyPositionIterationBenchMark>();
        }

        [Setup]
        public void Setup()
        {
            this.context = new BoardProperties();
            this.positions = new PositionFactory().Create(context);
            this.boardEmpty = new BoardFactory(context, positions).Create();
            this.boardFull = new BoardFactory(context, positions).Create();
            var piece = PieceType.P1;
            foreach (var p in this.positions.Positions)
            {
                this.boardFull.Set(p, piece);
                piece = piece.GetOther();
            }

            this.boardHalfFull = new BoardFactory(context, positions).Create();
            var ran = new Random(1);
            foreach (int i in Enumerable.Range(0, this.positions.Positions.Count() / 2))
            {
                var emptyPositions = this.positions.GetEmptyPositions(this.boardHalfFull);
                var p = emptyPositions.ElementAt(ran.Next(0, emptyPositions.Count()));
                this.boardHalfFull.Set(p, piece);
                piece = piece.GetOther();
            }
        }

        [Benchmark]
        public int GetEmptyByPositionManager()
        {
            int sum = 0;
            sum += GetEmptyByPositionManager(this.boardEmpty);
            sum += GetEmptyByPositionManager(this.boardFull);
            sum += GetEmptyByPositionManager(this.boardHalfFull);

            return sum;
        }

        [Benchmark]
        public int GetEmptyByForLoop()
        {
            int sum = 0;
            sum += GetEmptyByForLoop(this.boardEmpty);
            sum += GetEmptyByForLoop(this.boardFull);
            sum += GetEmptyByForLoop(this.boardHalfFull);

            return sum;
        }

        [Benchmark]
        public int GetEmptyByForLoopWithPreGeneratedPositions()
        {
            int sum = 0;
            var ps = NaiveGetAllPositions().ToList();
            sum += GetEmptyByForLoopWithPreGeneratedPositions(this.boardEmpty, ps);
            sum += GetEmptyByForLoopWithPreGeneratedPositions(this.boardFull, ps);
            sum += GetEmptyByForLoopWithPreGeneratedPositions(this.boardHalfFull, ps);

            return sum;
        }

        public int GetEmptyByPositionManager(IBoard board)
        {
            int sum = 0;
            foreach (var i in Enumerable.Range(0, this.PositionCountToCheck))
            {
                var ps = this.positions.GetEmptyPositions(board);
                foreach (var p in ps)
                {
                    sum++;
                }
            }

            return sum;
        }

        public int GetEmptyByForLoop(IBoard board)
        {
            int sum = 0;
            foreach (var i in Enumerable.Range(0, this.PositionCountToCheck))
            {
                var ps = NaiveGetEmpty(board);
                foreach (var p in ps)
                {
                    sum++;
                }
            }
            return sum;
        }

        public int GetEmptyByForLoopWithPreGeneratedPositions(IBoard board, List<Position> ps)
        {
            int sum = 0;
            foreach (var i in Enumerable.Range(0, this.PositionCountToCheck))
            {
                foreach (var p in ps)
                {
                    if (board.Get(p) == PieceType.Empty)
                    {
                        sum++;
                    }
                }
            }
            return sum;
        }

        private IEnumerable<Position> NaiveGetEmpty(IBoard board)
        {
            for (int r = 0; r < this.context.RowSize; r++)
            {
                for (int c = 0; c < this.context.ColSize; c++)
                {
                    Position p = new Position(r, c);
                    if (board.Get(p) == PieceType.Empty)
                    {
                        yield return p;
                    }
                }
            }
        }

        private IEnumerable<Position> NaiveGetAllPositions()
        {
            for (int r = 0; r < this.context.RowSize; r++)
            {
                for (int c = 0; c < this.context.ColSize; c++)
                {
                    Position p = new Position(r, c);
                    yield return p;
                }
            }
        }
    }
}
