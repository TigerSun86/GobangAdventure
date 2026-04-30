using System.Collections.Generic;
using BR3.Application;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Random;
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
            return new EnemyProgressState
            {
                Config = TestConfigFactory.CreateValidEnemyConfig(
                    enemyId: "enemy-alpha",
                    displayName: "Enemy Alpha",
                    maxHp: 20,
                    fixedDeck: fixedDeck),
                CurrentHp = 20,
                MaxHp = 20,
                BattlesPlayed = battlesPlayed,
                RewardsClaimed = 0,
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
