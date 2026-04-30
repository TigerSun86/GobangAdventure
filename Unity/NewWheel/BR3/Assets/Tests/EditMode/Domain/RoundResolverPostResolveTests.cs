using System.Collections.Generic;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Results;
using BR3.Domain.Rules;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Domain
{
    public sealed class RoundResolverPostResolveTests
    {
        [Test]
        public void ResolveRound_RegrowContributesConfiguredRawHealingWithoutChangingResolverHpAfter()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(regrowHeal: 3);
            BattleState battleState = CreateBattleState(roundIndex: 1);

            RoundResult roundResult = resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-regrow", RpsType.Rock, 4, TraitType.Regrow),
                TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                traitTuning,
                playerHp: 8,
                enemyHp: 10);

            Assert.That(roundResult.HealToPlayer, Is.EqualTo(3));
            Assert.That(roundResult.PlayerHpAfter, Is.Null);
        }

        [Test]
        public void ResolveRound_LifestealHealsByNewlyPlayedCardsOwnDamage()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning();
            BattleState battleState = CreateBattleState(roundIndex: 1);

            RoundResult roundResult = resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-lifesteal", RpsType.Rock, 6, TraitType.Lifesteal),
                TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                traitTuning,
                playerHp: 5,
                enemyHp: 10);

            Assert.That(roundResult.DamageToEnemy, Is.EqualTo(2));
            Assert.That(roundResult.HealToPlayer, Is.EqualTo(2));
            Assert.That(roundResult.PlayerHpAfter, Is.Null);
        }

        [Test]
        public void ResolveRound_LifestealDoesNotUseTotalTeamDamage()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning();
            BattleState battleState = CreateBattleState(roundIndex: 2);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("player-older-winner", RpsType.Rock, 4, BoardSide.Player, 1);
            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-older-loser", RpsType.Scissors, 4, BoardSide.Enemy, 1);

            RoundResult roundResult = resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-new-lifesteal", RpsType.Paper, 4, TraitType.Lifesteal),
                TestConfigFactory.CreateCard(RpsType.Paper, 4),
                traitTuning,
                playerHp: 5,
                enemyHp: 10);

            Assert.That(roundResult.DamageToEnemy, Is.EqualTo(1));
            Assert.That(roundResult.HealToPlayer, Is.EqualTo(0));
            Assert.That(roundResult.PlayerHpAfter, Is.Null);
        }

        [Test]
        public void ResolveRound_GrowthIncreasesPersistentPermanentPowerBonusWithoutChangingCurrentRoundCombatPower()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(growthBonus: 2);
            BattleState battleState = CreateBattleState(roundIndex: 1);
            CardInstance playerCard = CreateCardInstance("player-growth", RpsType.Rock, 4, TraitType.Growth);

            RoundResult roundResult = resolver.ResolveRound(
                battleState,
                playerCard,
                TestConfigFactory.CreateCard(RpsType.Scissors, 4),
                traitTuning,
                playerHp: 10,
                enemyHp: 10);

            Assert.That(roundResult.SlotResults[0].PlayerPower, Is.EqualTo(4));
            Assert.That(battleState.PlayerLane.Slots[0].Occupant.CurrentPower, Is.EqualTo(4));
            Assert.That(playerCard.PermanentPowerBonus, Is.EqualTo(2));
        }

        [Test]
        public void ResolveRound_PostResolveEffectsApplyOnlyToNewlyPlayedCards()
        {
            RoundResolver resolver = new RoundResolver();
            TraitTuning traitTuning = TestConfigFactory.CreateValidTraitTuning(regrowHeal: 2, growthBonus: 1);
            BattleState battleState = CreateBattleState(roundIndex: 2);

            CardInstance olderPlayerCard = CreateCardInstance("player-older-growth", RpsType.Rock, 4, TraitType.Regrow, TraitType.Growth);
            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = new BoardCard
            {
                SourceCard = olderPlayerCard,
                Side = BoardSide.Player,
                EnterRoundIndex = 1,
                FixedSelfPower = 4,
                CurrentPower = 4,
                DamageDealtThisRound = 3,
            };

            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-older", RpsType.Scissors, 4, BoardSide.Enemy, 1);

            RoundResult roundResult = resolver.ResolveRound(
                battleState,
                CreateCardInstance("player-new-plain", RpsType.Paper, 4),
                TestConfigFactory.CreateCard(RpsType.Paper, 4),
                traitTuning,
                playerHp: 5,
                enemyHp: 10);

            Assert.That(roundResult.HealToPlayer, Is.EqualTo(0));
            Assert.That(roundResult.PlayerHpAfter, Is.Null);
            Assert.That(olderPlayerCard.PermanentPowerBonus, Is.EqualTo(0));
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
