using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Running;
using GobangBenchMark.Utilities;
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
        public enum BoardType
        {
            Empty,
            HalfFull,
            Full
        }

        private BoardProperties context;
        private PositionManager positions;
        private IBoard boardEmpty;
        private IBoard boardFull;
        private IBoard boardHalfFull;
        private NaiveBoard naiveBoardEmpty;
        private NaiveBoard naiveBoardFull;
        private NaiveBoard naiveBoardHalfFull;

        /// <summary>
        /// 10, 100, 10000 shows similar ratio of result in Mean. But 1 result is not similar with them.
        /// </summary>
        [Params(10)]
        public int IterationCount { get; set; }

        [Params(BoardType.HalfFull)]
        public BoardType BoardTypePara { get; set; }

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
            this.naiveBoardEmpty = new NaiveBoard(this.context.RowSize, this.context.ColSize);

            this.boardFull = new BoardFactory(context, positions).Create();
            this.naiveBoardFull = new NaiveBoard(this.context.RowSize, this.context.ColSize);

            var piece = PieceType.P1;
            foreach (var p in this.positions.Positions)
            {
                this.boardFull.Set(p, piece);
                this.naiveBoardFull.Data[p.Row, p.Col] = piece;

                piece = piece.GetOther();
            }

            this.boardHalfFull = new BoardFactory(context, positions).Create();
            this.naiveBoardHalfFull = new NaiveBoard(this.context.RowSize, this.context.ColSize);

            var ran = new Random(1);
            foreach (int i in Enumerable.Range(0, this.positions.Positions.Count() / 2))
            {
                var emptyPositions = this.positions.GetEmptyPositions(this.boardHalfFull);
                var p = emptyPositions.ElementAt(ran.Next(0, emptyPositions.Count()));
                this.boardHalfFull.Set(p, piece);
                this.naiveBoardHalfFull.Data[p.Row, p.Col] = piece;

                piece = piece.GetOther();
            }
        }

        [Benchmark]
        public int GetEmptyByPositionManager()
        {
            int sum = 0;

            sum += GetEmptyByPositionManager(BoardTypeToIBoard());

            return sum;
        }

        [Benchmark]
        public int GetEmptyByForLoop()
        {
            int sum = 0;
            sum += GetEmptyByForLoop(BoardTypeToIBoard());

            return sum;
        }


        [Benchmark]
        public int GetEmptyByForLoopWithPreGeneratedPositions()
        {
            int sum = 0;
            var ps = NaiveGetAllPositions().ToList();

            sum += GetEmptyByForLoopWithPreGeneratedPositions(BoardTypeToIBoard(), ps);

            return sum;
        }

        [Benchmark]
        public int GetEmptyByNaiveBoard()
        {
            int sum = 0;
            sum += GetEmptyByNaiveBoard(BoardTypeToNaiveBoard());

            return sum;
        }

        [Benchmark]
        public int GetEmptyByNaiveBoard2WithLinq()
        {
            int sum = 0;
            sum += GetEmptyByNaiveBoard2WithLinq(BoardTypeToNaiveBoard());

            return sum;
        }

        [Benchmark]
        public int GetEmptyPositionsWithCompressedPosition()
        {
            int sum = 0;
            sum += GetEmptyPositionsWithCompressedPosition(BoardTypeToNaiveBoard());

            return sum;
        }

        [Benchmark]
        public int GetEmptyByTraversal()
        {
            int sum = 0;
            sum += GetEmptyByTraversal(BoardTypeToNaiveBoard());

            return sum;
        }


        [Benchmark]
        public int GetEmptyByTraversalWithEmptyCheck()
        {
            int sum = 0;
            sum += GetEmptyByTraversalWithEmptyCheck(BoardTypeToNaiveBoard());

            return sum;
        }

        private int GetEmptyByTraversal(NaiveBoard board)
        {
            int sum = 0;

            foreach (var i in Enumerable.Range(0, this.IterationCount))
            {
                Traversal(board, (r, c) =>
                {
                    if (board.Data[r, c] == PieceType.Empty)
                    {
                        sum += r + c;
                    }
                });
            }
            return sum;
        }

        private int GetEmptyByTraversalWithEmptyCheck(NaiveBoard board)
        {
            int sum = 0;

            foreach (var i in Enumerable.Range(0, this.IterationCount))
            {
                TraversalWithEmptyCheck(board, (r, c) =>
                {
                    sum += r + c;
                });
            }

            return sum;
        }

        private void Traversal(NaiveBoard b, Action<int, int> action)
        {
            for (int r = 0; r < b.RowSize; r++)
            {
                for (int c = 0; c < b.ColSize; c++)
                {
                    if (b.Data[r, c] == PieceType.Empty)
                    {
                        action(r, c);
                    }
                }
            }
        }

        private void TraversalWithEmptyCheck(NaiveBoard b, Action<int, int> action)
        {
            for (int r = 0; r < b.RowSize; r++)
            {
                for (int c = 0; c < b.ColSize; c++)
                {
                    if (b.Data[r, c] == PieceType.Empty)
                    {
                        action(r, c);
                    }
                }
            }
        }


        [Benchmark]
        public int GetEmptyByTraversal2()
        {
            int sum = 0;
            sum += GetEmptyByTraversal2(BoardTypeToNaiveBoard());

            return sum;
        }


        [Benchmark]
        public int GetEmptyByTraversalWithEmptyCheck2()
        {
            int sum = 0;
            sum += GetEmptyByTraversalWithEmptyCheck2(BoardTypeToNaiveBoard());

            return sum;
        }

        private int GetEmptyByTraversal2(NaiveBoard board)
        {
            int sum = 0;

            foreach (var i in Enumerable.Range(0, this.IterationCount))
            {
                board.Traversal((r, c) =>
                {
                    if (board.Data[r, c] == PieceType.Empty)
                    {
                        sum += r + c;
                    }
                });
            }
            return sum;
        }

        private int GetEmptyByTraversalWithEmptyCheck2(NaiveBoard board)
        {
            int sum = 0;

            foreach (var i in Enumerable.Range(0, this.IterationCount))
            {
                board.TraversalWithEmptyCheck((r, c) =>
                {
                    sum += r + c;
                });
            }

            return sum;
        }

        private int GetEmptyByPositionManager(IBoard board)
        {
            int sum = 0;
            foreach (var i in Enumerable.Range(0, this.IterationCount))
            {
                var ps = this.positions.GetEmptyPositions(board);
                foreach (var p in ps)
                {
                    sum += p.Row + p.Col;
                }
            }

            return sum;
        }

        private int GetEmptyByForLoop(IBoard board)
        {
            int sum = 0;
            foreach (var i in Enumerable.Range(0, this.IterationCount))
            {
                var ps = NaiveGetEmpty(board);
                foreach (var p in ps)
                {
                    sum += p.Row + p.Col;
                }
            }
            return sum;
        }

        private int GetEmptyByForLoopWithPreGeneratedPositions(IBoard board, List<Position> ps)
        {
            int sum = 0;
            foreach (var i in Enumerable.Range(0, this.IterationCount))
            {
                foreach (var p in ps)
                {
                    if (board.Get(p) == PieceType.Empty)
                    {
                        sum += p.Row + p.Col;
                    }
                }
            }
            return sum;
        }

        private int GetEmptyByNaiveBoard(NaiveBoard naiveBoardEmpty)
        {
            int sum = 0;
            foreach (var i in Enumerable.Range(0, this.IterationCount))
            {
                for (int r = 0; r < this.context.RowSize; r++)
                {
                    for (int c = 0; c < this.context.ColSize; c++)
                    {
                        if (naiveBoardEmpty.Data[r, c] == PieceType.Empty)
                        {
                            sum += r + c;
                        }
                    }
                }
            }
            return sum;
        }

        private int GetEmptyByNaiveBoard2WithLinq(NaiveBoard naiveBoardEmpty)
        {
            int sum = 0;
            foreach (var i in Enumerable.Range(0, this.IterationCount))
            {
                sum += GetEmptyPositionsWithLinqInternal(naiveBoardEmpty).Sum(p => p.Row + p.Col);
            }

            return sum;
        }

        public IEnumerable<Position> GetEmptyPositionsWithLinqInternal(NaiveBoard naiveBoardEmpty)
        {
            for (int r = 0; r < naiveBoardEmpty.RowSize; r++)
            {
                for (int c = 0; c < naiveBoardEmpty.ColSize; c++)
                {
                    if (naiveBoardEmpty.Data[r, c] == PieceType.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private int GetEmptyPositionsWithCompressedPosition(NaiveBoard naiveBoardEmpty)
        {
            int sum = 0;
            foreach (var i in Enumerable.Range(0, this.IterationCount))
            {
                sum += GetEmptyPositionsWithCompressedPositionInternal(naiveBoardEmpty).Sum(p => (p >> 8) + (p & 0xFF));
            }

            return sum;
        }

        private IEnumerable<int> GetEmptyPositionsWithCompressedPositionInternal(NaiveBoard naiveBoardEmpty)
        {
            for (int r = 0; r < naiveBoardEmpty.RowSize; r++)
            {
                for (int c = 0; c < naiveBoardEmpty.ColSize; c++)
                {
                    if (naiveBoardEmpty.Data[r, c] == PieceType.Empty)
                    {
                        yield return (r << 8) + c;
                    }
                }
            }
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

        private IBoard BoardTypeToIBoard()
        {
            switch (this.BoardTypePara)
            {
                case BoardType.Empty:
                    return this.boardEmpty;
                case BoardType.HalfFull:
                    return this.boardHalfFull;
                default:
                    return this.boardFull;
            }
        }

        private NaiveBoard BoardTypeToNaiveBoard()
        {
            switch (this.BoardTypePara)
            {
                case BoardType.Empty:
                    return this.naiveBoardEmpty;
                case BoardType.HalfFull:
                    return this.naiveBoardHalfFull;
                default:
                    return this.naiveBoardFull;
            }
        }
    }
}
