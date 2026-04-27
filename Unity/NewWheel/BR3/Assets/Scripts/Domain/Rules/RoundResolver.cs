using System;
using System.Collections.Generic;
using BR3.Config;
using BR3.Domain.Results;
using BR3.Domain.Runtime;

namespace BR3.Domain.Rules
{
    public sealed class RoundResolver
    {
        public RoundResult ResolveRound(
            BattleState battleState,
            CardInstance playerCard,
            CardSpec enemyCardSpec,
            int playerHp,
            int enemyHp)
        {
            if (battleState == null)
            {
                throw new ArgumentNullException(nameof(battleState));
            }

            if (playerCard == null)
            {
                throw new ArgumentNullException(nameof(playerCard));
            }

            if (enemyCardSpec == null)
            {
                throw new ArgumentNullException(nameof(enemyCardSpec));
            }

            int slotIndex = battleState.RoundIndex - 1;
            ValidateRoundSlot(battleState, slotIndex);

            RoundContext context = new RoundContext
            {
                RoundIndex = battleState.RoundIndex,
                PlayerHpBefore = playerHp,
                EnemyHpBefore = enemyHp,
                SlotResults = new List<SlotCombatResult>(),
                Logs = new List<string>(),
                Snapshots = new List<PhaseSnapshot>(),
            };

            ExecuteEnterPhase(battleState, playerCard, enemyCardSpec, slotIndex, context);
            ExecuteFixedSelfBaselinePhase(battleState);
            ExecuteMovementPhaseSkeleton();
            ExecuteBoardDerivedPhaseSkeleton();
            ExecuteResolveOpenSlotsPhase(battleState, context);
            ExecuteApplyMergedDamagePhase(context);
            ExecutePostResolvePhaseSkeleton(context);

            return new RoundResult
            {
                RoundIndex = context.RoundIndex,
                PlayerCardInstanceId = playerCard.InstanceId,
                EnemyCardReference = context.CurrentEnemyCardReference,
                DamageToPlayer = context.DamageToPlayer,
                DamageToEnemy = context.DamageToEnemy,
                HealToPlayer = context.HealToPlayer,
                HealToEnemy = context.HealToEnemy,
                PlayerHpBefore = context.PlayerHpBefore,
                PlayerHpAfter = context.PlayerHpAfter,
                EnemyHpBefore = context.EnemyHpBefore,
                EnemyHpAfter = context.EnemyHpAfter,
                SlotResults = context.SlotResults,
                Logs = context.Logs,
                Snapshots = context.Snapshots,
            };
        }

        private static void ExecuteEnterPhase(
            BattleState battleState,
            CardInstance playerCard,
            CardSpec enemyCardSpec,
            int slotIndex,
            RoundContext context)
        {
            BoardSlotState playerSlot = battleState.PlayerLane.Slots[slotIndex];
            BoardSlotState enemySlot = battleState.EnemyLane.Slots[slotIndex];

            playerSlot.IsOpen = true;
            enemySlot.IsOpen = true;

            BoardCard playerBoardCard = new BoardCard
            {
                SourceCard = playerCard,
                Side = BoardSide.Player,
                EnterRoundIndex = battleState.RoundIndex,
            };

            CardInstance enemySourceCard = new CardInstance
            {
                InstanceId = $"enemy-round-{battleState.RoundIndex}",
                RpsType = enemyCardSpec.rpsType,
                BasePower = enemyCardSpec.basePower,
                Traits = new List<TraitType>(enemyCardSpec.traits ?? new List<TraitType>()),
                PermanentPowerBonus = 0,
            };

            BoardCard enemyBoardCard = new BoardCard
            {
                SourceCard = enemySourceCard,
                Side = BoardSide.Enemy,
                EnterRoundIndex = battleState.RoundIndex,
            };

            playerSlot.Occupant = playerBoardCard;
            enemySlot.Occupant = enemyBoardCard;

            battleState.UsedPlayerCardIds.Add(playerCard.InstanceId);

            context.CurrentEnemyCardReference = CreateEnemyCardReference(enemyBoardCard);
            context.Logs.Add($"Enter: player card {playerCard.InstanceId} and enemy card entered slot {slotIndex}.");
        }

        private static void ExecuteFixedSelfBaselinePhase(BattleState battleState)
        {
            RecalculateLaneBaselinePower(battleState.PlayerLane);
            RecalculateLaneBaselinePower(battleState.EnemyLane);
        }

        private static void ExecuteMovementPhaseSkeleton()
        {
        }

        private static void ExecuteBoardDerivedPhaseSkeleton()
        {
        }

        private static void ExecuteResolveOpenSlotsPhase(BattleState battleState, RoundContext context)
        {
            for (int slotIndex = 0; slotIndex < battleState.PlayerLane.Slots.Count; slotIndex++)
            {
                BoardSlotState playerSlot = battleState.PlayerLane.Slots[slotIndex];
                BoardSlotState enemySlot = battleState.EnemyLane.Slots[slotIndex];

                if (!playerSlot.IsOpen || !enemySlot.IsOpen)
                {
                    continue;
                }

                if (playerSlot.Occupant == null || enemySlot.Occupant == null)
                {
                    continue;
                }

                SlotCombatResult slotResult = ResolveSlotCombat(slotIndex, playerSlot.Occupant, enemySlot.Occupant);
                context.SlotResults.Add(slotResult);
                context.DamageToPlayer += slotResult.DamageToPlayer;
                context.DamageToEnemy += slotResult.DamageToEnemy;
            }
        }

        private static void ExecuteApplyMergedDamagePhase(RoundContext context)
        {
            context.PlayerHpAfter = context.PlayerHpBefore - context.DamageToPlayer;
            context.EnemyHpAfter = context.EnemyHpBefore - context.DamageToEnemy;
        }

        private static void ExecutePostResolvePhaseSkeleton(RoundContext context)
        {
            context.HealToPlayer = 0;
            context.HealToEnemy = 0;
        }

        private static SlotCombatResult ResolveSlotCombat(int slotIndex, BoardCard playerBoardCard, BoardCard enemyBoardCard)
        {
            SlotWinnerSide winnerSide = DetermineWinner(playerBoardCard.SourceCard.RpsType, enemyBoardCard.SourceCard.RpsType);
            int damageToPlayer = 0;
            int damageToEnemy = 0;

            if (winnerSide == SlotWinnerSide.Player)
            {
                damageToEnemy = Math.Max(1, playerBoardCard.CurrentPower - enemyBoardCard.CurrentPower);
                playerBoardCard.DamageDealtThisRound = damageToEnemy;
                enemyBoardCard.DamageDealtThisRound = 0;
            }
            else if (winnerSide == SlotWinnerSide.Enemy)
            {
                damageToPlayer = Math.Max(1, enemyBoardCard.CurrentPower - playerBoardCard.CurrentPower);
                playerBoardCard.DamageDealtThisRound = 0;
                enemyBoardCard.DamageDealtThisRound = damageToPlayer;
            }
            else
            {
                playerBoardCard.DamageDealtThisRound = 0;
                enemyBoardCard.DamageDealtThisRound = 0;
            }

            return new SlotCombatResult
            {
                SlotIndex = slotIndex,
                PlayerCardInstanceId = playerBoardCard.SourceCard.InstanceId,
                EnemyCardReference = CreateEnemyCardReference(enemyBoardCard),
                PlayerPower = playerBoardCard.CurrentPower,
                EnemyPower = enemyBoardCard.CurrentPower,
                WinnerSide = winnerSide,
                DamageToPlayer = damageToPlayer,
                DamageToEnemy = damageToEnemy,
            };
        }

        private static SlotWinnerSide DetermineWinner(RpsType playerType, RpsType enemyType)
        {
            if (playerType == enemyType)
            {
                return SlotWinnerSide.Tie;
            }

            bool playerWins =
                (playerType == RpsType.Rock && enemyType == RpsType.Scissors)
                || (playerType == RpsType.Scissors && enemyType == RpsType.Paper)
                || (playerType == RpsType.Paper && enemyType == RpsType.Rock);

            return playerWins ? SlotWinnerSide.Player : SlotWinnerSide.Enemy;
        }

        private static void RecalculateLaneBaselinePower(LaneState laneState)
        {
            foreach (BoardSlotState slot in laneState.Slots)
            {
                if (slot.Occupant == null)
                {
                    continue;
                }

                int baselinePower = slot.Occupant.SourceCard.BasePower + slot.Occupant.SourceCard.PermanentPowerBonus;
                slot.Occupant.FixedSelfPower = baselinePower;
                slot.Occupant.CurrentPower = baselinePower;
                slot.Occupant.DamageDealtThisRound = 0;
            }
        }

        private static EnemyCardReference CreateEnemyCardReference(BoardCard enemyBoardCard)
        {
            return new EnemyCardReference
            {
                SequenceIndex = enemyBoardCard.EnterRoundIndex - 1,
                CardSpec = new CardSpec
                {
                    rpsType = enemyBoardCard.SourceCard.RpsType,
                    basePower = enemyBoardCard.SourceCard.BasePower,
                    traits = new List<TraitType>(enemyBoardCard.SourceCard.Traits ?? new List<TraitType>()),
                },
            };
        }

        private static void ValidateRoundSlot(BattleState battleState, int slotIndex)
        {
            if (battleState.PlayerLane == null || battleState.EnemyLane == null)
            {
                throw new InvalidOperationException("BattleState must provide both player and enemy lanes.");
            }

            if (battleState.PlayerLane.Slots == null || battleState.EnemyLane.Slots == null)
            {
                throw new InvalidOperationException("BattleState lanes must provide slot collections.");
            }

            if (slotIndex < 0 || slotIndex >= battleState.PlayerLane.Slots.Count || slotIndex >= battleState.EnemyLane.Slots.Count)
            {
                throw new InvalidOperationException($"Round index {battleState.RoundIndex} does not map to a valid board slot.");
            }
        }

        private sealed class RoundContext
        {
            public int RoundIndex;
            public int PlayerHpBefore;
            public int EnemyHpBefore;
            public int PlayerHpAfter;
            public int EnemyHpAfter;
            public int DamageToPlayer;
            public int DamageToEnemy;
            public int HealToPlayer;
            public int HealToEnemy;
            public EnemyCardReference CurrentEnemyCardReference;
            public List<SlotCombatResult> SlotResults;
            public List<string> Logs;
            public List<PhaseSnapshot> Snapshots;
        }
    }
}
