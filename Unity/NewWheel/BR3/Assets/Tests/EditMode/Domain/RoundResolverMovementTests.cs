using System.Collections.Generic;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Rules;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Domain
{
    public sealed class RoundResolverMovementTests
    {
        private static readonly TraitTuning TraitTuning = TestConfigFactory.CreateValidTraitTuning();

        [Test]
        public void ResolveRound_ShiftLeftMovesLeftWhenFriendlyCardExistsOnTheLeft()
        {
            RoundResolver resolver = new RoundResolver();
            BattleState battleState = CreateBattleState(roundIndex: 2);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("player-left", RpsType.Rock, 4, BoardSide.Player, enterRoundIndex: 1);
            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-left", RpsType.Rock, 4, BoardSide.Enemy, enterRoundIndex: 1);

            CardInstance newPlayerCard = CreateCardInstance("player-shift-left", RpsType.Paper, 4, TraitType.ShiftLeft);
            CardSpec newEnemyCard = TestConfigFactory.CreateCard(RpsType.Paper, 4);

            resolver.ResolveRound(battleState, newPlayerCard, newEnemyCard, TraitTuning, playerHp: 10, enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.SourceCard.InstanceId, Is.EqualTo("player-shift-left"));
            Assert.That(battleState.PlayerLane.Slots[1].Occupant.SourceCard.InstanceId, Is.EqualTo("player-left"));
        }

        [Test]
        public void ResolveRound_ShiftRightMovesRightWhenFriendlyCardExistsOnTheRight()
        {
            RoundResolver resolver = new RoundResolver();
            BattleState battleState = CreateBattleState(roundIndex: 2);

            battleState.PlayerLane.Slots[2].IsOpen = true;
            battleState.PlayerLane.Slots[2].Occupant = CreateBoardCard("player-right", RpsType.Rock, 4, BoardSide.Player, enterRoundIndex: 1);
            battleState.EnemyLane.Slots[2].IsOpen = true;
            battleState.EnemyLane.Slots[2].Occupant = CreateBoardCard("enemy-right", RpsType.Rock, 4, BoardSide.Enemy, enterRoundIndex: 1);

            CardInstance newPlayerCard = CreateCardInstance("player-shift-right", RpsType.Paper, 4, TraitType.ShiftRight);
            CardSpec newEnemyCard = TestConfigFactory.CreateCard(RpsType.Paper, 4);

            resolver.ResolveRound(battleState, newPlayerCard, newEnemyCard, TraitTuning, playerHp: 10, enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[1].Occupant.SourceCard.InstanceId, Is.EqualTo("player-right"));
            Assert.That(battleState.PlayerLane.Slots[2].Occupant.SourceCard.InstanceId, Is.EqualTo("player-shift-right"));
        }

        [Test]
        public void ResolveRound_MovementUsesRightToLeftProcessingOrder()
        {
            RoundResolver resolver = new RoundResolver();
            BattleState battleState = CreateBattleState(roundIndex: 3);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("player-anchor", RpsType.Rock, 4, BoardSide.Player, enterRoundIndex: 1);
            battleState.PlayerLane.Slots[1].IsOpen = true;
            battleState.PlayerLane.Slots[1].Occupant = CreateBoardCard("player-middle", RpsType.Paper, 4, BoardSide.Player, enterRoundIndex: 2, TraitType.ShiftLeft);

            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-0", RpsType.Rock, 4, BoardSide.Enemy, enterRoundIndex: 1);
            battleState.EnemyLane.Slots[1].IsOpen = true;
            battleState.EnemyLane.Slots[1].Occupant = CreateBoardCard("enemy-1", RpsType.Paper, 4, BoardSide.Enemy, enterRoundIndex: 2);

            CardInstance newPlayerCard = CreateCardInstance("player-new", RpsType.Scissors, 4, TraitType.ShiftLeft);
            CardSpec newEnemyCard = TestConfigFactory.CreateCard(RpsType.Scissors, 4);

            resolver.ResolveRound(battleState, newPlayerCard, newEnemyCard, TraitTuning, playerHp: 10, enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.SourceCard.InstanceId, Is.EqualTo("player-new"));
            Assert.That(battleState.PlayerLane.Slots[1].Occupant.SourceCard.InstanceId, Is.EqualTo("player-anchor"));
            Assert.That(battleState.PlayerLane.Slots[2].Occupant.SourceCard.InstanceId, Is.EqualTo("player-middle"));
        }

        [Test]
        public void ResolveRound_MovementPersistsAcrossRounds()
        {
            RoundResolver resolver = new RoundResolver();
            BattleState battleState = CreateBattleState(roundIndex: 2);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("player-existing", RpsType.Rock, 4, BoardSide.Player, enterRoundIndex: 1);
            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-existing", RpsType.Rock, 4, BoardSide.Enemy, enterRoundIndex: 1);

            resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-round-2", RpsType.Paper, 4, TraitType.ShiftLeft),
                TestConfigFactory.CreateCard(RpsType.Paper, 4),
                TraitTuning,
                playerHp: 10,
                enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.SourceCard.InstanceId, Is.EqualTo("player-round-2"));
            Assert.That(battleState.PlayerLane.Slots[1].Occupant.SourceCard.InstanceId, Is.EqualTo("player-existing"));

            battleState.RoundIndex = 3;
            resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-round-3", RpsType.Scissors, 4),
                TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                TraitTuning,
                playerHp: 10,
                enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.SourceCard.InstanceId, Is.EqualTo("player-round-2"));
            Assert.That(battleState.PlayerLane.Slots[1].Occupant.SourceCard.InstanceId, Is.EqualTo("player-existing"));
            Assert.That(battleState.PlayerLane.Slots[2].Occupant.SourceCard.InstanceId, Is.EqualTo("player-round-3"));
        }

        [Test]
        public void ResolveRound_CardsDoNotMovePastLaneBoundaries()
        {
            RoundResolver resolver = new RoundResolver();
            BattleState battleState = CreateBattleState(roundIndex: 3);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("player-left-edge", RpsType.Rock, 4, BoardSide.Player, enterRoundIndex: 1, TraitType.ShiftLeft);
            battleState.PlayerLane.Slots[1].IsOpen = true;
            battleState.PlayerLane.Slots[1].Occupant = CreateBoardCard("player-middle", RpsType.Paper, 4, BoardSide.Player, enterRoundIndex: 2);

            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-left", RpsType.Rock, 4, BoardSide.Enemy, enterRoundIndex: 1);
            battleState.EnemyLane.Slots[1].IsOpen = true;
            battleState.EnemyLane.Slots[1].Occupant = CreateBoardCard("enemy-middle", RpsType.Paper, 4, BoardSide.Enemy, enterRoundIndex: 2);

            CardInstance newPlayerCard = CreateCardInstance("player-right-edge", RpsType.Scissors, 4, TraitType.ShiftRight);
            CardSpec newEnemyCard = TestConfigFactory.CreateCard(RpsType.Scissors, 4);

            resolver.ResolveRound(battleState, newPlayerCard, newEnemyCard, TraitTuning, playerHp: 10, enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.SourceCard.InstanceId, Is.EqualTo("player-left-edge"));
            Assert.That(battleState.PlayerLane.Slots[2].Occupant.SourceCard.InstanceId, Is.EqualTo("player-right-edge"));
        }

        [Test]
        public void ResolveRound_MovementIsResolvedIndependentlyPerSide()
        {
            RoundResolver resolver = new RoundResolver();
            BattleState battleState = CreateBattleState(roundIndex: 2);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("player-left", RpsType.Rock, 4, BoardSide.Player, enterRoundIndex: 1);

            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-left", RpsType.Rock, 4, BoardSide.Enemy, enterRoundIndex: 1);

            CardInstance newPlayerCard = CreateCardInstance("player-shift-left", RpsType.Paper, 4, TraitType.ShiftLeft);
            CardSpec newEnemyCard = TestConfigFactory.CreateCard(RpsType.Paper, 4);

            resolver.ResolveRound(battleState, newPlayerCard, newEnemyCard, TraitTuning, playerHp: 10, enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.SourceCard.InstanceId, Is.EqualTo("player-shift-left"));
            Assert.That(battleState.PlayerLane.Slots[1].Occupant.SourceCard.InstanceId, Is.EqualTo("player-left"));
            Assert.That(battleState.EnemyLane.Slots[0].Occupant.SourceCard.InstanceId, Is.EqualTo("enemy-left"));
            Assert.That(battleState.EnemyLane.Slots[1].Occupant.SourceCard.InstanceId, Is.EqualTo("enemy-round-2"));
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
