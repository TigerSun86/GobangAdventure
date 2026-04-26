using BR3.Config;

namespace BR3.Domain.Runtime
{
    public sealed class EnemyProgressState
    {
        public EnemyConfig Config;
        public int CurrentHp;
        public int MaxHp;
        public int BattlesPlayed;
        public int RewardsClaimed;
    }
}
