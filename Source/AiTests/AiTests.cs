using System;
using System.Collections.Generic;
using System.Linq;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.PositionManagement;
using GoBangGameLibTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AiTests
{
    [TestClass]
    public class AiTests
    {

        [TestMethod]
        public void TestMethod1()
        {
            var boardString = new[]
            {
                "           ",
                "           ",
                "           ",
                "           ",
                "    XO     ",
                "    XXO    ",
                "      OO   ",
                "      X    ",
                "           ",
                "           ",
                "           ",
            };

            var context = new BoardProperties();
            var positions = new PositionFactory().Create(context);

            var result = Utils.ParseBoard(boardString, context, positions);
        }

    }
}
