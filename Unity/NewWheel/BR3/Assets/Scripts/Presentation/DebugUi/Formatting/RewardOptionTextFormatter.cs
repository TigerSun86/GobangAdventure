using System.Collections.Generic;
using BR3.Domain.Runtime;

namespace BR3.Presentation.DebugUi
{
    public static class RewardOptionTextFormatter
    {
        public static string Format(RewardOffer rewardOffer, IReadOnlyList<CardInstance> playerDeck)
        {
            if (rewardOffer == null || rewardOffer.Options == null || rewardOffer.Options.Count == 0)
            {
                return "No reward details available.";
            }

            string[] lines = new string[rewardOffer.Options.Count + 1];
            lines[0] = $"Reward {rewardOffer.RewardIndexForCurrentEnemy}: {rewardOffer.Options.Count} options";

            for (int index = 0; index < rewardOffer.Options.Count; index++)
            {
                lines[index + 1] = $"{index + 1}. {FormatOption(rewardOffer.Options[index], playerDeck)}";
            }

            return string.Join("\n", lines);
        }

        private static string FormatOption(RewardOption rewardOption, IReadOnlyList<CardInstance> playerDeck)
        {
            if (rewardOption == null)
            {
                return "-";
            }

            switch (rewardOption.Type)
            {
                case RewardOptionType.Upgrade:
                    CardInstance upgradeTargetCard = FindCard(playerDeck, rewardOption.UpgradePayload?.TargetCardInstanceId);
                    string targetCardName = CardTextFormatter.FormatTitle(upgradeTargetCard);
                    string addedTrait = rewardOption.UpgradePayload == null ? "-" : rewardOption.UpgradePayload.AddedTrait.ToString();
                    return $"Upgrade {targetCardName} with {addedTrait}";
                case RewardOptionType.Replace:
                    CardInstance replaceTargetCard = FindCard(playerDeck, rewardOption.ReplacePayload?.TargetCardInstanceId);
                    return $"Replace {CardTextFormatter.FormatTitle(replaceTargetCard)} with {CardTextFormatter.FormatTitle(rewardOption.ReplacePayload?.ReplacementCardSpec)} ({CardTextFormatter.FormatStats(rewardOption.ReplacePayload?.ReplacementCardSpec)})";
                case RewardOptionType.Skip:
                    return "Skip";
                default:
                    return rewardOption.Type.ToString();
            }
        }

        private static CardInstance FindCard(IReadOnlyList<CardInstance> playerDeck, string instanceId)
        {
            if (playerDeck == null || string.IsNullOrWhiteSpace(instanceId))
            {
                return null;
            }

            for (int index = 0; index < playerDeck.Count; index++)
            {
                if (playerDeck[index] != null && playerDeck[index].InstanceId == instanceId)
                {
                    return playerDeck[index];
                }
            }

            return null;
        }
    }
}
