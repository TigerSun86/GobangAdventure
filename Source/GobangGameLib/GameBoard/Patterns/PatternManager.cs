using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternManager
    {
        private readonly static IDictionary<PatternType, IEnumerable<Pattern>> P1Repo = new Dictionary<PatternType, IEnumerable<Pattern>>
        {
            {
                PatternType.Five,
                new []
                {
                    new Pattern(new[] { PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1 })
                }
            },
            {
                PatternType.OpenFour,
                new []
                {
                    new Pattern(new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.Empty })
                }
            },
            {
                PatternType.OpenThree,
                new []
                {
                    new Pattern(new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.P1, PieceType.Empty })
                }
            },
            {
                PatternType.OpenTwo,
                new []
                {
                    new Pattern(new[] { PieceType.Empty, PieceType.P1, PieceType.P1, PieceType.Empty})
                }
            }
        };

        private static PatternManager _patternManager;

        private IDictionary<PatternType, IPatternGroup> _patternRepo;

        private PatternManager()
        {
            _patternRepo = P1Repo.ToDictionary(kvp => kvp.Key, kvp => CreatePatternGroupFromP1Patterns(kvp.Value));
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
