using System.Collections.Generic;
using System.Linq;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Results;
using BR3.Domain.Rules;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Domain
{
    public sealed class RoundResolverSnapshotTests
    {
        [Test]
        public void ResolveRound_CapturesExactlyOneSnapshotPerPhaseInLockedOrder()
        {
            RoundResult roundResult = ResolveRoundWithStateChanges();

            RoundPhase[] phases = roundResult.Snapshots.Select(snapshot => snapshot.Phase).ToArray();

            Assert.That(roundResult.Snapshots, Has.Count.EqualTo(7));
            Assert.That(
                phases,
                Is.EqualTo(new[]
                {
                    RoundPhase.Enter,
                    RoundPhase.FixedSelf,
                    RoundPhase.Movement,
                    RoundPhase.BoardDerived,
                    RoundPhase.ResolveOpenSlots,
                    RoundPhase.ApplyMergedDamage,
                    RoundPhase.PostResolve,
                }));
        }

        [Test]
        public void ResolveRound_PopulatesReadableSnapshotsAndStableLogs()
        {
            RoundResult roundResult = ResolveRoundWithStateChanges();

            Assert.That(roundResult.Snapshots, Is.Not.Empty);
            Assert.That(roundResult.Logs, Is.Not.Empty);
            Assert.That(roundResult.Logs[0], Does.StartWith("Enter:"));
            Assert.That(roundResult.Logs.Any(log => log.StartsWith("FixedSelf:")), Is.True);
            Assert.That(roundResult.Logs.Any(log => log.StartsWith("Movement:")), Is.True);
            Assert.That(roundResult.Logs.Any(log => log.StartsWith("BoardDerived:")), Is.True);
            Assert.That(roundResult.Logs.Any(log => log.StartsWith("ResolveOpenSlots:")), Is.True);
            Assert.That(roundResult.Logs.Any(log => log.StartsWith("ApplyMergedDamage:")), Is.True);
            Assert.That(roundResult.Logs.Any(log => log.StartsWith("PostResolve:")), Is.True);

            foreach (PhaseSnapshot snapshot in roundResult.Snapshots)
            {
                Assert.That(snapshot.PlayerLaneStateText, Is.Not.Null.And.Not.Empty);
                Assert.That(snapshot.EnemyLaneStateText, Is.Not.Null.And.Not.Empty);
                Assert.That(snapshot.PlayerLaneStateText, Does.Contain("[0:"));
                Assert.That(snapshot.EnemyLaneStateText, Does.Contain("[0:"));
            }
        }

        [Test]
        public void ResolveRound_SnapshotsAndSlotResultsReflectPhaseProgression()
        {
            RoundResult roundResult = ResolveRoundWithStateChanges();

            Assert.That(roundResult.SlotResults, Has.Count.EqualTo(2));
            Assert.That(roundResult.SlotResults[0].SlotIndex, Is.EqualTo(0));
            Assert.That(roundResult.SlotResults[1].SlotIndex, Is.EqualTo(1));
            Assert.That(roundResult.SlotResults[0].PlayerCardInstanceId, Is.Not.Null.And.Not.Empty);
            Assert.That(roundResult.SlotResults[0].EnemyCardReference, Is.Not.Null);
            Assert.That(roundResult.SlotResults[1].PlayerCardInstanceId, Is.Not.Null.And.Not.Empty);
            Assert.That(roundResult.SlotResults[1].EnemyCardReference, Is.Not.Null);

            Assert.That(roundResult.Snapshots[0].PlayerLaneStateText, Is.Not.EqualTo(roundResult.Snapshots[1].PlayerLaneStateText));
            Assert.That(roundResult.Snapshots[1].PlayerLaneStateText, Is.Not.EqualTo(roundResult.Snapshots[2].PlayerLaneStateText));
            Assert.That(roundResult.Snapshots[2].PlayerLaneStateText, Is.Not.EqualTo(roundResult.Snapshots[3].PlayerLaneStateText));
            Assert.That(roundResult.Snapshots[4].PlayerLaneStateText, Does.Contain("damage=1"));
            Assert.That(roundResult.Snapshots[6].PlayerLaneStateText, Does.Contain("perm=1"));
        }

        private static RoundResult ResolveRoundWithStateChanges()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(
                adjacentAidBonus: 2,
                suppressPenalty: 2,
                growthBonus: 1);
            BattleState battleState = CreateBattleState(roundIndex: 2);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("player-anchor", RpsType.Rock, 4, BoardSide.Player, 1);
            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-suppress", RpsType.Rock, 4, BoardSide.Enemy, 1, TraitType.Suppress);

            return resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-moving-aid-growth", RpsType.Paper, 5, TraitType.ShiftLeft, TraitType.AdjacentAid, TraitType.Growth),
                TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                traitTuning,
                playerHp: 10,
                enemyHp: 10,
                playerMaxHp: 10,
                enemyMaxHp: 10);
        }

        private static BattleState CreateBattleState(int roundIndex)
        {
            return new BattleState
            {
                RoundIndex = roundIndex,
                PlayerLane = CreateLane(),
                EnemyLane = CreateLane(),
                UsedPlayerCardIds = new HashSet<string>(),
                EnemySequence = new List<CardSpec>(),
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

        private static BoardCard CreateBoardCard(
            string instanceId,
            RpsType rpsType,
            int basePower,
            BoardSide side,
            int enterRoundIndex,
            params TraitType[] traits)
        {
            return new BoardCard
            {
                SourceCard = CreateCardInstance(instanceId, rpsType, basePower, traits),
                Side = side,
                EnterRoundIndex = enterRoundIndex,
                FixedSelfPower = basePower,
                CurrentPower = basePower,
                DamageDealtThisRound = 0,
            };
        }
    }
}
