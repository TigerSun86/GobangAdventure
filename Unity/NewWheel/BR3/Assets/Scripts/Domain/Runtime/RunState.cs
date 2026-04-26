using System.Collections.Generic;

namespace BR3.Domain.Runtime
{
    public sealed class RunState
    {
        public int PlayerHp;
        public int PlayerMaxHp;
        public List<CardInstance> PlayerDeck;
        public int CurrentEnemyIndex;
        public EnemyProgressState CurrentEnemy;
        public BattleState ActiveBattle;
        public RewardOffer PendingRewardOffer;
        public RunFlowStage FlowStage;
    }
}
