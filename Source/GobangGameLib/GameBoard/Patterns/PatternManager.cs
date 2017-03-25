using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternManager
    {
        private readonly static List<Pattern[]> P1Repo = new List<Pattern[]>
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
            }
        };

        private static PatternManager _patternManager;
        private IDictionary<PatternType, IPatternGroup> _patternRepo;

        private PatternManager()
        {
            _patternRepo = P1Repo.ToDictionary(i => i[0].PatternType, i => CreatePatternGroupFromP1Patterns(i));
        }

        public static PatternManager Instance()
        {
            if (_patternManager == null)
            {
                _patternManager = new PatternManager();
            }

            return _patternManager;
        }

        public IDictionary<PatternType, IPatternGroup> PatternRepo
        {
            get
            {
                return _patternRepo;
            }
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
