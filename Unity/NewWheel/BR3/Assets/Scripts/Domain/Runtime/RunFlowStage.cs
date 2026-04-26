namespace BR3.Domain.Runtime
{
    public enum RunFlowStage
    {
        ReadyForNextBattle = 0,
        InBattle = 1,
        ChoosingReward = 2,
        Victory = 3,
        Defeat = 4,
    }
}
