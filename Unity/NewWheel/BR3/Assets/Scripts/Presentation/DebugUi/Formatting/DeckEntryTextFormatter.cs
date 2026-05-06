using BR3.Config;
using BR3.Domain.Runtime;

namespace BR3.Presentation.DebugUi
{
    public static class DeckEntryTextFormatter
    {
        public static DeckEntryViewData Format(
            CardInstance cardInstance,
            string deckLabel,
            bool isUsed,
            bool canSelectCard,
            TraitTuning traitTuning,
            string stateText)
        {
            bool isInteractable = canSelectCard && !isUsed && cardInstance != null;

            return new DeckEntryViewData
            {
                CardInstanceId = cardInstance?.InstanceId,
                TitleText = deckLabel,
                TraitsText = CardTextFormatter.FormatTraits(cardInstance, traitTuning),
                StatsText = CardTextFormatter.FormatStats(cardInstance),
                StateText = stateText,
                IsInteractable = isInteractable,
            };
        }
    }
}
