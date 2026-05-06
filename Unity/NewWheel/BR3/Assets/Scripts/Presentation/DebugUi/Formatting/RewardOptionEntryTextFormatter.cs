using BR3.Config;
using BR3.Domain.Runtime;

namespace BR3.Presentation.DebugUi
{
    public static class RewardOptionEntryTextFormatter
    {
        public static RewardOptionEntryViewData Format(
            RewardOption rewardOption,
            CardInstance targetCard,
            string targetCardLabel,
            TraitTuning traitTuning,
            bool isInteractable)
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
                TitleText = FormatTitle(rewardOption, targetCardLabel),
                DetailsText = FormatDetails(rewardOption, targetCardLabel, traitTuning),
                IsInteractable = isInteractable,
            };
        }

        private static string FormatTitle(RewardOption rewardOption, string targetCardLabel)
        {
            switch (rewardOption.Type)
            {
                case RewardOptionType.Upgrade:
                    return $"Upgrade {targetCardLabel}";
                case RewardOptionType.Replace:
                    return $"Replace {targetCardLabel}";
                case RewardOptionType.Skip:
                    return "Skip";
                default:
                    return rewardOption.Type.ToString();
            }
        }

        private static string FormatDetails(RewardOption rewardOption, string targetCardLabel, TraitTuning traitTuning)
        {
            switch (rewardOption.Type)
            {
                case RewardOptionType.Upgrade:
                    string addedTrait = rewardOption.UpgradePayload == null
                        ? "-"
                        : TraitListFormatter.FormatTrait(rewardOption.UpgradePayload.AddedTrait, traitTuning);
                    return $"Add: {addedTrait}";
                case RewardOptionType.Replace:
                    string replacementTitle = CardTextFormatter.FormatTitle(rewardOption.ReplacePayload?.ReplacementCardSpec);
                    string replacementTraits = CardTextFormatter.FormatTraits(rewardOption.ReplacePayload?.ReplacementCardSpec, traitTuning);
                    return $"With: {replacementTitle} | {replacementTraits}";
                case RewardOptionType.Skip:
                    return "Leave the deck unchanged.";
                default:
                    return targetCardLabel;
            }
        }
    }
}
