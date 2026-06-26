using BR3.Config;

namespace BR3.Presentation.DebugUi
{
    public static class EnemySequenceTextFormatter
    {
        public static EnemySequenceRowViewData Format(int cardIndex, CardSpec cardSpec)
        {
            return new EnemySequenceRowViewData
            {
                SequenceIndexText = $"Card {cardIndex}",
                SequenceCardText = $"{CardTextFormatter.FormatTitle(cardSpec)} | {CardTextFormatter.FormatStats(cardSpec)} | {CardTextFormatter.FormatTraits(cardSpec)}",
            };
        }
    }
}
