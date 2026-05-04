using BR3.Domain.Runtime;

namespace BR3.Presentation.DebugUi
{
    public static class RewardOptionEntryTextFormatter
    {
        public static RewardOptionEntryViewData Format(RewardOption rewardOption, CardInstance targetCard, bool isInteractable)
        {
            if (rewardOption == null)
            {
                return new RewardOptionEntryViewData
                {
                    OptionId = string.Empty,
                    TitleText = "-",
                    DetailsText = "-",
                    IsInteractable = false,
                };
            }

            return new RewardOptionEntryViewData
            {
                OptionId = rewardOption.OptionId,
                TitleText = rewardOption.Type.ToString(),
                DetailsText = FormatDetails(rewardOption, targetCard),
                IsInteractable = isInteractable,
            };
        }

        private static string FormatDetails(RewardOption rewardOption, CardInstance targetCard)
        {
            switch (rewardOption.Type)
            {
                case RewardOptionType.Upgrade:
                    string upgradeTarget = CardTextFormatter.FormatTitle(targetCard);
                    string addedTrait = rewardOption.UpgradePayload == null ? "-" : rewardOption.UpgradePayload.AddedTrait.ToString();
                    return $"{upgradeTarget}\nAdd: {addedTrait}";
                case RewardOptionType.Replace:
                    string replaceTarget = CardTextFormatter.FormatTitle(targetCard);
                    string replacementTitle = CardTextFormatter.FormatTitle(rewardOption.ReplacePayload?.ReplacementCardSpec);
                    string replacementStats = CardTextFormatter.FormatStats(rewardOption.ReplacePayload?.ReplacementCardSpec);
                    return $"{replaceTarget}\nWith: {replacementTitle} | {replacementStats}";
                case RewardOptionType.Skip:
                    return "Leave the deck unchanged.";
                default:
                    return rewardOption.Type.ToString();
            }
        }
    }
}
