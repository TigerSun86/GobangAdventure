using System;
using System.Collections.Generic;
using System.Linq;

namespace GobangGameLib.GameBoard.PositionManagement
{
    public enum LineType
    {
        Row,
        Column,
        DiagonalOne,
        DiagonalTwo
    }

    public static class LineTypeExtensions
    {
        public static IEnumerable<LineType> GetAll()
        {
            return Enum.GetValues(typeof(LineType)).Cast<LineType>();
        }
    }
}
