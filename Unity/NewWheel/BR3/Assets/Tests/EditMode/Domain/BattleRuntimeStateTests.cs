using System.Collections.Generic;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Domain
{
    public sealed class BattleRuntimeStateTests
    {
        [Test]
        public void BattleState_CanCarryExpectedBattleRuntimeShape()
        {
            BattleState battleState = new BattleState
            {
                BattleIndexForEnemy = 2,
                RoundIndex = 1,
                PlayerLane = CreateLane(),
                EnemyLane = CreateLane(),
                UsedPlayerCardIds = new HashSet<string> { "card-0001" },
                EnemySequence = new List<CardSpec>
                {
                    TestConfigFactory.CreateCard(RpsType.Rock, 4),
                    TestConfigFactory.CreateCard(RpsType.Scissors, 5),
                    TestConfigFactory.CreateCard(RpsType.Paper, 4),
                },
                BattleFlowStage = BattleFlowStage.WaitingForPlayerCard,
            };

            Assert.That(battleState.PlayerLane.Slots, Has.Count.EqualTo(3));
            Assert.That(battleState.EnemyLane.Slots, Has.Count.EqualTo(3));
            Assert.That(battleState.UsedPlayerCardIds.Contains("card-0001"), Is.True);
            Assert.That(battleState.EnemySequence, Has.Count.EqualTo(3));
            Assert.That(battleState.BattleFlowStage, Is.EqualTo(BattleFlowStage.WaitingForPlayerCard));
        }

        [Test]
        public void BoardSlotState_DistinguishesClosedOpenEmptyAndOccupied()
        {
            BoardSlotState closedSlot = new BoardSlotState
            {
                Index = 0,
                IsOpen = false,
                Occupant = null,
            };

            BoardSlotState openEmptySlot = new BoardSlotState
            {
                Index = 1,
                IsOpen = true,
                Occupant = null,
            };

            BoardSlotState occupiedSlot = new BoardSlotState
            {
                Index = 2,
                IsOpen = true,
                Occupant = new BoardCard
                {
                    SourceCard = new CardInstance { InstanceId = "card-0001" },
                    Side = BoardSide.Player,
                },
            };

            Assert.That(closedSlot.IsOpen, Is.False);
            Assert.That(closedSlot.Occupant, Is.Null);
            Assert.That(openEmptySlot.IsOpen, Is.True);
            Assert.That(openEmptySlot.Occupant, Is.Null);
            Assert.That(occupiedSlot.IsOpen, Is.True);
            Assert.That(occupiedSlot.Occupant, Is.Not.Null);
        }

        [Test]
        public void BoardCard_ReferencesSourceCardAndBattleLocalValues()
        {
            CardInstance sourceCard = new CardInstance
            {
                InstanceId = "card-0002",
                RpsType = RpsType.Paper,
                BasePower = 4,
                Traits = new List<TraitType> { TraitType.Empower },
                PermanentPowerBonus = 1,
            };

            BoardCard boardCard = new BoardCard
            {
                SourceCard = sourceCard,
                Side = BoardSide.Enemy,
                EnterRoundIndex = 2,
                FixedSelfPower = 8,
                CurrentPower = 10,
                DamageDealtThisRound = 3,
            };

            Assert.That(boardCard.SourceCard, Is.SameAs(sourceCard));
            Assert.That(boardCard.Side, Is.EqualTo(BoardSide.Enemy));
            Assert.That(boardCard.EnterRoundIndex, Is.EqualTo(2));
            Assert.That(boardCard.FixedSelfPower, Is.EqualTo(8));
            Assert.That(boardCard.CurrentPower, Is.EqualTo(10));
            Assert.That(boardCard.DamageDealtThisRound, Is.EqualTo(3));
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
    }
}
