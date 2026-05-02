using System;
using System.Collections.Generic;
using BR3.Config;
using BR3.Domain.Results;
using BR3.Domain.Random;
using BR3.Domain.Rules;
using BR3.Domain.Runtime;

namespace BR3.Application
{
    public sealed class BattleService
    {
        private readonly IGameRandom _gameRandom;

        public BattleService()
            : this(new SystemGameRandom())
        {
        }

        public BattleService(IGameRandom gameRandom)
        {
            _gameRandom = gameRandom ?? throw new ArgumentNullException(nameof(gameRandom));
        }

        public BattleState StartBattle(EnemyProgressState enemyProgressState)
        {
            if (enemyProgressState == null)
            {
                throw new ArgumentNullException(nameof(enemyProgressState));
            }

            if (enemyProgressState.Config == null)
            {
                throw new InvalidOperationException("EnemyProgressState must provide an enemy config.");
            }

            if (enemyProgressState.Config.fixedDeck == null || enemyProgressState.Config.fixedDeck.Count < 3)
            {
                throw new InvalidOperationException("Enemy config must provide at least three cards for battle sequence generation.");
            }

            BattleState battleState = new BattleState
            {
                BattleIndexForEnemy = enemyProgressState.BattlesPlayed + 1,
                RoundIndex = 1,
                PlayerLane = CreateLane(),
                EnemyLane = CreateLane(),
                UsedPlayerCardIds = new HashSet<string>(),
                EnemySequence = CreateEnemySequence(enemyProgressState.Config.fixedDeck),
                RoundResults = new List<BR3.Domain.Results.RoundResult>(),
                Logs = new List<string>(),
                Snapshots = new List<BR3.Domain.Results.PhaseSnapshot>(),
                BattleFlowStage = BattleFlowStage.WaitingForPlayerCard,
            };

            battleState.Logs.Add(
                $"BattleStart: enemy {enemyProgressState.Config.enemyId} battle {battleState.BattleIndexForEnemy} initialized with {battleState.EnemySequence.Count} enemy cards.");

            return battleState;
        }

        public BattleCommandResult SubmitPlayerCard(
            RunState runState,
            EnemyProgressState enemyProgressState,
            BattleState battleState,
            string cardInstanceId,
            TraitTuning traitTuning,
            RoundResolver roundResolver)
        {
            if (runState == null)
            {
                throw new ArgumentNullException(nameof(runState));
            }

            if (enemyProgressState == null)
            {
                throw new ArgumentNullException(nameof(enemyProgressState));
            }

            if (battleState == null)
            {
                throw new ArgumentNullException(nameof(battleState));
            }

            if (traitTuning == null)
            {
                throw new ArgumentNullException(nameof(traitTuning));
            }

            if (roundResolver == null)
            {
                throw new ArgumentNullException(nameof(roundResolver));
            }

            if (battleState.BattleFlowStage != BattleFlowStage.WaitingForPlayerCard)
            {
                return CreateFailureResult(battleState, "Battle is not currently waiting for player card input.");
            }

            battleState.UsedPlayerCardIds ??= new HashSet<string>();

            CardInstance playerCard = FindPlayerCard(runState.PlayerDeck, cardInstanceId);
            if (playerCard == null)
            {
                return CreateFailureResult(battleState, $"Could not find player card instance '{cardInstanceId}' in the player deck.");
            }

            if (battleState.UsedPlayerCardIds != null && battleState.UsedPlayerCardIds.Contains(cardInstanceId))
            {
                return CreateFailureResult(battleState, $"Player card instance '{cardInstanceId}' has already been used in this battle.");
            }

            int enemySequenceIndex = battleState.RoundIndex - 1;
            if (enemySequenceIndex < 0 || battleState.EnemySequence == null || enemySequenceIndex >= battleState.EnemySequence.Count)
            {
                return CreateFailureResult(battleState, $"Round index {battleState.RoundIndex} does not map to a valid enemy sequence entry.");
            }

            CardSpec enemyCard = battleState.EnemySequence[enemySequenceIndex];
            if (enemyCard == null)
            {
                return CreateFailureResult(battleState, $"Enemy sequence entry for round {battleState.RoundIndex} is missing.");
            }

            battleState.BattleFlowStage = BattleFlowStage.ResolvingRound;

            RoundResult roundResult = roundResolver.ResolveRound(
                battleState,
                playerCard,
                enemyCard,
                traitTuning,
                runState.PlayerHp,
                enemyProgressState.CurrentHp);

            ApplyRoundResult(runState, enemyProgressState, battleState, roundResult);
            battleState.BattleFlowStage = BattleFlowStage.PresentingRoundResult;

            return new BattleCommandResult
            {
                Success = true,
                FailureReason = null,
                BattleFlowStage = battleState.BattleFlowStage,
                RoundResult = roundResult,
                IsBattleComplete = false,
                BattleOutcome = null,
            };
        }

        private List<CardSpec> CreateEnemySequence(List<CardSpec> fixedDeck)
        {
            List<CardSpec> shuffledDeck = new List<CardSpec>(fixedDeck.Count);
            for (int cardIndex = 0; cardIndex < fixedDeck.Count; cardIndex++)
            {
                shuffledDeck.Add(CloneCardSpec(fixedDeck[cardIndex]));
            }

            for (int cardIndex = shuffledDeck.Count - 1; cardIndex > 0; cardIndex--)
            {
                int swapIndex = _gameRandom.NextInt(0, cardIndex + 1);
                CardSpec temporaryCard = shuffledDeck[cardIndex];
                shuffledDeck[cardIndex] = shuffledDeck[swapIndex];
                shuffledDeck[swapIndex] = temporaryCard;
            }

            return shuffledDeck.GetRange(0, 3);
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

        private static void ApplyRoundResult(
            RunState runState,
            EnemyProgressState enemyProgressState,
            BattleState battleState,
            RoundResult roundResult)
        {
            int playerHpAfterDamage = runState.PlayerHp - roundResult.DamageToPlayer;
            int enemyHpAfterDamage = enemyProgressState.CurrentHp - roundResult.DamageToEnemy;

            int finalPlayerHp = Math.Min(runState.PlayerMaxHp, playerHpAfterDamage + roundResult.HealToPlayer);
            int finalEnemyHp = Math.Min(enemyProgressState.MaxHp, enemyHpAfterDamage + roundResult.HealToEnemy);

            runState.PlayerHp = finalPlayerHp;
            enemyProgressState.CurrentHp = finalEnemyHp;
            roundResult.PlayerHpAfter = finalPlayerHp;
            roundResult.EnemyHpAfter = finalEnemyHp;

            battleState.RoundResults ??= new List<RoundResult>();
            battleState.Logs ??= new List<string>();
            battleState.Snapshots ??= new List<BR3.Domain.Results.PhaseSnapshot>();

            battleState.RoundResults.Add(roundResult);
            battleState.Logs.AddRange(roundResult.Logs);
            battleState.Snapshots.AddRange(roundResult.Snapshots);
        }

        private static CardInstance FindPlayerCard(List<CardInstance> playerDeck, string cardInstanceId)
        {
            if (playerDeck == null)
            {
                return null;
            }

            for (int i = 0; i < playerDeck.Count; i++)
            {
                if (playerDeck[i].InstanceId == cardInstanceId)
                {
                    return playerDeck[i];
                }
            }

            return null;
        }

        private static BattleCommandResult CreateFailureResult(BattleState battleState, string failureReason)
        {
            return new BattleCommandResult
            {
                Success = false,
                FailureReason = failureReason,
                BattleFlowStage = battleState.BattleFlowStage,
                RoundResult = null,
                IsBattleComplete = false,
                BattleOutcome = null,
            };
        }

        private static CardSpec CloneCardSpec(CardSpec cardSpec)
        {
            return new CardSpec
            {
                rpsType = cardSpec.rpsType,
                basePower = cardSpec.basePower,
                traits = new List<BR3.Domain.TraitType>(cardSpec.traits ?? new List<BR3.Domain.TraitType>()),
            };
        }
    }
}
