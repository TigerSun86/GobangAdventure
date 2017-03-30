using System;
using System.Diagnostics;
using AI.Scorer;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.Players;

namespace AI
{
    public class AbPruningAi : IPlayer
    {
        private PieceType _player;
        private readonly PositionManager _positions;
        private readonly int _maxDepth;
        private readonly IScorer _scorer;

        public AbPruningAi(PieceType player, PositionManager positions, int maxDepth, IScorer scorer)
        {
            this._player = player;
            this._positions = positions;
            this._maxDepth = maxDepth;
            this._scorer = scorer;
        }

        public Position MakeAMove(IBoard board)
        {
            IBoard boardCopy = board.DeepClone();
            Tuple<double, Position> scoreAndMove = MaxSearch(boardCopy, _player, depth: 0,
                minPossibleScore: double.NegativeInfinity, maxPossibleScore: double.PositiveInfinity);
            Debug.WriteLine($"{_player} best move {scoreAndMove.Item2}, score {scoreAndMove.Item1}.");
            return scoreAndMove.Item2;
        }

        private Tuple<double, Position> MaxSearch(IBoard board, PieceType curPlayer, int depth, double minPossibleScore, double maxPossibleScore)
        {
            if (depth >= _maxDepth || board.IsFull())
            {
                // Return the score of current board.
                return new Tuple<double, Position>(_scorer.GetScore(board, _player), null);
            }

            PieceType otherPlayer = curPlayer.GetOther();
            Position bestMove = null;
            foreach (Position move in _positions.GetEmptyPositions(board))
            {
                // Make a move.
                board.Set(move, curPlayer);
                // Search deeper moves.
                Tuple<double, Position> scoreAndMove = MinSearch(board, otherPlayer, depth + 1, minPossibleScore, maxPossibleScore);
                // Undo the move.
                board.Set(move, PieceType.Empty);

                double score = scoreAndMove.Item1;
                if (score >= maxPossibleScore)
                {
                    // Result is better than max possible score, so this whole max search branch is meaningless.
                    // Just stop searching and return meaningless result.
                    return scoreAndMove;
                }
                if (score > minPossibleScore)
                {
                    minPossibleScore = score;
                    bestMove = move;
                }

                // else (score <= minPossibleScore) ignore.
            }

            return new Tuple<double, Position>(minPossibleScore, bestMove);
        }

        private Tuple<double, Position> MinSearch(IBoard board, PieceType curPlayer, int depth, double minPossibleScore, double maxPossibleScore)
        {
            if (depth >= _maxDepth || board.IsFull())
            {
                // Return the score of current board.
                return new Tuple<double, Position>(_scorer.GetScore(board, _player), null);
            }

            PieceType otherPlayer = curPlayer.GetOther();
            Position bestMove = null;
            foreach (Position move in _positions.GetEmptyPositions(board))
            {
                // Make a move.
                board.Set(move, curPlayer);
                // Search deeper moves.
                Tuple<double, Position> scoreAndMove = MaxSearch(board, otherPlayer, depth + 1, minPossibleScore, maxPossibleScore);
                // Undo the move.
                board.Set(move, PieceType.Empty);

                double score = scoreAndMove.Item1;
                if (score <= minPossibleScore)
                {
                    // Result is worse than min possible score, so this whole min search branch is meaningless.
                    // Just stop searching and return meaningless result.
                    return scoreAndMove;
                }
                if (score < maxPossibleScore)
                {
                    maxPossibleScore = score;
                    bestMove = move;
                }

                // else (score > maxPossibleScore) ignore.
            }

            return new Tuple<double, Position>(maxPossibleScore, bestMove);
        }
    }
}
