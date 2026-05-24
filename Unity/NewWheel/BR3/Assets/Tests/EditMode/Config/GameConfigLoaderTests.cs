using System;
using System.Collections.Generic;
using BR3.Config;
using BR3.Domain;
using BR3.Tests.EditMode.TestHelpers;
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
            string json = JsonUtility.ToJson(TestConfigFactory.CreateValidAuthoredGameConfig());

            GameConfig config = loader.LoadFromJson(json);

            Assert.That(config, Is.Not.Null);
            Assert.That(config.playerStart.playerMaxHp, Is.EqualTo(30));
            Assert.That(config.playerStart.startingDeck, Has.Count.EqualTo(6));
            Assert.That(config.enemies, Has.Count.EqualTo(3));
            Assert.That(config.enemies[0].battleLimit, Is.EqualTo(3));
            Assert.That(config.rewardGeneration.rewardOfferTotalOptions, Is.EqualTo(4));
            Assert.That(config.rewardGeneration.upgradeTarget, Is.EqualTo(2));
            Assert.That(config.rewardGeneration.replacementTraitCount, Is.EqualTo(2));
            Assert.That(config.traitTuning.empowerBonus, Is.EqualTo(3));
        }

        [Test]
        public void LoadFromTextAsset_WithValidConfig_ReturnsValidatedConfig()
        {
            TextAsset textAsset = new TextAsset(JsonUtility.ToJson(TestConfigFactory.CreateValidAuthoredGameConfig()));

            GameConfig config = loader.LoadFromTextAsset(textAsset);

            Assert.That(config.enemies[0].enemyId, Is.EqualTo("enemy-1"));
        }

        [Test]
        public void LoadFromJson_WhenRequiredSectionIsMissing_Throws()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.playerStart = null;

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Is.Not.Empty);
        }

        [Test]
        public void LoadFromJson_WhenEnemyListContainsOneEntry_Succeeds()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.enemies.RemoveRange(1, config.enemies.Count - 1);

            GameConfig loadedConfig = loader.LoadFromJson(JsonUtility.ToJson(config));

            Assert.That(loadedConfig.enemies, Has.Count.EqualTo(1));
        }

        [Test]
        public void LoadFromJson_WhenEnemyListIsEmpty_Throws()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.enemies.Clear();

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("GameConfig.enemies"));
            Assert.That(exception.Message, Does.Contain("at least 1"));
        }

        [Test]
        public void LoadFromJson_WhenStartingDeckHasThreeCards_Succeeds()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.playerStart.startingDeck = new List<AuthoredCardSpec>
            {
                TestConfigFactory.CreateAuthoredCard("Rock", 4, "Empower"),
                TestConfigFactory.CreateAuthoredCard("Scissors", 4, "ShiftLeft"),
                TestConfigFactory.CreateAuthoredCard("Paper", 4, "AdjacentAid"),
            };

            GameConfig loadedConfig = loader.LoadFromJson(JsonUtility.ToJson(config));

            Assert.That(loadedConfig.playerStart.startingDeck, Has.Count.EqualTo(3));
        }

        [Test]
        public void LoadFromJson_WhenEnemyFixedDeckHasFewerThanThreeCards_Throws()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.enemies[0].fixedDeck = new List<AuthoredCardSpec>
            {
                TestConfigFactory.CreateAuthoredCard("Rock", 4),
                TestConfigFactory.CreateAuthoredCard("Scissors", 4),
            };

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("fixedDeck"));
            Assert.That(exception.Message, Does.Contain("at least 3"));
        }

        [Test]
        public void LoadFromJson_WhenEnemyBattleLimitIsLessThanOne_Throws()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.enemies[0].battleLimit = 0;

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("battleLimit"));
        }

        [Test]
        public void LoadFromJson_WhenRewardOfferTotalOptionsIsLessThanTwo_Throws()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.rewardGeneration.rewardOfferTotalOptions = 1;

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("rewardOfferTotalOptions"));
        }

        [Test]
        public void LoadFromJson_WhenUpgradeTargetExceedsNonSkipCapacity_Throws()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.rewardGeneration.upgradeTarget = config.rewardGeneration.rewardOfferTotalOptions;

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("upgradeTarget"));
        }

        [Test]
        public void LoadFromJson_WhenReplacementTraitCountIsWithinRange_Succeeds()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.rewardGeneration.replacementTraitCount = 3;

            GameConfig loadedConfig = loader.LoadFromJson(JsonUtility.ToJson(config));

            Assert.That(loadedConfig.rewardGeneration.replacementTraitCount, Is.EqualTo(3));
        }

        [Test]
        public void LoadFromJson_WhenReplacementTraitCountIsZero_AllowsEmptyReplacementTraitPool()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.rewardGeneration.replacementTraitCount = 0;
            config.rewardGeneration.allowedReplacementTraits = new List<string>();

            GameConfig loadedConfig = loader.LoadFromJson(JsonUtility.ToJson(config));

            Assert.That(loadedConfig.rewardGeneration.replacementTraitCount, Is.EqualTo(0));
            Assert.That(loadedConfig.rewardGeneration.allowedReplacementTraits, Is.Empty);
        }

        [Test]
        public void LoadFromJson_WhenReplacementTraitCountExceedsAvailableTraitPool_Throws()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.rewardGeneration.replacementTraitCount = 3;
            config.rewardGeneration.allowedReplacementTraits = new List<string> { "Empower", "Suppress" };

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("allowedReplacementTraits"));
            Assert.That(exception.Message, Does.Contain("replacementTraitCount"));
        }

        [Test]
        public void LoadFromJson_WhenReplacementTraitCountExceedsAllowedRange_Throws()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.rewardGeneration.replacementTraitCount = 4;

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("replacementTraitCount"));
        }

        [Test]
        public void LoadFromJson_WhenCardSpecContainsDuplicateTrait_Throws()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.playerStart.startingDeck[0].traits = new List<string> { "Empower", "Empower" };

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("duplicate trait"));
        }

        [Test]
        public void LoadFromJson_WhenCardSpecContainsConflictingMovementTraits_Throws()
        {
            AuthoredGameConfig config = TestConfigFactory.CreateValidAuthoredGameConfig();
            config.playerStart.startingDeck[0].traits = new List<string> { "ShiftLeft", "ShiftRight" };

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("ShiftLeft"));
            Assert.That(exception.Message, Does.Contain("ShiftRight"));
        }
    }
}
