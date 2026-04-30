using System.Collections.Generic;
using BR3.Domain;
using BR3.Domain.Reward;
using BR3.Domain.Runtime;
using NUnit.Framework;

namespace BR3.Tests.EditMode.Domain
{
    public sealed class RewardCanonicalSignatureTests
    {
        [Test]
        public void CreateCardSignature_IgnoresTraitOrder()
        {
            CardInstance firstCard = CreateCardInstance(
                "card-0001",
                RpsType.Rock,
                4,
                1,
                TraitType.Regrow,
                TraitType.Empower);

            CardInstance secondCard = CreateCardInstance(
                "card-9999",
                RpsType.Rock,
                4,
                1,
                TraitType.Empower,
                TraitType.Regrow);

            CanonicalCardSignature firstSignature = RewardCanonicalSignatureFactory.CreateCardSignature(firstCard);
            CanonicalCardSignature secondSignature = RewardCanonicalSignatureFactory.CreateCardSignature(secondCard);

            Assert.That(firstSignature, Is.EqualTo(secondSignature));
            Assert.That(firstSignature.Traits, Is.EqualTo(new[]
            {
                TraitType.Empower,
                TraitType.Regrow,
            }));
        }

        [Test]
        public void CreateDeckSignature_IgnoresDeckOrderAndInstanceIdentity()
        {
            List<CardInstance> firstDeck = new List<CardInstance>
            {
                CreateCardInstance("card-0001", RpsType.Rock, 4, 0, TraitType.Empower),
                CreateCardInstance("card-0002", RpsType.Scissors, 5, 2, TraitType.Suppress),
            };

            List<CardInstance> secondDeck = new List<CardInstance>
            {
                CreateCardInstance("card-7000", RpsType.Scissors, 5, 2, TraitType.Suppress),
                CreateCardInstance("card-8000", RpsType.Rock, 4, 0, TraitType.Empower),
            };

            CanonicalDeckSignature firstSignature = RewardCanonicalSignatureFactory.CreateDeckSignature(firstDeck);
            CanonicalDeckSignature secondSignature = RewardCanonicalSignatureFactory.CreateDeckSignature(secondDeck);

            Assert.That(firstSignature, Is.EqualTo(secondSignature));
            Assert.That(firstSignature.Cards, Has.Count.EqualTo(2));
            Assert.That(firstSignature.Cards[0].SignatureText, Is.EqualTo(secondSignature.Cards[0].SignatureText));
        }

        [Test]
        public void CreateCardSignature_DifferentPermanentPowerBonusChangesSignature()
        {
            CardInstance lowerBonusCard = CreateCardInstance(
                "card-0001",
                RpsType.Paper,
                4,
                0,
                TraitType.Growth);

            CardInstance higherBonusCard = CreateCardInstance(
                "card-0002",
                RpsType.Paper,
                4,
                1,
                TraitType.Growth);

            CanonicalCardSignature lowerBonusSignature = RewardCanonicalSignatureFactory.CreateCardSignature(lowerBonusCard);
            CanonicalCardSignature higherBonusSignature = RewardCanonicalSignatureFactory.CreateCardSignature(higherBonusCard);

            Assert.That(lowerBonusSignature, Is.Not.EqualTo(higherBonusSignature));
        }

        [Test]
        public void CreateCardSignature_DifferentGameplayValuesProduceDifferentSignatures()
        {
            CanonicalCardSignature rockSignature = RewardCanonicalSignatureFactory.CreateCardSignature(
                CreateCardInstance("card-0001", RpsType.Rock, 4, 0, TraitType.Empower));

            CanonicalCardSignature paperSignature = RewardCanonicalSignatureFactory.CreateCardSignature(
                CreateCardInstance("card-0002", RpsType.Paper, 4, 0, TraitType.Empower));

            CanonicalCardSignature higherBasePowerSignature = RewardCanonicalSignatureFactory.CreateCardSignature(
                CreateCardInstance("card-0003", RpsType.Rock, 5, 0, TraitType.Empower));

            CanonicalCardSignature differentTraitSignature = RewardCanonicalSignatureFactory.CreateCardSignature(
                CreateCardInstance("card-0004", RpsType.Rock, 4, 0, TraitType.Suppress));

            Assert.That(rockSignature, Is.Not.EqualTo(paperSignature));
            Assert.That(rockSignature, Is.Not.EqualTo(higherBasePowerSignature));
            Assert.That(rockSignature, Is.Not.EqualTo(differentTraitSignature));
        }

        private static CardInstance CreateCardInstance(
            string instanceId,
            RpsType rpsType,
            int basePower,
            int permanentPowerBonus,
            params TraitType[] traits)
        {
            return new CardInstance
            {
                InstanceId = instanceId,
                RpsType = rpsType,
                BasePower = basePower,
                PermanentPowerBonus = permanentPowerBonus,
                Traits = new List<TraitType>(traits),
            };
        }
    }
}
