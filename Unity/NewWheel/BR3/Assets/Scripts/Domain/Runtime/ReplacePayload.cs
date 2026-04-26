using BR3.Config;

namespace BR3.Domain.Runtime
{
    public sealed class ReplacePayload
    {
        public string TargetCardInstanceId;
        public CardSpec ReplacementCardSpec;
    }
}
