using System.Linq;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PositionManagement;
using GoBangGameLibTest.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoBangGameLibTest
{
    [TestClass]
    public class ConnectionTests
    {
        [TestMethod]
        public void MatchOpenThreePattenThenFound()
        {
            // Arrange
            var positions = new[]
            {
                new Position(0, 0),
                new Position(0, 1),
                new Position(0, 2),
                new Position(0, 3),
                new Position(0, 4)
            };
            var pieces = new[]
            {
                PieceType.Empty,
                PieceType.P1,
                PieceType.P1,
                PieceType.P1,
                PieceType.Empty
            };

            var context = new BoardProperties();

            IBoard board = new Board(context);
            IPositions line = new FreeLine(positions);
            foreach (var i in Enumerable.Range(0, positions.Count()))
            {
                board.Set(positions[i], pieces[i]);
            }

            IPattern pattern = new Pattern(PatternType.OpenThree, PieceType.P1, pieces);

            var matcher = new PatternMatcher();

            // Act
            var result = matcher.MatchInternal(board, line, new[] { pattern });

            // Assert
            Assert.AreEqual(1, result.Count());
            CollectionAssert.AreEqual(positions, result.First().Positions.ToList());
        }

        [TestMethod]
        public void MatchOpenThreePattenOnEdgeThenNotFound()
        {
            // Arrange
            var positions = new[]
            {
                new Position(0, 0),
                new Position(0, 1),
                new Position(0, 2),
                new Position(0, 3),
                new Position(0, 4)
            };
            var pieces = new[]
            {
                PieceType.Empty,
                PieceType.P1,
                PieceType.P1,
                PieceType.P1,
                PieceType.Empty
            };

            var context = new BoardProperties();

            IBoard board = new Board(context);
            IPositions line = new FreeLine(positions);
            foreach (var i in Enumerable.Range(0, positions.Count() - 1))
            {
                board.Set(positions[i], pieces[i + 1]);
            }

            IPattern pattern = new Pattern(PatternType.OpenThree, PieceType.P1, pieces);

            var matcher = new PatternMatcher();

            // Act
            var result = matcher.MatchInternal(board, line, new[] { pattern });

            // Assert
            Assert.AreEqual(0, result.Count());
        }
    }
}