using System;
using System.Collections.Generic;
using BR3.Config;
using BR3.Domain;
using BR3.Domain.Runtime;

namespace BR3.Application
{
    public sealed class RuntimeStateFactory
    {
        private int nextCardInstanceId = 1;

        public RunState CreateRunState(GameConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            List<CardInstance> playerDeck = new List<CardInstance>(config.playerStart.startingDeck.Count);
            for (int cardIndex = 0; cardIndex < config.playerStart.startingDeck.Count; cardIndex++)
            {
                playerDeck.Add(CreateCardInstance(config.playerStart.startingDeck[cardIndex]));
            }

            return new RunState
            {
                PlayerHp = config.playerStart.playerMaxHp,
                PlayerMaxHp = config.playerStart.playerMaxHp,
                PlayerDeck = playerDeck,
                CurrentEnemyIndex = 0,
                CurrentEnemy = CreateEnemyProgressState(config.enemies[0]),
                ActiveBattle = null,
                PendingRewardOffer = null,
                FlowStage = RunFlowStage.ReadyForNextBattle,
            };
        }

        public CardInstance CreateCardInstance(CardSpec spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            return new CardInstance
            {
                InstanceId = CreateNextCardInstanceId(),
                RpsType = spec.rpsType,
                BasePower = spec.basePower,
                Traits = new List<TraitType>(spec.traits),
                PermanentPowerBonus = 0,
            };
        }

        public EnemyProgressState CreateEnemyProgressState(EnemyConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return new EnemyProgressState
            {
                Config = config,
                CurrentHp = config.maxHp,
                MaxHp = config.maxHp,
                BattlesPlayed = 0,
                RewardsClaimed = 0,
            };
        }

        private string CreateNextCardInstanceId()
        {
            string instanceId = $"card-{nextCardInstanceId:D4}";
            nextCardInstanceId++;
            return instanceId;
        }
    }
}
