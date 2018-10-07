using GobangGameLib.GameBoard;
using System;
using System.Diagnostics;

namespace GobangBenchMark.Utilities
{
    public class NaiveMinmaxSearchAi
    {
        private readonly PieceType _player;
        private readonly int _maxDepth;
        private readonly NaiveCenterScorer _scorer;
        private int leafCount;

        public NaiveMinmaxSearchAi(PieceType player, int maxDepth)
        {
            this._player = player;
            this._maxDepth = maxDepth;
            this._scorer = new NaiveCenterScorer();
        }

        public Position MakeAMove(NaiveBoard board)
        {
            this.leafCount = 0;

            NaiveBoard boardCopy = board.DeepClone();
            Tuple<double, Position> scoreAndMove = MaxSearch(boardCopy, _player, 0);
            Debug.WriteLine($"{_player} best move {scoreAndMove.Item2}, score {scoreAndMove.Item1}, leaf count {this.leafCount}.");
            return scoreAndMove.Item2;
        }

        public Position MakeAMoveWithAction(NaiveBoard board)
        {
            this.leafCount = 0;

            NaiveBoard boardCopy = board.DeepClone();
            Tuple<double, Position> scoreAndMove = MaxSearchWithAction(boardCopy, _player, 0);
            Debug.WriteLine($"{_player} best move {scoreAndMove.Item2}, score {scoreAndMove.Item1}, leaf count {this.leafCount}.");
            return scoreAndMove.Item2;
        }

        private Tuple<double, Position> MaxSearch(NaiveBoard board, PieceType curPlayer, int depth)
        {
            if (depth >= _maxDepth)
            {
                this.leafCount++;

                // Return the score of current board.
                return new Tuple<double, Position>(_scorer.GetScore(board, _player), null);
            }

            PieceType otherPlayer = curPlayer.GetOther();
            double maxScore = double.NegativeInfinity;
            Position bestMove = null;
            for (int r = 0; r < board.RowSize; r++)
            {
                for (int c = 0; c < board.ColSize; c++)
                {
                    if (board.Data[r, c] == PieceType.Empty)
                    {
                        // Make a move.
                        board.Data[r, c] = curPlayer;
                        // Search deeper moves.
                        Tuple<double, Position> scoreAndMove = MinSearch(board, otherPlayer, depth + 1);
                        double score = scoreAndMove.Item1;
                        if (maxScore < score)
                        {
                            maxScore = score;
                            bestMove = new Position(r, c);
                        }
                        // Undo the move.
                        board.Data[r, c] = PieceType.Empty;
                    }
                }
            }

            return new Tuple<double, Position>(maxScore, bestMove);
        }

        private Tuple<double, Position> MinSearch(NaiveBoard board, PieceType curPlayer, int depth)
        {
            if (depth >= _maxDepth)
            {
                this.leafCount++;

                // Return the score of current board.
                return new Tuple<double, Position>(_scorer.GetScore(board, _player), null);
            }

            PieceType otherPlayer = curPlayer.GetOther();
            double minScore = double.PositiveInfinity;
            Position bestMove = null;
            for (int r = 0; r < board.RowSize; r++)
            {
                for (int c = 0; c < board.ColSize; c++)
                {
                    if (board.Data[r, c] == PieceType.Empty)
                    {
                        // Make a move.
                        board.Data[r, c] = curPlayer;
                        // Search deeper moves.
                        Tuple<double, Position> scoreAndMove = MaxSearch(board, otherPlayer, depth + 1);
                        double score = scoreAndMove.Item1;
                        if (minScore > score)
                        {
                            minScore = score;
                            bestMove = new Position(r, c);
                        }
                        // Undo the move.
                        board.Data[r, c] = PieceType.Empty;
                    }
                }
            }

            return new Tuple<double, Position>(minScore, bestMove);
        }
        
        private Tuple<double, Position> MaxSearchWithAction(NaiveBoard board, PieceType curPlayer, int depth)
        {
            if (depth >= _maxDepth)
            {
                this.leafCount++;

                // Return the score of current board.
                return new Tuple<double, Position>(_scorer.GetScoreWithAction(board, _player), null);
            }

            PieceType otherPlayer = curPlayer.GetOther();
            double maxScore = double.NegativeInfinity;
            Position bestMove = null;
            board.TraversalWithEmptyCheck((r, c) =>
            {
                // Make a move.
                board.Data[r, c] = curPlayer;
                // Search deeper moves.
                Tuple<double, Position> scoreAndMove = MinSearchWithAction(board, otherPlayer, depth + 1);
                double score = scoreAndMove.Item1;
                if (maxScore < score)
                {
                    maxScore = score;
                    bestMove = new Position(r, c);
                }
                // Undo the move.
                board.Data[r, c] = PieceType.Empty;
            });

            return new Tuple<double, Position>(maxScore, bestMove);
        }

        private Tuple<double, Position> MinSearchWithAction(NaiveBoard board, PieceType curPlayer, int depth)
        {
            if (depth >= _maxDepth)
            {
                this.leafCount++;

                // Return the score of current board.
                return new Tuple<double, Position>(_scorer.GetScoreWithAction(board, _player), null);
            }

            PieceType otherPlayer = curPlayer.GetOther();
            double minScore = double.PositiveInfinity;
            Position bestMove = null;
            board.TraversalWithEmptyCheck((r, c) =>
            {
                // Make a move.
                board.Data[r, c] = curPlayer;
                // Search deeper moves.
                Tuple<double, Position> scoreAndMove = MaxSearchWithAction(board, otherPlayer, depth + 1);
                double score = scoreAndMove.Item1;
                if (minScore > score)
                {
                    minScore = score;
                    bestMove = new Position(r, c);
                }
                // Undo the move.
                board.Data[r, c] = PieceType.Empty;
            });

            return new Tuple<double, Position>(minScore, bestMove);
        }
    }
}
