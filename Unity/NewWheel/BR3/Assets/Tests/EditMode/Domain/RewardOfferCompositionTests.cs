using System.Collections.Generic;
using System.Linq;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Random;
using BR3.Domain.Reward;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Domain
{
    public sealed class RewardOfferCompositionTests
    {
        [Test]
        public void GenerateRewardOffer_WhenTwoOrMoreDistinctUpgradesExist_UsesTwoUpgradeOneReplaceOneSkip()
        {
            RewardOffer rewardOffer = GenerateOffer(
                CreateDeck(
                    CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                    CreateCardInstance("card-0002", RpsType.Scissors, 4, TraitType.Suppress)),
                CreateConfig());

            AssertOfferShape(rewardOffer, expectedUpgrades: 2, expectedReplaces: 1, expectedSkips: 1);
        }

        [Test]
        public void GenerateRewardOffer_WhenExactlyOneDistinctUpgradeExists_UsesOneUpgradeTwoReplaceOneSkip()
        {
            RewardOffer rewardOffer = GenerateOffer(
                CreateDeck(
                    CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                    CreateCardInstance("card-0002", RpsType.Rock, 4, TraitType.Empower)),
                CreateConfig(
                    allowedReplacementTraits: new List<TraitType> { TraitType.Suppress }));

            AssertOfferShape(rewardOffer, expectedUpgrades: 1, expectedReplaces: 2, expectedSkips: 1);
        }

        [Test]
        public void GenerateRewardOffer_WhenNoDistinctUpgradesExist_UsesThreeReplaceOneSkip()
        {
            RewardOffer rewardOffer = GenerateOffer(
                CreateDeck(
                    CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower, TraitType.Suppress, TraitType.Growth),
                    CreateCardInstance("card-0002", RpsType.Scissors, 4, TraitType.Empower, TraitType.Suppress, TraitType.Growth)),
                CreateConfig(
                    allowedReplacementTraits: new List<TraitType> { TraitType.Empower, TraitType.Suppress, TraitType.Growth }));

            AssertOfferShape(rewardOffer, expectedUpgrades: 0, expectedReplaces: 3, expectedSkips: 1);
        }

        [Test]
        public void GenerateRewardOffer_IsDeterministicForRepeatedInputs()
        {
            List<CardInstance> deck = CreateDeck(
                CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                CreateCardInstance("card-0002", RpsType.Scissors, 4, TraitType.Suppress));

            RewardGenerationConfig config = CreateConfig();

            RewardOffer firstOffer = GenerateOffer(deck, config, new SystemGameRandom(12345));
            RewardOffer secondOffer = GenerateOffer(deck, config, new SystemGameRandom(12345));

            List<string> firstOptionShapes = firstOffer.Options
                .Select(ToOptionShape)
                .ToList();

            List<string> secondOptionShapes = secondOffer.Options
                .Select(ToOptionShape)
                .ToList();

            Assert.That(firstOptionShapes, Is.EqualTo(secondOptionShapes));
        }

        [Test]
        public void GenerateRewardOffer_DifferentDeterministicRandomInputsCanProduceDifferentOffers()
        {
            List<CardInstance> deck = CreateDeck(
                CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                CreateCardInstance("card-0002", RpsType.Scissors, 4, TraitType.Suppress));

            RewardGenerationConfig config = CreateConfig();

            RewardOffer firstOffer = GenerateOffer(deck, config, new SystemGameRandom(11111));
            RewardOffer secondOffer = GenerateOffer(deck, config, new SystemGameRandom(22222));

            List<string> firstOptionShapes = firstOffer.Options
                .Select(ToOptionShape)
                .ToList();

            List<string> secondOptionShapes = secondOffer.Options
                .Select(ToOptionShape)
                .ToList();

            Assert.That(firstOptionShapes, Is.Not.EqualTo(secondOptionShapes));
            AssertOfferShape(firstOffer, expectedUpgrades: 2, expectedReplaces: 1, expectedSkips: 1);
            AssertOfferShape(secondOffer, expectedUpgrades: 2, expectedReplaces: 1, expectedSkips: 1);
        }

        private static RewardOffer GenerateOffer(List<CardInstance> deck, RewardGenerationConfig config, IGameRandom gameRandom = null)
        {
            RewardOfferGenerator generator = gameRandom == null
                ? new RewardOfferGenerator()
                : new RewardOfferGenerator(gameRandom);
            return generator.GenerateRewardOffer(deck, config, rewardIndexForCurrentEnemy: 1);
        }

        private static void AssertOfferShape(RewardOffer rewardOffer, int expectedUpgrades, int expectedReplaces, int expectedSkips)
        {
            Assert.That(rewardOffer.Options, Has.Count.EqualTo(4));
            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Upgrade), Is.EqualTo(expectedUpgrades));
            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Replace), Is.EqualTo(expectedReplaces));
            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Skip), Is.EqualTo(expectedSkips));
        }

        private static string ToOptionShape(RewardOption option)
        {
            if (option.Type == RewardOptionType.Upgrade)
            {
                return option.Type + ":" + option.UpgradePayload.TargetCardInstanceId + ":" + option.UpgradePayload.AddedTrait;
            }

            if (option.Type == RewardOptionType.Replace)
            {
                return option.Type + ":" + option.ReplacePayload.TargetCardInstanceId + ":"
                    + option.ReplacePayload.ReplacementCardSpec.rpsType + ":"
                    + option.ReplacePayload.ReplacementCardSpec.basePower + ":"
                    + string.Join(",", option.ReplacePayload.ReplacementCardSpec.traits);
            }

            return option.Type.ToString();
        }

        private static RewardGenerationConfig CreateConfig(List<TraitType> allowedReplacementTraits = null)
        {
            return TestConfigFactory.CreateValidRewardGenerationConfig(
                allowedReplacementRpsTypes: new List<RpsType> { RpsType.Rock, RpsType.Scissors, RpsType.Paper },
                allowedReplacementBasePowers: new List<int> { 4 },
                allowedReplacementTraits: allowedReplacementTraits ?? new List<TraitType>
                {
                    TraitType.Empower,
                    TraitType.Suppress,
                    TraitType.Growth,
                },
                replacementTraitCount: 1);
        }

        private static List<CardInstance> CreateDeck(params CardInstance[] cards)
        {
            return new List<CardInstance>(cards);
        }

        private static CardInstance CreateCardInstance(
            string instanceId,
            RpsType rpsType,
            int basePower,
            params TraitType[] traits)
        {
            return new CardInstance
            {
                InstanceId = instanceId,
                RpsType = rpsType,
                BasePower = basePower,
                PermanentPowerBonus = 0,
                Traits = new List<TraitType>(traits),
            };
        }
    }
}
