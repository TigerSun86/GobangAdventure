using System.Collections.Generic;
using BR3.Config;

namespace BR3.Domain.Runtime
{
    public sealed class BattleState
    {
        public int BattleIndexForEnemy;
        public int RoundIndex;
        public LaneState PlayerLane;
        public LaneState EnemyLane;
        public HashSet<string> UsedPlayerCardIds;
        public List<CardSpec> EnemySequence;
        public BattleFlowStage BattleFlowStage;
    }
}
