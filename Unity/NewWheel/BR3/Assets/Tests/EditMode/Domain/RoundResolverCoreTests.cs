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
    public sealed class RoundResolverCoreTests
    {
        private static readonly TraitTuning TraitTuning = TestConfigFactory.CreateValidTraitTuning();

        [Test]
        public void ResolveRound_AppliesStandardRpsOutcomeRules()
        {
            RoundResolver resolver = new RoundResolver();
            BattleState battleState = CreateBattleState(roundIndex: 1);
            CardInstance playerCard = CreatePlayerCard("card-0001", RpsType.Rock, 4);
            CardSpec enemyCard = TestConfigFactory.CreateCard(RpsType.Scissors, 4);

            RoundResult roundResult = resolver.ResolveRound(battleState, playerCard, enemyCard, TraitTuning, playerHp: 10, enemyHp: 10);

            Assert.That(roundResult.DamageToPlayer, Is.EqualTo(0));
            Assert.That(roundResult.DamageToEnemy, Is.EqualTo(1));
            Assert.That(roundResult.SlotResults, Has.Count.EqualTo(1));
            Assert.That(roundResult.SlotResults[0].WinnerSide, Is.EqualTo(SlotWinnerSide.Player));
        }

        [Test]
        public void ResolveRound_TieDealsNoDamage()
        {
            RoundResolver resolver = new RoundResolver();
            BattleState battleState = CreateBattleState(roundIndex: 1);
            CardInstance playerCard = CreatePlayerCard("card-0001", RpsType.Paper, 4);
            CardSpec enemyCard = TestConfigFactory.CreateCard(RpsType.Paper, 7);

            RoundResult roundResult = resolver.ResolveRound(battleState, playerCard, enemyCard, TraitTuning, playerHp: 10, enemyHp: 10);

            Assert.That(roundResult.DamageToPlayer, Is.EqualTo(0));
            Assert.That(roundResult.DamageToEnemy, Is.EqualTo(0));
            Assert.That(roundResult.PlayerHpAfter, Is.Null);
            Assert.That(roundResult.EnemyHpAfter, Is.Null);
            Assert.That(roundResult.SlotResults[0].WinnerSide, Is.EqualTo(SlotWinnerSide.Tie));
        }

        [Test]
        public void ResolveRound_UsesMinimumDamageRule()
        {
            RoundResolver resolver = new RoundResolver();
            BattleState battleState = CreateBattleState(roundIndex: 1);
            CardInstance playerCard = CreatePlayerCard("card-0001", RpsType.Rock, 4);
            CardSpec enemyCard = TestConfigFactory.CreateCard(RpsType.Scissors, 4);

            RoundResult roundResult = resolver.ResolveRound(battleState, playerCard, enemyCard, TraitTuning, playerHp: 10, enemyHp: 10);

            Assert.That(roundResult.SlotResults[0].PlayerPower, Is.EqualTo(4));
            Assert.That(roundResult.SlotResults[0].EnemyPower, Is.EqualTo(4));
            Assert.That(roundResult.DamageToEnemy, Is.EqualTo(1));
        }

        [Test]
        public void ResolveRound_AccumulatesDamageAcrossOpenSlotsBeforeApply()
        {
            RoundResolver resolver = new RoundResolver();
            BattleState battleState = CreateBattleState(roundIndex: 2);

            battleState.PlayerLane.Slots[0].IsOpen = true;
            battleState.EnemyLane.Slots[0].IsOpen = true;
            battleState.PlayerLane.Slots[0].Occupant = CreateBoardCard("card-previous-player", RpsType.Rock, 4, BoardSide.Player, enterRoundIndex: 1);
            battleState.EnemyLane.Slots[0].Occupant = CreateBoardCard("enemy-previous", RpsType.Scissors, 4, BoardSide.Enemy, enterRoundIndex: 1);

            CardInstance playerCard = CreatePlayerCard("card-0002", RpsType.Scissors, 4);
            CardSpec enemyCard = TestConfigFactory.CreateCard(RpsType.Rock, 4);

            RoundResult roundResult = resolver.ResolveRound(battleState, playerCard, enemyCard, TraitTuning, playerHp: 5, enemyHp: 1);

            Assert.That(roundResult.RoundIndex, Is.EqualTo(2));
            Assert.That(roundResult.PlayerHpBefore, Is.EqualTo(5));
            Assert.That(roundResult.EnemyHpBefore, Is.EqualTo(1));
            Assert.That(roundResult.SlotResults, Has.Count.EqualTo(2));
            Assert.That(roundResult.DamageToPlayer, Is.EqualTo(1));
            Assert.That(roundResult.DamageToEnemy, Is.EqualTo(1));
            Assert.That(roundResult.PlayerHpAfter, Is.Null);
            Assert.That(roundResult.EnemyHpAfter, Is.Null);
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

        private static CardInstance CreatePlayerCard(string instanceId, RpsType rpsType, int basePower)
        {
            return new CardInstance
            {
                InstanceId = instanceId,
                RpsType = rpsType,
                BasePower = basePower,
                PermanentPowerBonus = 0,
                Traits = new List<TraitType>(),
            };
        }

        private static BoardCard CreateBoardCard(
            string instanceId,
            RpsType rpsType,
            int basePower,
            BoardSide side,
            int enterRoundIndex)
        {
            CardInstance sourceCard = CreatePlayerCard(instanceId, rpsType, basePower);
            return new BoardCard
            {
                SourceCard = sourceCard,
                Side = side,
                EnterRoundIndex = enterRoundIndex,
                FixedSelfPower = basePower,
                CurrentPower = basePower,
                DamageDealtThisRound = 0,
            };
        }
    }
}
