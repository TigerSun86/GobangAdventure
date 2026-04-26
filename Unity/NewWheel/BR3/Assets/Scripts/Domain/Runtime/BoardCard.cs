namespace BR3.Domain.Runtime
{
    public sealed class BoardCard
    {
        public CardInstance SourceCard;
        public BoardSide Side;
        public int EnterRoundIndex;
        public int FixedSelfPower;
        public int CurrentPower;
        public int DamageDealtThisRound;
    }
}
