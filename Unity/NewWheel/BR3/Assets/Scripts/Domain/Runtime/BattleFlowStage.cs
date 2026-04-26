namespace BR3.Domain.Runtime
{
    public enum BattleFlowStage
    {
        WaitingForPlayerCard = 0,
        ResolvingRound = 1,
        PresentingRoundResult = 2,
        BattleComplete = 3,
    }
}
