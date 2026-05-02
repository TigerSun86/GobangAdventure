using System;
using BR3.Config;
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
            };
        }
    }
}
