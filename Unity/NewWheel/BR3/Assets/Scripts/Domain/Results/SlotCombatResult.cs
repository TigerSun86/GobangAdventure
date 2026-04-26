namespace BR3.Domain.Results
{
    public sealed class SlotCombatResult
    {
        public int SlotIndex;
        public string PlayerCardInstanceId;
        public EnemyCardReference EnemyCardReference;
        public int PlayerPower;
        public int EnemyPower;
        public SlotWinnerSide WinnerSide;
        public int DamageToPlayer;
        public int DamageToEnemy;
    }
}
