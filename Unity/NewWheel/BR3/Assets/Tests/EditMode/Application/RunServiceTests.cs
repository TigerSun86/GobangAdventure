using System.Collections.Generic;
using System.Linq;
using BR3.Application;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Random;
using BR3.Domain.Reward;
using BR3.Domain.Results;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Application
{
    public sealed class RunServiceTests
    {
        [Test]
        public void CreateNewRun_WithValidConfig_ReturnsExpectedInitialRunState()
        {
            RunService runService = new RunService();
            RuntimeStateFactory runtimeStateFactory = new RuntimeStateFactory();
            GameConfig config = TestConfigFactory.CreateValidGameConfig();

            RunState runState = runService.CreateNewRun(config, runtimeStateFactory);

            Assert.That(runState, Is.Not.Null);
            Assert.That(runState.PlayerHp, Is.EqualTo(config.playerStart.playerMaxHp));
            Assert.That(runState.PlayerMaxHp, Is.EqualTo(config.playerStart.playerMaxHp));
            Assert.That(runState.PlayerDeck, Has.Count.EqualTo(config.playerStart.startingDeck.Count));
            Assert.That(runState.PlayerDeck.Select(card => card.InstanceId).Distinct().Count(), Is.EqualTo(runState.PlayerDeck.Count));
            Assert.That(runState.CurrentEnemyIndex, Is.EqualTo(0));
            Assert.That(runState.CurrentEnemy, Is.Not.Null);
            Assert.That(runState.CurrentEnemy.Config, Is.SameAs(config.enemies[0]));
            Assert.That(runState.CurrentEnemy.CurrentHp, Is.EqualTo(config.enemies[0].maxHp));
            Assert.That(runState.CurrentEnemy.MaxHp, Is.EqualTo(config.enemies[0].maxHp));
            Assert.That(runState.ActiveBattle, Is.Null);
            Assert.That(runState.PendingRewardOffer, Is.Null);
            Assert.That(runState.FlowStage, Is.EqualTo(RunFlowStage.ReadyForNextBattle));
        }

        [Test]
        public void CreateNewRun_RepeatedCallsReturnFreshIndependentRunState()
        {
            RunService runService = new RunService();
            RuntimeStateFactory runtimeStateFactory = new RuntimeStateFactory();
            GameConfig config = TestConfigFactory.CreateValidGameConfig();

            RunState firstRun = runService.CreateNewRun(config, runtimeStateFactory);
            RunState secondRun = runService.CreateNewRun(config, runtimeStateFactory);

            Assert.That(firstRun, Is.Not.SameAs(secondRun));
            Assert.That(firstRun.PlayerDeck, Is.Not.SameAs(secondRun.PlayerDeck));
            Assert.That(firstRun.PlayerDeck[0], Is.Not.SameAs(secondRun.PlayerDeck[0]));
            Assert.That(firstRun.CurrentEnemy, Is.Not.SameAs(secondRun.CurrentEnemy));
            Assert.That(firstRun.PlayerDeck[0].InstanceId, Is.Not.EqualTo(secondRun.PlayerDeck[0].InstanceId));
        }

        [Test]
        public void CanStartNextBattle_WhenRunIsReady_ReturnsTrue()
        {
            RunService runService = new RunService();
            RunState runState = CreateReadyRunState();

            bool canStartNextBattle = runService.CanStartNextBattle(runState);

            Assert.That(canStartNextBattle, Is.True);
        }

        [Test]
        public void CanStartNextBattle_WhenRunIsInIllegalState_ReturnsFalse()
        {
            RunService runService = new RunService();

            Assert.That(runService.CanStartNextBattle(CreateRunStateWithFlowStage(RunFlowStage.InBattle)), Is.False);
            Assert.That(runService.CanStartNextBattle(CreateRunStateWithFlowStage(RunFlowStage.ChoosingReward)), Is.False);
            Assert.That(runService.CanStartNextBattle(CreateRunStateWithFlowStage(RunFlowStage.Victory)), Is.False);
            Assert.That(runService.CanStartNextBattle(CreateRunStateWithFlowStage(RunFlowStage.Defeat)), Is.False);

            RunState runWithActiveBattle = CreateReadyRunState();
            runWithActiveBattle.ActiveBattle = CreateBattleState();
            Assert.That(runService.CanStartNextBattle(runWithActiveBattle), Is.False);

            RunState runWithPendingReward = CreateReadyRunState();
            runWithPendingReward.PendingRewardOffer = new RewardOffer
            {
                OfferId = "reward-1",
                RewardIndexForCurrentEnemy = 1,
                Options = new List<RewardOption>
                {
                    new RewardOption
                    {
                        OptionId = "skip-1",
                        Type = RewardOptionType.Skip,
                    },
                },
            };
            Assert.That(runService.CanStartNextBattle(runWithPendingReward), Is.False);

            RunState runWithoutCurrentEnemy = CreateReadyRunState();
            runWithoutCurrentEnemy.CurrentEnemy = null;
            Assert.That(runService.CanStartNextBattle(runWithoutCurrentEnemy), Is.False);
        }

        [Test]
        public void StartNextBattle_FromLegalReadyState_AttachesBattleAndTransitionsToInBattle()
        {
            RunService runService = new RunService();
            BattleService battleService = new BattleService(new FixedGameRandom(0, 0, 0, 0, 0));
            RunState runState = CreateReadyRunState();

            RunCommandResult commandResult = runService.StartNextBattle(runState, battleService);

            Assert.That(commandResult.Success, Is.True);
            Assert.That(commandResult.FailureReason, Is.Null);
            Assert.That(commandResult.FlowStage, Is.EqualTo(RunFlowStage.InBattle));
            Assert.That(commandResult.ActiveBattle, Is.Not.Null);
            Assert.That(commandResult.PendingRewardOffer, Is.Null);
            Assert.That(commandResult.IsRunComplete, Is.False);
            Assert.That(commandResult.ActiveBattle, Is.SameAs(runState.ActiveBattle));
            Assert.That(runState.FlowStage, Is.EqualTo(RunFlowStage.InBattle));
            Assert.That(runState.ActiveBattle.BattleIndexForEnemy, Is.EqualTo(1));
            Assert.That(runState.ActiveBattle.BattleFlowStage, Is.EqualTo(BattleFlowStage.WaitingForPlayerCard));
            Assert.That(runState.ActiveBattle.EnemySequence, Has.Count.EqualTo(3));
        }

        [Test]
        public void StartNextBattle_WhenRunCannotStart_ReturnsFailureAndDoesNotMutateRunState()
        {
            RunService runService = new RunService();
            BattleService battleService = new BattleService(new FixedGameRandom(0, 0, 0, 0, 0));
            RunState runState = CreateReadyRunState();
            RewardOffer pendingRewardOffer = new RewardOffer
            {
                OfferId = "reward-blocker",
                RewardIndexForCurrentEnemy = 1,
                Options = new List<RewardOption>
                {
                    new RewardOption
                    {
                        OptionId = "skip-1",
                        Type = RewardOptionType.Skip,
                    },
                },
            };

            runState.PendingRewardOffer = pendingRewardOffer;
            runState.FlowStage = RunFlowStage.ChoosingReward;

            RunCommandResult commandResult = runService.StartNextBattle(runState, battleService);

            Assert.That(commandResult.Success, Is.False);
            Assert.That(commandResult.FailureReason, Does.Contain("not currently allowed"));
            Assert.That(commandResult.FlowStage, Is.EqualTo(RunFlowStage.ChoosingReward));
            Assert.That(commandResult.ActiveBattle, Is.Null);
            Assert.That(commandResult.PendingRewardOffer, Is.SameAs(pendingRewardOffer));
            Assert.That(commandResult.IsRunComplete, Is.False);
            Assert.That(runState.FlowStage, Is.EqualTo(RunFlowStage.ChoosingReward));
            Assert.That(runState.ActiveBattle, Is.Null);
            Assert.That(runState.PendingRewardOffer, Is.SameAs(pendingRewardOffer));
        }

        [Test]
        public void AcceptCompletedBattle_WhenNonFinalEnemyIsDefeated_EntersRewardFlowAndClearsActiveBattle()
        {
            RunService runService = new RunService();
            GameConfig config = TestConfigFactory.CreateValidGameConfig();
            RunState runState = CreateRunStateForCompletedBattle(config, currentEnemyIndex: 0, battleIndexForEnemy: 2, battlesPlayed: 1);
            RewardService rewardService = CreateRewardService();

            RunCommandResult commandResult = runService.AcceptCompletedBattle(
                runState,
                CreateBattleOutcome(battleIndexForEnemy: 2, roundsPlayed: 2, enemyDefeated: true),
                rewardService,
                config);

            Assert.That(commandResult.Success, Is.True);
            Assert.That(runState.CurrentEnemy.BattlesPlayed, Is.EqualTo(2));
            Assert.That(runState.ActiveBattle, Is.Null);
            Assert.That(runState.FlowStage, Is.EqualTo(RunFlowStage.ChoosingReward));
            Assert.That(runState.PendingRewardOffer, Is.Not.Null);
            Assert.That(commandResult.FlowStage, Is.EqualTo(RunFlowStage.ChoosingReward));
            Assert.That(commandResult.ActiveBattle, Is.Null);
            Assert.That(commandResult.PendingRewardOffer, Is.SameAs(runState.PendingRewardOffer));
            Assert.That(commandResult.PendingRewardOffer.RewardIndexForCurrentEnemy, Is.EqualTo(1));
            Assert.That(commandResult.IsRunComplete, Is.False);
        }

        [Test]
        public void AcceptCompletedBattle_WhenFinalEnemyIsDefeated_EntersVictoryWithoutPendingRewardOffer()
        {
            RunService runService = new RunService();
            GameConfig config = TestConfigFactory.CreateValidGameConfig();
            int finalEnemyIndex = config.enemies.Count - 1;
            RunState runState = CreateRunStateForCompletedBattle(config, currentEnemyIndex: finalEnemyIndex, battleIndexForEnemy: 1, battlesPlayed: 0);
            RewardService rewardService = CreateRewardService();

            RunCommandResult commandResult = runService.AcceptCompletedBattle(
                runState,
                CreateBattleOutcome(battleIndexForEnemy: 1, roundsPlayed: 1, enemyDefeated: true),
                rewardService,
                config);

            Assert.That(commandResult.Success, Is.True);
            Assert.That(runState.CurrentEnemy.BattlesPlayed, Is.EqualTo(1));
            Assert.That(runState.ActiveBattle, Is.Null);
            Assert.That(runState.PendingRewardOffer, Is.Null);
            Assert.That(runState.FlowStage, Is.EqualTo(RunFlowStage.Victory));
            Assert.That(commandResult.FlowStage, Is.EqualTo(RunFlowStage.Victory));
            Assert.That(commandResult.PendingRewardOffer, Is.Null);
            Assert.That(commandResult.IsRunComplete, Is.True);
        }

        [Test]
        public void AcceptCompletedBattle_WhenEnemySurvivesAndBattlesRemain_EntersRewardFlow()
        {
            RunService runService = new RunService();
            GameConfig config = TestConfigFactory.CreateValidGameConfig();
            RunState runState = CreateRunStateForCompletedBattle(config, currentEnemyIndex: 0, battleIndexForEnemy: 1, battlesPlayed: 0);
            runState.CurrentEnemy.RewardsClaimed = 1;
            RewardService rewardService = CreateRewardService();

            RunCommandResult commandResult = runService.AcceptCompletedBattle(
                runState,
                CreateBattleOutcome(battleIndexForEnemy: 1, roundsPlayed: 3, enemyDefeated: false),
                rewardService,
                config);

            Assert.That(commandResult.Success, Is.True);
            Assert.That(runState.CurrentEnemy.BattlesPlayed, Is.EqualTo(1));
            Assert.That(runState.ActiveBattle, Is.Null);
            Assert.That(runState.FlowStage, Is.EqualTo(RunFlowStage.ChoosingReward));
            Assert.That(runState.PendingRewardOffer, Is.Not.Null);
            Assert.That(runState.PendingRewardOffer.RewardIndexForCurrentEnemy, Is.EqualTo(2));
            Assert.That(commandResult.PendingRewardOffer, Is.SameAs(runState.PendingRewardOffer));
            Assert.That(commandResult.IsRunComplete, Is.False);
        }

        [Test]
        public void AcceptCompletedBattle_WhenEnemySurvivesAfterThirdBattle_EntersDefeatWithoutPendingRewardOffer()
        {
            RunService runService = new RunService();
            GameConfig config = TestConfigFactory.CreateValidGameConfig();
            RunState runState = CreateRunStateForCompletedBattle(config, currentEnemyIndex: 1, battleIndexForEnemy: 3, battlesPlayed: 2);
            RewardService rewardService = CreateRewardService();

            RunCommandResult commandResult = runService.AcceptCompletedBattle(
                runState,
                CreateBattleOutcome(battleIndexForEnemy: 3, roundsPlayed: 3, enemyDefeated: false),
                rewardService,
                config);

            Assert.That(commandResult.Success, Is.True);
            Assert.That(runState.CurrentEnemy.BattlesPlayed, Is.EqualTo(3));
            Assert.That(runState.ActiveBattle, Is.Null);
            Assert.That(runState.PendingRewardOffer, Is.Null);
            Assert.That(runState.FlowStage, Is.EqualTo(RunFlowStage.Defeat));
            Assert.That(commandResult.FlowStage, Is.EqualTo(RunFlowStage.Defeat));
            Assert.That(commandResult.PendingRewardOffer, Is.Null);
            Assert.That(commandResult.IsRunComplete, Is.True);
        }

        [Test]
        public void AcceptCompletedBattle_WhenRunCannotAccept_ReturnsFailureAndDoesNotMutateRunState()
        {
            RunService runService = new RunService();
            GameConfig config = TestConfigFactory.CreateValidGameConfig();
            RunState runState = CreateRunStateForCompletedBattle(config, currentEnemyIndex: 0, battleIndexForEnemy: 2, battlesPlayed: 1);
            RewardService rewardService = CreateRewardService();
            BattleState originalBattle = runState.ActiveBattle;

            runState.ActiveBattle.BattleFlowStage = BattleFlowStage.PresentingRoundResult;

            RunCommandResult commandResult = runService.AcceptCompletedBattle(
                runState,
                CreateBattleOutcome(battleIndexForEnemy: 2, roundsPlayed: 2, enemyDefeated: true),
                rewardService,
                config);

            Assert.That(commandResult.Success, Is.False);
            Assert.That(commandResult.FailureReason, Does.Contain("not currently allowed"));
            Assert.That(commandResult.FlowStage, Is.EqualTo(RunFlowStage.InBattle));
            Assert.That(commandResult.ActiveBattle, Is.SameAs(originalBattle));
            Assert.That(commandResult.PendingRewardOffer, Is.Null);
            Assert.That(commandResult.IsRunComplete, Is.False);
            Assert.That(runState.CurrentEnemy.BattlesPlayed, Is.EqualTo(1));
            Assert.That(runState.ActiveBattle, Is.SameAs(originalBattle));
            Assert.That(runState.PendingRewardOffer, Is.Null);
            Assert.That(runState.FlowStage, Is.EqualTo(RunFlowStage.InBattle));
        }

        private static RunState CreateReadyRunState()
        {
            RuntimeStateFactory runtimeStateFactory = new RuntimeStateFactory();
            return runtimeStateFactory.CreateRunState(TestConfigFactory.CreateValidGameConfig());
        }

        private static RunState CreateRunStateForCompletedBattle(
            GameConfig config,
            int currentEnemyIndex,
            int battleIndexForEnemy,
            int battlesPlayed)
        {
            RuntimeStateFactory runtimeStateFactory = new RuntimeStateFactory();
            RunState runState = runtimeStateFactory.CreateRunState(config);
            runState.CurrentEnemyIndex = currentEnemyIndex;
            runState.CurrentEnemy = runtimeStateFactory.CreateEnemyProgressState(config.enemies[currentEnemyIndex]);
            runState.CurrentEnemy.BattlesPlayed = battlesPlayed;
            runState.ActiveBattle = CreateBattleState();
            runState.ActiveBattle.BattleIndexForEnemy = battleIndexForEnemy;
            runState.ActiveBattle.BattleFlowStage = BattleFlowStage.BattleComplete;
            runState.PendingRewardOffer = null;
            runState.FlowStage = RunFlowStage.InBattle;
            return runState;
        }

        private static RewardService CreateRewardService()
        {
            RuntimeStateFactory runtimeStateFactory = new RuntimeStateFactory();
            RewardOfferGenerator rewardOfferGenerator = new RewardOfferGenerator(new SystemGameRandom(12345));
            return new RewardService(runtimeStateFactory, rewardOfferGenerator);
        }

        private static BattleOutcome CreateBattleOutcome(int battleIndexForEnemy, int roundsPlayed, bool enemyDefeated)
        {
            return new BattleOutcome
            {
                BattleIndexForEnemy = battleIndexForEnemy,
                RoundsPlayed = roundsPlayed,
                EnemyDefeated = enemyDefeated,
                PlayerHpAfterBattle = 20,
                EnemyHpAfterBattle = enemyDefeated ? 0 : 5,
            };
        }

        private static RunState CreateRunStateWithFlowStage(RunFlowStage flowStage)
        {
            RunState runState = CreateReadyRunState();
            runState.FlowStage = flowStage;
            return runState;
        }

        private static BattleState CreateBattleState()
        {
            return new BattleState
            {
                BattleIndexForEnemy = 1,
                RoundIndex = 1,
                PlayerLane = null,
                EnemyLane = null,
                UsedPlayerCardIds = new HashSet<string>(),
                EnemySequence = new List<CardSpec>(),
                RoundResults = new List<BR3.Domain.Results.RoundResult>(),
                Logs = new List<string>(),
                Snapshots = new List<BR3.Domain.Results.PhaseSnapshot>(),
                BattleFlowStage = BattleFlowStage.WaitingForPlayerCard,
            };
        }

        private sealed class FixedGameRandom : IGameRandom
        {
            private readonly Queue<int> _values;

            public FixedGameRandom(params int[] values)
            {
                _values = new Queue<int>(values);
            }

            public int NextInt(int minInclusive, int maxExclusive)
            {
                int value = _values.Count > 0 ? _values.Dequeue() : minInclusive;
                Assert.That(value, Is.InRange(minInclusive, maxExclusive - 1));
                return value;
            }
        }
    }
}
