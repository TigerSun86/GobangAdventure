using System.Collections.Generic;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Domain
{
    public sealed class RewardRuntimeStateTests
    {
        [Test]
        public void RewardOffer_CanCarryMultipleRewardOptions()
        {
            RewardOffer rewardOffer = new RewardOffer
            {
                OfferId = "offer-0001",
                RewardIndexForCurrentEnemy = 1,
                Options = new List<RewardOption>
                {
                    new RewardOption
                    {
                        OptionId = "option-0001",
                        Type = RewardOptionType.Upgrade,
                        UpgradePayload = new UpgradePayload
                        {
                            TargetCardInstanceId = "card-0001",
                            AddedTrait = TraitType.Empower,
                        },
                    },
                    new RewardOption
                    {
                        OptionId = "option-0002",
                        Type = RewardOptionType.Replace,
                        ReplacePayload = new ReplacePayload
                        {
                            TargetCardInstanceId = "card-0002",
                            ReplacementCardSpec = TestConfigFactory.CreateCard(
                                RpsType.Paper,
                                4,
                                TraitType.Growth,
                                TraitType.Regrow),
                        },
                    },
                    new RewardOption
                    {
                        OptionId = "option-0003",
                        Type = RewardOptionType.Skip,
                    },
                },
            };

            Assert.That(rewardOffer.OfferId, Is.EqualTo("offer-0001"));
            Assert.That(rewardOffer.RewardIndexForCurrentEnemy, Is.EqualTo(1));
            Assert.That(rewardOffer.Options, Has.Count.EqualTo(3));
            Assert.That(rewardOffer.Options[2].Type, Is.EqualTo(RewardOptionType.Skip));
        }

        [Test]
        public void RewardOption_CanRepresentUpgradeReplaceAndSkipDistinctly()
        {
            RewardOption upgradeOption = new RewardOption
            {
                OptionId = "option-upgrade",
                Type = RewardOptionType.Upgrade,
                UpgradePayload = new UpgradePayload
                {
                    TargetCardInstanceId = "card-0001",
                    AddedTrait = TraitType.Suppress,
                },
            };

            RewardOption replaceOption = new RewardOption
            {
                OptionId = "option-replace",
                Type = RewardOptionType.Replace,
                ReplacePayload = new ReplacePayload
                {
                    TargetCardInstanceId = "card-0002",
                    ReplacementCardSpec = TestConfigFactory.CreateCard(
                        RpsType.Scissors,
                        4,
                        TraitType.AdjacentAid,
                        TraitType.Growth),
                },
            };

            RewardOption skipOption = new RewardOption
            {
                OptionId = "option-skip",
                Type = RewardOptionType.Skip,
            };

            Assert.That(upgradeOption.UpgradePayload, Is.Not.Null);
            Assert.That(upgradeOption.ReplacePayload, Is.Null);
            Assert.That(replaceOption.ReplacePayload, Is.Not.Null);
            Assert.That(replaceOption.UpgradePayload, Is.Null);
            Assert.That(skipOption.UpgradePayload, Is.Null);
            Assert.That(skipOption.ReplacePayload, Is.Null);
        }

        [Test]
        public void ReplacePayload_KeepsReplacementAsAuthoredCardSpecData()
        {
            CardSpec replacementCardSpec = TestConfigFactory.CreateCard(
                RpsType.Rock,
                4,
                TraitType.Empower,
                TraitType.Lifesteal);

            ReplacePayload replacePayload = new ReplacePayload
            {
                TargetCardInstanceId = "card-0004",
                ReplacementCardSpec = replacementCardSpec,
            };

            Assert.That(replacePayload.TargetCardInstanceId, Is.EqualTo("card-0004"));
            Assert.That(replacePayload.ReplacementCardSpec, Is.SameAs(replacementCardSpec));
            Assert.That(replacePayload.ReplacementCardSpec.rpsType, Is.EqualTo(RpsType.Rock));
            Assert.That(replacePayload.ReplacementCardSpec.basePower, Is.EqualTo(4));
            Assert.That(replacePayload.ReplacementCardSpec.traits, Is.EqualTo(new[]
            {
                TraitType.Empower,
                TraitType.Lifesteal,
            }));
        }
    }
}
