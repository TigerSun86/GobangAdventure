using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Running;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;

namespace GobangBenchMark
{
    [MemoryDiagnoser]
    [InliningDiagnoser]
    public class BoardBenchMark
    {
        private BoardProperties context;
        private PositionManager positions;
        private IBoard boardEmpty;
        private IBoard boardFull;
        private IBoard boardHalfFull;

        [Params(1, 100, 10000)]
        public int Count { get; set; }

        public void Run()
        {
            var summary = BenchmarkRunner.Run<BoardBenchMark>();
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
        public void GetEmptyByPositionManager()
        {
            int sum = 0;
            sum += GetEmptyByPositionManager(this.boardEmpty);
            sum += GetEmptyByPositionManager(this.boardFull);
            sum += GetEmptyByPositionManager(this.boardHalfFull);
        }

        public int GetEmptyByPositionManager(IBoard board)
        {
            int sum = 0;
            foreach (var i in Enumerable.Range(0, this.Count))
            {
                var ps = this.positions.GetEmptyPositions(board);
                foreach (var p in ps)
                {
                    sum++;
                }
            }

            return sum;
        }

        [Benchmark]
        public void GetEmptyByForLoop()
        {
            int sum = 0;
            sum += GetEmptyByForLoop(this.boardEmpty);
            sum += GetEmptyByForLoop(this.boardFull);
            sum += GetEmptyByForLoop(this.boardHalfFull);
        }

        public int GetEmptyByForLoop(IBoard board)
        {
            int sum = 0;
            foreach (var i in Enumerable.Range(0, this.Count))
            {
                var ps = NaiveGetEmpty(board);
                foreach (var p in ps)
                {
                    sum++;
                }
            }
            return sum;
        }


        [Benchmark]
        public void GetEmptyByForLoop2()
        {
            int sum = 0;
            var ps = NaiveGetEmpty2().ToList();
            sum += GetEmptyByForLoop2(this.boardEmpty, ps);
            sum += GetEmptyByForLoop2(this.boardFull, ps);
            sum += GetEmptyByForLoop2(this.boardHalfFull, ps);
        }

        public int GetEmptyByForLoop2(IBoard board, List<Position> ps)
        {
            int sum = 0;
            foreach (var i in Enumerable.Range(0, this.Count))
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


        private IEnumerable<Position> NaiveGetEmpty2()
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
