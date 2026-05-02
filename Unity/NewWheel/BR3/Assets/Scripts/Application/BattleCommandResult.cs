using BR3.Domain.Results;
using BR3.Domain.Runtime;

namespace BR3.Application
{
    public sealed class BattleCommandResult
    {
        public bool Success;
        public string FailureReason;
        public BattleFlowStage BattleFlowStage;
        public RoundResult RoundResult;
        public bool IsBattleComplete;
        public BattleOutcome BattleOutcome;
    }
}
