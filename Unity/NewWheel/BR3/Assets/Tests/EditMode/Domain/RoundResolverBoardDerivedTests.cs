using System.Collections.Generic;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Rules;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Domain
{
    public sealed class RoundResolverBoardDerivedTests
    {
        [Test]
        public void ResolveRound_AdjacentAidBuffsAdjacentAlliesWithoutBuffingItself()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(adjacentAidBonus: 5);
            BattleState battleState = CreateBattleState(roundIndex: 3);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("player-left", RpsType.Rock, 4, BoardSide.Player, 1);
            battleState.PlayerLane.Slots[1].IsOpen = true;
            battleState.PlayerLane.Slots[1].Occupant = CreateBoardCard("player-aid", RpsType.Paper, 4, BoardSide.Player, 2, TraitType.AdjacentAid);

            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-left", RpsType.Rock, 4, BoardSide.Enemy, 1);
            battleState.EnemyLane.Slots[1].IsOpen = true;
            battleState.EnemyLane.Slots[1].Occupant = CreateBoardCard("enemy-middle", RpsType.Paper, 4, BoardSide.Enemy, 2);

            resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-right", RpsType.Scissors, 4),
                TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                traitTuning,
                playerHp: 10,
                enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.CurrentPower, Is.EqualTo(9));
            Assert.That(battleState.PlayerLane.Slots[1].Occupant.CurrentPower, Is.EqualTo(4));
            Assert.That(battleState.PlayerLane.Slots[2].Occupant.CurrentPower, Is.EqualTo(9));
        }

        [Test]
        public void ResolveRound_SuppressAppliesConfiguredPenaltyToOpposingOccupiedSlot()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(suppressPenalty: 4);
            BattleState battleState = CreateBattleState(roundIndex: 1);

            resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-suppress", RpsType.Rock, 6, TraitType.Suppress),
                TestConfigFactory.CreateCard(RpsType.Scissors, 6),
                traitTuning,
                playerHp: 10,
                enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.CurrentPower, Is.EqualTo(6));
            Assert.That(battleState.EnemyLane.Slots[0].Occupant.CurrentPower, Is.EqualTo(2));
        }

        [Test]
        public void ResolveRound_BoardDerivedUsesPostMovementBoardPositions()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(adjacentAidBonus: 3);
            BattleState battleState = CreateBattleState(roundIndex: 3);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("player-left", RpsType.Rock, 4, BoardSide.Player, 1);
            battleState.PlayerLane.Slots[1].IsOpen = true;
            battleState.PlayerLane.Slots[1].Occupant = CreateBoardCard("player-center", RpsType.Paper, 4, BoardSide.Player, 2);

            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-left", RpsType.Rock, 4, BoardSide.Enemy, 1);
            battleState.EnemyLane.Slots[1].IsOpen = true;
            battleState.EnemyLane.Slots[1].Occupant = CreateBoardCard("enemy-center", RpsType.Paper, 4, BoardSide.Enemy, 2);

            resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-moving-aid", RpsType.Scissors, 4, TraitType.ShiftLeft, TraitType.AdjacentAid),
                TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                traitTuning,
                playerHp: 10,
                enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.SourceCard.InstanceId, Is.EqualTo("player-moving-aid"));
            Assert.That(battleState.PlayerLane.Slots[0].Occupant.CurrentPower, Is.EqualTo(4));
            Assert.That(battleState.PlayerLane.Slots[1].Occupant.CurrentPower, Is.EqualTo(7));
            Assert.That(battleState.PlayerLane.Slots[2].Occupant.CurrentPower, Is.EqualTo(4));
        }

        [Test]
        public void ResolveRound_BoardDerivedAppliesDeltasWithoutSamePhaseOrderDependence()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(adjacentAidBonus: 2);
            BattleState battleState = CreateBattleState(roundIndex: 3);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("player-left-aid", RpsType.Rock, 4, BoardSide.Player, 1, TraitType.AdjacentAid);
            battleState.PlayerLane.Slots[1].IsOpen = true;
            battleState.PlayerLane.Slots[1].Occupant = CreateBoardCard("player-middle", RpsType.Paper, 4, BoardSide.Player, 2);

            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-left", RpsType.Rock, 4, BoardSide.Enemy, 1);
            battleState.EnemyLane.Slots[1].IsOpen = true;
            battleState.EnemyLane.Slots[1].Occupant = CreateBoardCard("enemy-middle", RpsType.Paper, 4, BoardSide.Enemy, 2);

            resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-right-aid", RpsType.Scissors, 4, TraitType.AdjacentAid),
                TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                traitTuning,
                playerHp: 10,
                enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.CurrentPower, Is.EqualTo(4));
            Assert.That(battleState.PlayerLane.Slots[1].Occupant.CurrentPower, Is.EqualTo(8));
            Assert.That(battleState.PlayerLane.Slots[2].Occupant.CurrentPower, Is.EqualTo(4));
        }

        [Test]
        public void ResolveRound_AdjacentAidAndSuppressContributeCombinedDeltas()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(adjacentAidBonus: 2, suppressPenalty: 3);
            BattleState battleState = CreateBattleState(roundIndex: 2);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("player-target", RpsType.Rock, 5, BoardSide.Player, 1);
            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-suppress", RpsType.Rock, 5, BoardSide.Enemy, 1, TraitType.Suppress);

            resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-aid", RpsType.Paper, 4, TraitType.AdjacentAid),
                TestConfigFactory.CreateCard(RpsType.Paper, 6),
                traitTuning,
                playerHp: 10,
                enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.CurrentPower, Is.EqualTo(4));
            Assert.That(battleState.PlayerLane.Slots[1].Occupant.CurrentPower, Is.EqualTo(4));
            Assert.That(battleState.EnemyLane.Slots[0].Occupant.CurrentPower, Is.EqualTo(5));
            Assert.That(battleState.EnemyLane.Slots[1].Occupant.CurrentPower, Is.EqualTo(6));
        }

        [Test]
        public void ResolveRound_SuppressHasNoEffectWhenOpposingSlotIsEmpty()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(suppressPenalty: 9);
            BattleState battleState = CreateBattleState(roundIndex: 2);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("player-existing-suppress", RpsType.Rock, 4, BoardSide.Player, 1, TraitType.Suppress);

            resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-round-2", RpsType.Paper, 4),
                TestConfigFactory.CreateCard(RpsType.Paper, 4),
                traitTuning,
                playerHp: 10,
                enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.CurrentPower, Is.EqualTo(4));
            Assert.That(battleState.EnemyLane.Slots[0].Occupant, Is.Null);
            Assert.That(battleState.PlayerLane.Slots[1].Occupant.CurrentPower, Is.EqualTo(4));
            Assert.That(battleState.EnemyLane.Slots[1].Occupant.CurrentPower, Is.EqualTo(4));
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
