using System;
using System.Diagnostics;
using AI.Scorer;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.Players;

namespace AI
{
    public class MinmaxSearchAi : IPlayer
    {
        private PieceType _player;
        private readonly PositionManager _positions;
        private readonly int _maxDepth;
        private readonly IScorer _scorer;

        public MinmaxSearchAi(PieceType player, PositionManager positions, int maxDepth, IScorer scorer)
        {
            this._player = player;
            this._positions = positions;
            this._maxDepth = maxDepth;
            this._scorer = scorer;
        }

        public PieceType Player
        {
            set
            {
                _player = value;
            }
        }

        public Position MakeAMove(IBoard board)
        {
            IBoard boardCopy = board.DeepClone();
            Tuple<double, Position> scoreAndMove = MaxSearch(boardCopy, _player, 0);
            Debug.WriteLine($"{_player} best move {scoreAndMove.Item2}, score {scoreAndMove.Item1}.");
            return scoreAndMove.Item2;
        }

        private Tuple<double, Position> MaxSearch(IBoard board, PieceType curPlayer, int depth)
        {
            if (depth >= _maxDepth || board.IsFull())
            {
                // Return the score of current board.
                return new Tuple<double, Position>(_scorer.GetScore(board, _player), null);
            }

            PieceType otherPlayer = curPlayer.GetOther();
            double maxScore = double.NegativeInfinity;
            Position bestMove = null;
            foreach (Position move in _positions.GetEmptyPositions(board))
            {
                // Make a move.
                board.Set(move, curPlayer);
                // Search deeper moves.
                Tuple<double, Position> scoreAndMove = MinSearch(board, otherPlayer, depth + 1);
                double score = scoreAndMove.Item1;
                if (maxScore < score)
                {
                    maxScore = score;
                    bestMove = move;
                }
                // Undo the move.
                board.Set(move, PieceType.Empty);
            }
            return new Tuple<double, Position>(maxScore, bestMove);
        }

        private Tuple<double, Position> MinSearch(IBoard board, PieceType curPlayer, int depth)
        {
            if (depth >= _maxDepth || board.IsFull())
            {
                // Return the score of current board.
                return new Tuple<double, Position>(_scorer.GetScore(board, _player), null);
            }

            PieceType otherPlayer = curPlayer.GetOther();
            double minScore = double.PositiveInfinity;
            Position bestMove = null;
            foreach (Position move in _positions.GetEmptyPositions(board))
            {
                // Make a move.
                board.Set(move, curPlayer);
                // Search deeper moves.
                Tuple<double, Position> scoreAndMove = MaxSearch(board, otherPlayer, depth + 1);
                double score = scoreAndMove.Item1;
                if (minScore > score)
                {
                    minScore = score;
                    bestMove = move;
                }
                // Undo the move.
                board.Set(move, PieceType.Empty);
            }
            return new Tuple<double, Position>(minScore, bestMove);
        }
    }
}
