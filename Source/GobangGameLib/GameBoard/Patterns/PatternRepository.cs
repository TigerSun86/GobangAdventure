using System.Collections.Generic;
using System.Linq;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternRepository
    {
        private readonly static IList<PieceType> Players = PieceTypeExtensions.GetAllPieces().ToList();
        private readonly static IList<PatternType> PatternTypes = PatternTypeExtensions.GetAll().ToList();

        // patterns[PieceType][PatternType]
        private IList<IPattern>[,] patterns;

        public PatternRepository(IList<IPattern>[,] patterns)
        {
            this.patterns = patterns;
        }

        public IEnumerable<IPattern> Get(PieceType player, PatternType patternType)
        {
            return this.patterns[(int)player, (int)patternType];
        }

        public IEnumerable<IPattern> Get(PieceType player)
        {
            return PatternRepository.PatternTypes.SelectMany(patternType => Get(player, patternType));
        }

        public IEnumerable<IPattern> Get(PatternType patternType)
        {
            return PatternRepository.Players.SelectMany(player => Get(player, patternType));
        }

        public IEnumerable<IPattern> Get()
        {
            return PatternRepository.Players.SelectMany(player => Get(player));
        }
    }
}
