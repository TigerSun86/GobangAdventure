using System.Collections.Generic;
using System.Linq;
using BR3.Domain;
using BR3.Domain.Reward;
using BR3.Domain.Runtime;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Domain
{
    public sealed class RewardUpgradeGenerationTests
    {
        [Test]
        public void IsLegalUpgradeCandidate_RejectsDuplicateTrait()
        {
            List<CardInstance> deck = new List<CardInstance>
            {
                CreateCardInstance("card-0001", TraitType.Empower),
            };

            bool isLegal = RewardRawCandidateGenerator.IsLegalUpgradeCandidate(deck, "card-0001", TraitType.Empower);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void IsLegalUpgradeCandidate_RejectsFourthTrait()
        {
            List<CardInstance> deck = new List<CardInstance>
            {
                CreateCardInstance("card-0001", TraitType.Empower, TraitType.Suppress, TraitType.Regrow),
            };

            bool isLegal = RewardRawCandidateGenerator.IsLegalUpgradeCandidate(deck, "card-0001", TraitType.Growth);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void IsLegalUpgradeCandidate_RejectsShiftConflict()
        {
            List<CardInstance> deck = new List<CardInstance>
            {
                CreateCardInstance("card-0001", TraitType.ShiftLeft),
            };

            bool isLegal = RewardRawCandidateGenerator.IsLegalUpgradeCandidate(deck, "card-0001", TraitType.ShiftRight);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void EnumerateUpgradeCandidates_ReturnsExpectedLegalCandidates()
        {
            List<CardInstance> deck = new List<CardInstance>
            {
                CreateCardInstance("card-0001", TraitType.Empower),
                CreateCardInstance("card-0002", TraitType.ShiftLeft),
            };

            List<UpgradeCandidate> candidates = RewardRawCandidateGenerator.EnumerateUpgradeCandidates(
                deck,
                new[]
                {
                    TraitType.Empower,
                    TraitType.Suppress,
                    TraitType.ShiftRight,
                });

            List<string> actualKeys = candidates
                .Select(candidate => candidate.TargetCardInstanceId + ":" + candidate.AddedTrait)
                .ToList();

            Assert.That(actualKeys, Is.EqualTo(new[]
            {
                "card-0001:Suppress",
                "card-0001:ShiftRight",
                "card-0002:Empower",
                "card-0002:Suppress",
            }));
        }

        [Test]
        public void EnumerateUpgradeCandidates_KeepsDistinctRawCandidatesForDifferentTargetCards()
        {
            List<CardInstance> deck = new List<CardInstance>
            {
                CreateCardInstance("card-0001", TraitType.Empower),
                CreateCardInstance("card-0002", TraitType.Empower),
            };

            List<UpgradeCandidate> candidates = RewardRawCandidateGenerator.EnumerateUpgradeCandidates(
                deck,
                new[]
                {
                    TraitType.Suppress,
                });

            Assert.That(candidates, Has.Count.EqualTo(2));
            Assert.That(candidates.Select(candidate => candidate.TargetCardInstanceId), Is.EqualTo(new[]
            {
                "card-0001",
                "card-0002",
            }));
            Assert.That(candidates.All(candidate => candidate.AddedTrait == TraitType.Suppress), Is.True);
        }

        private static CardInstance CreateCardInstance(string instanceId, params TraitType[] traits)
        {
            return new CardInstance
            {
                InstanceId = instanceId,
                RpsType = RpsType.Rock,
                BasePower = 4,
                PermanentPowerBonus = 0,
                Traits = new List<TraitType>(traits),
            };
        }
    }
}
