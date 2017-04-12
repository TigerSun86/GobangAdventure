using System.Linq;
using GobangGameLib.Game;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.GameJudge;
using GobangGameLib.Players;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoBangGameLibTest.BoardTests
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void DeepClone()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);

            Board board = new Board(context);
            var board2 = (Board)board.DeepClone();

            board2.Set(new Position(0, 0), PieceType.P1);

            // Assert
            Assert.AreEqual(PieceType.P1, board2.Get(new Position(0, 0)));
            Assert.AreEqual(PieceType.Empty, board.Get(new Position(0, 0)));
        }

        [TestMethod]
        public void PositionCount()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);

            Assert.AreEqual(context.RowSize * context.ColSize, positions.Positions.Count());
        }

        [TestMethod]
        public void IsFull()
        {
            var context = new BoardProperties(4, 4, 5);
            var positions = new PositionFactory().Create(context);
            var patterns = new PatternFactory().Create();
            var matcher = new PatternMatcher();
            var boardFactories = new IBoardFactory[]
            {
                new BoardFactory(context, positions),
                new PatternBoardFactory(context, positions, patterns, matcher)
            };

            foreach (var boardFactory in boardFactories)
            {
                IGame game = new GameFactory().CreateGame(
                boardFactory,
                new NextAvailablePlayer(positions),
                new NextAvailablePlayer(positions),
                new BasicJudge(context, positions)
                );

                game.Start();

                foreach (var i in Enumerable.Range(0, context.RowSize * context.ColSize))
                {
                    Assert.AreEqual(game.GameStatus, GameStatus.NotEnd);
                    game.Run();
                }

                Assert.AreEqual(game.GameStatus, GameStatus.Tie);
                Assert.IsTrue(game.Board.IsFull());
            }
        }

        [TestMethod]
        public void FullBoardDeepCloneIsFull()
        {
            var context = new BoardProperties(4, 4, 5);
            var positions = new PositionFactory().Create(context);
            var patterns = new PatternFactory().Create();
            var matcher = new PatternMatcher();
            var boardFactories = new IBoardFactory[]
            {
                new BoardFactory(context, positions),
                new PatternBoardFactory(context, positions, patterns, matcher)
            };

            foreach (var boardFactory in boardFactories)
            {
                IGame game = new GameFactory().CreateGame(
                boardFactory,
                new NextAvailablePlayer(positions),
                new NextAvailablePlayer(positions),
                new BasicJudge(context, positions)
                );

                game.Start();

                foreach (var i in Enumerable.Range(0, context.RowSize * context.ColSize))
                {
                    Assert.AreEqual(game.GameStatus, GameStatus.NotEnd);
                    game.Run();
                }

                Assert.AreEqual(game.GameStatus, GameStatus.Tie);
                Assert.IsTrue(game.Board.IsFull());

                var board2 = game.Board.DeepClone();
                Assert.IsTrue(board2.IsFull());
            }
        }

        [TestMethod]
        public void TestDiagonalIndexes()
        {
            var b = new BoardProperties();
            var positions = new PositionFactory().Create(b);

            var d1Lines = positions.LineGroups[LineType.DiagonalOne];

            foreach (var p in positions.Positions)
            {
                int index = positions.GetDiagonalOneIndex(p);
                Assert.IsTrue(d1Lines.Lines[index].Positions.Contains(p));
            }

            var d2Lines = positions.LineGroups[LineType.DiagonalTwo];

            foreach (var p in positions.Positions)
            {
                int index = positions.GetDiagonalTwoIndex(p);
                Assert.IsTrue(d2Lines.Lines[index].Positions.Contains(p));
            }
        }
    }
}
