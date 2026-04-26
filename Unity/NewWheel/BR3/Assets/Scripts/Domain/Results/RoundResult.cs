using System.Collections.Generic;

namespace BR3.Domain.Results
{
    public sealed class RoundResult
    {
        public int RoundIndex;
        public string PlayerCardInstanceId;
        public EnemyCardReference EnemyCardReference;
        public int DamageToPlayer;
        public int DamageToEnemy;
        public int HealToPlayer;
        public int HealToEnemy;
        public int PlayerHpBefore;
        public int PlayerHpAfter;
        public int EnemyHpBefore;
        public int EnemyHpAfter;
        public List<SlotCombatResult> SlotResults;
        public List<string> Logs;
        public List<PhaseSnapshot> Snapshots;
    }
}
