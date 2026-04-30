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
            TraitTuning traitTuning,
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

            if (traitTuning == null)
            {
                throw new ArgumentNullException(nameof(traitTuning));
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
            CapturePhaseSnapshot(battleState, context, RoundPhase.Enter);
            ExecuteFixedSelfBaselinePhase(battleState, traitTuning, context);
            CapturePhaseSnapshot(battleState, context, RoundPhase.FixedSelf);
            ExecuteMovementPhase(battleState, context);
            CapturePhaseSnapshot(battleState, context, RoundPhase.Movement);
            ExecuteBoardDerivedPhase(battleState, traitTuning, context);
            CapturePhaseSnapshot(battleState, context, RoundPhase.BoardDerived);
            ExecuteResolveOpenSlotsPhase(battleState, context);
            CapturePhaseSnapshot(battleState, context, RoundPhase.ResolveOpenSlots);
            ExecuteApplyMergedDamagePhase(context);
            CapturePhaseSnapshot(battleState, context, RoundPhase.ApplyMergedDamage);
            ExecutePostResolvePhase(context, traitTuning);
            CapturePhaseSnapshot(battleState, context, RoundPhase.PostResolve);

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
                PlayerHpAfter = null,
                EnemyHpBefore = context.EnemyHpBefore,
                EnemyHpAfter = null,
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

            context.NewPlayerBoardCard = playerBoardCard;
            context.NewEnemyBoardCard = enemyBoardCard;
            context.CurrentEnemyCardReference = CreateEnemyCardReference(enemyBoardCard);
            context.Logs.Add($"Enter: player card {playerCard.InstanceId} entered slot {slotIndex}.");
            context.Logs.Add($"Enter: enemy card enemy-round-{battleState.RoundIndex} entered slot {slotIndex}.");
        }

        private static void ExecuteFixedSelfBaselinePhase(BattleState battleState, TraitTuning traitTuning, RoundContext context)
        {
            RecalculateLaneBaselinePower(battleState.PlayerLane, traitTuning, context, "player");
            RecalculateLaneBaselinePower(battleState.EnemyLane, traitTuning, context, "enemy");
        }

        private static void ExecuteMovementPhase(BattleState battleState, RoundContext context)
        {
            ResolveLaneMovement(battleState.PlayerLane, context, "player");
            ResolveLaneMovement(battleState.EnemyLane, context, "enemy");
        }

        private static void ExecuteBoardDerivedPhase(BattleState battleState, TraitTuning traitTuning, RoundContext context)
        {
            int slotCount = battleState.PlayerLane.Slots.Count;
            int[] playerDeltas = new int[slotCount];
            int[] enemyDeltas = new int[slotCount];

            CollectAdjacentAidDeltas(battleState.PlayerLane, playerDeltas, traitTuning.adjacentAidBonus);
            CollectAdjacentAidDeltas(battleState.EnemyLane, enemyDeltas, traitTuning.adjacentAidBonus);
            CollectSuppressDeltas(battleState.PlayerLane, battleState.EnemyLane, enemyDeltas, traitTuning.suppressPenalty);
            CollectSuppressDeltas(battleState.EnemyLane, battleState.PlayerLane, playerDeltas, traitTuning.suppressPenalty);

            ApplyBoardDerivedDeltas(battleState.PlayerLane, playerDeltas, context, "player");
            ApplyBoardDerivedDeltas(battleState.EnemyLane, enemyDeltas, context, "enemy");
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
                context.Logs.Add(
                    $"ResolveOpenSlots: slot {slotIndex} winner {slotResult.WinnerSide}, damage to player {slotResult.DamageToPlayer}, damage to enemy {slotResult.DamageToEnemy}.");
            }
        }

        private static void ExecuteApplyMergedDamagePhase(RoundContext context)
        {
            int provisionalPlayerHpAfterDamage = context.PlayerHpBefore - context.DamageToPlayer;
            int provisionalEnemyHpAfterDamage = context.EnemyHpBefore - context.DamageToEnemy;
            context.Logs.Add(
                $"ApplyMergedDamage: raw damage totals imply provisional player HP {context.PlayerHpBefore}->{provisionalPlayerHpAfterDamage} and provisional enemy HP {context.EnemyHpBefore}->{provisionalEnemyHpAfterDamage} before battle-layer application.");
        }

        private static void ExecutePostResolvePhase(RoundContext context, TraitTuning traitTuning)
        {
            ProcessPostResolveForCard(
                context.NewPlayerBoardCard,
                traitTuning,
                ref context.HealToPlayer,
                context,
                "player");

            ProcessPostResolveForCard(
                context.NewEnemyBoardCard,
                traitTuning,
                ref context.HealToEnemy,
                context,
                "enemy");

            context.Logs.Add(
                $"PostResolve: player raw heal {context.HealToPlayer}, enemy raw heal {context.HealToEnemy}, authoritative HP-after values remain pending for battle application.");
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

        private static void RecalculateLaneBaselinePower(
            LaneState laneState,
            TraitTuning traitTuning,
            RoundContext context,
            string laneName)
        {
            foreach (BoardSlotState slot in laneState.Slots)
            {
                if (slot.Occupant == null)
                {
                    continue;
                }

                int baselinePower = slot.Occupant.SourceCard.BasePower + slot.Occupant.SourceCard.PermanentPowerBonus;
                if (slot.Occupant.SourceCard.Traits != null && slot.Occupant.SourceCard.Traits.Contains(TraitType.Empower))
                {
                    baselinePower += traitTuning.empowerBonus;
                }

                slot.Occupant.FixedSelfPower = baselinePower;
                slot.Occupant.CurrentPower = baselinePower;
                slot.Occupant.DamageDealtThisRound = 0;
                context.Logs.Add(
                    $"FixedSelf: {laneName} slot {slot.Index} baseline set to {baselinePower} for {slot.Occupant.SourceCard.InstanceId}.");
            }
        }

        private static void ResolveLaneMovement(LaneState laneState, RoundContext context, string laneName)
        {
            for (int slotIndex = laneState.Slots.Count - 1; slotIndex >= 0; slotIndex--)
            {
                BoardSlotState currentSlot = laneState.Slots[slotIndex];
                BoardCard currentCard = currentSlot.Occupant;
                if (currentCard == null || currentCard.SourceCard?.Traits == null)
                {
                    continue;
                }

                if (currentCard.SourceCard.Traits.Contains(TraitType.ShiftLeft))
                {
                    int leftIndex = slotIndex - 1;
                    if (leftIndex >= 0 && laneState.Slots[leftIndex].Occupant != null)
                    {
                        SwapOccupants(currentSlot, laneState.Slots[leftIndex]);
                        context.Logs.Add($"Movement: {laneName} slot {slotIndex} swapped left with slot {leftIndex}.");
                    }

                    continue;
                }

                if (currentCard.SourceCard.Traits.Contains(TraitType.ShiftRight))
                {
                    int rightIndex = slotIndex + 1;
                    if (rightIndex < laneState.Slots.Count && laneState.Slots[rightIndex].Occupant != null)
                    {
                        SwapOccupants(currentSlot, laneState.Slots[rightIndex]);
                        context.Logs.Add($"Movement: {laneName} slot {slotIndex} swapped right with slot {rightIndex}.");
                    }
                }
            }
        }

        private static void SwapOccupants(BoardSlotState firstSlot, BoardSlotState secondSlot)
        {
            BoardCard temporaryOccupant = firstSlot.Occupant;
            firstSlot.Occupant = secondSlot.Occupant;
            secondSlot.Occupant = temporaryOccupant;
        }

        private static void CollectAdjacentAidDeltas(LaneState laneState, int[] deltas, int adjacentAidBonus)
        {
            for (int slotIndex = 0; slotIndex < laneState.Slots.Count; slotIndex++)
            {
                BoardCard currentCard = laneState.Slots[slotIndex].Occupant;
                if (currentCard?.SourceCard?.Traits == null || !currentCard.SourceCard.Traits.Contains(TraitType.AdjacentAid))
                {
                    continue;
                }

                int leftIndex = slotIndex - 1;
                if (leftIndex >= 0 && laneState.Slots[leftIndex].Occupant != null)
                {
                    deltas[leftIndex] += adjacentAidBonus;
                }

                int rightIndex = slotIndex + 1;
                if (rightIndex < laneState.Slots.Count && laneState.Slots[rightIndex].Occupant != null)
                {
                    deltas[rightIndex] += adjacentAidBonus;
                }
            }
        }

        private static void CollectSuppressDeltas(
            LaneState sourceLane,
            LaneState opposingLane,
            int[] opposingDeltas,
            int suppressPenalty)
        {
            for (int slotIndex = 0; slotIndex < sourceLane.Slots.Count; slotIndex++)
            {
                BoardCard sourceCard = sourceLane.Slots[slotIndex].Occupant;
                BoardCard opposingCard = opposingLane.Slots[slotIndex].Occupant;
                if (sourceCard?.SourceCard?.Traits == null || !sourceCard.SourceCard.Traits.Contains(TraitType.Suppress))
                {
                    continue;
                }

                if (opposingCard != null)
                {
                    opposingDeltas[slotIndex] -= suppressPenalty;
                }
            }
        }

        private static void ApplyBoardDerivedDeltas(
            LaneState laneState,
            int[] deltas,
            RoundContext context,
            string laneName)
        {
            for (int slotIndex = 0; slotIndex < laneState.Slots.Count; slotIndex++)
            {
                BoardCard occupant = laneState.Slots[slotIndex].Occupant;
                if (occupant == null || deltas[slotIndex] == 0)
                {
                    continue;
                }

                occupant.CurrentPower += deltas[slotIndex];
                context.Logs.Add($"BoardDerived: {laneName} slot {slotIndex} power delta {deltas[slotIndex]}.");
            }
        }

        private static void ProcessPostResolveForCard(
            BoardCard boardCard,
            TraitTuning traitTuning,
            ref int healTotal,
            RoundContext context,
            string sideName)
        {
            if (boardCard?.SourceCard?.Traits == null)
            {
                return;
            }

            List<TraitType> traits = boardCard.SourceCard.Traits;

            if (traits.Contains(TraitType.Regrow))
            {
                healTotal += traitTuning.regrowHeal;
                context.Logs.Add($"PostResolve: {sideName} regrow contributed raw heal {traitTuning.regrowHeal}.");
            }

            if (traits.Contains(TraitType.Lifesteal))
            {
                healTotal += boardCard.DamageDealtThisRound;
                context.Logs.Add($"PostResolve: {sideName} lifesteal contributed raw heal {boardCard.DamageDealtThisRound}.");
            }

            if (traits.Contains(TraitType.Growth))
            {
                boardCard.SourceCard.PermanentPowerBonus += traitTuning.growthBonus;
                context.Logs.Add($"PostResolve: {sideName} growth increased permanent power by {traitTuning.growthBonus}.");
            }
        }

        private static void CapturePhaseSnapshot(BattleState battleState, RoundContext context, RoundPhase phase)
        {
            context.Snapshots.Add(new PhaseSnapshot
            {
                Phase = phase,
                PlayerLaneStateText = FormatLaneState(battleState.PlayerLane),
                EnemyLaneStateText = FormatLaneState(battleState.EnemyLane),
            });

            context.Logs.Add($"Snapshot: captured {phase}.");
        }

        private static string FormatLaneState(LaneState laneState)
        {
            List<string> slotTexts = new List<string>(laneState.Slots.Count);
            foreach (BoardSlotState slot in laneState.Slots)
            {
                slotTexts.Add(FormatSlotState(slot));
            }

            return string.Join(" | ", slotTexts);
        }

        private static string FormatSlotState(BoardSlotState slot)
        {
            if (!slot.IsOpen)
            {
                return $"[{slot.Index}:closed]";
            }

            if (slot.Occupant == null)
            {
                return $"[{slot.Index}:open-empty]";
            }

            BoardCard occupant = slot.Occupant;
            string traitText = occupant.SourceCard.Traits == null || occupant.SourceCard.Traits.Count == 0
                ? "-"
                : string.Join(",", occupant.SourceCard.Traits);

            return
                $"[{slot.Index}:{occupant.SourceCard.InstanceId}/{occupant.SourceCard.RpsType}/base={occupant.SourceCard.BasePower}/perm={occupant.SourceCard.PermanentPowerBonus}/fixed={occupant.FixedSelfPower}/current={occupant.CurrentPower}/damage={occupant.DamageDealtThisRound}/traits={traitText}]";
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
            public int DamageToPlayer;
            public int DamageToEnemy;
            public int HealToPlayer;
            public int HealToEnemy;
            public BoardCard NewPlayerBoardCard;
            public BoardCard NewEnemyBoardCard;
            public EnemyCardReference CurrentEnemyCardReference;
            public List<SlotCombatResult> SlotResults;
            public List<string> Logs;
            public List<PhaseSnapshot> Snapshots;
        }
    }
}
