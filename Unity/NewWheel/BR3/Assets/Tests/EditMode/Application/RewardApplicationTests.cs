using System.Collections.Generic;
using BR3.Application;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Application
{
    public sealed class RewardApplicationTests
    {
        [Test]
        public void ApplyRewardOption_WithUpgrade_ModifiesExistingCardInPlace()
        {
            RuntimeStateFactory runtimeStateFactory = new RuntimeStateFactory();
            RewardService rewardService = new RewardService(runtimeStateFactory);
            List<CardInstance> playerDeck = CreateDeck(runtimeStateFactory);

            CardInstance originalTargetCard = playerDeck[0];
            string originalInstanceId = originalTargetCard.InstanceId;
            CardInstance untouchedCard = playerDeck[1];

            rewardService.ApplyRewardOption(playerDeck, new RewardOption
            {
                Type = RewardOptionType.Upgrade,
                UpgradePayload = new UpgradePayload
                {
                    TargetCardInstanceId = originalInstanceId,
                    AddedTrait = TraitType.Suppress,
                },
            });

            Assert.That(playerDeck, Has.Count.EqualTo(2));
            Assert.That(playerDeck[0], Is.SameAs(originalTargetCard));
            Assert.That(playerDeck[0].InstanceId, Is.EqualTo(originalInstanceId));
            Assert.That(playerDeck[0].RpsType, Is.EqualTo(RpsType.Rock));
            Assert.That(playerDeck[0].BasePower, Is.EqualTo(4));
            Assert.That(playerDeck[0].PermanentPowerBonus, Is.EqualTo(0));
            Assert.That(playerDeck[0].Traits, Is.EqualTo(new[] { TraitType.Empower, TraitType.Suppress }));
            Assert.That(playerDeck[1], Is.SameAs(untouchedCard));
        }

        [Test]
        public void ApplyRewardOption_WithReplace_CreatesNewCardAtSameDeckPosition()
        {
            RuntimeStateFactory runtimeStateFactory = new RuntimeStateFactory();
            RewardService rewardService = new RewardService(runtimeStateFactory);
            List<CardInstance> playerDeck = CreateDeck(runtimeStateFactory);

            CardInstance originalTargetCard = playerDeck[0];
            string originalInstanceId = originalTargetCard.InstanceId;
            playerDeck[0].PermanentPowerBonus = 3;
            CardInstance untouchedCard = playerDeck[1];

            rewardService.ApplyRewardOption(playerDeck, new RewardOption
            {
                Type = RewardOptionType.Replace,
                ReplacePayload = new ReplacePayload
                {
                    TargetCardInstanceId = originalInstanceId,
                    ReplacementCardSpec = TestConfigFactory.CreateCard(
                        RpsType.Paper,
                        4,
                        TraitType.Regrow,
                        TraitType.Growth),
                },
            });

            Assert.That(playerDeck, Has.Count.EqualTo(2));
            Assert.That(playerDeck[0], Is.Not.SameAs(originalTargetCard));
            Assert.That(playerDeck[0].InstanceId, Is.Not.EqualTo(originalInstanceId));
            Assert.That(playerDeck[0].RpsType, Is.EqualTo(RpsType.Paper));
            Assert.That(playerDeck[0].BasePower, Is.EqualTo(4));
            Assert.That(playerDeck[0].Traits, Is.EqualTo(new[] { TraitType.Regrow, TraitType.Growth }));
            Assert.That(playerDeck[0].PermanentPowerBonus, Is.EqualTo(0));
            Assert.That(playerDeck[1], Is.SameAs(untouchedCard));
        }

        [Test]
        public void ApplyRewardOption_WithSkip_LeavesDeckUnchanged()
        {
            RuntimeStateFactory runtimeStateFactory = new RuntimeStateFactory();
            RewardService rewardService = new RewardService(runtimeStateFactory);
            List<CardInstance> playerDeck = CreateDeck(runtimeStateFactory);

            CardInstance firstCard = playerDeck[0];
            CardInstance secondCard = playerDeck[1];
            string firstInstanceId = firstCard.InstanceId;
            string secondInstanceId = secondCard.InstanceId;

            rewardService.ApplyRewardOption(playerDeck, new RewardOption
            {
                Type = RewardOptionType.Skip,
            });

            Assert.That(playerDeck, Has.Count.EqualTo(2));
            Assert.That(playerDeck[0], Is.SameAs(firstCard));
            Assert.That(playerDeck[1], Is.SameAs(secondCard));
            Assert.That(playerDeck[0].InstanceId, Is.EqualTo(firstInstanceId));
            Assert.That(playerDeck[1].InstanceId, Is.EqualTo(secondInstanceId));
        }

        [Test]
        public void ApplyRewardOption_WithMismatchedPayload_ThrowsClearly()
        {
            RuntimeStateFactory runtimeStateFactory = new RuntimeStateFactory();
            RewardService rewardService = new RewardService(runtimeStateFactory);
            List<CardInstance> playerDeck = CreateDeck(runtimeStateFactory);

            Assert.That(
                () => rewardService.ApplyRewardOption(playerDeck, new RewardOption
                {
                    Type = RewardOptionType.Upgrade,
                    UpgradePayload = null,
                }),
                Throws.InvalidOperationException.With.Message.Contains("UpgradePayload"));
        }

        [Test]
        public void ApplyRewardOption_WhenTargetCardIsMissing_ThrowsClearly()
        {
            RuntimeStateFactory runtimeStateFactory = new RuntimeStateFactory();
            RewardService rewardService = new RewardService(runtimeStateFactory);
            List<CardInstance> playerDeck = CreateDeck(runtimeStateFactory);

            Assert.That(
                () => rewardService.ApplyRewardOption(playerDeck, new RewardOption
                {
                    Type = RewardOptionType.Replace,
                    ReplacePayload = new ReplacePayload
                    {
                        TargetCardInstanceId = "missing-card",
                        ReplacementCardSpec = TestConfigFactory.CreateCard(RpsType.Paper, 4, TraitType.Regrow),
                    },
                }),
                Throws.InvalidOperationException.With.Message.Contains("missing-card"));
        }

        private static List<CardInstance> CreateDeck(RuntimeStateFactory runtimeStateFactory)
        {
            return new List<CardInstance>
            {
                runtimeStateFactory.CreateCardInstance(TestConfigFactory.CreateCard(RpsType.Rock, 4, TraitType.Empower)),
                runtimeStateFactory.CreateCardInstance(TestConfigFactory.CreateCard(RpsType.Scissors, 4, TraitType.Suppress)),
            };
        }
    }
}
