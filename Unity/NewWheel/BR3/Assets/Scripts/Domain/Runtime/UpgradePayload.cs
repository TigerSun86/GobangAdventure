using BR3.Domain;

namespace BR3.Domain.Runtime
{
    public sealed class UpgradePayload
    {
        public string TargetCardInstanceId;
        public TraitType AddedTrait;
    }
}
