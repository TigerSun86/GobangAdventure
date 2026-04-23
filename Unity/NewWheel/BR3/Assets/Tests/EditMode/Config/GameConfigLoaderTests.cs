using System;
using System.Collections.Generic;
using BR3.Config;
using BR3.Domain;
using NUnit.Framework;
using UnityEngine;

namespace BR3.Tests.EditMode.Config
{
    public sealed class GameConfigLoaderTests
    {
        private GameConfigLoader loader;

        [SetUp]
        public void SetUp()
        {
            loader = new GameConfigLoader();
        }

        [Test]
        public void LoadFromJson_WithValidConfig_ReturnsValidatedConfig()
        {
            string json = JsonUtility.ToJson(CreateValidConfig());

            GameConfig config = loader.LoadFromJson(json);

            Assert.That(config, Is.Not.Null);
            Assert.That(config.playerStart.playerMaxHp, Is.EqualTo(30));
            Assert.That(config.playerStart.startingDeck, Has.Count.EqualTo(6));
            Assert.That(config.enemies, Has.Count.EqualTo(3));
            Assert.That(config.rewardGeneration.replacementTraitCount, Is.EqualTo(2));
            Assert.That(config.traitTuning.empowerBonus, Is.EqualTo(3));
        }

        [Test]
        public void LoadFromTextAsset_WithValidConfig_ReturnsValidatedConfig()
        {
            TextAsset textAsset = new TextAsset(JsonUtility.ToJson(CreateValidConfig()));

            GameConfig config = loader.LoadFromTextAsset(textAsset);

            Assert.That(config.enemies[0].enemyId, Is.EqualTo("enemy-1"));
        }

        [Test]
        public void LoadFromJson_WhenRequiredSectionIsMissing_Throws()
        {
            GameConfig config = CreateValidConfig();
            config.playerStart = null;

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Is.Not.Empty);
        }

        [Test]
        public void LoadFromJson_WhenEnemyCountIsInvalid_Throws()
        {
            GameConfig config = CreateValidConfig();
            config.enemies.RemoveAt(config.enemies.Count - 1);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("GameConfig.enemies"));
            Assert.That(exception.Message, Does.Contain("exactly 3"));
        }

        [Test]
        public void LoadFromJson_WhenCardSpecContainsDuplicateTrait_Throws()
        {
            GameConfig config = CreateValidConfig();
            config.playerStart.startingDeck[0].traits = new List<TraitType> { TraitType.Empower, TraitType.Empower };

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("duplicate trait"));
        }

        [Test]
        public void LoadFromJson_WhenCardSpecContainsConflictingMovementTraits_Throws()
        {
            GameConfig config = CreateValidConfig();
            config.playerStart.startingDeck[0].traits = new List<TraitType> { TraitType.ShiftLeft, TraitType.ShiftRight };

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("ShiftLeft"));
            Assert.That(exception.Message, Does.Contain("ShiftRight"));
        }

        private static GameConfig CreateValidConfig()
        {
            return new GameConfig
            {
                playerStart = new PlayerStartConfig
                {
                    playerMaxHp = 30,
                    startingDeck = new List<CardSpec>
                    {
                        CreateCard(RpsType.Rock, 4, TraitType.Empower),
                        CreateCard(RpsType.Rock, 4, TraitType.ShiftLeft),
                        CreateCard(RpsType.Scissors, 4, TraitType.Empower),
                        CreateCard(RpsType.Scissors, 4, TraitType.ShiftLeft),
                        CreateCard(RpsType.Paper, 4, TraitType.Empower),
                        CreateCard(RpsType.Paper, 4, TraitType.ShiftLeft),
                    },
                },
                enemies = new List<EnemyConfig>
                {
                    CreateEnemy("enemy-1", "Enemy 1", 18, 4, 5),
                    CreateEnemy("enemy-2", "Enemy 2", 24, 5, 6),
                    CreateEnemy("enemy-3", "Enemy 3", 30, 6, 7),
                },
                rewardGeneration = new RewardGenerationConfig
                {
                    allowedReplacementRpsTypes = new List<RpsType> { RpsType.Rock, RpsType.Scissors, RpsType.Paper },
                    allowedReplacementBasePowers = new List<int> { 4 },
                    allowedReplacementTraits = new List<TraitType>
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
                    replacementTraitCount = 2,
                },
                traitTuning = new TraitTuning
                {
                    empowerBonus = 3,
                    adjacentAidBonus = 2,
                    suppressPenalty = 2,
                    regrowHeal = 2,
                    growthBonus = 1,
                },
            };
        }

        private static EnemyConfig CreateEnemy(string enemyId, string displayName, int maxHp, int lowPower, int highPower)
        {
            return new EnemyConfig
            {
                enemyId = enemyId,
                displayName = displayName,
                maxHp = maxHp,
                fixedDeck = new List<CardSpec>
                {
                    CreateCard(RpsType.Rock, lowPower),
                    CreateCard(RpsType.Rock, highPower),
                    CreateCard(RpsType.Scissors, lowPower),
                    CreateCard(RpsType.Scissors, highPower),
                    CreateCard(RpsType.Paper, lowPower),
                    CreateCard(RpsType.Paper, highPower),
                },
            };
        }

        private static CardSpec CreateCard(RpsType rpsType, int basePower, params TraitType[] traits)
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
