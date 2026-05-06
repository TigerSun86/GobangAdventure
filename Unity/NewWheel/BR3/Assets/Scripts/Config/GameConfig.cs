using System;
using System.Collections.Generic;
using BR3.Domain;

namespace BR3.Config
{
    [Serializable]
    public sealed class GameConfig
    {
        public PlayerStartConfig playerStart;
        public List<EnemyConfig> enemies;
        public RewardGenerationConfig rewardGeneration;
        public TraitTuning traitTuning;
    }

    [Serializable]
    public sealed class AuthoredGameConfig
    {
        public AuthoredPlayerStartConfig playerStart;
        public List<AuthoredEnemyConfig> enemies;
        public AuthoredRewardGenerationConfig rewardGeneration;
        public TraitTuning traitTuning;
    }

    [Serializable]
    public sealed class PlayerStartConfig
    {
        public int playerMaxHp;
        public List<CardSpec> startingDeck;
    }

    [Serializable]
    public sealed class AuthoredPlayerStartConfig
    {
        public int playerMaxHp;
        public List<AuthoredCardSpec> startingDeck;
    }

    [Serializable]
    public sealed class EnemyConfig
    {
        public string enemyId;
        public string displayName;
        public int maxHp;
        public List<CardSpec> fixedDeck;
    }

    [Serializable]
    public sealed class AuthoredEnemyConfig
    {
        public string enemyId;
        public string displayName;
        public int maxHp;
        public List<AuthoredCardSpec> fixedDeck;
    }

    [Serializable]
    public sealed class RewardGenerationConfig
    {
        public List<RpsType> allowedReplacementRpsTypes;
        public List<int> allowedReplacementBasePowers;
        public List<TraitType> allowedReplacementTraits;
        public int replacementTraitCount;
    }

    [Serializable]
    public sealed class AuthoredRewardGenerationConfig
    {
        public List<string> allowedReplacementRpsTypes;
        public List<int> allowedReplacementBasePowers;
        public List<string> allowedReplacementTraits;
        public int replacementTraitCount;
    }

    [Serializable]
    public sealed class TraitTuning
    {
        public int empowerBonus;
        public int adjacentAidBonus;
        public int suppressPenalty;
        public int regrowHeal;
        public int growthBonus;
    }

    [Serializable]
    public sealed class CardSpec
    {
        public RpsType rpsType;
        public int basePower;
        public List<TraitType> traits;
    }

    [Serializable]
    public sealed class AuthoredCardSpec
    {
        public string rpsType;
        public int basePower;
        public List<string> traits;
    }
}
