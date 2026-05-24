using System;
using BR3.Config;
using BR3.Domain.Results;
using BR3.Domain.Runtime;

namespace BR3.Application
{
    public sealed class RunService
    {
        public RunState CreateNewRun(GameConfig config, RuntimeStateFactory runtimeStateFactory)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (runtimeStateFactory == null)
            {
                throw new ArgumentNullException(nameof(runtimeStateFactory));
            }

            return runtimeStateFactory.CreateRunState(config);
        }

        public bool CanStartNextBattle(RunState runState)
        {
            if (runState == null)
            {
                throw new ArgumentNullException(nameof(runState));
            }

            return runState.FlowStage == RunFlowStage.ReadyForNextBattle
                && runState.ActiveBattle == null
                && runState.PendingRewardOffer == null
                && runState.CurrentEnemy != null
                && runState.FlowStage != RunFlowStage.Victory
                && runState.FlowStage != RunFlowStage.Defeat;
        }

        public RunCommandResult StartNextBattle(RunState runState, BattleService battleService)
        {
            if (runState == null)
            {
                throw new ArgumentNullException(nameof(runState));
            }

            if (battleService == null)
            {
                throw new ArgumentNullException(nameof(battleService));
            }

            if (!CanStartNextBattle(runState))
            {
                return CreateFailureResult(runState, "Run is not currently allowed to start the next battle.");
            }

            BattleState battleState = battleService.StartBattle(runState.CurrentEnemy);
            runState.ActiveBattle = battleState;
            runState.FlowStage = RunFlowStage.InBattle;

            return new RunCommandResult
            {
                Success = true,
                FailureReason = null,
                FlowStage = runState.FlowStage,
                ActiveBattle = battleState,
                PendingRewardOffer = runState.PendingRewardOffer,
                IsRunComplete = false,
            };
        }

        public RunCommandResult AcceptCompletedBattle(
            RunState runState,
            BattleOutcome battleOutcome,
            RewardService rewardService,
            GameConfig config)
        {
            if (runState == null)
            {
                throw new ArgumentNullException(nameof(runState));
            }

            if (rewardService == null)
            {
                throw new ArgumentNullException(nameof(rewardService));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (battleOutcome == null)
            {
                return CreateFailureResult(runState, "Run cannot accept a null battle outcome.");
            }

            if (!CanAcceptCompletedBattle(runState, battleOutcome, config))
            {
                return CreateFailureResult(runState, "Run is not currently allowed to accept a completed battle.");
            }

            runState.CurrentEnemy.BattlesPlayed++;
            runState.ActiveBattle = null;
            runState.PendingRewardOffer = null;

            if (battleOutcome.EnemyDefeated)
            {
                if (IsFinalEnemy(runState, config))
                {
                    runState.FlowStage = RunFlowStage.Victory;
                    return CreateSuccessResult(runState);
                }

                return EnterRewardFlow(runState, rewardService, config);
            }

            if (runState.CurrentEnemy.BattlesPlayed >= GetRewardTotalForCurrentEnemy(runState))
            {
                runState.FlowStage = RunFlowStage.Defeat;
                return CreateSuccessResult(runState);
            }

            return EnterRewardFlow(runState, rewardService, config);
        }

        public RunCommandResult ChooseReward(
            RunState runState,
            string optionId,
            RewardService rewardService,
            GameConfig config,
            RuntimeStateFactory runtimeStateFactory)
        {
            if (runState == null)
            {
                throw new ArgumentNullException(nameof(runState));
            }

            if (rewardService == null)
            {
                throw new ArgumentNullException(nameof(rewardService));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (runtimeStateFactory == null)
            {
                throw new ArgumentNullException(nameof(runtimeStateFactory));
            }

            if (!CanChooseReward(runState, optionId, out RewardOption selectedOption))
            {
                return CreateFailureResult(runState, "Run is not currently allowed to choose the requested reward option.");
            }

            rewardService.ApplyRewardOption(runState.PlayerDeck, selectedOption);
            runState.CurrentEnemy.RewardsClaimed++;
            runState.PendingRewardOffer = null;

            bool enemyDefeated = runState.CurrentEnemy.CurrentHp <= 0;
            if (!enemyDefeated)
            {
                runState.FlowStage = RunFlowStage.ReadyForNextBattle;
                return CreateSuccessResult(runState);
            }

            if (IsFinalEnemy(runState, config))
            {
                runState.FlowStage = RunFlowStage.Victory;
                return CreateSuccessResult(runState);
            }

            if (runState.CurrentEnemy.RewardsClaimed < GetRewardTotalForCurrentEnemy(runState))
            {
                return EnterRewardFlow(runState, rewardService, config);
            }

            AdvanceToNextEnemy(runState, config, runtimeStateFactory);
            return CreateSuccessResult(runState);
        }

        private static bool CanAcceptCompletedBattle(RunState runState, BattleOutcome battleOutcome, GameConfig config)
        {
            int rewardTotalForCurrentEnemy = GetRewardTotalForCurrentEnemy(runState);

            if (runState.CurrentEnemy == null
                || runState.ActiveBattle == null
                || runState.PendingRewardOffer != null
                || runState.FlowStage != RunFlowStage.InBattle
                || runState.ActiveBattle.BattleFlowStage != BattleFlowStage.BattleComplete
                || runState.CurrentEnemy.RewardsClaimed < 0
                || runState.CurrentEnemy.RewardsClaimed >= rewardTotalForCurrentEnemy
                || config.enemies == null
                || config.enemies.Count == 0
                || runState.CurrentEnemyIndex < 0
                || runState.CurrentEnemyIndex >= config.enemies.Count
                || battleOutcome.BattleIndexForEnemy != runState.ActiveBattle.BattleIndexForEnemy
                || battleOutcome.RoundsPlayed <= 0)
            {
                return false;
            }

            return true;
        }

        private static bool CanChooseReward(RunState runState, string optionId, out RewardOption selectedOption)
        {
            selectedOption = null;

            if (runState.FlowStage != RunFlowStage.ChoosingReward
                || runState.CurrentEnemy == null
                || runState.PendingRewardOffer == null
                || runState.PendingRewardOffer.Options == null
                || string.IsNullOrWhiteSpace(optionId))
            {
                return false;
            }

            for (int optionIndex = 0; optionIndex < runState.PendingRewardOffer.Options.Count; optionIndex++)
            {
                RewardOption candidate = runState.PendingRewardOffer.Options[optionIndex];
                if (candidate != null && candidate.OptionId == optionId)
                {
                    selectedOption = candidate;
                    return true;
                }
            }

            return false;
        }

        private static int GetRewardTotalForCurrentEnemy(RunState runState)
        {
            return runState.CurrentEnemy?.Config?.battleLimit ?? 0;
        }

        private static bool IsFinalEnemy(RunState runState, GameConfig config)
        {
            return runState.CurrentEnemyIndex == config.enemies.Count - 1;
        }

        private static void AdvanceToNextEnemy(RunState runState, GameConfig config, RuntimeStateFactory runtimeStateFactory)
        {
            int nextEnemyIndex = runState.CurrentEnemyIndex + 1;
            runState.CurrentEnemyIndex = nextEnemyIndex;
            runState.CurrentEnemy = runtimeStateFactory.CreateEnemyProgressState(config.enemies[nextEnemyIndex]);
            runState.ActiveBattle = null;
            runState.PendingRewardOffer = null;
            runState.FlowStage = RunFlowStage.ReadyForNextBattle;
        }

        private static RunCommandResult EnterRewardFlow(RunState runState, RewardService rewardService, GameConfig config)
        {
            runState.PendingRewardOffer = rewardService.CreateRewardOffer(
                runState.PlayerDeck,
                config.rewardGeneration,
                runState.CurrentEnemy.RewardsClaimed + 1);
            runState.FlowStage = RunFlowStage.ChoosingReward;
            return CreateSuccessResult(runState);
        }

        private static RunCommandResult CreateSuccessResult(RunState runState)
        {
            return new RunCommandResult
            {
                Success = true,
                FailureReason = null,
                FlowStage = runState.FlowStage,
                ActiveBattle = runState.ActiveBattle,
                PendingRewardOffer = runState.PendingRewardOffer,
                IsRunComplete = runState.FlowStage == RunFlowStage.Victory || runState.FlowStage == RunFlowStage.Defeat,
            };
        }

        private static RunCommandResult CreateFailureResult(RunState runState, string failureReason)
        {
            return new RunCommandResult
            {
                Success = false,
                FailureReason = failureReason,
                FlowStage = runState.FlowStage,
                ActiveBattle = runState.ActiveBattle,
                PendingRewardOffer = runState.PendingRewardOffer,
                IsRunComplete = runState.FlowStage == RunFlowStage.Victory || runState.FlowStage == RunFlowStage.Defeat,
            };
        }
    }
}
