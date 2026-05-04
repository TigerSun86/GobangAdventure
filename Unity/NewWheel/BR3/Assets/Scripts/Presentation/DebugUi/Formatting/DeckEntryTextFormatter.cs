using BR3.Domain.Runtime;

namespace BR3.Presentation.DebugUi
{
    public static class DeckEntryTextFormatter
    {
        public static DeckEntryViewData Format(
            CardInstance cardInstance,
            bool isUsed,
            bool canSelectCard,
            string stateText)
        {
            bool isInteractable = canSelectCard && !isUsed && cardInstance != null;

            return new DeckEntryViewData
            {
                CardInstanceId = cardInstance?.InstanceId,
                TitleText = CardTextFormatter.FormatTitle(cardInstance),
                TraitsText = CardTextFormatter.FormatTraits(cardInstance),
                StatsText = CardTextFormatter.FormatStats(cardInstance),
                StateText = stateText,
                IsInteractable = isInteractable,
            };
        }
    }
}
