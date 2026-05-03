using System;
using BR3.Application;
using BR3.Config;
using BR3.Domain.Results;
using BR3.Domain.Rules;
using BR3.Domain.Runtime;
using TMPro;
using UnityEngine;

namespace BR3.Presentation.DebugUi
{
    public sealed class DebugSceneController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private TextAsset configTextAsset;

        [Header("Panels")]
        [SerializeField] private RunSummaryPanelView runSummaryPanelView;
        [SerializeField] private BoardPanelView boardPanelView;
        [SerializeField] private EnemySequencePanelView enemySequencePanelView;
        [SerializeField] private PlayerDeckPanelView playerDeckPanelView;
        [SerializeField] private RewardPanelView rewardPanelView;
        [SerializeField] private InspectorPanelView inspectorPanelView;
        [SerializeField] private ActionBarView actionBarView;

        [Header("Status")]
        [SerializeField] private TMP_Text statusMessageText;

        private readonly GameConfigLoader gameConfigLoader = new GameConfigLoader();

        private DebugUiState debugUiState;
        private RuntimeStateFactory runtimeStateFactory;
        private RunService runService;
        private BattleService battleService;
        private RewardService rewardService;
        private RoundResolver roundResolver;
        private GameConfig currentConfig;
        private RunState currentRun;

        public TextAsset ConfigTextAsset => configTextAsset;
        public GameConfig CurrentConfig => currentConfig;
        public RunState CurrentRun => currentRun;
        public DebugUiState UiState => debugUiState;

        private void Awake()
        {
            debugUiState = DebugUiState.CreateDefault();
            runtimeStateFactory = new RuntimeStateFactory();
            runService = new RunService();
            battleService = new BattleService();
            rewardService = new RewardService(runtimeStateFactory);
            roundResolver = new RoundResolver();

            actionBarView?.Bind(OnLoadConfigButtonPressed, OnNewRunButtonPressed, OnStartBattleButtonPressed, OnContinueButtonPressed);
            inspectorPanelView?.BindSnapshotPhaseChanged(OnSnapshotPhaseChanged);
            InitializeInspectorShell();
            RefreshShell();
        }

        public void OnLoadConfigButtonPressed()
        {
            if (configTextAsset == null)
            {
                SetStatusMessage("Assign a GameConfig TextAsset before loading config.");
                RefreshShell();
                return;
            }

            try
            {
                currentConfig = gameConfigLoader.LoadFromTextAsset(configTextAsset);

                if (currentRun == null)
                {
                    SetStatusMessage("Config loaded. Click New Run to create runtime state.");
                }
                else
                {
                    SetStatusMessage("Config reloaded. Click New Run to apply it to a new run.");
                }
            }
            catch (Exception exception)
            {
                currentConfig = null;
                SetStatusMessage($"Config load failed: {exception.Message}");
            }

            RefreshShell();
        }

        public void OnNewRunButtonPressed()
        {
            if (currentConfig == null)
            {
                SetStatusMessage("Load a valid config before creating a new run.");
                RefreshShell();
                return;
            }

            currentRun = runService.CreateNewRun(currentConfig, runtimeStateFactory);
            debugUiState = DebugUiState.CreateDefault();
            SetStatusMessage("New run created. Start Battle is now available.");
            RefreshShell();
        }

        public void OnStartBattleButtonPressed()
        {
            if (currentRun == null)
            {
                SetStatusMessage("Create a run before starting a battle.");
                RefreshShell();
                return;
            }

            RunCommandResult result = runService.StartNextBattle(currentRun, battleService);
            SetStatusMessage(result.Success
                ? "Battle started. Full board and deck rendering will be added in Task 7C3."
                : result.FailureReason);

            RefreshShell();
        }

        public void OnContinueButtonPressed()
        {
            SetStatusMessage("Continue flow is reserved for Task 7C3 after round-result presentation is implemented.");
            RefreshShell();
        }

        public void OnSnapshotPhaseChanged(int selectedIndex)
        {
            debugUiState.SelectedSnapshotPhaseIndex = Mathf.Max(0, selectedIndex);
            RefreshShell();
        }

        private void InitializeInspectorShell()
        {
            string[] phaseNames = Enum.GetNames(typeof(RoundPhase));
            inspectorPanelView?.SetSnapshotPhaseOptions(phaseNames, debugUiState.SelectedSnapshotPhaseIndex);
        }

        private void RefreshShell()
        {
            runSummaryPanelView?.SetVisible(true);
            runSummaryPanelView?.Render(BuildRunSummaryViewData());

            boardPanelView?.SetVisible(true);
            boardPanelView?.Render(BuildBoardViewData());

            enemySequencePanelView?.SetVisible(true);

            playerDeckPanelView?.SetVisible(true);

            rewardPanelView?.SetVisible(true);
            rewardPanelView?.Render(BuildRewardPanelViewData());

            inspectorPanelView?.SetVisible(true);
            inspectorPanelView?.Render(BuildInspectorPanelViewData());

            bool hasLoadedConfig = currentConfig != null;
            bool hasRun = currentRun != null;

            actionBarView?.SetVisible(true);
            actionBarView?.SetButtonsInteractable(
                canLoadConfig: true,
                canCreateRun: hasLoadedConfig,
                canStartBattle: hasRun,
                canContinue: false);
            actionBarView?.SetStatusMessage(debugUiState.StatusMessage);

            if (statusMessageText != null)
            {
                statusMessageText.text = debugUiState.StatusMessage;
            }
        }

        private void SetStatusMessage(string message)
        {
            debugUiState.StatusMessage = message;
        }

        private RunSummaryViewData BuildRunSummaryViewData()
        {
            if (currentRun == null)
            {
                return new RunSummaryViewData
                {
                    PlayerHpText = "-",
                    EnemyIndexText = "-",
                    EnemyHpText = "-",
                    BattlesPlayedText = "-",
                    RewardsClaimedText = "-",
                    RunStageText = "-",
                    BattleStageText = "-",
                    RoundText = "-",
                };
            }

            string enemyIndexText = currentConfig == null || currentConfig.enemies == null || currentConfig.enemies.Count == 0
                ? $"{currentRun.CurrentEnemyIndex + 1}"
                : $"{currentRun.CurrentEnemyIndex + 1}/{currentConfig.enemies.Count}";

            BattleState activeBattle = currentRun.ActiveBattle;

            return new RunSummaryViewData
            {
                PlayerHpText = $"{currentRun.PlayerHp}/{currentRun.PlayerMaxHp}",
                EnemyIndexText = enemyIndexText,
                EnemyHpText = currentRun.CurrentEnemy == null ? "-" : $"{currentRun.CurrentEnemy.CurrentHp}/{currentRun.CurrentEnemy.MaxHp}",
                BattlesPlayedText = currentRun.CurrentEnemy == null ? "-" : $"{currentRun.CurrentEnemy.BattlesPlayed}/3",
                RewardsClaimedText = currentRun.CurrentEnemy == null ? "-" : $"{currentRun.CurrentEnemy.RewardsClaimed}/3",
                RunStageText = FlowStageTextFormatter.Format(currentRun.FlowStage),
                BattleStageText = FlowStageTextFormatter.Format(activeBattle?.BattleFlowStage),
                RoundText = activeBattle == null ? "-" : activeBattle.RoundIndex.ToString(),
            };
        }

        private BoardViewData BuildBoardViewData()
        {
            LaneState enemyLane = currentRun?.ActiveBattle?.EnemyLane;
            LaneState playerLane = currentRun?.ActiveBattle?.PlayerLane;

            return new BoardViewData
            {
                EnemySlot1 = BuildBoardSlotViewData(enemyLane, 0),
                EnemySlot2 = BuildBoardSlotViewData(enemyLane, 1),
                EnemySlot3 = BuildBoardSlotViewData(enemyLane, 2),
                PlayerSlot1 = BuildBoardSlotViewData(playerLane, 0),
                PlayerSlot2 = BuildBoardSlotViewData(playerLane, 1),
                PlayerSlot3 = BuildBoardSlotViewData(playerLane, 2),
            };
        }

        private RewardPanelViewData BuildRewardPanelViewData()
        {
            if (currentRun?.PendingRewardOffer == null)
            {
                return new RewardPanelViewData
                {
                    PlaceholderText = "No reward pending.",
                };
            }

            RewardOffer rewardOffer = currentRun.PendingRewardOffer;
            int optionCount = rewardOffer.Options?.Count ?? 0;
            return new RewardPanelViewData
            {
                PlaceholderText = $"Reward {rewardOffer.RewardIndexForCurrentEnemy} ready with {optionCount} options.",
            };
        }

        private InspectorPanelViewData BuildInspectorPanelViewData()
        {
            RoundResult latestRoundResult = GetLatestRoundResult();
            PhaseSnapshot selectedSnapshot = GetSelectedSnapshot(latestRoundResult);
            CardInstance latestPlayerCard = FindPlayerCard(latestRoundResult?.PlayerCardInstanceId);

            return new InspectorPanelViewData
            {
                LatestRoundResult = new LatestRoundResultViewData
                {
                    SummaryText = RoundResultTextFormatter.FormatSummary(latestRoundResult, latestPlayerCard),
                },
                SnapshotText = SnapshotTextFormatter.Format(selectedSnapshot),
                RewardDetailsText = RewardOptionTextFormatter.Format(currentRun?.PendingRewardOffer, currentRun?.PlayerDeck),
            };
        }

        private RoundResult GetLatestRoundResult()
        {
            if (currentRun?.ActiveBattle?.RoundResults == null || currentRun.ActiveBattle.RoundResults.Count == 0)
            {
                return null;
            }

            return currentRun.ActiveBattle.RoundResults[currentRun.ActiveBattle.RoundResults.Count - 1];
        }

        private PhaseSnapshot GetSelectedSnapshot(RoundResult latestRoundResult)
        {
            if (latestRoundResult?.Snapshots == null || latestRoundResult.Snapshots.Count == 0)
            {
                return null;
            }

            for (int snapshotIndex = 0; snapshotIndex < latestRoundResult.Snapshots.Count; snapshotIndex++)
            {
                PhaseSnapshot snapshot = latestRoundResult.Snapshots[snapshotIndex];
                if ((int)snapshot.Phase == debugUiState.SelectedSnapshotPhaseIndex)
                {
                    return snapshot;
                }
            }

            return latestRoundResult.Snapshots[0];
        }

        private static BoardSlotState GetSlot(LaneState laneState, int index)
        {
            if (laneState?.Slots == null || index < 0 || index >= laneState.Slots.Count)
            {
                return null;
            }

            return laneState.Slots[index];
        }

        private static BoardSlotViewData BuildBoardSlotViewData(LaneState laneState, int index)
        {
            BoardSlotState slotState = GetSlot(laneState, index);
            if (slotState != null)
            {
                return BoardSlotTextFormatter.Format(slotState);
            }

            return new BoardSlotViewData
            {
                SlotTitleText = $"Slot {index + 1}",
                OccupantNameText = "-",
                TraitsText = "Traits: -",
                PowerText = "Power: -",
                ExtraText = "No active battle",
            };
        }

        private CardInstance FindPlayerCard(string instanceId)
        {
            if (currentRun?.PlayerDeck == null || string.IsNullOrWhiteSpace(instanceId))
            {
                return null;
            }

            for (int index = 0; index < currentRun.PlayerDeck.Count; index++)
            {
                CardInstance card = currentRun.PlayerDeck[index];
                if (card != null && card.InstanceId == instanceId)
                {
                    return card;
                }
            }

            return null;
        }
    }
}
