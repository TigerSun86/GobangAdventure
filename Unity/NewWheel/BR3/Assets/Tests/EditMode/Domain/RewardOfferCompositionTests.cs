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
        public void GenerateRewardOffer_UsesConfiguredOfferSizeAndExactlyOneSkip()
        {
            RewardGenerationConfig config = CreateConfig(
                replacementTraitCount: 1,
                rewardOfferTotalOptions: 5,
                upgradeTarget: 1);

            RewardOffer rewardOffer = GenerateOffer(
                CreateDeck(
                    CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                    CreateCardInstance("card-0002", RpsType.Scissors, 4, TraitType.Suppress)),
                config);

            Assert.That(rewardOffer.Options, Has.Count.EqualTo(config.rewardOfferTotalOptions));
            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Skip), Is.EqualTo(1));
        }

        [Test]
        public void GenerateRewardOffer_SelectedUpgradeCountNeverExceedsConfiguredUpgradeTarget()
        {
            RewardGenerationConfig config = CreateConfig(
                replacementTraitCount: 1,
                rewardOfferTotalOptions: 5,
                upgradeTarget: 1);

            RewardOffer rewardOffer = GenerateOffer(
                CreateDeck(
                    CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                    CreateCardInstance("card-0002", RpsType.Scissors, 4, TraitType.Suppress)),
                config);

            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Upgrade), Is.LessThanOrEqualTo(config.upgradeTarget));
        }

        [Test]
        public void GenerateRewardOffer_ReplaceCountFillsRemainingNonSkipCapacity()
        {
            RewardGenerationConfig config = CreateConfig(
                replacementTraitCount: 1,
                rewardOfferTotalOptions: 5,
                upgradeTarget: 1);

            RewardOffer rewardOffer = GenerateOffer(
                CreateDeck(
                    CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                    CreateCardInstance("card-0002", RpsType.Scissors, 4, TraitType.Suppress)),
                config);

            int actualUpgradeCount = rewardOffer.Options.Count(option => option.Type == RewardOptionType.Upgrade);
            int actualReplaceCount = rewardOffer.Options.Count(option => option.Type == RewardOptionType.Replace);
            int expectedReplaceCount = config.rewardOfferTotalOptions - 1 - actualUpgradeCount;

            Assert.That(actualReplaceCount, Is.EqualTo(expectedReplaceCount));
        }

        [Test]
        public void GenerateRewardOffer_NonSkipOptionsAreDistinctByCanonicalResultingDeckState()
        {
            RewardGenerationConfig config = CreateConfig(
                replacementTraitCount: 1,
                rewardOfferTotalOptions: 4,
                upgradeTarget: 2);

            List<CardInstance> deck = CreateDeck(
                CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                CreateCardInstance("card-0002", RpsType.Rock, 4, TraitType.Empower));

            RewardOffer rewardOffer = GenerateOffer(deck, config);

            List<string> resultingDeckSignatures = rewardOffer.Options
                .Where(option => option.Type != RewardOptionType.Skip)
                .Select(option => CreateResultingDeckSignature(deck, option))
                .ToList();

            Assert.That(resultingDeckSignatures.Distinct().Count(), Is.EqualTo(resultingDeckSignatures.Count));
        }

        [Test]
        public void GenerateRewardOffer_WhenDefaultBaselineHasAtLeastTwoDistinctUpgrades_UsesTwoUpgradeOneReplaceOneSkip()
        {
            RewardOffer rewardOffer = GenerateOffer(
                CreateDeck(
                    CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                    CreateCardInstance("card-0002", RpsType.Scissors, 4, TraitType.Suppress)),
                CreateConfig(
                    replacementTraitCount: 1,
                    rewardOfferTotalOptions: 4,
                    upgradeTarget: 2));

            AssertOfferShape(rewardOffer, expectedTotalOptions: 4, expectedUpgrades: 2, expectedReplaces: 1, expectedSkips: 1);
        }

        [Test]
        public void GenerateRewardOffer_WhenDefaultBaselineHasExactlyOneDistinctUpgrade_UsesOneUpgradeTwoReplaceOneSkip()
        {
            RewardOffer rewardOffer = GenerateOffer(
                CreateDeck(
                    CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                    CreateCardInstance("card-0002", RpsType.Rock, 4, TraitType.Empower)),
                CreateConfig(
                    allowedReplacementTraits: new List<TraitType> { TraitType.Suppress },
                    replacementTraitCount: 1,
                    rewardOfferTotalOptions: 4,
                    upgradeTarget: 2));

            AssertOfferShape(rewardOffer, expectedTotalOptions: 4, expectedUpgrades: 1, expectedReplaces: 2, expectedSkips: 1);
        }

        [Test]
        public void GenerateRewardOffer_WhenDefaultBaselineHasNoDistinctUpgrades_UsesZeroUpgradeThreeReplaceOneSkip()
        {
            RewardOffer rewardOffer = GenerateOffer(
                CreateDeck(
                    CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower, TraitType.Suppress, TraitType.Growth),
                    CreateCardInstance("card-0002", RpsType.Scissors, 4, TraitType.Empower, TraitType.Suppress, TraitType.Growth)),
                CreateConfig(
                    allowedReplacementTraits: new List<TraitType> { TraitType.Empower, TraitType.Suppress, TraitType.Growth },
                    replacementTraitCount: 2,
                    rewardOfferTotalOptions: 4,
                    upgradeTarget: 2));

            AssertOfferShape(rewardOffer, expectedTotalOptions: 4, expectedUpgrades: 0, expectedReplaces: 3, expectedSkips: 1);
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
            AssertOfferShape(firstOffer, expectedTotalOptions: 4, expectedUpgrades: 2, expectedReplaces: 1, expectedSkips: 1);
            AssertOfferShape(secondOffer, expectedTotalOptions: 4, expectedUpgrades: 2, expectedReplaces: 1, expectedSkips: 1);
        }

        private static RewardOffer GenerateOffer(List<CardInstance> deck, RewardGenerationConfig config, IGameRandom gameRandom = null)
        {
            RewardOfferGenerator generator = gameRandom == null
                ? new RewardOfferGenerator()
                : new RewardOfferGenerator(gameRandom);
            return generator.GenerateRewardOffer(deck, config, rewardIndexForCurrentEnemy: 1);
        }

        private static void AssertOfferShape(RewardOffer rewardOffer, int expectedTotalOptions, int expectedUpgrades, int expectedReplaces, int expectedSkips)
        {
            Assert.That(rewardOffer.Options, Has.Count.EqualTo(expectedTotalOptions));
            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Upgrade), Is.EqualTo(expectedUpgrades));
            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Replace), Is.EqualTo(expectedReplaces));
            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Skip), Is.EqualTo(expectedSkips));
        }

        private static string CreateResultingDeckSignature(IEnumerable<CardInstance> deck, RewardOption option)
        {
            List<CardInstance> simulatedDeck = deck
                .Select(CloneCardInstance)
                .ToList();

            switch (option.Type)
            {
                case RewardOptionType.Upgrade:
                    CardInstance upgradeTarget = simulatedDeck.Single(card => card.InstanceId == option.UpgradePayload.TargetCardInstanceId);
                    upgradeTarget.Traits.Add(option.UpgradePayload.AddedTrait);
                    break;
                case RewardOptionType.Replace:
                    int replaceIndex = simulatedDeck.FindIndex(card => card.InstanceId == option.ReplacePayload.TargetCardInstanceId);
                    simulatedDeck[replaceIndex] = new CardInstance
                    {
                        InstanceId = "simulated-replacement",
                        RpsType = option.ReplacePayload.ReplacementCardSpec.rpsType,
                        BasePower = option.ReplacePayload.ReplacementCardSpec.basePower,
                        PermanentPowerBonus = 0,
                        Traits = new List<TraitType>(option.ReplacePayload.ReplacementCardSpec.traits),
                    };
                    break;
            }

            return RewardCanonicalSignatureFactory.CreateDeckSignature(simulatedDeck).SignatureText;
        }

        private static CardInstance CloneCardInstance(CardInstance card)
        {
            return new CardInstance
            {
                InstanceId = card.InstanceId,
                RpsType = card.RpsType,
                BasePower = card.BasePower,
                PermanentPowerBonus = card.PermanentPowerBonus,
                Traits = new List<TraitType>(card.Traits),
            };
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

        private static RewardGenerationConfig CreateConfig(
            List<TraitType> allowedReplacementTraits = null,
            int replacementTraitCount = 1,
            int rewardOfferTotalOptions = 4,
            int upgradeTarget = 2)
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
                replacementTraitCount: replacementTraitCount,
                rewardOfferTotalOptions: rewardOfferTotalOptions,
                upgradeTarget: upgradeTarget);
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
