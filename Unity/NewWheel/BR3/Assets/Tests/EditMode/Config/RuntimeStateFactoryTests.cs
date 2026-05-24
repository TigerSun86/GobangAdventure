using System.Linq;
using BR3.Application;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Config
{
    public sealed class RuntimeStateFactoryTests
    {
        private RuntimeStateFactory factory;

        [SetUp]
        public void SetUp()
        {
            factory = new RuntimeStateFactory();
        }

        [Test]
        public void CreateRunState_WithValidGameConfig_CreatesExpectedInitialRunState()
        {
            GameConfig config = TestConfigFactory.CreateValidGameConfig();

            RunState runState = factory.CreateRunState(config);

            Assert.That(runState.PlayerHp, Is.EqualTo(config.playerStart.playerMaxHp));
            Assert.That(runState.PlayerMaxHp, Is.EqualTo(config.playerStart.playerMaxHp));
            Assert.That(runState.PlayerDeck, Has.Count.EqualTo(config.playerStart.startingDeck.Count));
            Assert.That(runState.PlayerDeck.Select(card => card.InstanceId).Distinct().Count(), Is.EqualTo(runState.PlayerDeck.Count));
            Assert.That(runState.PlayerDeck.All(card => card.PermanentPowerBonus == 0), Is.True);
            Assert.That(runState.CurrentEnemyIndex, Is.EqualTo(0));
            Assert.That(runState.CurrentEnemy.Config, Is.SameAs(config.enemies[0]));
            Assert.That(runState.CurrentEnemy.CurrentHp, Is.EqualTo(config.enemies[0].maxHp));
            Assert.That(runState.CurrentEnemy.MaxHp, Is.EqualTo(config.enemies[0].maxHp));
            Assert.That(runState.ActiveBattle, Is.Null);
            Assert.That(runState.PendingRewardOffer, Is.Null);
            Assert.That(runState.FlowStage, Is.EqualTo(RunFlowStage.ReadyForNextBattle));
        }

        [Test]
        public void CreateCardInstance_WithValidCardSpec_CreatesIndependentRuntimeCard()
        {
            CardSpec spec = TestConfigFactory.CreateCard(RpsType.Scissors, 5, TraitType.Empower, TraitType.Growth);

            CardInstance cardInstance = factory.CreateCardInstance(spec);

            Assert.That(cardInstance.InstanceId, Is.EqualTo("card-0001"));
            Assert.That(cardInstance.RpsType, Is.EqualTo(spec.rpsType));
            Assert.That(cardInstance.BasePower, Is.EqualTo(spec.basePower));
            Assert.That(cardInstance.Traits, Is.EqualTo(spec.traits));
            Assert.That(cardInstance.Traits, Is.Not.SameAs(spec.traits));
            Assert.That(cardInstance.PermanentPowerBonus, Is.EqualTo(0));
        }

        [Test]
        public void CreateEnemyProgressState_WithValidEnemyConfig_CreatesExpectedProgressState()
        {
            EnemyConfig enemyConfig = TestConfigFactory.CreateValidEnemyConfig("enemy-2", "Enemy 2", 24, 5, 6, battleLimit: 4);

            EnemyProgressState enemyProgressState = factory.CreateEnemyProgressState(enemyConfig);

            Assert.That(enemyProgressState.Config, Is.SameAs(enemyConfig));
            Assert.That(enemyProgressState.CurrentHp, Is.EqualTo(enemyConfig.maxHp));
            Assert.That(enemyProgressState.MaxHp, Is.EqualTo(enemyConfig.maxHp));
            Assert.That(enemyProgressState.BattlesPlayed, Is.EqualTo(0));
            Assert.That(enemyProgressState.RewardsClaimed, Is.EqualTo(0));
        }

        [Test]
        public void CreateEnemyProgressState_UsesConfigBattleLimitAsSingleRewardProgressSourceOfTruth()
        {
            EnemyConfig enemyConfig = TestConfigFactory.CreateValidEnemyConfig(battleLimit: 5);

            EnemyProgressState enemyProgressState = factory.CreateEnemyProgressState(enemyConfig);

            Assert.That(enemyProgressState.Config.battleLimit, Is.EqualTo(5));
            Assert.That(typeof(EnemyProgressState).GetField("RewardsTotal"), Is.Null);
        }

        [Test]
        public void CreateRunState_CopiesPlayerCardTraitsIntoIndependentLists()
        {
            GameConfig config = TestConfigFactory.CreateValidGameConfig();

            RunState runState = factory.CreateRunState(config);

            Assert.That(runState.PlayerDeck[0].Traits, Is.Not.SameAs(config.playerStart.startingDeck[0].traits));
        }
    }
}
