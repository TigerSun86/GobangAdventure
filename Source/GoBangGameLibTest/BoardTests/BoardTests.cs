using GobangGameLib.GameBoard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.Game;
using GobangGameLib.Players;

namespace GoBangGameLibTest.BoardTests
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void DeepClone()
        {
            Board board = new Board();
            var board2 = (Board)board.DeepClone();

            board2.Set(new Position(0, 0), PieceType.P1);

            // Assert
            Assert.AreEqual(PieceType.P1, board2.Get(new Position(0, 0)));
            Assert.AreEqual(PieceType.Empty, board.Get(new Position(0, 0)));
        }

        [TestMethod]
        public void PositionCount()
        {
            Assert.AreEqual(BoardProperties.RowSize * BoardProperties.ColSize, PositionManager.Instance().Positions.Count());
        }

        [TestMethod]
        public void IsFull()
        {
            IGame game = new GameFactory().CreateGame(
                new NextAvailablePlayer(),
                new NextAvailablePlayer()
                );
            game.Start();

            BoardProperties.ColSize = 4;
            foreach (var i in Enumerable.Range(0, BoardProperties.RowSize * BoardProperties.ColSize))
            {
                Assert.AreEqual(game.GameStatus, GameStatus.NotEnd);
                game.Run();
            }

            Assert.AreEqual(game.GameStatus, GameStatus.Tie);
            Assert.IsTrue(game.Board.IsFull());

            BoardProperties.ColSize = 11;
        }

        [TestMethod]
        public void FullBoardDeepCloneIsFull()
        {
            IGame game = new GameFactory().CreateGame(
                new NextAvailablePlayer(),
                new NextAvailablePlayer()
                );
            game.Start();

            BoardProperties.ColSize = 4;
            foreach (var i in Enumerable.Range(0, BoardProperties.RowSize * BoardProperties.ColSize))
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
}
