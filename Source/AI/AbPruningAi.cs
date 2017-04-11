using System.Diagnostics;
using AI.Moves;
using AI.Scorer;
using GobangGameLib.GameBoard;
using GobangGameLib.GameJudge;
using GobangGameLib.Players;

namespace AI
{
    public class AbPruningAi : IPlayer
    {
        private readonly PieceType player;
        private readonly int maxDepth;
        private readonly IScorer scorer;
        private readonly IMoveEnumerator moveEnumerator;
        private readonly IBoardFactory boardFactory;
        private readonly IJudge judge;

        private int leafCount;

        public AbPruningAi(PieceType player,
            int maxDepth,
            IScorer scorer,
            IMoveEnumerator moveEnumerator,
            IBoardFactory boardFactory,
            IJudge judge)
        {
            this.player = player;
            this.maxDepth = maxDepth;
            this.scorer = scorer;
            this.moveEnumerator = moveEnumerator;
            this.boardFactory = boardFactory;
            this.judge = judge;
        }

        public Position MakeAMove(IBoard board)
        {
            this.leafCount = 0;

            IBoard boardCopy = this.boardFactory.DeepCloneBoard(board);

            AiSearchResult result = Search(isMaxSearch: true,
                board: boardCopy,
                curPlayer: player,
                depth: 0,
                minPossibleScore: double.NegativeInfinity,
                maxPossibleScore: double.PositiveInfinity);

            Position bestMove = result.Moves.Peek().Move;

            Debug.WriteLine($"{player} best move {bestMove}, score {result.Score},"
                + $" leaf count {this.leafCount}, moves {string.Join(",", result.Moves)}.");

            return bestMove;
        }

        private AiSearchResult Search(bool isMaxSearch,
            IBoard board,
            PieceType curPlayer,
            int depth,
            double minPossibleScore,
            double maxPossibleScore)
        {
            if (depth >= maxDepth || board.IsFull() || HasWinner(board))
            {
                this.leafCount++;

                // Return the score of current board.
                double score = scorer.GetScore(board, player);
                return new AiSearchResult(score);
            }

            PieceType otherPlayer = curPlayer.GetOther();

            double worseScore = isMaxSearch ? minPossibleScore : maxPossibleScore;
            AiSearchResult bestMove = new AiSearchResult(worseScore);
            foreach (Position move in this.moveEnumerator.GetMoves(board, curPlayer))
            {
                // Make a move.
                board.Set(move, curPlayer);
                // Search deeper moves.
                AiSearchResult result = Search(!isMaxSearch, board, otherPlayer, depth + 1, minPossibleScore, maxPossibleScore);
                // Undo the move.
                board.Set(move, PieceType.Empty);

                double score = result.Score;

                if (isMaxSearch)
                {
                    if (score > bestMove.Score)
                    {
                        result.Moves.Push(new PlayerAndMove(curPlayer, move));
                        bestMove = result;

                        if (score >= maxPossibleScore)
                        {
                            // Result is better than max possible score, so this whole max search branch is meaningless.
                            // Just stop searching and return meaningless result.
                            break;
                        }

                        minPossibleScore = score;
                    }
                }
                else
                {
                    if (score < bestMove.Score)
                    {
                        result.Moves.Push(new PlayerAndMove(curPlayer, move));
                        bestMove = result;

                        if (score <= minPossibleScore)
                        {
                            // Result is worse than min possible score, so this whole min search branch is meaningless.
                            // Just stop searching and return meaningless result.
                            break;
                        }

                        maxPossibleScore = score;
                    }
                }
            }

            return bestMove;
        }

        private bool HasWinner(IBoard board)
        {
            return PieceType.Empty != this.judge.GetWinner(board);
        }
    }
}
