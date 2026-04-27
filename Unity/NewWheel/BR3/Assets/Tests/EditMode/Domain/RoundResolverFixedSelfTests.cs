using System.Collections.Generic;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Rules;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Domain
{
    public sealed class RoundResolverFixedSelfTests
    {
        [Test]
        public void ResolveRound_FixedSelfRecalculatesEmpowerAndPermanentBonusEachRound()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(empowerBonus: 3);
            BattleState battleState = CreateBattleState(roundIndex: 2);

            BoardCard existingPlayerBoardCard = CreateBoardCard(
                "card-existing-player",
                RpsType.Rock,
                4,
                BoardSide.Player,
                enterRoundIndex: 1,
                permanentPowerBonus: 2,
                currentPower: 999,
                damageDealtThisRound: 5,
                TraitType.Empower);

            BoardCard existingEnemyBoardCard = CreateBoardCard(
                "card-existing-enemy",
                RpsType.Rock,
                4,
                BoardSide.Enemy,
                enterRoundIndex: 1,
                permanentPowerBonus: 1,
                currentPower: 777,
                damageDealtThisRound: 4,
                TraitType.Empower);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = existingPlayerBoardCard;
            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = existingEnemyBoardCard;

            CardInstance newPlayerCard = CreateCardInstance("card-new-player", RpsType.Paper, 4, 1, TraitType.Empower);
            CardSpec newEnemyCard = TestConfigFactory.CreateCard(RpsType.Paper, 4, TraitType.Empower);

            resolver.ResolveRound(battleState, newPlayerCard, newEnemyCard, traitTuning, playerHp: 10, enemyHp: 10);

            Assert.That(existingPlayerBoardCard.FixedSelfPower, Is.EqualTo(9));
            Assert.That(existingPlayerBoardCard.CurrentPower, Is.EqualTo(9));
            Assert.That(existingPlayerBoardCard.DamageDealtThisRound, Is.EqualTo(0));

            Assert.That(existingEnemyBoardCard.FixedSelfPower, Is.EqualTo(8));
            Assert.That(existingEnemyBoardCard.CurrentPower, Is.EqualTo(8));
            Assert.That(existingEnemyBoardCard.DamageDealtThisRound, Is.EqualTo(0));

            Assert.That(battleState.PlayerLane.Slots[1].Occupant.FixedSelfPower, Is.EqualTo(8));
            Assert.That(battleState.EnemyLane.Slots[1].Occupant.FixedSelfPower, Is.EqualTo(7));
            Assert.That(battleState.PlayerLane.Slots[1].Occupant.DamageDealtThisRound, Is.EqualTo(0));
            Assert.That(battleState.EnemyLane.Slots[1].Occupant.DamageDealtThisRound, Is.EqualTo(0));
        }

        [Test]
        public void ResolveRound_FixedSelfDoesNotAddExtraPowerWithoutEmpower()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(empowerBonus: 3);
            BattleState battleState = CreateBattleState(roundIndex: 1);
            CardInstance playerCard = CreateCardInstance("card-player", RpsType.Rock, 4, 2);
            CardSpec enemyCard = TestConfigFactory.CreateCard(RpsType.Scissors, 4);

            resolver.ResolveRound(battleState, playerCard, enemyCard, traitTuning, playerHp: 10, enemyHp: 10);

            Assert.That(battleState.PlayerLane.Slots[0].Occupant.FixedSelfPower, Is.EqualTo(6));
            Assert.That(battleState.PlayerLane.Slots[0].Occupant.CurrentPower, Is.EqualTo(6));
            Assert.That(battleState.EnemyLane.Slots[0].Occupant.FixedSelfPower, Is.EqualTo(4));
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
            int permanentPowerBonus,
            params TraitType[] traits)
        {
            return new CardInstance
            {
                InstanceId = instanceId,
                RpsType = rpsType,
                BasePower = basePower,
                PermanentPowerBonus = permanentPowerBonus,
                Traits = new List<TraitType>(traits),
            };
        }

        private static BoardCard CreateBoardCard(
            string instanceId,
            RpsType rpsType,
            int basePower,
            BoardSide side,
            int enterRoundIndex,
            int permanentPowerBonus,
            int currentPower,
            int damageDealtThisRound,
            params TraitType[] traits)
        {
            return new BoardCard
            {
                SourceCard = CreateCardInstance(instanceId, rpsType, basePower, permanentPowerBonus, traits),
                Side = side,
                EnterRoundIndex = enterRoundIndex,
                FixedSelfPower = -1,
                CurrentPower = currentPower,
                DamageDealtThisRound = damageDealtThisRound,
            };
        }
    }
}
