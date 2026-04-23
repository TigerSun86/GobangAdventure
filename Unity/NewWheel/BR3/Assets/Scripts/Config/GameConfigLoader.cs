using System;
using System.Collections.Generic;
using BR3.Domain;
using UnityEngine;

namespace BR3.Config
{
    public sealed class GameConfigLoader
    {
        private const int DemoEnemyCount = 3;
        private const int DemoDeckSize = 6;
        private const int DemoReplacementTraitCount = 2;
        private const int MaxAuthoredTraitCount = 3;

        public GameConfig LoadFromJson(string jsonText)
        {
            if (string.IsNullOrWhiteSpace(jsonText))
            {
                throw new ArgumentException("Config JSON text must not be null or empty.", nameof(jsonText));
            }

            GameConfig config = JsonUtility.FromJson<GameConfig>(jsonText);
            if (config == null)
            {
                throw new InvalidOperationException("Config JSON could not be deserialized into GameConfig.");
            }

            Validate(config);
            return config;
        }

        public GameConfig LoadFromTextAsset(TextAsset textAsset)
        {
            if (textAsset == null)
            {
                throw new ArgumentNullException(nameof(textAsset));
            }

            return LoadFromJson(textAsset.text);
        }

        private static void Validate(GameConfig config)
        {
            Require(config.playerStart != null, "GameConfig.playerStart must exist.");
            Require(config.enemies != null, "GameConfig.enemies must exist.");
            Require(config.enemies.Count == DemoEnemyCount, $"GameConfig.enemies must contain exactly {DemoEnemyCount} entries for the current demo.");
            Require(config.rewardGeneration != null, "GameConfig.rewardGeneration must exist.");
            Require(config.traitTuning != null, "GameConfig.traitTuning must exist.");

            ValidatePlayerStart(config.playerStart);

            for (int enemyIndex = 0; enemyIndex < config.enemies.Count; enemyIndex++)
            {
                ValidateEnemy(config.enemies[enemyIndex], enemyIndex);
            }

            ValidateRewardGeneration(config.rewardGeneration);
            ValidateTraitTuning(config.traitTuning);
        }

        private static void ValidatePlayerStart(PlayerStartConfig playerStart)
        {
            Require(playerStart.playerMaxHp > 0, "PlayerStartConfig.playerMaxHp must be greater than 0.");
            Require(playerStart.startingDeck != null, "PlayerStartConfig.startingDeck must exist.");
            Require(playerStart.startingDeck.Count == DemoDeckSize, $"PlayerStartConfig.startingDeck must contain exactly {DemoDeckSize} cards.");

            ValidateCardList(playerStart.startingDeck, "PlayerStartConfig.startingDeck");
        }

        private static void ValidateEnemy(EnemyConfig enemy, int enemyIndex)
        {
            string prefix = $"EnemyConfig[{enemyIndex}]";

            Require(enemy != null, $"{prefix} must exist.");
            Require(!string.IsNullOrWhiteSpace(enemy.enemyId), $"{prefix}.enemyId must not be null or empty.");
            Require(enemy.maxHp > 0, $"{prefix}.maxHp must be greater than 0.");
            Require(enemy.fixedDeck != null, $"{prefix}.fixedDeck must exist.");
            Require(enemy.fixedDeck.Count == DemoDeckSize, $"{prefix}.fixedDeck must contain exactly {DemoDeckSize} cards.");

            ValidateCardList(enemy.fixedDeck, $"{prefix}.fixedDeck");
        }

        private static void ValidateRewardGeneration(RewardGenerationConfig rewardGeneration)
        {
            Require(rewardGeneration.allowedReplacementRpsTypes != null && rewardGeneration.allowedReplacementRpsTypes.Count > 0,
                "RewardGenerationConfig.allowedReplacementRpsTypes must exist and contain at least one value.");
            Require(rewardGeneration.allowedReplacementBasePowers != null && rewardGeneration.allowedReplacementBasePowers.Count > 0,
                "RewardGenerationConfig.allowedReplacementBasePowers must exist and contain at least one value.");
            Require(rewardGeneration.allowedReplacementTraits != null && rewardGeneration.allowedReplacementTraits.Count > 0,
                "RewardGenerationConfig.allowedReplacementTraits must exist and contain at least one value.");
            Require(rewardGeneration.replacementTraitCount == DemoReplacementTraitCount,
                $"RewardGenerationConfig.replacementTraitCount must equal {DemoReplacementTraitCount} for the current demo.");
        }

        private static void ValidateTraitTuning(TraitTuning traitTuning)
        {
            Require(traitTuning.empowerBonus >= 0, "TraitTuning.empowerBonus must be non-negative.");
            Require(traitTuning.adjacentAidBonus >= 0, "TraitTuning.adjacentAidBonus must be non-negative.");
            Require(traitTuning.suppressPenalty >= 0, "TraitTuning.suppressPenalty must be non-negative.");
            Require(traitTuning.regrowHeal >= 0, "TraitTuning.regrowHeal must be non-negative.");
            Require(traitTuning.growthBonus >= 0, "TraitTuning.growthBonus must be non-negative.");
        }

        private static void ValidateCardList(List<CardSpec> cards, string path)
        {
            for (int cardIndex = 0; cardIndex < cards.Count; cardIndex++)
            {
                ValidateCardSpec(cards[cardIndex], $"{path}[{cardIndex}]");
            }
        }

        private static void ValidateCardSpec(CardSpec cardSpec, string path)
        {
            Require(cardSpec != null, $"{path} must exist.");
            Require(cardSpec.basePower >= 0, $"{path}.basePower must be non-negative.");
            Require(cardSpec.traits != null, $"{path}.traits must exist.");
            Require(cardSpec.traits.Count <= MaxAuthoredTraitCount, $"{path}.traits must not contain more than {MaxAuthoredTraitCount} authored traits.");

            HashSet<TraitType> uniqueTraits = new HashSet<TraitType>();
            bool hasShiftLeft = false;
            bool hasShiftRight = false;

            for (int traitIndex = 0; traitIndex < cardSpec.traits.Count; traitIndex++)
            {
                TraitType trait = cardSpec.traits[traitIndex];
                Require(uniqueTraits.Add(trait), $"{path}.traits contains a duplicate trait: {trait}.");

                if (trait == TraitType.ShiftLeft)
                {
                    hasShiftLeft = true;
                }

                if (trait == TraitType.ShiftRight)
                {
                    hasShiftRight = true;
                }
            }

            Require(!(hasShiftLeft && hasShiftRight), $"{path}.traits must not contain both ShiftLeft and ShiftRight.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
