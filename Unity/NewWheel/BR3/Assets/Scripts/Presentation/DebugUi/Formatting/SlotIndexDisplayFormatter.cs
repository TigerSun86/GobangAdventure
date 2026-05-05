using System.Text.RegularExpressions;

namespace BR3.Presentation.DebugUi
{
    public static class SlotIndexDisplayFormatter
    {
        private static readonly Regex InlineSlotRegex = new Regex(@"\bslot (\d+)\b", RegexOptions.Compiled);
        private static readonly Regex SnapshotSlotRegex = new Regex(@"\[(\d+):", RegexOptions.Compiled);

        public static string NormalizeVisibleSlotNumbers(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            string normalizedText = InlineSlotRegex.Replace(text, ReplaceInlineSlot);
            normalizedText = SnapshotSlotRegex.Replace(normalizedText, ReplaceSnapshotSlot);
            return normalizedText;
        }

        private static string ReplaceInlineSlot(Match match)
        {
            int slotIndex = int.Parse(match.Groups[1].Value);
            return $"slot {slotIndex + 1}";
        }

        private static string ReplaceSnapshotSlot(Match match)
        {
            int slotIndex = int.Parse(match.Groups[1].Value);
            return $"[{slotIndex + 1}:";
        }
    }
}
