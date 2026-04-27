using System.Collections.Generic;
using System.Linq;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Reward;
using BR3.Domain.Runtime;
using BR3.Tests.EditMode.TestHelpers;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Domain
{
    public sealed class RewardReplaceGenerationTests
    {
        [Test]
        public void IsLegalReplacementSpec_UsesAuthoredRewardGenerationConfig()
        {
            RewardGenerationConfig rewardGenerationConfig = TestConfigFactory.CreateValidRewardGenerationConfig(
                allowedReplacementRpsTypes: new List<RpsType> { RpsType.Paper },
                allowedReplacementBasePowers: new List<int> { 6 },
                allowedReplacementTraits: new List<TraitType> { TraitType.Empower, TraitType.Growth },
                replacementTraitCount: 2);

            CardSpec replacementSpec = TestConfigFactory.CreateCard(
                RpsType.Paper,
                6,
                TraitType.Empower,
                TraitType.Growth);

            bool isLegal = RewardRawCandidateGenerator.IsLegalReplacementSpec(replacementSpec, rewardGenerationConfig);

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReplacementSpec_RejectsDisallowedRpsType()
        {
            RewardGenerationConfig rewardGenerationConfig = TestConfigFactory.CreateValidRewardGenerationConfig(
                allowedReplacementRpsTypes: new List<RpsType> { RpsType.Rock });

            CardSpec replacementSpec = TestConfigFactory.CreateCard(
                RpsType.Paper,
                4,
                TraitType.Empower,
                TraitType.Growth);

            bool isLegal = RewardRawCandidateGenerator.IsLegalReplacementSpec(replacementSpec, rewardGenerationConfig);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void IsLegalReplacementSpec_RejectsDisallowedBasePower()
        {
            RewardGenerationConfig rewardGenerationConfig = TestConfigFactory.CreateValidRewardGenerationConfig(
                allowedReplacementBasePowers: new List<int> { 5 });

            CardSpec replacementSpec = TestConfigFactory.CreateCard(
                RpsType.Rock,
                4,
                TraitType.Empower,
                TraitType.Growth);

            bool isLegal = RewardRawCandidateGenerator.IsLegalReplacementSpec(replacementSpec, rewardGenerationConfig);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void IsLegalReplacementSpec_RejectsWrongTraitCount()
        {
            RewardGenerationConfig rewardGenerationConfig = TestConfigFactory.CreateValidRewardGenerationConfig(
                replacementTraitCount: 2);

            CardSpec replacementSpec = TestConfigFactory.CreateCard(
                RpsType.Rock,
                4,
                TraitType.Empower);

            bool isLegal = RewardRawCandidateGenerator.IsLegalReplacementSpec(replacementSpec, rewardGenerationConfig);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void IsLegalReplacementSpec_RejectsDuplicateTraits()
        {
            RewardGenerationConfig rewardGenerationConfig = TestConfigFactory.CreateValidRewardGenerationConfig();

            CardSpec replacementSpec = TestConfigFactory.CreateCard(
                RpsType.Rock,
                4,
                TraitType.Empower,
                TraitType.Empower);

            bool isLegal = RewardRawCandidateGenerator.IsLegalReplacementSpec(replacementSpec, rewardGenerationConfig);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void IsLegalReplacementSpec_RejectsShiftConflict()
        {
            RewardGenerationConfig rewardGenerationConfig = TestConfigFactory.CreateValidRewardGenerationConfig();

            CardSpec replacementSpec = TestConfigFactory.CreateCard(
                RpsType.Rock,
                4,
                TraitType.ShiftLeft,
                TraitType.ShiftRight);

            bool isLegal = RewardRawCandidateGenerator.IsLegalReplacementSpec(replacementSpec, rewardGenerationConfig);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void EnumerateLegalReplacementSpecs_ReturnsExpectedSpecsFromConfig()
        {
            RewardGenerationConfig rewardGenerationConfig = TestConfigFactory.CreateValidRewardGenerationConfig(
                allowedReplacementRpsTypes: new List<RpsType> { RpsType.Rock },
                allowedReplacementBasePowers: new List<int> { 4 },
                allowedReplacementTraits: new List<TraitType>
                {
                    TraitType.Empower,
                    TraitType.Suppress,
                    TraitType.Growth,
                },
                replacementTraitCount: 2);

            List<CardSpec> specs = RewardRawCandidateGenerator.EnumerateLegalReplacementSpecs(rewardGenerationConfig);
            List<string> actualKeys = specs
                .Select(spec => spec.rpsType + ":" + spec.basePower + ":" + string.Join(",", spec.traits))
                .ToList();

            Assert.That(actualKeys, Is.EqualTo(new[]
            {
                "Rock:4:Empower,Suppress",
                "Rock:4:Empower,Growth",
                "Rock:4:Suppress,Growth",
            }));
        }

        [Test]
        public void EnumerateReplaceCandidates_FiltersMeaninglessReplacements()
        {
            List<CardInstance> deck = new List<CardInstance>
            {
                CreateCardInstance("card-0001", RpsType.Rock, 4, TraitType.Empower, TraitType.Suppress),
            };

            RewardGenerationConfig rewardGenerationConfig = TestConfigFactory.CreateValidRewardGenerationConfig(
                allowedReplacementRpsTypes: new List<RpsType> { RpsType.Rock },
                allowedReplacementBasePowers: new List<int> { 4 },
                allowedReplacementTraits: new List<TraitType>
                {
                    TraitType.Empower,
                    TraitType.Suppress,
                    TraitType.Growth,
                },
                replacementTraitCount: 2);

            List<ReplaceCandidate> candidates = RewardRawCandidateGenerator.EnumerateReplaceCandidates(
                deck,
                rewardGenerationConfig);

            List<string> actualKeys = candidates
                .Select(candidate => candidate.TargetCardInstanceId + ":" + string.Join(",", candidate.ReplacementCardSpec.traits))
                .ToList();

            Assert.That(actualKeys, Is.EqualTo(new[]
            {
                "card-0001:Empower,Growth",
                "card-0001:Suppress,Growth",
            }));
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
