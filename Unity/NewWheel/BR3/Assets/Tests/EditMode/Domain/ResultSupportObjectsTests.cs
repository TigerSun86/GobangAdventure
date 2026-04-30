using System.Collections.Generic;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Results;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Domain
{
    public sealed class ResultSupportObjectsTests
    {
        [Test]
        public void RoundResult_CanCarrySlotResultsLogsAndSnapshots()
        {
            EnemyCardReference enemyCardReference = CreateEnemyCardReference();

            RoundResult roundResult = new RoundResult
            {
                RoundIndex = 2,
                PlayerCardInstanceId = "card-0003",
                EnemyCardReference = enemyCardReference,
                DamageToPlayer = 3,
                DamageToEnemy = 4,
                HealToPlayer = 1,
                HealToEnemy = 0,
                PlayerHpBefore = 10,
                PlayerHpAfter = 8,
                EnemyHpBefore = 12,
                EnemyHpAfter = 8,
                SlotResults = new List<SlotCombatResult>
                {
                    new SlotCombatResult
                    {
                        SlotIndex = 1,
                        PlayerCardInstanceId = "card-0003",
                        EnemyCardReference = enemyCardReference,
                        PlayerPower = 7,
                        EnemyPower = 4,
                        WinnerSide = SlotWinnerSide.Player,
                        DamageToPlayer = 0,
                        DamageToEnemy = 3,
                    },
                },
                Logs = new List<string>
                {
                    "Movement resolved.",
                    "Damage applied simultaneously.",
                },
                Snapshots = new List<PhaseSnapshot>
                {
                    new PhaseSnapshot
                    {
                        Phase = RoundPhase.Movement,
                        PlayerLaneStateText = "[closed][card-0003][empty]",
                        EnemyLaneStateText = "[empty][enemy-1][closed]",
                    },
                },
            };

            Assert.That(roundResult.RoundIndex, Is.EqualTo(2));
            Assert.That(roundResult.PlayerCardInstanceId, Is.EqualTo("card-0003"));
            Assert.That(roundResult.EnemyCardReference, Is.SameAs(enemyCardReference));
            Assert.That(roundResult.SlotResults, Has.Count.EqualTo(1));
            Assert.That(roundResult.Logs, Has.Count.EqualTo(2));
            Assert.That(roundResult.Snapshots, Has.Count.EqualTo(1));
            Assert.That(roundResult.Snapshots[0].Phase, Is.EqualTo(RoundPhase.Movement));
        }

        [Test]
        public void SlotCombatResult_CarriesCompleteSlotLevelCombatFacts()
        {
            SlotCombatResult slotCombatResult = new SlotCombatResult
            {
                SlotIndex = 0,
                PlayerCardInstanceId = "card-0001",
                EnemyCardReference = CreateEnemyCardReference(),
                PlayerPower = 5,
                EnemyPower = 5,
                WinnerSide = SlotWinnerSide.Tie,
                DamageToPlayer = 1,
                DamageToEnemy = 1,
            };

            Assert.That(slotCombatResult.SlotIndex, Is.EqualTo(0));
            Assert.That(slotCombatResult.PlayerCardInstanceId, Is.EqualTo("card-0001"));
            Assert.That(slotCombatResult.PlayerPower, Is.EqualTo(5));
            Assert.That(slotCombatResult.EnemyPower, Is.EqualTo(5));
            Assert.That(slotCombatResult.WinnerSide, Is.EqualTo(SlotWinnerSide.Tie));
            Assert.That(slotCombatResult.DamageToPlayer, Is.EqualTo(1));
            Assert.That(slotCombatResult.DamageToEnemy, Is.EqualTo(1));
        }

        [Test]
        public void PhaseSnapshot_CarriesReadableLaneStateForFixedRoundPhase()
        {
            PhaseSnapshot phaseSnapshot = new PhaseSnapshot
            {
                Phase = RoundPhase.ApplyMergedDamage,
                PlayerLaneStateText = "[card-0001:4][empty][closed]",
                EnemyLaneStateText = "[enemy-0:2][empty][closed]",
            };

            Assert.That(phaseSnapshot.Phase, Is.EqualTo(RoundPhase.ApplyMergedDamage));
            Assert.That(phaseSnapshot.PlayerLaneStateText, Does.Contain("card-0001"));
            Assert.That(phaseSnapshot.EnemyLaneStateText, Does.Contain("enemy-0"));
        }

        [Test]
        public void BattleOutcome_CarriesExpectedBattleToRunSummary()
        {
            BattleOutcome battleOutcome = new BattleOutcome
            {
                BattleIndexForEnemy = 3,
                RoundsPlayed = 4,
                EnemyDefeated = true,
                PlayerHpAfterBattle = 11,
                EnemyHpAfterBattle = 0,
            };

            Assert.That(battleOutcome.BattleIndexForEnemy, Is.EqualTo(3));
            Assert.That(battleOutcome.RoundsPlayed, Is.EqualTo(4));
            Assert.That(battleOutcome.EnemyDefeated, Is.True);
            Assert.That(battleOutcome.PlayerHpAfterBattle, Is.EqualTo(11));
            Assert.That(battleOutcome.EnemyHpAfterBattle, Is.EqualTo(0));
        }

        private static EnemyCardReference CreateEnemyCardReference()
        {
            return new EnemyCardReference
            {
                SequenceIndex = 1,
                CardSpec = TestConfigFactory.CreateCard(
                    RpsType.Scissors,
                    4,
                    TraitType.Suppress),
            };
        }
    }
}
