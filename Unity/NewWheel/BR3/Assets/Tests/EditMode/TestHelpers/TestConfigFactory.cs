using System.Collections.Generic;
using BR3.Config;
using BR3.Domain;

namespace BR3.Tests.EditMode.TestHelpers
{
    public static class TestConfigFactory
    {
        public static GameConfig CreateValidGameConfig(
            PlayerStartConfig playerStart = null,
            List<EnemyConfig> enemies = null,
            RewardGenerationConfig rewardGeneration = null,
            TraitTuning traitTuning = null)
        {
            return new GameConfig
            {
                playerStart = playerStart ?? CreateValidPlayerStartConfig(),
                enemies = enemies ?? CreateValidEnemies(),
                rewardGeneration = rewardGeneration ?? CreateValidRewardGenerationConfig(),
                traitTuning = traitTuning ?? CreateValidTraitTuning(),
            };
        }

        public static PlayerStartConfig CreateValidPlayerStartConfig(
            int playerMaxHp = 30,
            List<CardSpec> startingDeck = null)
        {
            return new PlayerStartConfig
            {
                playerMaxHp = playerMaxHp,
                startingDeck = startingDeck ?? CreateValidStartingDeck(),
            };
        }

        public static List<EnemyConfig> CreateValidEnemies()
        {
            return new List<EnemyConfig>
            {
                CreateValidEnemyConfig("enemy-1", "Enemy 1", 18, 4, 5),
                CreateValidEnemyConfig("enemy-2", "Enemy 2", 24, 5, 6),
                CreateValidEnemyConfig("enemy-3", "Enemy 3", 30, 6, 7),
            };
        }

        public static EnemyConfig CreateValidEnemyConfig(
            string enemyId = "enemy-1",
            string displayName = "Enemy 1",
            int maxHp = 18,
            int lowPower = 4,
            int highPower = 5,
            List<CardSpec> fixedDeck = null)
        {
            return new EnemyConfig
            {
                enemyId = enemyId,
                displayName = displayName,
                maxHp = maxHp,
                fixedDeck = fixedDeck ?? CreateValidEnemyDeck(lowPower, highPower),
            };
        }

        public static RewardGenerationConfig CreateValidRewardGenerationConfig(
            List<RpsType> allowedReplacementRpsTypes = null,
            List<int> allowedReplacementBasePowers = null,
            List<TraitType> allowedReplacementTraits = null,
            int replacementTraitCount = 2)
        {
            return new RewardGenerationConfig
            {
                allowedReplacementRpsTypes = allowedReplacementRpsTypes ?? new List<RpsType> { RpsType.Rock, RpsType.Scissors, RpsType.Paper },
                allowedReplacementBasePowers = allowedReplacementBasePowers ?? new List<int> { 4 },
                allowedReplacementTraits = allowedReplacementTraits ?? new List<TraitType>
                {
                    TraitType.Empower,
                    TraitType.ShiftLeft,
                    TraitType.ShiftRight,
                    TraitType.AdjacentAid,
                    TraitType.Suppress,
                    TraitType.Regrow,
                    TraitType.Lifesteal,
                    TraitType.Growth,
                },
                replacementTraitCount = replacementTraitCount,
            };
        }

        public static TraitTuning CreateValidTraitTuning(
            int empowerBonus = 3,
            int adjacentAidBonus = 2,
            int suppressPenalty = 2,
            int regrowHeal = 2,
            int growthBonus = 1)
        {
            return new TraitTuning
            {
                empowerBonus = empowerBonus,
                adjacentAidBonus = adjacentAidBonus,
                suppressPenalty = suppressPenalty,
                regrowHeal = regrowHeal,
                growthBonus = growthBonus,
            };
        }

        public static List<CardSpec> CreateValidStartingDeck()
        {
            return new List<CardSpec>
            {
                CreateCard(RpsType.Rock, 4, TraitType.Empower),
                CreateCard(RpsType.Rock, 4, TraitType.ShiftLeft),
                CreateCard(RpsType.Scissors, 4, TraitType.Empower),
                CreateCard(RpsType.Scissors, 4, TraitType.ShiftLeft),
                CreateCard(RpsType.Paper, 4, TraitType.Empower),
                CreateCard(RpsType.Paper, 4, TraitType.ShiftLeft),
            };
        }

        public static List<CardSpec> CreateValidEnemyDeck(int lowPower, int highPower)
        {
            return new List<CardSpec>
            {
                CreateCard(RpsType.Rock, lowPower),
                CreateCard(RpsType.Rock, highPower),
                CreateCard(RpsType.Scissors, lowPower),
                CreateCard(RpsType.Scissors, highPower),
                CreateCard(RpsType.Paper, lowPower),
                CreateCard(RpsType.Paper, highPower),
            };
        }

        public static CardSpec CreateCard(RpsType rpsType, int basePower, params TraitType[] traits)
        {
            return new CardSpec
            {
                rpsType = rpsType,
                basePower = basePower,
                traits = new List<TraitType>(traits),
            };
        }
    }
}
