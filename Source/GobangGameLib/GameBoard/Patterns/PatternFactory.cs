using System.Linq;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternFactory
    {
        private readonly static IPattern[] P1Patterns = new[]
        {
            new Pattern(PatternType.Five, PieceType.P1,
                new[] { PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1 }),
            new Pattern(PatternType.OpenFour, PieceType.P1,
                new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.Empty }),
            new Pattern(PatternType.OpenThree, PieceType.P1,
                new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.Empty }),
            new Pattern(PatternType.OpenTwo, PieceType.P1,
                new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.Empty}),
            new Pattern(PatternType.OpenOne, PieceType.P1,
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
                pattern.Player.GetOther(),
                pattern.Pieces.Select(p => p.GetOther()));
        }
    }
}
