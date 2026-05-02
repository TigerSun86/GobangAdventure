using System.Collections.Generic;
using BR3.Application;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Random;
using BR3.Domain.Results;
using BR3.Domain.Rules;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Application
{
    public sealed class BattleServiceTests
    {
        [Test]
        public void StartBattle_CreatesValidBattleStateInWaitingForPlayerCard()
        {
            BattleService battleService = new BattleService(new FixedGameRandom(0, 0, 0, 0, 0));
            EnemyProgressState enemyProgressState = CreateEnemyProgressState(
                battlesPlayed: 1,
                fixedDeck: CreateEnemyDeck());

            BattleState battleState = battleService.StartBattle(enemyProgressState);

            Assert.That(battleState, Is.Not.Null);
            Assert.That(battleState.BattleIndexForEnemy, Is.EqualTo(2));
            Assert.That(battleState.RoundIndex, Is.EqualTo(1));
            Assert.That(battleState.BattleFlowStage, Is.EqualTo(BattleFlowStage.WaitingForPlayerCard));
            Assert.That(battleState.UsedPlayerCardIds, Is.Empty);
            Assert.That(battleState.EnemySequence, Has.Count.EqualTo(3));
            Assert.That(battleState.RoundResults, Is.Empty);
            Assert.That(battleState.Logs, Has.Count.EqualTo(1));
            Assert.That(battleState.Snapshots, Is.Empty);

            AssertLaneInitialized(battleState.PlayerLane);
            AssertLaneInitialized(battleState.EnemyLane);
        }

        [Test]
        public void StartBattle_PreparesEnemySequenceFromCurrentEnemyContextDeterministically()
        {
            List<CardSpec> fixedDeck = CreateEnemyDeck();
            BattleService battleService = new BattleService(new FixedGameRandom(0, 0, 0, 0, 0));
            EnemyProgressState enemyProgressState = CreateEnemyProgressState(
                battlesPlayed: 0,
                fixedDeck: fixedDeck);

            BattleState battleState = battleService.StartBattle(enemyProgressState);

            Assert.That(ToCardShape(battleState.EnemySequence[0]), Is.EqualTo(ToCardShape(fixedDeck[1])));
            Assert.That(ToCardShape(battleState.EnemySequence[1]), Is.EqualTo(ToCardShape(fixedDeck[2])));
            Assert.That(ToCardShape(battleState.EnemySequence[2]), Is.EqualTo(ToCardShape(fixedDeck[3])));
            Assert.That(ReferenceEquals(battleState.EnemySequence[0], fixedDeck[1]), Is.False);
            Assert.That(ReferenceEquals(battleState.EnemySequence[1], fixedDeck[2]), Is.False);
            Assert.That(ReferenceEquals(battleState.EnemySequence[2], fixedDeck[3]), Is.False);
        }

        [Test]
        public void StartBattle_RepeatedCallsCreateFreshBattleStateObjects()
        {
            EnemyProgressState enemyProgressState = CreateEnemyProgressState(
                battlesPlayed: 0,
                fixedDeck: CreateEnemyDeck());

            BattleState firstBattle = new BattleService(new FixedGameRandom(0, 0, 0, 0, 0)).StartBattle(enemyProgressState);
            BattleState secondBattle = new BattleService(new FixedGameRandom(0, 0, 0, 0, 0)).StartBattle(enemyProgressState);

            Assert.That(ReferenceEquals(firstBattle, secondBattle), Is.False);
            Assert.That(ReferenceEquals(firstBattle.PlayerLane, secondBattle.PlayerLane), Is.False);
            Assert.That(ReferenceEquals(firstBattle.EnemyLane, secondBattle.EnemyLane), Is.False);
            Assert.That(ReferenceEquals(firstBattle.UsedPlayerCardIds, secondBattle.UsedPlayerCardIds), Is.False);
            Assert.That(ReferenceEquals(firstBattle.EnemySequence, secondBattle.EnemySequence), Is.False);
        }

        [Test]
        public void SubmitPlayerCard_WithValidSelection_ResolvesAppliesAndTransitionsToPresentingRoundResult()
        {
            BattleService battleService = new BattleService(new FixedGameRandom(0, 0, 0, 0, 0));
            RoundResolver roundResolver = new RoundResolver();
            RunState runState = CreateRunState(
                playerHp: 9,
                playerMaxHp: 10,
                playerDeck: new List<CardInstance>
                {
                    CreateCardInstance("player-regrow", RpsType.Rock, 4, TraitType.Regrow),
                });
            EnemyProgressState enemyProgressState = CreateEnemyProgressState(
                battlesPlayed: 0,
                fixedDeck: CreateEnemyDeck(),
                currentHp: 9,
                maxHp: 10);
            BattleState battleState = CreateBattleState(
                roundIndex: 1,
                enemySequence: new List<CardSpec>
                {
                    TestConfigFactory.CreateCard(RpsType.Paper, 4, TraitType.Regrow),
                });
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(regrowHeal: 2);

            BattleCommandResult commandResult = battleService.SubmitPlayerCard(
                runState,
                enemyProgressState,
                battleState,
                "player-regrow",
                traitTuning,
                roundResolver);

            Assert.That(commandResult.Success, Is.True);
            Assert.That(commandResult.BattleFlowStage, Is.EqualTo(BattleFlowStage.PresentingRoundResult));
            Assert.That(commandResult.RoundResult, Is.Not.Null);
            Assert.That(commandResult.RoundResult.EnemyCardReference.CardSpec.rpsType, Is.EqualTo(RpsType.Paper));
            Assert.That(commandResult.RoundResult.DamageToPlayer, Is.EqualTo(1));
            Assert.That(commandResult.RoundResult.HealToPlayer, Is.EqualTo(2));
            Assert.That(commandResult.RoundResult.PlayerHpAfter, Is.EqualTo(10));
            Assert.That(commandResult.RoundResult.EnemyHpAfter, Is.EqualTo(10));
            Assert.That(runState.PlayerHp, Is.EqualTo(10));
            Assert.That(enemyProgressState.CurrentHp, Is.EqualTo(10));
            Assert.That(battleState.RoundResults, Has.Count.EqualTo(1));
            Assert.That(battleState.RoundResults[0], Is.SameAs(commandResult.RoundResult));
            Assert.That(battleState.Logs.Count, Is.GreaterThan(0));
            Assert.That(battleState.Snapshots, Has.Count.EqualTo(7));
            Assert.That(battleState.BattleFlowStage, Is.EqualTo(BattleFlowStage.PresentingRoundResult));
            Assert.That(battleState.UsedPlayerCardIds.Contains("player-regrow"), Is.True);
        }

        [Test]
        public void SubmitPlayerCard_UsesPreparedEnemySequenceForCurrentRound()
        {
            BattleService battleService = new BattleService(new FixedGameRandom(0, 0, 0, 0, 0));
            RoundResolver roundResolver = new RoundResolver();
            RunState runState = CreateRunState(
                playerHp: 10,
                playerMaxHp: 10,
                playerDeck: new List<CardInstance>
                {
                    CreateCardInstance("player-rock", RpsType.Rock, 4),
                });
            EnemyProgressState enemyProgressState = CreateEnemyProgressState(
                battlesPlayed: 0,
                fixedDeck: CreateEnemyDeck(),
                currentHp: 10,
                maxHp: 10);
            BattleState battleState = CreateBattleState(
                roundIndex: 2,
                enemySequence: new List<CardSpec>
                {
                    TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                    TestConfigFactory.CreateCard(RpsType.Paper, 4),
                });

            BattleCommandResult commandResult = battleService.SubmitPlayerCard(
                runState,
                enemyProgressState,
                battleState,
                "player-rock",
                TestConfigFactory.CreateValidTraitTuning(),
                roundResolver);

            Assert.That(commandResult.Success, Is.True);
            Assert.That(commandResult.RoundResult.EnemyCardReference.CardSpec.rpsType, Is.EqualTo(RpsType.Paper));
            Assert.That(commandResult.RoundResult.DamageToPlayer, Is.EqualTo(1));
            Assert.That(commandResult.RoundResult.DamageToEnemy, Is.EqualTo(0));
            Assert.That(runState.PlayerHp, Is.EqualTo(9));
        }

        [Test]
        public void SubmitPlayerCard_RejectsMissingPlayerCardWithoutMutatingBattleState()
        {
            BattleService battleService = new BattleService(new FixedGameRandom(0, 0, 0, 0, 0));
            RunState runState = CreateRunState(
                playerHp: 10,
                playerMaxHp: 10,
                playerDeck: new List<CardInstance>
                {
                    CreateCardInstance("player-present", RpsType.Rock, 4),
                });
            EnemyProgressState enemyProgressState = CreateEnemyProgressState(
                battlesPlayed: 0,
                fixedDeck: CreateEnemyDeck());
            BattleState battleState = CreateBattleState(
                roundIndex: 1,
                enemySequence: new List<CardSpec>
                {
                    TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                });

            BattleCommandResult commandResult = battleService.SubmitPlayerCard(
                runState,
                enemyProgressState,
                battleState,
                "missing-card",
                TestConfigFactory.CreateValidTraitTuning(),
                new RoundResolver());

            Assert.That(commandResult.Success, Is.False);
            Assert.That(commandResult.FailureReason, Does.Contain("missing-card"));
            Assert.That(battleState.BattleFlowStage, Is.EqualTo(BattleFlowStage.WaitingForPlayerCard));
            Assert.That(runState.PlayerHp, Is.EqualTo(10));
            Assert.That(enemyProgressState.CurrentHp, Is.EqualTo(20));
            Assert.That(battleState.RoundResults, Is.Empty);
            Assert.That(battleState.Logs, Is.Empty);
            Assert.That(battleState.Snapshots, Is.Empty);
        }

        [Test]
        public void SubmitPlayerCard_RejectsUsedPlayerCard()
        {
            BattleService battleService = new BattleService(new FixedGameRandom(0, 0, 0, 0, 0));
            RunState runState = CreateRunState(
                playerHp: 10,
                playerMaxHp: 10,
                playerDeck: new List<CardInstance>
                {
                    CreateCardInstance("player-used", RpsType.Rock, 4),
                });
            EnemyProgressState enemyProgressState = CreateEnemyProgressState(
                battlesPlayed: 0,
                fixedDeck: CreateEnemyDeck());
            BattleState battleState = CreateBattleState(
                roundIndex: 1,
                enemySequence: new List<CardSpec>
                {
                    TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                });
            battleState.UsedPlayerCardIds.Add("player-used");

            BattleCommandResult commandResult = battleService.SubmitPlayerCard(
                runState,
                enemyProgressState,
                battleState,
                "player-used",
                TestConfigFactory.CreateValidTraitTuning(),
                new RoundResolver());

            Assert.That(commandResult.Success, Is.False);
            Assert.That(commandResult.FailureReason, Does.Contain("already been used"));
            Assert.That(battleState.BattleFlowStage, Is.EqualTo(BattleFlowStage.WaitingForPlayerCard));
            Assert.That(battleState.RoundResults, Is.Empty);
        }

        [Test]
        public void SubmitPlayerCard_RejectsWhenBattleFlowIsNotWaitingForPlayerCard()
        {
            BattleService battleService = new BattleService(new FixedGameRandom(0, 0, 0, 0, 0));
            RunState runState = CreateRunState(
                playerHp: 10,
                playerMaxHp: 10,
                playerDeck: new List<CardInstance>
                {
                    CreateCardInstance("player-card", RpsType.Rock, 4),
                });
            EnemyProgressState enemyProgressState = CreateEnemyProgressState(
                battlesPlayed: 0,
                fixedDeck: CreateEnemyDeck());
            BattleState battleState = CreateBattleState(
                roundIndex: 1,
                enemySequence: new List<CardSpec>
                {
                    TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                });
            battleState.BattleFlowStage = BattleFlowStage.ResolvingRound;

            BattleCommandResult commandResult = battleService.SubmitPlayerCard(
                runState,
                enemyProgressState,
                battleState,
                "player-card",
                TestConfigFactory.CreateValidTraitTuning(),
                new RoundResolver());

            Assert.That(commandResult.Success, Is.False);
            Assert.That(commandResult.FailureReason, Does.Contain("not currently waiting"));
            Assert.That(battleState.BattleFlowStage, Is.EqualTo(BattleFlowStage.ResolvingRound));
            Assert.That(battleState.RoundResults, Is.Empty);
        }

        [Test]
        public void SubmitPlayerCard_RejectsInvalidCurrentRoundEnemySequenceIndex()
        {
            BattleService battleService = new BattleService(new FixedGameRandom(0, 0, 0, 0, 0));
            RunState runState = CreateRunState(
                playerHp: 10,
                playerMaxHp: 10,
                playerDeck: new List<CardInstance>
                {
                    CreateCardInstance("player-card", RpsType.Rock, 4),
                });
            EnemyProgressState enemyProgressState = CreateEnemyProgressState(
                battlesPlayed: 0,
                fixedDeck: CreateEnemyDeck());
            BattleState battleState = CreateBattleState(
                roundIndex: 3,
                enemySequence: new List<CardSpec>
                {
                    TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                });

            BattleCommandResult commandResult = battleService.SubmitPlayerCard(
                runState,
                enemyProgressState,
                battleState,
                "player-card",
                TestConfigFactory.CreateValidTraitTuning(),
                new RoundResolver());

            Assert.That(commandResult.Success, Is.False);
            Assert.That(commandResult.FailureReason, Does.Contain("Round index"));
            Assert.That(runState.PlayerHp, Is.EqualTo(10));
            Assert.That(enemyProgressState.CurrentHp, Is.EqualTo(20));
            Assert.That(battleState.RoundResults, Is.Empty);
        }

        private static void AssertLaneInitialized(LaneState laneState)
        {
            Assert.That(laneState, Is.Not.Null);
            Assert.That(laneState.Slots, Has.Count.EqualTo(3));

            for (int slotIndex = 0; slotIndex < laneState.Slots.Count; slotIndex++)
            {
                Assert.That(laneState.Slots[slotIndex].Index, Is.EqualTo(slotIndex));
                Assert.That(laneState.Slots[slotIndex].IsOpen, Is.False);
                Assert.That(laneState.Slots[slotIndex].Occupant, Is.Null);
            }
        }

        private static EnemyProgressState CreateEnemyProgressState(int battlesPlayed, List<CardSpec> fixedDeck)
        {
            return CreateEnemyProgressState(battlesPlayed, fixedDeck, currentHp: 20, maxHp: 20);
        }

        private static EnemyProgressState CreateEnemyProgressState(int battlesPlayed, List<CardSpec> fixedDeck, int currentHp, int maxHp)
        {
            return new EnemyProgressState
            {
                Config = TestConfigFactory.CreateValidEnemyConfig(
                    enemyId: "enemy-alpha",
                    displayName: "Enemy Alpha",
                    maxHp: maxHp,
                    fixedDeck: fixedDeck),
                CurrentHp = currentHp,
                MaxHp = maxHp,
                BattlesPlayed = battlesPlayed,
                RewardsClaimed = 0,
            };
        }

        private static RunState CreateRunState(int playerHp, int playerMaxHp, List<CardInstance> playerDeck)
        {
            return new RunState
            {
                PlayerHp = playerHp,
                PlayerMaxHp = playerMaxHp,
                PlayerDeck = playerDeck,
                CurrentEnemyIndex = 0,
                CurrentEnemy = null,
                ActiveBattle = null,
                PendingRewardOffer = null,
                FlowStage = RunFlowStage.InBattle,
            };
        }

        private static BattleState CreateBattleState(int roundIndex, List<CardSpec> enemySequence)
        {
            return new BattleState
            {
                BattleIndexForEnemy = 1,
                RoundIndex = roundIndex,
                PlayerLane = CreateLane(),
                EnemyLane = CreateLane(),
                UsedPlayerCardIds = new HashSet<string>(),
                EnemySequence = enemySequence,
                RoundResults = new List<RoundResult>(),
                Logs = new List<string>(),
                Snapshots = new List<PhaseSnapshot>(),
                BattleFlowStage = BattleFlowStage.WaitingForPlayerCard,
            };
        }

        private static LaneState CreateLane()
        {
            return new LaneState
            {
                Slots = new List<BoardSlotState>
                {
                    new BoardSlotState { Index = 0, IsOpen = false, Occupant = null },
                    new BoardSlotState { Index = 1, IsOpen = false, Occupant = null },
                    new BoardSlotState { Index = 2, IsOpen = false, Occupant = null },
                },
            };
        }

        private static List<CardSpec> CreateEnemyDeck()
        {
            return new List<CardSpec>
            {
                TestConfigFactory.CreateCard(RpsType.Rock, 4, TraitType.Empower),
                TestConfigFactory.CreateCard(RpsType.Scissors, 5, TraitType.ShiftLeft),
                TestConfigFactory.CreateCard(RpsType.Paper, 6, TraitType.Suppress),
                TestConfigFactory.CreateCard(RpsType.Rock, 7, TraitType.Regrow),
                TestConfigFactory.CreateCard(RpsType.Scissors, 8, TraitType.Growth),
                TestConfigFactory.CreateCard(RpsType.Paper, 9, TraitType.Lifesteal),
            };
        }

        private static string ToCardShape(CardSpec cardSpec)
        {
            return $"{cardSpec.rpsType}:{cardSpec.basePower}:{string.Join(",", cardSpec.traits)}";
        }

        private static CardInstance CreateCardInstance(
            string instanceId,
            RpsType rpsType,
            int basePower,
            params TraitType[] traits)
        {
            return new CardInstance
            {
                InstanceId = instanceId,
                RpsType = rpsType,
                BasePower = basePower,
                PermanentPowerBonus = 0,
                Traits = new List<TraitType>(traits),
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
