using BR3.Domain.Results;
using BR3.Domain.Runtime;

namespace BR3.Presentation.DebugUi
{
    public static class SlotCombatResultTextFormatter
    {
        public static SlotResultRowViewData Format(SlotCombatResult slotCombatResult, CardInstance playerCard)
        {
            if (slotCombatResult == null)
            {
                return new SlotResultRowViewData
                {
                    SlotNameText = "Slot -",
                    SummaryText = "-",
                };
            }

            string playerCardName = CardTextFormatter.FormatTitle(playerCard);
            string enemyCardName = CardTextFormatter.FormatTitle(slotCombatResult.EnemyCardReference?.CardSpec);

            return new SlotResultRowViewData
            {
                SlotNameText = $"Slot {slotCombatResult.SlotIndex + 1}",
                SummaryText = $"{playerCardName} {slotCombatResult.PlayerPower} vs {enemyCardName} {slotCombatResult.EnemyPower} | Winner: {slotCombatResult.WinnerSide} | Damage P/E: {slotCombatResult.DamageToPlayer}/{slotCombatResult.DamageToEnemy}",
            };
        }
    }
}
