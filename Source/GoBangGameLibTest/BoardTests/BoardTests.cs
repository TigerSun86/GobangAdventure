using GobangGameLib.GameBoard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard.PositionManagement;

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
    }
}
