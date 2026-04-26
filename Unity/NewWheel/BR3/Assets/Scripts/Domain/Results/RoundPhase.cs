namespace BR3.Domain.Results
{
    public enum RoundPhase
    {
        Enter = 0,
        FixedSelf = 1,
        Movement = 2,
        BoardDerived = 3,
        ResolveOpenSlots = 4,
        ApplyMergedDamage = 5,
        PostResolve = 6,
    }
}
