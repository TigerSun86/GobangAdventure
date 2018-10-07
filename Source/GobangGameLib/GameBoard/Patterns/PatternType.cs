using System;
using System.Collections.Generic;
using System.Linq;

namespace GobangGameLib.GameBoard.Patterns
{
    public enum PatternType
    {
        Five,
        OpenFour,
        HalfFour,
        OpenThree,
        HalfThree,
        OpenTwo,
        OpenOne
    }

    public static class PatternTypeExtensions
    {
        public static IEnumerable<PatternType> GetAll()
        {
            return Enum.GetValues(typeof(PatternType)).Cast<PatternType>();
        }
    }
}
