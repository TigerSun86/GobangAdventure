using System.Collections.Generic;

namespace BR3.Domain.Runtime
{
    public sealed class CardInstance
    {
        public string InstanceId;
        public RpsType RpsType;
        public int BasePower;
        public List<TraitType> Traits;
        public int PermanentPowerBonus;
    }
}
