using System.Linq;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternFactory
    {
        private readonly static IPattern[] P1Patterns =
        {
            new Pattern(PatternType.Five, PatternPositionType.Any, PieceType.P1,
                new[] { PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1 }),

            new Pattern(PatternType.OpenFour, PatternPositionType.Any, PieceType.P1,
                new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.Empty }),

            new Pattern(PatternType.HalfFour, PatternPositionType.Any, PieceType.P1,
                new[] { PieceType.P2, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.Empty }),

            new Pattern(PatternType.HalfFour, PatternPositionType.Any, PieceType.P1,
                new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P2 }),

            new Pattern(PatternType.HalfFour, PatternPositionType.Head, PieceType.P1,
                new[] { PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.Empty }),

            new Pattern(PatternType.HalfFour, PatternPositionType.Tail, PieceType.P1,
                new[] { PieceType.Empty , PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1}),

            new Pattern(PatternType.OpenThree, PatternPositionType.Any, PieceType.P1,
                new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.Empty }),

            new Pattern(PatternType.OpenTwo, PatternPositionType.Any, PieceType.P1,
                new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.Empty}),

            new Pattern(PatternType.OpenOne, PatternPositionType.Any, PieceType.P1,
                new[] { PieceType.Empty, PieceType.P1, PieceType.Empty})
        };

        public PatternRepository Create()
        {
            PatternRepository patternRepository = new PatternRepository();

            foreach (var p1 in P1Patterns)
            {
                patternRepository.Add(p1.Player, p1.PatternType, p1);

                var p2 = GetOther(p1);
                patternRepository.Add(p2.Player, p2.PatternType, p2);
            }

            return patternRepository;
        }

        private IPattern GetOther(IPattern pattern)
        {
            return new Pattern(pattern.PatternType,
                pattern.PatternPositionType,
                pattern.Player.GetOther(),
                pattern.Pieces.Select(p => p.GetOther()));
        }
    }
}
