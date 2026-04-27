using System;
using System.Collections.Generic;
using System.Linq;
using BR3.Domain.Runtime;

namespace BR3.Domain.Reward
{
    public static class RewardCanonicalSignatureFactory
    {
        public static CanonicalCardSignature CreateCardSignature(CardInstance cardInstance)
        {
            if (cardInstance == null)
            {
                throw new ArgumentNullException(nameof(cardInstance));
            }

            List<TraitType> sortedTraits = cardInstance.Traits == null
                ? new List<TraitType>()
                : cardInstance.Traits
                    .OrderBy(trait => trait)
                    .ToList();

            string traitText = string.Join(",", sortedTraits.Select(trait => ((int)trait).ToString()));
            string signatureText = string.Format(
                "rps={0}|base={1}|bonus={2}|traits={3}",
                (int)cardInstance.RpsType,
                cardInstance.BasePower,
                cardInstance.PermanentPowerBonus,
                traitText);

            return new CanonicalCardSignature(
                cardInstance.RpsType,
                cardInstance.BasePower,
                cardInstance.PermanentPowerBonus,
                sortedTraits,
                signatureText);
        }

        public static CanonicalDeckSignature CreateDeckSignature(IEnumerable<CardInstance> deck)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            List<CanonicalCardSignature> sortedCards = deck
                .Select(CreateCardSignature)
                .OrderBy(card => card.SignatureText, StringComparer.Ordinal)
                .ToList();

            string signatureText = string.Join("||", sortedCards.Select(card => card.SignatureText));

            return new CanonicalDeckSignature(sortedCards, signatureText);
        }
    }
}
