using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.GameJudge;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoBangGameLibTest.BoardTests
{
    [TestClass]
    public class BoardGetWinnerTests
    {
        [TestMethod]
        public void TestEmpty()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);

            Board board = new Board(context);
            PieceType result = GetJudge().GetWinner(board);
            PieceType expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        public void TestRow1()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);

            Board board = new Board(context);
            board.Set(new Position(3, 3), PieceType.P1);
            board.Set(new Position(3, 4), PieceType.P1);
            board.Set(new Position(3, 5), PieceType.P1);
            board.Set(new Position(3, 6), PieceType.P1);
            board.Set(new Position(3, 7), PieceType.P1);
            PieceType result = GetJudge().GetWinner(board);
            PieceType expect = PieceType.P1;
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        public void TestRow2()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);

            Board board = new Board(context);
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 3), PieceType.P1);
            board.Set(new Position(3, 4), PieceType.P1);
            board.Set(new Position(3, 5), PieceType.P1);
            board.Set(new Position(3, 6), PieceType.P1);
            board.Set(new Position(3, 7), PieceType.P2);
            result = GetJudge().GetWinner(board);
            expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestCol1()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);

            Board board = new Board(context);
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 3), PieceType.P1);
            board.Set(new Position(4, 3), PieceType.P1);
            board.Set(new Position(5, 3), PieceType.P1);
            board.Set(new Position(6, 3), PieceType.P1);
            board.Set(new Position(7, 3), PieceType.P1);
            result = GetJudge().GetWinner(board);
            expect = PieceType.P1;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestCol2()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);

            Board board = new Board(context);
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 3), PieceType.P1);
            board.Set(new Position(4, 3), PieceType.P2);
            board.Set(new Position(5, 3), PieceType.P1);
            board.Set(new Position(6, 3), PieceType.P1);
            board.Set(new Position(7, 3), PieceType.P1);
            result = GetJudge().GetWinner(board);
            expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD11()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);

            Board board = new Board(context);
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 5), PieceType.P1);
            board.Set(new Position(4, 6), PieceType.P1);
            board.Set(new Position(5, 7), PieceType.P1);
            board.Set(new Position(6, 8), PieceType.P1);
            board.Set(new Position(7, 9), PieceType.P1);
            result = GetJudge().GetWinner(board);
            expect = PieceType.P1;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD12()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);

            Board board = new Board(context);
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 5), PieceType.P1);
            board.Set(new Position(4, 6), PieceType.P1);
            board.Set(new Position(5, 7), PieceType.P2);
            board.Set(new Position(6, 8), PieceType.P1);
            board.Set(new Position(7, 9), PieceType.P1);
            result = GetJudge().GetWinner(board);
            expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD13()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);

            Board board = new Board(context);
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 0), PieceType.P1);
            board.Set(new Position(4, 1), PieceType.P1);
            board.Set(new Position(5, 2), PieceType.P1);
            board.Set(new Position(6, 3), PieceType.P1);
            board.Set(new Position(7, 4), PieceType.P1);
            result = GetJudge().GetWinner(board);
            expect = PieceType.P1;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD14()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);
            Board board = new Board(context);
            PieceType result;
            PieceType expect;
            board.Set(new Position(3, 0), PieceType.P2);
            board.Set(new Position(4, 1), PieceType.P1);
            board.Set(new Position(5, 2), PieceType.P1);
            board.Set(new Position(6, 3), PieceType.P1);
            board.Set(new Position(7, 4), PieceType.P1);
            result = GetJudge().GetWinner(board);
            expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD21()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);
            Board board = new Board(context);
            PieceType result;
            PieceType expect;
            board.Set(new Position(0, 5), PieceType.P1);
            board.Set(new Position(1, 4), PieceType.P1);
            board.Set(new Position(2, 3), PieceType.P1);
            board.Set(new Position(3, 2), PieceType.P1);
            board.Set(new Position(4, 1), PieceType.P1);
            result = GetJudge().GetWinner(board);
            expect = PieceType.P1;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD22()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);
            Board board = new Board(context);
            PieceType result;
            PieceType expect;
            board.Set(new Position(0, 5), PieceType.P1);
            board.Set(new Position(1, 4), PieceType.P1);
            board.Set(new Position(2, 3), PieceType.P1);
            board.Set(new Position(3, 2), PieceType.P2);
            board.Set(new Position(4, 1), PieceType.P1);
            result = GetJudge().GetWinner(board);
            expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD23()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);
            Board board = new Board(context);
            PieceType result;
            PieceType expect;
            board.Set(new Position(6, 10), PieceType.P1);
            board.Set(new Position(7, 9), PieceType.P1);
            board.Set(new Position(8, 8), PieceType.P1);
            board.Set(new Position(9, 7), PieceType.P1);
            board.Set(new Position(10, 6), PieceType.P1);
            result = GetJudge().GetWinner(board);
            expect = PieceType.P1;
            Assert.AreEqual(expect, result);
        }
        [TestMethod]
        public void TestD24()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);
            Board board = new Board(context);
            PieceType result;
            PieceType expect;
            board.Set(new Position(6, 10), PieceType.P1);
            board.Set(new Position(7, 9), PieceType.P1);
            board.Set(new Position(8, 8), PieceType.P1);
            board.Set(new Position(9, 7), PieceType.P2);
            board.Set(new Position(10, 6), PieceType.P1);
            result = GetJudge().GetWinner(board);
            expect = PieceType.Empty;
            Assert.AreEqual(expect, result);
        }

        private static IJudge GetJudge()
        {
            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);
            return new PatternJudge(positions, new PatternFactory().Create(), new PatternMatcher());
        }
    }
}
