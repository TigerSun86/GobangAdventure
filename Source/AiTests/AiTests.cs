using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using AI.Moves;
using AI.Scorer;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.GameJudge;
using GobangGameLib.Players;
using GoBangGameLibTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AiTests
{
    [TestClass]
    public class AiTests
    {
        [TestMethod]
        public void WhenOpponentHasOpenThreeThenBlockIt()
        {
            var boardString = new[]
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
            };

            var context = new BoardProperties(boardString.Length, boardString[0].Length);
            var positions = new PositionFactory().Create(context);
            var board = Utils.ParseBoard(boardString, context, positions);
            var expected = new[] { new Position(2, 3), new Position(6, 7) };

            int count = 0;
            foreach (var aiPlayer in GetAiPlayers(context, positions, PieceType.P1))
            {
                var move = aiPlayer.MakeAMove(board);
                Assert.IsTrue(expected.Contains(move), $"Assertion failed on player {count}, whose move is {move.ToString()}.");

                count++;
            }
        }

        [TestMethod]
        public void WhenOpponentHasOpenThreeThenBlockIt2()
        {
            var boardString = new[]
            {
                // 23456
                "       ", // 0
                "       ", // 1
                "  X    ", // 2
                "  XO   ", // 3
                "  XXO  ", // 4
                "    O  ", // 5
                "       ", // 6
                "       ", // 7
            };

            var context = new BoardProperties(boardString.Length, boardString[0].Length);
            var positions = new PositionFactory().Create(context);
            var board = Utils.ParseBoard(boardString, context, positions);
            var expected = new[] { new Position(1, 2), new Position(5, 2) };

            int count = 0;
            foreach (var aiPlayer in GetAiPlayers(context, positions, PieceType.P2))
            {
                var move = aiPlayer.MakeAMove(board);
                Assert.IsTrue(expected.Contains(move), $"Assertion failed on player {count}, whose move is {move.ToString()}.");

                count++;
            }
        }

        private IEnumerable<IPlayer> GetAiPlayers(BoardProperties context, PositionManager positions, PieceType player)
        {
            var patterns = new PatternFactory().Create();
            var matcher = new PatternMatcher();
            var patternBoardFactory = new PatternBoardFactory(context, positions, patterns, matcher);
            var centerScorer = new CenterScorer(context, positions);
            var patternScorer = new PatternScorer(positions, patterns, matcher);
            var aggregatedScorer = new AggregatedScorer(new[]
            {
                new Tuple<IScorer, double>(patternScorer, 1),
                new Tuple<IScorer, double>(centerScorer, 0.01)
            });
            var judge = new BasicJudge(context, positions);
            var emptyMoveEnumerator = new EmptyPositionMoveEnumerator(positions);
            var scoredMoveEnumerator = new ScoredMoveEnumerator(positions, aggregatedScorer);

            return new[]
            {
                new AbPruningAi(player, 1, aggregatedScorer, scoredMoveEnumerator, patternBoardFactory, judge),
                new AbPruningAi(player, 2, aggregatedScorer, scoredMoveEnumerator, patternBoardFactory, judge),
                new AbPruningAi(player, 3, aggregatedScorer, scoredMoveEnumerator, patternBoardFactory, judge)
            };
        }

    }
}
