using System;
using System.Collections.Generic;
using BR3.Config;
using BR3.Domain.Random;
using BR3.Domain.Runtime;

namespace BR3.Application
{
    public sealed class BattleService
    {
        private readonly IGameRandom _gameRandom;

        public BattleService()
            : this(new SystemGameRandom())
        {
        }

        public BattleService(IGameRandom gameRandom)
        {
            _gameRandom = gameRandom ?? throw new ArgumentNullException(nameof(gameRandom));
        }

        public BattleState StartBattle(EnemyProgressState enemyProgressState)
        {
            if (enemyProgressState == null)
            {
                throw new ArgumentNullException(nameof(enemyProgressState));
            }

            if (enemyProgressState.Config == null)
            {
                throw new InvalidOperationException("EnemyProgressState must provide an enemy config.");
            }

            if (enemyProgressState.Config.fixedDeck == null || enemyProgressState.Config.fixedDeck.Count < 3)
            {
                throw new InvalidOperationException("Enemy config must provide at least three cards for battle sequence generation.");
            }

            BattleState battleState = new BattleState
            {
                BattleIndexForEnemy = enemyProgressState.BattlesPlayed + 1,
                RoundIndex = 1,
                PlayerLane = CreateLane(),
                EnemyLane = CreateLane(),
                UsedPlayerCardIds = new HashSet<string>(),
                EnemySequence = CreateEnemySequence(enemyProgressState.Config.fixedDeck),
                RoundResults = new List<BR3.Domain.Results.RoundResult>(),
                Logs = new List<string>(),
                Snapshots = new List<BR3.Domain.Results.PhaseSnapshot>(),
                BattleFlowStage = BattleFlowStage.WaitingForPlayerCard,
            };

            battleState.Logs.Add(
                $"BattleStart: enemy {enemyProgressState.Config.enemyId} battle {battleState.BattleIndexForEnemy} initialized with {battleState.EnemySequence.Count} enemy cards.");

            return battleState;
        }

        private List<CardSpec> CreateEnemySequence(List<CardSpec> fixedDeck)
        {
            List<CardSpec> shuffledDeck = new List<CardSpec>(fixedDeck.Count);
            for (int cardIndex = 0; cardIndex < fixedDeck.Count; cardIndex++)
            {
                shuffledDeck.Add(CloneCardSpec(fixedDeck[cardIndex]));
            }

            for (int cardIndex = shuffledDeck.Count - 1; cardIndex > 0; cardIndex--)
            {
                int swapIndex = _gameRandom.NextInt(0, cardIndex + 1);
                CardSpec temporaryCard = shuffledDeck[cardIndex];
                shuffledDeck[cardIndex] = shuffledDeck[swapIndex];
                shuffledDeck[swapIndex] = temporaryCard;
            }

            return shuffledDeck.GetRange(0, 3);
        }

        private static LaneState CreateLane()
        {
            return new LaneState
            {
                Slots = new List<BoardSlotState>
                {
                    new BoardSlotState { Index = 0, IsOpen = false, Occupant = null },
                    new BoardSlotState { Index = 1, IsOpen = false, Occupant = null },
                    new BoardSlotState { Index = 2, IsOpen = false, Occupant = null },
                },
            };
        }

        private static CardSpec CloneCardSpec(CardSpec cardSpec)
        {
            return new CardSpec
            {
                rpsType = cardSpec.rpsType,
                basePower = cardSpec.basePower,
                traits = new List<BR3.Domain.TraitType>(cardSpec.traits ?? new List<BR3.Domain.TraitType>()),
            };
        }
    }
}
