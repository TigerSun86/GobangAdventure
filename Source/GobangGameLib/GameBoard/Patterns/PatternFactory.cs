using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternFactory
    {
        private readonly static List<Pattern[]> P1Patterns = new List<Pattern[]>
        {
            new []
            {
                new Pattern(PatternType.Five, new[] { PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1 })
            },
            new []
            {
                new Pattern(PatternType.OpenFour, new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.Empty })
            },
            new []
            {
                new Pattern(PatternType.OpenThree, new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.Empty })
            },
            new []
            {
                new Pattern(PatternType.OpenTwo, new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.Empty})
            },
            new []
            {
                new Pattern(PatternType.OpenOne, new[] { PieceType.Empty, PieceType.P1, PieceType.Empty})
            }
        };

        public PatternRepository Create()
        {
            IDictionary<PatternType, IPatternGroup> patterns = P1Patterns.ToDictionary(i => i[0].PatternType, i => CreatePatternGroupFromP1Patterns(i));
            return new PatternRepository(patterns);
        }

        private IPatternGroup CreatePatternGroupFromP1Patterns(IEnumerable<IPattern> p1Patterns)
        {
            return new PatternGroup(new Dictionary<PieceType, IEnumerable<IPattern>>
            {
                { PieceType.P1, p1Patterns },
                { PieceType.P2, p1Patterns.Select(p => p.GetOther()).ToList() }
            });
        }
    }
}
