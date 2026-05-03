using BR3.Domain.Runtime;

namespace BR3.Presentation.DebugUi
{
    public static class FlowStageTextFormatter
    {
        public static string Format(RunFlowStage? runFlowStage)
        {
            return runFlowStage?.ToString() ?? "-";
        }

        public static string Format(BattleFlowStage? battleFlowStage)
        {
            return battleFlowStage?.ToString() ?? "-";
        }
    }
}
