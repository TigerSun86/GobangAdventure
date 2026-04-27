using System;
using System.Collections.Generic;
using System.Linq;
using BR3.Config;
using BR3.Domain.Random;
using BR3.Domain.Runtime;

namespace BR3.Domain.Reward
{
    public sealed class RewardOfferGenerator
    {
        private int _nextOfferId = 1;
        private int _nextOptionId = 1;
        private readonly IGameRandom _gameRandom;

        public RewardOfferGenerator()
            : this(new SystemGameRandom())
        {
        }

        public RewardOfferGenerator(IGameRandom gameRandom)
        {
            _gameRandom = gameRandom ?? throw new ArgumentNullException(nameof(gameRandom));
        }

        public List<UpgradeCandidate> DeduplicateUpgradeCandidates(IEnumerable<CardInstance> deck, IEnumerable<UpgradeCandidate> rawCandidates)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            if (rawCandidates == null)
            {
                throw new ArgumentNullException(nameof(rawCandidates));
            }

            List<UpgradeCandidate> survivingCandidates = new List<UpgradeCandidate>();
            HashSet<string> seenDeckSignatures = new HashSet<string>(StringComparer.Ordinal);

            foreach (UpgradeCandidate candidate in rawCandidates)
            {
                List<CardInstance> simulatedDeck = SimulateUpgrade(deck, candidate);
                string deckSignature = RewardCanonicalSignatureFactory.CreateDeckSignature(simulatedDeck).SignatureText;

                if (!seenDeckSignatures.Add(deckSignature))
                {
                    continue;
                }

                survivingCandidates.Add(new UpgradeCandidate
                {
                    TargetCardInstanceId = candidate.TargetCardInstanceId,
                    AddedTrait = candidate.AddedTrait,
                });
            }

            return survivingCandidates;
        }

        public List<ReplaceCandidate> DeduplicateReplaceCandidates(IEnumerable<CardInstance> deck, IEnumerable<ReplaceCandidate> rawCandidates)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            if (rawCandidates == null)
            {
                throw new ArgumentNullException(nameof(rawCandidates));
            }

            List<ReplaceCandidate> survivingCandidates = new List<ReplaceCandidate>();
            HashSet<string> seenDeckSignatures = new HashSet<string>(StringComparer.Ordinal);

            foreach (ReplaceCandidate candidate in rawCandidates)
            {
                List<CardInstance> simulatedDeck = SimulateReplace(deck, candidate);
                string deckSignature = RewardCanonicalSignatureFactory.CreateDeckSignature(simulatedDeck).SignatureText;

                if (!seenDeckSignatures.Add(deckSignature))
                {
                    continue;
                }

                survivingCandidates.Add(new ReplaceCandidate
                {
                    TargetCardInstanceId = candidate.TargetCardInstanceId,
                    ReplacementCardSpec = CloneCardSpec(candidate.ReplacementCardSpec),
                });
            }

            return survivingCandidates;
        }

        public RewardOffer GenerateRewardOffer(
            IEnumerable<CardInstance> deck,
            RewardGenerationConfig rewardGenerationConfig,
            int rewardIndexForCurrentEnemy)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            if (rewardGenerationConfig == null)
            {
                throw new ArgumentNullException(nameof(rewardGenerationConfig));
            }

            List<CardInstance> deckList = deck.Select(CloneCardInstance).ToList();

            List<UpgradeCandidate> rawUpgradeCandidates = RewardRawCandidateGenerator.EnumerateUpgradeCandidates(
                deckList,
                rewardGenerationConfig.allowedReplacementTraits ?? new List<TraitType>());

            List<UpgradeCandidate> dedupedUpgradeCandidates = DeduplicateUpgradeCandidates(deckList, rawUpgradeCandidates);
            int selectedUpgradeCount = Math.Min(2, dedupedUpgradeCandidates.Count);
            int selectedReplaceCount = 3 - selectedUpgradeCount;
            List<UpgradeCandidate> selectedUpgradeCandidates = SelectCandidates(dedupedUpgradeCandidates, selectedUpgradeCount);

            List<ReplaceCandidate> rawReplaceCandidates = RewardRawCandidateGenerator.EnumerateReplaceCandidates(
                deckList,
                rewardGenerationConfig);

            List<ReplaceCandidate> dedupedReplaceCandidates = DeduplicateReplaceCandidates(deckList, rawReplaceCandidates);
            if (dedupedReplaceCandidates.Count < selectedReplaceCount)
            {
                throw new InvalidOperationException("Not enough distinct replace candidates to compose a reward offer.");
            }

            List<ReplaceCandidate> selectedReplaceCandidates = SelectCandidates(dedupedReplaceCandidates, selectedReplaceCount);

            List<RewardOption> options = new List<RewardOption>();

            foreach (UpgradeCandidate candidate in selectedUpgradeCandidates)
            {
                options.Add(new RewardOption
                {
                    OptionId = CreateNextOptionId(),
                    Type = RewardOptionType.Upgrade,
                    UpgradePayload = new UpgradePayload
                    {
                        TargetCardInstanceId = candidate.TargetCardInstanceId,
                        AddedTrait = candidate.AddedTrait,
                    },
                });
            }

            foreach (ReplaceCandidate candidate in selectedReplaceCandidates)
            {
                options.Add(new RewardOption
                {
                    OptionId = CreateNextOptionId(),
                    Type = RewardOptionType.Replace,
                    ReplacePayload = new ReplacePayload
                    {
                        TargetCardInstanceId = candidate.TargetCardInstanceId,
                        ReplacementCardSpec = CloneCardSpec(candidate.ReplacementCardSpec),
                    },
                });
            }

            options.Add(new RewardOption
            {
                OptionId = CreateNextOptionId(),
                Type = RewardOptionType.Skip,
            });

            return new RewardOffer
            {
                OfferId = CreateNextOfferId(),
                RewardIndexForCurrentEnemy = rewardIndexForCurrentEnemy,
                Options = options,
            };
        }

        private List<T> SelectCandidates<T>(IReadOnlyList<T> candidates, int count)
        {
            if (count == 0)
            {
                return new List<T>();
            }

            List<T> shuffledCandidates = new List<T>(candidates);
            ShuffleInPlace(shuffledCandidates);
            return shuffledCandidates.Take(count).ToList();
        }

        private string CreateNextOfferId()
        {
            return "offer-" + _nextOfferId++.ToString("0000");
        }

        private string CreateNextOptionId()
        {
            return "option-" + _nextOptionId++.ToString("0000");
        }

        private static List<CardInstance> SimulateUpgrade(IEnumerable<CardInstance> deck, UpgradeCandidate candidate)
        {
            List<CardInstance> clonedDeck = deck.Select(CloneCardInstance).ToList();
            CardInstance targetCard = clonedDeck.First(card => card.InstanceId == candidate.TargetCardInstanceId);
            targetCard.Traits.Add(candidate.AddedTrait);
            return clonedDeck;
        }

        private static List<CardInstance> SimulateReplace(IEnumerable<CardInstance> deck, ReplaceCandidate candidate)
        {
            List<CardInstance> clonedDeck = deck.Select(CloneCardInstance).ToList();
            int targetIndex = clonedDeck.FindIndex(card => card.InstanceId == candidate.TargetCardInstanceId);
            clonedDeck[targetIndex] = new CardInstance
            {
                InstanceId = "simulated-replace",
                RpsType = candidate.ReplacementCardSpec.rpsType,
                BasePower = candidate.ReplacementCardSpec.basePower,
                Traits = new List<TraitType>(candidate.ReplacementCardSpec.traits ?? new List<TraitType>()),
                PermanentPowerBonus = 0,
            };

            return clonedDeck;
        }

        private static CardInstance CloneCardInstance(CardInstance original)
        {
            return new CardInstance
            {
                InstanceId = original.InstanceId,
                RpsType = original.RpsType,
                BasePower = original.BasePower,
                Traits = new List<TraitType>(original.Traits ?? new List<TraitType>()),
                PermanentPowerBonus = original.PermanentPowerBonus,
            };
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

        private void ShuffleInPlace<T>(IList<T> values)
        {
            for (int i = values.Count - 1; i > 0; i--)
            {
                int swapIndex = _gameRandom.NextInt(0, i + 1);
                T temp = values[i];
                values[i] = values[swapIndex];
                values[swapIndex] = temp;
            }
        }
    }
}
