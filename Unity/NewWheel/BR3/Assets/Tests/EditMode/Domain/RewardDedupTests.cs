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
    public sealed class RewardDedupTests
    {
        [Test]
        public void DeduplicateUpgradeCandidates_CollapsesEquivalentResultsAcrossIdenticalCards()
        {
            RewardOfferGenerator generator = new RewardOfferGenerator();
            List<CardInstance> deck = new List<CardInstance>
            {
                CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                CreateCardInstance("card-0002", RpsType.Rock, 4, TraitType.Empower),
            };

            List<UpgradeCandidate> rawCandidates = new List<UpgradeCandidate>
            {
                new UpgradeCandidate { TargetCardInstanceId = "card-0001", AddedTrait = TraitType.Suppress },
                new UpgradeCandidate { TargetCardInstanceId = "card-0002", AddedTrait = TraitType.Suppress },
            };

            List<UpgradeCandidate> dedupedCandidates = generator.DeduplicateUpgradeCandidates(deck, rawCandidates);

            Assert.That(dedupedCandidates, Has.Count.EqualTo(1));
        }

        [Test]
        public void DeduplicateReplaceCandidates_CollapsesTraitOrderOnlyDifferences()
        {
            RewardOfferGenerator generator = new RewardOfferGenerator();
            List<CardInstance> deck = new List<CardInstance>
            {
                CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
            };

            List<ReplaceCandidate> rawCandidates = new List<ReplaceCandidate>
            {
                new ReplaceCandidate
                {
                    TargetCardInstanceId = "card-0001",
                    ReplacementCardSpec = TestConfigFactory.CreateCard(RpsType.Paper, 4, TraitType.Suppress, TraitType.Growth),
                },
                new ReplaceCandidate
                {
                    TargetCardInstanceId = "card-0001",
                    ReplacementCardSpec = TestConfigFactory.CreateCard(RpsType.Paper, 4, TraitType.Growth, TraitType.Suppress),
                },
            };

            List<ReplaceCandidate> dedupedCandidates = generator.DeduplicateReplaceCandidates(deck, rawCandidates);

            Assert.That(dedupedCandidates, Has.Count.EqualTo(1));
        }

        [Test]
        public void DeduplicateReplaceCandidates_CollapsesEquivalentTargetsAcrossIdenticalCards()
        {
            RewardOfferGenerator generator = new RewardOfferGenerator();
            List<CardInstance> deck = new List<CardInstance>
            {
                CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                CreateCardInstance("card-0002", RpsType.Rock, 4, TraitType.Empower),
            };

            List<ReplaceCandidate> rawCandidates = new List<ReplaceCandidate>
            {
                new ReplaceCandidate
                {
                    TargetCardInstanceId = "card-0001",
                    ReplacementCardSpec = TestConfigFactory.CreateCard(RpsType.Paper, 4, TraitType.Suppress),
                },
                new ReplaceCandidate
                {
                    TargetCardInstanceId = "card-0002",
                    ReplacementCardSpec = TestConfigFactory.CreateCard(RpsType.Paper, 4, TraitType.Suppress),
                },
            };

            List<ReplaceCandidate> dedupedCandidates = generator.DeduplicateReplaceCandidates(deck, rawCandidates);

            Assert.That(dedupedCandidates, Has.Count.EqualTo(1));
        }

        [Test]
        public void GenerateRewardOffer_UsesDedupedCandidatePools()
        {
            RewardOfferGenerator generator = new RewardOfferGenerator(new SystemGameRandom(12345));
            List<CardInstance> deck = new List<CardInstance>
            {
                CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                CreateCardInstance("card-0002", RpsType.Rock, 4, TraitType.Empower),
            };

            RewardGenerationConfig config = TestConfigFactory.CreateValidRewardGenerationConfig(
                allowedReplacementRpsTypes: new List<RpsType> { RpsType.Rock, RpsType.Paper },
                allowedReplacementBasePowers: new List<int> { 4 },
                allowedReplacementTraits: new List<TraitType> { TraitType.Suppress },
                replacementTraitCount: 1);

            RewardOffer rewardOffer = generator.GenerateRewardOffer(deck, config, 1);

            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Upgrade), Is.EqualTo(1));
            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Replace), Is.EqualTo(2));
            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Skip), Is.EqualTo(1));

            List<string> resultingDeckSignatures = rewardOffer.Options
                .Where(option => option.Type != RewardOptionType.Skip)
                .Select(option => CreateResultingDeckSignature(deck, option))
                .ToList();

            Assert.That(resultingDeckSignatures.Distinct().Count(), Is.EqualTo(resultingDeckSignatures.Count));
        }

        [Test]
        public void GenerateRewardOffer_ConfigDrivenCountsStillPreserveCanonicalDedup()
        {
            RewardOfferGenerator generator = new RewardOfferGenerator(new SystemGameRandom(12345));
            List<CardInstance> deck = new List<CardInstance>
            {
                CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower),
                CreateCardInstance("card-0002", RpsType.Scissors, 4, TraitType.Empower),
            };

            RewardGenerationConfig config = TestConfigFactory.CreateValidRewardGenerationConfig(
                allowedReplacementRpsTypes: new List<RpsType> { RpsType.Rock, RpsType.Paper },
                allowedReplacementBasePowers: new List<int> { 4 },
                allowedReplacementTraits: new List<TraitType> { TraitType.Suppress },
                replacementTraitCount: 1,
                rewardOfferTotalOptions: 5,
                upgradeTarget: 1);

            RewardOffer rewardOffer = generator.GenerateRewardOffer(deck, config, 1);

            Assert.That(rewardOffer.Options, Has.Count.EqualTo(5));
            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Upgrade), Is.EqualTo(1));
            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Replace), Is.EqualTo(3));
            Assert.That(rewardOffer.Options.Count(option => option.Type == RewardOptionType.Skip), Is.EqualTo(1));

            List<string> resultingDeckSignatures = rewardOffer.Options
                .Where(option => option.Type != RewardOptionType.Skip)
                .Select(option => CreateResultingDeckSignature(deck, option))
                .ToList();

            Assert.That(resultingDeckSignatures.Distinct().Count(), Is.EqualTo(resultingDeckSignatures.Count));
        }

        private static string CreateResultingDeckSignature(IEnumerable<CardInstance> deck, RewardOption option)
        {
            List<CardInstance> simulatedDeck = deck
                .Select(card => new CardInstance
                {
                    InstanceId = card.InstanceId,
                    RpsType = card.RpsType,
                    BasePower = card.BasePower,
                    PermanentPowerBonus = card.PermanentPowerBonus,
                    Traits = new List<TraitType>(card.Traits),
                })
                .ToList();

            switch (option.Type)
            {
                case RewardOptionType.Upgrade:
                    simulatedDeck.Single(card => card.InstanceId == option.UpgradePayload.TargetCardInstanceId)
                        .Traits
                        .Add(option.UpgradePayload.AddedTrait);
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
