using System;
using System.Collections.Generic;
using BR3.Domain;
using BR3.Domain.Runtime;

namespace BR3.Application
{
    public sealed class RewardService
    {
        private readonly RuntimeStateFactory _runtimeStateFactory;

        public RewardService(RuntimeStateFactory runtimeStateFactory)
        {
            _runtimeStateFactory = runtimeStateFactory ?? throw new ArgumentNullException(nameof(runtimeStateFactory));
        }

        public void ApplyRewardOption(List<CardInstance> playerDeck, RewardOption rewardOption)
        {
            if (playerDeck == null)
            {
                throw new ArgumentNullException(nameof(playerDeck));
            }

            if (rewardOption == null)
            {
                throw new ArgumentNullException(nameof(rewardOption));
            }

            switch (rewardOption.Type)
            {
                case RewardOptionType.Upgrade:
                    ApplyUpgrade(playerDeck, rewardOption);
                    break;
                case RewardOptionType.Replace:
                    ApplyReplace(playerDeck, rewardOption);
                    break;
                case RewardOptionType.Skip:
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported reward option type: {rewardOption.Type}.");
            }
        }

        private static void ApplyUpgrade(List<CardInstance> playerDeck, RewardOption rewardOption)
        {
            if (rewardOption.UpgradePayload == null)
            {
                throw new InvalidOperationException("Upgrade reward option must provide an UpgradePayload.");
            }

            CardInstance targetCard = FindTargetCard(playerDeck, rewardOption.UpgradePayload.TargetCardInstanceId);

            if (targetCard.Traits.Contains(rewardOption.UpgradePayload.AddedTrait))
            {
                throw new InvalidOperationException("Upgrade reward option cannot add a duplicate trait.");
            }

            if (targetCard.Traits.Count >= 3)
            {
                throw new InvalidOperationException("Upgrade reward option cannot add a fourth trait.");
            }

            bool hasShiftLeft = targetCard.Traits.Contains(TraitType.ShiftLeft) || rewardOption.UpgradePayload.AddedTrait == TraitType.ShiftLeft;
            bool hasShiftRight = targetCard.Traits.Contains(TraitType.ShiftRight) || rewardOption.UpgradePayload.AddedTrait == TraitType.ShiftRight;
            if (hasShiftLeft && hasShiftRight)
            {
                throw new InvalidOperationException("Upgrade reward option cannot create a ShiftLeft and ShiftRight conflict.");
            }

            targetCard.Traits.Add(rewardOption.UpgradePayload.AddedTrait);
        }

        private void ApplyReplace(List<CardInstance> playerDeck, RewardOption rewardOption)
        {
            if (rewardOption.ReplacePayload == null)
            {
                throw new InvalidOperationException("Replace reward option must provide a ReplacePayload.");
            }

            if (rewardOption.ReplacePayload.ReplacementCardSpec == null)
            {
                throw new InvalidOperationException("Replace reward option must provide a replacement CardSpec.");
            }

            int targetCardIndex = FindTargetCardIndex(playerDeck, rewardOption.ReplacePayload.TargetCardInstanceId);
            CardInstance replacementCard = _runtimeStateFactory.CreateCardInstance(rewardOption.ReplacePayload.ReplacementCardSpec);
            playerDeck[targetCardIndex] = replacementCard;
        }

        private static CardInstance FindTargetCard(List<CardInstance> playerDeck, string targetCardInstanceId)
        {
            int targetCardIndex = FindTargetCardIndex(playerDeck, targetCardInstanceId);
            return playerDeck[targetCardIndex];
        }

        private static int FindTargetCardIndex(List<CardInstance> playerDeck, string targetCardInstanceId)
        {
            for (int i = 0; i < playerDeck.Count; i++)
            {
                if (playerDeck[i].InstanceId == targetCardInstanceId)
                {
                    return i;
                }
            }

            throw new InvalidOperationException($"Could not find target card instance '{targetCardInstanceId}' in the player deck.");
        }
    }
}
