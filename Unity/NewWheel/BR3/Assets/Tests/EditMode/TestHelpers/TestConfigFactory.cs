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

        public static AuthoredGameConfig CreateValidAuthoredGameConfig(
            AuthoredPlayerStartConfig playerStart = null,
            List<AuthoredEnemyConfig> enemies = null,
            AuthoredRewardGenerationConfig rewardGeneration = null,
            TraitTuning traitTuning = null)
        {
            return new AuthoredGameConfig
            {
                playerStart = playerStart ?? CreateValidAuthoredPlayerStartConfig(),
                enemies = enemies ?? CreateValidAuthoredEnemies(),
                rewardGeneration = rewardGeneration ?? CreateValidAuthoredRewardGenerationConfig(),
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

        public static AuthoredPlayerStartConfig CreateValidAuthoredPlayerStartConfig(
            int playerMaxHp = 30,
            List<AuthoredCardSpec> startingDeck = null)
        {
            return new AuthoredPlayerStartConfig
            {
                playerMaxHp = playerMaxHp,
                startingDeck = startingDeck ?? CreateValidAuthoredStartingDeck(),
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

        public static List<AuthoredEnemyConfig> CreateValidAuthoredEnemies()
        {
            return new List<AuthoredEnemyConfig>
            {
                CreateValidAuthoredEnemyConfig("enemy-1", "Enemy 1", 18, 4, 5),
                CreateValidAuthoredEnemyConfig("enemy-2", "Enemy 2", 24, 5, 6),
                CreateValidAuthoredEnemyConfig("enemy-3", "Enemy 3", 30, 6, 7),
            };
        }

        public static EnemyConfig CreateValidEnemyConfig(
            string enemyId = "enemy-1",
            string displayName = "Enemy 1",
            int maxHp = 18,
            int lowPower = 4,
            int highPower = 5,
            List<CardSpec> fixedDeck = null,
            int battleLimit = 3)
        {
            return new EnemyConfig
            {
                enemyId = enemyId,
                displayName = displayName,
                maxHp = maxHp,
                battleLimit = battleLimit,
                fixedDeck = fixedDeck ?? CreateValidEnemyDeck(lowPower, highPower),
            };
        }

        public static AuthoredEnemyConfig CreateValidAuthoredEnemyConfig(
            string enemyId = "enemy-1",
            string displayName = "Enemy 1",
            int maxHp = 18,
            int lowPower = 4,
            int highPower = 5,
            List<AuthoredCardSpec> fixedDeck = null,
            int battleLimit = 3)
        {
            return new AuthoredEnemyConfig
            {
                enemyId = enemyId,
                displayName = displayName,
                maxHp = maxHp,
                battleLimit = battleLimit,
                fixedDeck = fixedDeck ?? CreateValidAuthoredEnemyDeck(lowPower, highPower),
            };
        }

        public static RewardGenerationConfig CreateValidRewardGenerationConfig(
            List<RpsType> allowedReplacementRpsTypes = null,
            List<int> allowedReplacementBasePowers = null,
            List<TraitType> allowedReplacementTraits = null,
            int replacementTraitCount = 2,
            int rewardOfferTotalOptions = 4,
            int upgradeTarget = 2)
        {
            return new RewardGenerationConfig
            {
                rewardOfferTotalOptions = rewardOfferTotalOptions,
                upgradeTarget = upgradeTarget,
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

        public static AuthoredRewardGenerationConfig CreateValidAuthoredRewardGenerationConfig(
            List<string> allowedReplacementRpsTypes = null,
            List<int> allowedReplacementBasePowers = null,
            List<string> allowedReplacementTraits = null,
            int replacementTraitCount = 2,
            int rewardOfferTotalOptions = 4,
            int upgradeTarget = 2)
        {
            return new AuthoredRewardGenerationConfig
            {
                rewardOfferTotalOptions = rewardOfferTotalOptions,
                upgradeTarget = upgradeTarget,
                allowedReplacementRpsTypes = allowedReplacementRpsTypes ?? new List<string> { "Rock", "Scissors", "Paper" },
                allowedReplacementBasePowers = allowedReplacementBasePowers ?? new List<int> { 4 },
                allowedReplacementTraits = allowedReplacementTraits ?? new List<string>
                {
                    "Empower",
                    "ShiftLeft",
                    "ShiftRight",
                    "AdjacentAid",
                    "Suppress",
                    "Regrow",
                    "Lifesteal",
                    "Growth",
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

        public static List<AuthoredCardSpec> CreateValidAuthoredStartingDeck()
        {
            return new List<AuthoredCardSpec>
            {
                CreateAuthoredCard("Rock", 4, "Empower"),
                CreateAuthoredCard("Rock", 4, "ShiftLeft"),
                CreateAuthoredCard("Scissors", 4, "Empower"),
                CreateAuthoredCard("Scissors", 4, "ShiftLeft"),
                CreateAuthoredCard("Paper", 4, "Empower"),
                CreateAuthoredCard("Paper", 4, "ShiftLeft"),
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

        public static List<AuthoredCardSpec> CreateValidAuthoredEnemyDeck(int lowPower, int highPower)
        {
            return new List<AuthoredCardSpec>
            {
                CreateAuthoredCard("Rock", lowPower),
                CreateAuthoredCard("Rock", highPower),
                CreateAuthoredCard("Scissors", lowPower),
                CreateAuthoredCard("Scissors", highPower),
                CreateAuthoredCard("Paper", lowPower),
                CreateAuthoredCard("Paper", highPower),
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

        public static AuthoredCardSpec CreateAuthoredCard(string rpsType, int basePower, params string[] traits)
        {
            return new AuthoredCardSpec
            {
                rpsType = rpsType,
                basePower = basePower,
                traits = new List<string>(traits),
            };
        }
    }
}
