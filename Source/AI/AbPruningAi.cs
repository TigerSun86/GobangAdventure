using System;
using System.Diagnostics;
using AI.Scorer;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.GameJudge;
using GobangGameLib.Players;

namespace AI
{
    public class AbPruningAi : IPlayer
    {
        private readonly PieceType player;
        private readonly PositionManager positions;
        private readonly int maxDepth;
        private readonly IScorer scorer;
        private readonly IBoardFactory boardFactory;
        private readonly IJudge judge;

        private int leafCount;

        public AbPruningAi(PieceType player, PositionManager positions, int maxDepth, IScorer scorer, IBoardFactory boardFactory, IJudge judge)
        {
            this.boardFactory = boardFactory;
            this.player = player;
            this.positions = positions;
            this.maxDepth = maxDepth;
            this.scorer = scorer;
            this.judge = judge;
        }

        public Position MakeAMove(IBoard board)
        {
            this.leafCount = 0;

            IBoard boardCopy = this.boardFactory.DeepCloneBoard(board);

            Tuple<double, Position> scoreAndMove = MaxSearch(boardCopy, player, depth: 0,
                minPossibleScore: double.NegativeInfinity, maxPossibleScore: double.PositiveInfinity);

            Debug.WriteLine($"{player} best move {scoreAndMove.Item2}, score {scoreAndMove.Item1}, leaf count {this.leafCount}.");
            return scoreAndMove.Item2;
        }

        private Tuple<double, Position> MaxSearch(IBoard board, PieceType curPlayer, int depth, double minPossibleScore, double maxPossibleScore)
        {
            if (depth >= maxDepth || board.IsFull() || HasWinner(board))
            {
                this.leafCount++;

                // Return the score of current board.
                return new Tuple<double, Position>(scorer.GetScore(board, player), null);
            }

            PieceType otherPlayer = curPlayer.GetOther();
            Position bestMove = null;
            foreach (Position move in positions.GetEmptyPositions(board))
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
            if (depth >= maxDepth || board.IsFull() || HasWinner(board))
            {
                this.leafCount++;

                // Return the score of current board.
                return new Tuple<double, Position>(scorer.GetScore(board, player), null);
            }

            PieceType otherPlayer = curPlayer.GetOther();
            Position bestMove = null;
            foreach (Position move in positions.GetEmptyPositions(board))
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

        private bool HasWinner(IBoard board)
        {
            return PieceType.Empty != this.judge.GetWinner(board);
        }
    }
}
