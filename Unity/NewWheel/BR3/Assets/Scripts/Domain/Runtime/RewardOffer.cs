using System.Collections.Generic;

namespace BR3.Domain.Runtime
{
    public sealed class RewardOffer
    {
        public string OfferId;
        public List<RewardOption> Options;
        public int RewardIndexForCurrentEnemy;
    }
}
