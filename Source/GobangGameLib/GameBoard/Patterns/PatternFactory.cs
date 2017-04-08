using System.Collections.Generic;
using System.Linq;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternFactory
    {
        private readonly static List<Pattern[]> P1Patterns = new List<Pattern[]>
        {
            new []
            {
                new Pattern(PatternType.Five, PieceType.P1,
                    new[] { PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1 })
            },
            new []
            {
                new Pattern(PatternType.OpenFour, PieceType.P1,
                    new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.Empty })
            },
            new []
            {
                new Pattern(PatternType.OpenThree, PieceType.P1,
                    new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.Empty })
            },
            new []
            {
                new Pattern(PatternType.OpenTwo, PieceType.P1,
                    new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.Empty})
            },
            new []
            {
                new Pattern(PatternType.OpenOne, PieceType.P1,
                    new[] { PieceType.Empty, PieceType.P1, PieceType.Empty})
            }
        };

        public PatternRepository Create()
        {
            IDictionary<PatternType, List<IPattern>> p1Patterns = P1Patterns.ToDictionary(i => i[0].PatternType, i => i.ToList<IPattern>());
            var patterns = new List<IPattern>[PieceTypeExtensions.GetAll().Count(), PatternTypeExtensions.GetAll().Count()];
            foreach (var kvp in p1Patterns)
            {
                patterns[(int)PieceType.P1, (int)kvp.Key] = kvp.Value;
                patterns[(int)PieceType.P2, (int)kvp.Key] = kvp.Value.Select(p => GetOther(p)).ToList();
            }

            return new PatternRepository(patterns);
        }

        private IPattern GetOther(IPattern pattern)
        {
            return new Pattern(pattern.PatternType,
                pattern.Player.GetOther(),
                pattern.Pieces.Select(p => p.GetOther()));
        }
    }
}
