using System;
using System.Collections.Generic;
using System.Linq;

namespace GobangGameLib.GameBoard.Patterns
{
    public class PatternRepositoryBase<TContainer, TValue> where TContainer : ICollection<TValue>, new()
    {
        private readonly static IList<PieceType> Players = PieceTypeExtensions.GetAllPieces().ToList();
        private readonly static IList<PatternType> PatternTypes = PatternTypeExtensions.GetAll().ToList();

        // patterns[PieceType][PatternType]
        private TContainer[,] patterns;

        public PatternRepositoryBase()
        {
            this.patterns = new TContainer[PieceTypeExtensions.GetAll().Count(), PatternTypeExtensions.GetAll().Count()];
        }

        public void Add(PieceType player, PatternType patternType, TValue pattern)
        {
            EnsureContainerExist(player, patternType);

            this.patterns[(int)player, (int)patternType].Add(pattern);
        }

        public bool Remove(PieceType player, PatternType patternType, TValue pattern)
        {
            EnsureContainerExist(player, patternType);

            return this.patterns[(int)player, (int)patternType].Remove(pattern);
        }

        public TContainer Get(PieceType player, PatternType patternType)
        {
            EnsureContainerExist(player, patternType);

            return this.patterns[(int)player, (int)patternType];
        }

        public IEnumerable<TValue> Get(PieceType player)
        {
            return PatternTypes.SelectMany(patternType => Get(player, patternType));
        }

        public IEnumerable<TValue> Get(PatternType patternType)
        {
            return Players.SelectMany(player => Get(player, patternType));
        }

        public IEnumerable<TValue> Get()
        {
            return Players.SelectMany(player => Get(player));
        }

        private void EnsureContainerExist(PieceType player, PatternType patternType)
        {
            if (player == PieceType.Empty)
            {
                throw new ArgumentException($"Unsupported PieceType: {player}.");
            }

            int playerIndex = (int)player;
            int patternIndex = (int)patternType;
            if (this.patterns[playerIndex, patternIndex] == null)
            {
                this.patterns[playerIndex, patternIndex] = new TContainer();
            }
        }
    }
}
