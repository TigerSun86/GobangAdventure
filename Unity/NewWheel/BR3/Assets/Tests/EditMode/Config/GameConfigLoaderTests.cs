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
            string json = JsonUtility.ToJson(TestConfigFactory.CreateValidGameConfig());

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
            TextAsset textAsset = new TextAsset(JsonUtility.ToJson(TestConfigFactory.CreateValidGameConfig()));

            GameConfig config = loader.LoadFromTextAsset(textAsset);

            Assert.That(config.enemies[0].enemyId, Is.EqualTo("enemy-1"));
        }

        [Test]
        public void LoadFromJson_WhenRequiredSectionIsMissing_Throws()
        {
            GameConfig config = TestConfigFactory.CreateValidGameConfig();
            config.playerStart = null;

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Is.Not.Empty);
        }

        [Test]
        public void LoadFromJson_WhenEnemyCountIsInvalid_Throws()
        {
            GameConfig config = TestConfigFactory.CreateValidGameConfig();
            config.enemies.RemoveAt(config.enemies.Count - 1);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("GameConfig.enemies"));
            Assert.That(exception.Message, Does.Contain("exactly 3"));
        }

        [Test]
        public void LoadFromJson_WhenCardSpecContainsDuplicateTrait_Throws()
        {
            GameConfig config = TestConfigFactory.CreateValidGameConfig();
            config.playerStart.startingDeck[0].traits = new List<TraitType> { TraitType.Empower, TraitType.Empower };

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("duplicate trait"));
        }

        [Test]
        public void LoadFromJson_WhenCardSpecContainsConflictingMovementTraits_Throws()
        {
            GameConfig config = TestConfigFactory.CreateValidGameConfig();
            config.playerStart.startingDeck[0].traits = new List<TraitType> { TraitType.ShiftLeft, TraitType.ShiftRight };

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => loader.LoadFromJson(JsonUtility.ToJson(config)));

            Assert.That(exception.Message, Does.Contain("ShiftLeft"));
            Assert.That(exception.Message, Does.Contain("ShiftRight"));
        }

    }
}
