using System;
using System.Collections.Generic;
using System.Linq;
using BR3.Config;
using BR3.Domain.Runtime;

namespace BR3.Domain.Reward
{
    public static class RewardRawCandidateGenerator
    {
        public static bool IsLegalUpgradeCandidate(
            IEnumerable<CardInstance> deck,
            string targetCardInstanceId,
            TraitType addedTrait)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            if (string.IsNullOrEmpty(targetCardInstanceId))
            {
                return false;
            }

            CardInstance targetCard = deck.FirstOrDefault(card => card.InstanceId == targetCardInstanceId);
            if (targetCard == null)
            {
                return false;
            }

            return IsLegalUpgradeForCard(targetCard, addedTrait);
        }

        public static List<UpgradeCandidate> EnumerateUpgradeCandidates(
            IEnumerable<CardInstance> deck,
            IEnumerable<TraitType> candidateTraits)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            if (candidateTraits == null)
            {
                throw new ArgumentNullException(nameof(candidateTraits));
            }

            List<UpgradeCandidate> candidates = new List<UpgradeCandidate>();
            List<TraitType> candidateTraitList = candidateTraits.ToList();

            foreach (CardInstance card in deck)
            {
                foreach (TraitType candidateTrait in candidateTraitList)
                {
                    if (!IsLegalUpgradeForCard(card, candidateTrait))
                    {
                        continue;
                    }

                    candidates.Add(new UpgradeCandidate
                    {
                        TargetCardInstanceId = card.InstanceId,
                        AddedTrait = candidateTrait,
                    });
                }
            }

            return candidates;
        }

        public static bool IsLegalReplacementSpec(CardSpec replacementCardSpec, RewardGenerationConfig rewardGenerationConfig)
        {
            if (replacementCardSpec == null || rewardGenerationConfig == null)
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(RpsType), replacementCardSpec.rpsType))
            {
                return false;
            }

            if (rewardGenerationConfig.allowedReplacementRpsTypes == null
                || !rewardGenerationConfig.allowedReplacementRpsTypes.Contains(replacementCardSpec.rpsType))
            {
                return false;
            }

            if (rewardGenerationConfig.allowedReplacementBasePowers == null
                || !rewardGenerationConfig.allowedReplacementBasePowers.Contains(replacementCardSpec.basePower))
            {
                return false;
            }

            if (replacementCardSpec.traits == null
                || replacementCardSpec.traits.Count != rewardGenerationConfig.replacementTraitCount)
            {
                return false;
            }

            if (replacementCardSpec.traits.Distinct().Count() != replacementCardSpec.traits.Count)
            {
                return false;
            }

            if (replacementCardSpec.traits.Any(trait => !Enum.IsDefined(typeof(TraitType), trait)))
            {
                return false;
            }

            return !HasShiftConflict(replacementCardSpec.traits, null);
        }

        public static List<CardSpec> EnumerateLegalReplacementSpecs(RewardGenerationConfig rewardGenerationConfig)
        {
            if (rewardGenerationConfig == null)
            {
                throw new ArgumentNullException(nameof(rewardGenerationConfig));
            }

            List<CardSpec> legalSpecs = new List<CardSpec>();
            List<List<TraitType>> traitCombinations = EnumerateTraitCombinations(
                rewardGenerationConfig.allowedReplacementTraits ?? new List<TraitType>(),
                rewardGenerationConfig.replacementTraitCount);

            foreach (RpsType rpsType in rewardGenerationConfig.allowedReplacementRpsTypes ?? new List<RpsType>())
            {
                foreach (int basePower in rewardGenerationConfig.allowedReplacementBasePowers ?? new List<int>())
                {
                    foreach (List<TraitType> traitCombination in traitCombinations)
                    {
                        CardSpec replacementSpec = new CardSpec
                        {
                            rpsType = rpsType,
                            basePower = basePower,
                            traits = new List<TraitType>(traitCombination),
                        };

                        if (!IsLegalReplacementSpec(replacementSpec, rewardGenerationConfig))
                        {
                            continue;
                        }

                        legalSpecs.Add(replacementSpec);
                    }
                }
            }

            return legalSpecs;
        }

        public static List<ReplaceCandidate> EnumerateReplaceCandidates(
            IEnumerable<CardInstance> deck,
            RewardGenerationConfig rewardGenerationConfig)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            if (rewardGenerationConfig == null)
            {
                throw new ArgumentNullException(nameof(rewardGenerationConfig));
            }

            List<ReplaceCandidate> candidates = new List<ReplaceCandidate>();
            List<CardSpec> replacementSpecs = EnumerateLegalReplacementSpecs(rewardGenerationConfig);

            foreach (CardInstance card in deck)
            {
                foreach (CardSpec replacementSpec in replacementSpecs)
                {
                    if (!IsMeaningfulReplace(card, replacementSpec))
                    {
                        continue;
                    }

                    candidates.Add(new ReplaceCandidate
                    {
                        TargetCardInstanceId = card.InstanceId,
                        ReplacementCardSpec = CloneCardSpec(replacementSpec),
                    });
                }
            }

            return candidates;
        }

        public static bool IsMeaningfulReplace(CardInstance targetCard, CardSpec replacementCardSpec)
        {
            if (targetCard == null)
            {
                throw new ArgumentNullException(nameof(targetCard));
            }

            if (replacementCardSpec == null)
            {
                throw new ArgumentNullException(nameof(replacementCardSpec));
            }

            if (targetCard.RpsType != replacementCardSpec.rpsType)
            {
                return true;
            }

            if (targetCard.BasePower != replacementCardSpec.basePower)
            {
                return true;
            }

            return !HaveEquivalentTraitSet(targetCard.Traits, replacementCardSpec.traits);
        }

        private static bool IsLegalUpgradeForCard(CardInstance targetCard, TraitType addedTrait)
        {
            List<TraitType> currentTraits = targetCard.Traits ?? new List<TraitType>();

            if (currentTraits.Count >= 3)
            {
                return false;
            }

            if (currentTraits.Contains(addedTrait))
            {
                return false;
            }

            return !HasShiftConflict(currentTraits, addedTrait);
        }

        private static bool HasShiftConflict(IEnumerable<TraitType> traits, TraitType? addedTrait)
        {
            bool hasShiftLeft = traits.Contains(TraitType.ShiftLeft) || addedTrait == TraitType.ShiftLeft;
            bool hasShiftRight = traits.Contains(TraitType.ShiftRight) || addedTrait == TraitType.ShiftRight;
            return hasShiftLeft && hasShiftRight;
        }

        private static List<List<TraitType>> EnumerateTraitCombinations(IReadOnlyList<TraitType> traits, int combinationSize)
        {
            List<List<TraitType>> combinations = new List<List<TraitType>>();

            if (combinationSize < 0)
            {
                return combinations;
            }

            CollectTraitCombinations(traits, combinationSize, 0, new List<TraitType>(), combinations);
            return combinations;
        }

        private static void CollectTraitCombinations(
            IReadOnlyList<TraitType> traits,
            int combinationSize,
            int startIndex,
            List<TraitType> current,
            List<List<TraitType>> combinations)
        {
            if (current.Count == combinationSize)
            {
                combinations.Add(new List<TraitType>(current));
                return;
            }

            for (int i = startIndex; i < traits.Count; i++)
            {
                current.Add(traits[i]);
                CollectTraitCombinations(traits, combinationSize, i + 1, current, combinations);
                current.RemoveAt(current.Count - 1);
            }
        }

        private static bool HaveEquivalentTraitSet(IReadOnlyCollection<TraitType> first, IReadOnlyCollection<TraitType> second)
        {
            List<TraitType> firstTraits = first == null
                ? new List<TraitType>()
                : first.OrderBy(trait => trait).ToList();

            List<TraitType> secondTraits = second == null
                ? new List<TraitType>()
                : second.OrderBy(trait => trait).ToList();

            return firstTraits.SequenceEqual(secondTraits);
        }

        private static CardSpec CloneCardSpec(CardSpec original)
        {
            return new CardSpec
            {
                rpsType = original.rpsType,
                basePower = original.basePower,
                traits = new List<TraitType>(original.traits ?? new List<TraitType>()),
            };
        }
    }
}
