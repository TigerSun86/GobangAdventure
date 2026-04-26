namespace BR3.Domain.Runtime
{
    public sealed class RewardOption
    {
        public string OptionId;
        public RewardOptionType Type;
        public UpgradePayload UpgradePayload;
        public ReplacePayload ReplacePayload;
    }
}
