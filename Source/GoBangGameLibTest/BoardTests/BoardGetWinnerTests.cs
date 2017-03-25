using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GobangGameLib.Game;
using GobangGameLib.GameBoard;
using GobangGameLib.GameJudge;

namespace GoBangGameLibTest.BoardTests
{
    [TestClass]
    public class BoardGetWinnerTests
    {
        [TestMethod]
        public void TestEmpty()
        {
            Board board = new Board();
            PieceType result = new Judge().GetWinner(board);
            PieceType expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        public void TestRow1()
        {
            Board board = new Board();
            board.Set(new Position(3, 3), PieceType.P1);
            board.Set(new Position(3, 4), PieceType.P1);
            board.Set(new Position(3, 5), PieceType.P1);
            board.Set(new Position(3, 6), PieceType.P1);
            board.Set(new Position(3, 7), PieceType.P1);
            PieceType result = new Judge().GetWinner(board);
            PieceType expect = PieceType.P1;
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        public void TestRow2()
        {
            Board board = new Board();
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 3), PieceType.P1);
            board.Set(new Position(3, 4), PieceType.P1);
            board.Set(new Position(3, 5), PieceType.P1);
            board.Set(new Position(3, 6), PieceType.P1);
            board.Set(new Position(3, 7), PieceType.P2);
            result = new Judge().GetWinner(board);
            expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestCol1()
        {
            Board board = new Board();
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 3), PieceType.P1);
            board.Set(new Position(4, 3), PieceType.P1);
            board.Set(new Position(5, 3), PieceType.P1);
            board.Set(new Position(6, 3), PieceType.P1);
            board.Set(new Position(7, 3), PieceType.P1);
            result = new Judge().GetWinner(board);
            expect = PieceType.P1;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestCol2()
        {
            Board board = new Board();
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 3), PieceType.P1);
            board.Set(new Position(4, 3), PieceType.P2);
            board.Set(new Position(5, 3), PieceType.P1);
            board.Set(new Position(6, 3), PieceType.P1);
            board.Set(new Position(7, 3), PieceType.P1);
            result = new Judge().GetWinner(board);
            expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD11()
        {
            Board board = new Board();
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 5), PieceType.P1);
            board.Set(new Position(4, 6), PieceType.P1);
            board.Set(new Position(5, 7), PieceType.P1);
            board.Set(new Position(6, 8), PieceType.P1);
            board.Set(new Position(7, 9), PieceType.P1);
            result = new Judge().GetWinner(board);
            expect = PieceType.P1;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD12()
        {
            Board board = new Board();
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 5), PieceType.P1);
            board.Set(new Position(4, 6), PieceType.P1);
            board.Set(new Position(5, 7), PieceType.P2);
            board.Set(new Position(6, 8), PieceType.P1);
            board.Set(new Position(7, 9), PieceType.P1);
            result = new Judge().GetWinner(board);
            expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD13()
        {
            Board board = new Board();
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 0), PieceType.P1);
            board.Set(new Position(4, 1), PieceType.P1);
            board.Set(new Position(5, 2), PieceType.P1);
            board.Set(new Position(6, 3), PieceType.P1);
            board.Set(new Position(7, 4), PieceType.P1);
            result = new Judge().GetWinner(board);
            expect = PieceType.P1;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD14()
        {
            Board board = new Board();
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 0), PieceType.P2);
            board.Set(new Position(4, 1), PieceType.P1);
            board.Set(new Position(5, 2), PieceType.P1);
            board.Set(new Position(6, 3), PieceType.P1);
            board.Set(new Position(7, 4), PieceType.P1);
            result = new Judge().GetWinner(board);
            expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD21()
        {
            Board board = new Board();
            PieceType result;
            PieceType expect;
            board.Set(new Position(0, 5), PieceType.P1);
            board.Set(new Position(1, 4), PieceType.P1);
            board.Set(new Position(2, 3), PieceType.P1);
            board.Set(new Position(3, 2), PieceType.P1);
            board.Set(new Position(4, 1), PieceType.P1);
            result = new Judge().GetWinner(board);
            expect = PieceType.P1;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD22()
        {
            Board board = new Board();
            PieceType result;
            PieceType expect;
            board.Set(new Position(0, 5), PieceType.P1);
            board.Set(new Position(1, 4), PieceType.P1);
            board.Set(new Position(2, 3), PieceType.P1);
            board.Set(new Position(3, 2), PieceType.P2);
            board.Set(new Position(4, 1), PieceType.P1);
            result = new Judge().GetWinner(board);
            expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD23()
        {
            Board board = new Board();
            PieceType result;
            PieceType expect;
            board.Set(new Position(6, 10), PieceType.P1);
            board.Set(new Position(7, 9), PieceType.P1);
            board.Set(new Position(8, 8), PieceType.P1);
            board.Set(new Position(9, 7), PieceType.P1);
            board.Set(new Position(10, 6), PieceType.P1);
            result = new Judge().GetWinner(board);
            expect = PieceType.P1;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD24()
        {
            Board board = new Board();
            PieceType result;
            PieceType expect;
            board.Set(new Position(6, 10), PieceType.P1);
            board.Set(new Position(7, 9), PieceType.P1);
            board.Set(new Position(8, 8), PieceType.P1);
            board.Set(new Position(9, 7), PieceType.P2);
            board.Set(new Position(10, 6), PieceType.P1);
            result = new Judge().GetWinner(board);
            expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }
    }
}
