using BR3.Domain.Runtime;

namespace BR3.Application
{
    public sealed class RunCommandResult
    {
        public bool Success;
        public string FailureReason;
        public RunFlowStage FlowStage;
        public BattleState ActiveBattle;
        public RewardOffer PendingRewardOffer;
        public bool IsRunComplete;
    }
}
