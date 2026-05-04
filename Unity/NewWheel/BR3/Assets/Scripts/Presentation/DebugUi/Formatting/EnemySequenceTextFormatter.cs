using BR3.Config;

namespace BR3.Presentation.DebugUi
{
    public static class EnemySequenceTextFormatter
    {
        public static EnemySequenceRowViewData Format(int roundIndex, CardSpec cardSpec)
        {
            return new EnemySequenceRowViewData
            {
                SequenceIndexText = $"R{roundIndex}",
                SequenceCardText = $"{CardTextFormatter.FormatTitle(cardSpec)} | {CardTextFormatter.FormatStats(cardSpec)} | {CardTextFormatter.FormatTraits(cardSpec)}",
            };
        }
    }
}
