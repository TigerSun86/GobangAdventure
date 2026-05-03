using BR3.Domain.Runtime;
using BR3.Domain.Results;

namespace BR3.Presentation.DebugUi
{
    public static class RoundResultTextFormatter
    {
        public static string FormatSummary(RoundResult roundResult, CardInstance playerCard)
        {
            if (roundResult == null)
            {
                return "-";
            }

            string playerCardName = CardTextFormatter.FormatTitle(playerCard);
            string enemyCardName = CardTextFormatter.FormatTitle(roundResult.EnemyCardReference?.CardSpec);
            string playerHpAfter = roundResult.PlayerHpAfter?.ToString() ?? "-";
            string enemyHpAfter = roundResult.EnemyHpAfter?.ToString() ?? "-";

            return
                $"Round {roundResult.RoundIndex}\n" +
                $"Player Card: {playerCardName}\n" +
                $"Enemy Card: {enemyCardName}\n" +
                $"Damage P/E: {roundResult.DamageToPlayer} / {roundResult.DamageToEnemy}\n" +
                $"Heal P/E: {roundResult.HealToPlayer} / {roundResult.HealToEnemy}\n" +
                $"Player HP: {roundResult.PlayerHpBefore} -> {playerHpAfter}\n" +
                $"Enemy HP: {roundResult.EnemyHpBefore} -> {enemyHpAfter}";
        }
    }
}
