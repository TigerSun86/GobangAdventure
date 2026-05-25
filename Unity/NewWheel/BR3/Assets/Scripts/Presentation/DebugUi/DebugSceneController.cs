using System;
using System.IO;
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
        private const string StreamingConfigFileName = "game_config.json";

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
        private string streamingConfigPath;

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
            streamingConfigPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, StreamingConfigFileName);

            actionBarView?.Bind(OnLoadConfigButtonPressed, OnNewRunButtonPressed, OnStartBattleButtonPressed, OnContinueButtonPressed, OnQuitButtonPressed);
            inspectorPanelView?.BindSnapshotPhaseChanged(OnSnapshotPhaseChanged);
            RefreshAll();
        }

        public void OnLoadConfigButtonPressed()
        {
            try
            {
                currentConfig = gameConfigLoader.LoadFromFile(streamingConfigPath);

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
                SetStatusMessage($"Config load failed: {exception.Message} ({streamingConfigPath})");
            }

            RefreshAll();
        }

        public void OnNewRunButtonPressed()
        {
            if (currentConfig == null)
            {
                SetStatusMessage("Load a valid config before creating a new run.");
                RefreshAll();
                return;
            }

            currentRun = runService.CreateNewRun(currentConfig, runtimeStateFactory);
            debugUiState = DebugUiState.CreateDefault();
            SetStatusMessage("New run created. Start Battle is now available.");
            RefreshAll();
        }

        public void OnStartBattleButtonPressed()
        {
            if (currentRun == null)
            {
                SetStatusMessage("Create a run before starting a battle.");
                RefreshAll();
                return;
            }

            RunCommandResult result = runService.StartNextBattle(currentRun, battleService);
            SetStatusMessage(result.Success ? "Battle started." : result.FailureReason);

            RefreshAll();
        }

        public void OnPlayerCardSelected(string cardInstanceId)
        {
            if (currentRun?.ActiveBattle == null || currentRun.CurrentEnemy == null)
            {
                SetStatusMessage("No active battle is ready for card selection.");
                RefreshAll();
                return;
            }

            if (currentConfig?.traitTuning == null)
            {
                SetStatusMessage("No loaded config is available for round resolution.");
                RefreshAll();
                return;
            }

            BattleCommandResult result = battleService.SubmitPlayerCard(
                currentRun,
                currentRun.CurrentEnemy,
                currentRun.ActiveBattle,
                cardInstanceId,
                currentConfig.traitTuning,
                roundResolver);

            SetStatusMessage(result.Success ? "Player card submitted." : result.FailureReason);
            RefreshAll();
        }

        public void OnRewardOptionSelected(string optionId)
        {
            if (currentRun == null || currentConfig == null)
            {
                SetStatusMessage("No loaded run/config is available for reward selection.");
                RefreshAll();
                return;
            }

            RunCommandResult result = runService.ChooseReward(
                currentRun,
                optionId,
                rewardService,
                currentConfig,
                runtimeStateFactory);

            SetStatusMessage(result.Success ? "Reward chosen." : result.FailureReason);
            RefreshAll();
        }

        public void OnContinueButtonPressed()
        {
            if (currentRun?.ActiveBattle == null || currentRun.CurrentEnemy == null)
            {
                SetStatusMessage("No active battle is ready to continue.");
                RefreshAll();
                return;
            }

            BattleCommandResult result = battleService.FinishRoundPresentation(currentRun, currentRun.CurrentEnemy, currentRun.ActiveBattle);
            if (!result.Success)
            {
                SetStatusMessage(result.FailureReason);
                RefreshAll();
                return;
            }

            if (result.IsBattleComplete && result.BattleOutcome != null)
            {
                if (currentConfig == null)
                {
                    SetStatusMessage("Battle completed, but no config is available to advance run flow.");
                    RefreshAll();
                    return;
                }

                RunCommandResult runResult = runService.AcceptCompletedBattle(currentRun, result.BattleOutcome, rewardService, currentConfig);
                SetStatusMessage(GetRunOutcomeStatusMessage(runResult) ?? (runResult.Success ? "Battle completed." : runResult.FailureReason));
            }
            else
            {
                SetStatusMessage("Round presentation finished.");
            }

            RefreshAll();
        }

        public void OnSnapshotPhaseChanged(int selectedIndex)
        {
            debugUiState.SelectedSnapshotPhaseIndex = Mathf.Max(0, selectedIndex);
            RefreshAll();
        }

        public void OnQuitButtonPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        public void RefreshAll()
        {
            runSummaryPanelView?.SetVisible(true);
            runSummaryPanelView?.Render(BuildRunSummaryViewData());

            boardPanelView?.SetVisible(true);
            boardPanelView?.Render(BuildBoardViewData());

            enemySequencePanelView?.SetVisible(true);
            enemySequencePanelView?.RenderSequenceRows(BuildEnemySequenceRowViewData());

            playerDeckPanelView?.SetVisible(true);
            playerDeckPanelView?.RenderDeckEntries(BuildDeckEntryViewData(), OnPlayerCardSelected);

            rewardPanelView?.SetVisible(true);
            rewardPanelView?.Render(BuildRewardPanelViewData());
            rewardPanelView?.RenderRewardOptions(BuildRewardOptionEntryViewData(), OnRewardOptionSelected);

            RefreshSnapshotSelector();

            inspectorPanelView?.SetVisible(true);
            inspectorPanelView?.Render(BuildInspectorPanelViewData());
            inspectorPanelView?.RenderLogRows(BuildLogEntryViewData());
            inspectorPanelView?.RenderSlotResultRows(BuildSlotResultRowViewData());

            bool hasLoadedConfig = currentConfig != null;
            bool canCreateRun = hasLoadedConfig;
            bool canStartBattle = currentRun != null && runService.CanStartNextBattle(currentRun);
            bool canContinue = currentRun?.ActiveBattle != null
                && currentRun.ActiveBattle.BattleFlowStage == BattleFlowStage.PresentingRoundResult;

            actionBarView?.SetVisible(true);
            actionBarView?.SetButtonsInteractable(
                canLoadConfig: true,
                canCreateRun: canCreateRun,
                canStartBattle: canStartBattle,
                canContinue: canContinue,
                canQuit: true);
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

        private static string GetRunOutcomeStatusMessage(RunCommandResult runCommandResult)
        {
            if (runCommandResult == null || !runCommandResult.Success)
            {
                return null;
            }

            switch (runCommandResult.FlowStage)
            {
                case RunFlowStage.Victory:
                    return "Victory.";
                case RunFlowStage.Defeat:
                    return "Defeat.";
                default:
                    return null;
            }
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
            EnemyProgressState currentEnemy = currentRun.CurrentEnemy;
            int? battleLimit = currentEnemy?.Config?.battleLimit;

            return new RunSummaryViewData
            {
                PlayerHpText = $"HP {currentRun.PlayerHp}/{currentRun.PlayerMaxHp}",
                EnemyIndexText = $"Enemy {enemyIndexText}",
                EnemyHpText = currentEnemy == null ? "Enemy HP -" : $"Enemy HP {currentEnemy.CurrentHp}/{currentEnemy.MaxHp}",
                BattlesPlayedText = FormatProgressText("Battles", currentEnemy?.BattlesPlayed, battleLimit),
                RewardsClaimedText = FormatProgressText("Rewards", currentEnemy?.RewardsClaimed, battleLimit),
                RunStageText = $"Run {FlowStageTextFormatter.Format(currentRun.FlowStage)}",
                BattleStageText = $"Battle {FlowStageTextFormatter.Format(activeBattle?.BattleFlowStage)}",
                RoundText = activeBattle == null ? "Round -" : $"Round {activeBattle.RoundIndex}",
            };
        }

        private static string FormatProgressText(string label, int? currentValue, int? totalValue)
        {
            if (!currentValue.HasValue)
            {
                return $"{label} -";
            }

            return totalValue.HasValue
                ? $"{label} {currentValue.Value}/{totalValue.Value}"
                : $"{label} {currentValue.Value}";
        }

        private BoardViewData BuildBoardViewData()
        {
            LaneState enemyLane = currentRun?.ActiveBattle?.EnemyLane;
            LaneState playerLane = currentRun?.ActiveBattle?.PlayerLane;
            int? currentRoundIndex = currentRun?.ActiveBattle?.RoundIndex;

            return new BoardViewData
            {
                EnemySlot1 = BuildBoardSlotViewData(enemyLane, 0, currentRoundIndex),
                EnemySlot2 = BuildBoardSlotViewData(enemyLane, 1, currentRoundIndex),
                EnemySlot3 = BuildBoardSlotViewData(enemyLane, 2, currentRoundIndex),
                PlayerSlot1 = BuildBoardSlotViewData(playerLane, 0, currentRoundIndex),
                PlayerSlot2 = BuildBoardSlotViewData(playerLane, 1, currentRoundIndex),
                PlayerSlot3 = BuildBoardSlotViewData(playerLane, 2, currentRoundIndex),
            };
        }

        private RewardPanelViewData BuildRewardPanelViewData()
        {
            bool hasPendingReward = currentRun?.PendingRewardOffer != null
                && currentRun.FlowStage == RunFlowStage.ChoosingReward;

            if (!hasPendingReward)
            {
                return new RewardPanelViewData
                {
                    PlaceholderText = "No reward pending.",
                };
            }

            RewardOffer rewardOffer = currentRun.PendingRewardOffer;
            return new RewardPanelViewData
            {
                PlaceholderText = string.Empty,
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
                RewardDetailsText = RewardOptionTextFormatter.Format(currentRun?.PendingRewardOffer, currentRun?.PlayerDeck, currentConfig?.traitTuning),
            };
        }

        private void RefreshSnapshotSelector()
        {
            string[] phaseLabels = BuildSnapshotPhaseLabels();
            int selectedIndex = ClampSnapshotSelectionIndex(phaseLabels.Length);
            debugUiState.SelectedSnapshotPhaseIndex = selectedIndex;

            inspectorPanelView?.SetSnapshotPhaseOptions(phaseLabels, selectedIndex);
            inspectorPanelView?.SetSnapshotPhaseInteractable(phaseLabels.Length > 1 || (GetLatestRoundResult()?.Snapshots?.Count ?? 0) > 0);
        }

        private string[] BuildSnapshotPhaseLabels()
        {
            RoundResult latestRoundResult = GetLatestRoundResult();
            if (latestRoundResult?.Snapshots == null || latestRoundResult.Snapshots.Count == 0)
            {
                return new[] { "No Snapshots" };
            }

            string[] phaseLabels = new string[latestRoundResult.Snapshots.Count];
            for (int index = 0; index < latestRoundResult.Snapshots.Count; index++)
            {
                phaseLabels[index] = latestRoundResult.Snapshots[index].Phase.ToString();
            }

            return phaseLabels;
        }

        private int ClampSnapshotSelectionIndex(int optionCount)
        {
            if (optionCount <= 0)
            {
                return 0;
            }

            return Mathf.Clamp(debugUiState.SelectedSnapshotPhaseIndex, 0, optionCount - 1);
        }

        private EnemySequenceRowViewData[] BuildEnemySequenceRowViewData()
        {
            if (currentRun?.ActiveBattle?.EnemySequence == null)
            {
                return Array.Empty<EnemySequenceRowViewData>();
            }

            EnemySequenceRowViewData[] rows = new EnemySequenceRowViewData[currentRun.ActiveBattle.EnemySequence.Count];
            for (int index = 0; index < currentRun.ActiveBattle.EnemySequence.Count; index++)
            {
                rows[index] = EnemySequenceTextFormatter.Format(index + 1, currentRun.ActiveBattle.EnemySequence[index]);
            }

            return rows;
        }

        private DeckEntryViewData[] BuildDeckEntryViewData()
        {
            if (currentRun?.PlayerDeck == null)
            {
                return Array.Empty<DeckEntryViewData>();
            }

            bool isWaitingForPlayerCard = currentRun.ActiveBattle != null
                && currentRun.ActiveBattle.BattleFlowStage == BattleFlowStage.WaitingForPlayerCard;
            bool hasActiveBattle = currentRun.ActiveBattle != null;

            DeckEntryViewData[] entries = new DeckEntryViewData[currentRun.PlayerDeck.Count];
            for (int index = 0; index < currentRun.PlayerDeck.Count; index++)
            {
                CardInstance card = currentRun.PlayerDeck[index];
                bool isUsed = currentRun.ActiveBattle?.UsedPlayerCardIds != null
                    && card != null
                    && currentRun.ActiveBattle.UsedPlayerCardIds.Contains(card.InstanceId);

                string stateText;
                if (!hasActiveBattle)
                {
                    stateText = currentRun.FlowStage == RunFlowStage.ChoosingReward ? "Reward stage" : "No active battle";
                }
                else if (isUsed)
                {
                    stateText = "Already used";
                }
                else if (isWaitingForPlayerCard)
                {
                    stateText = "Available";
                }
                else
                {
                    stateText = "Not selectable now";
                }

                string deckLabel = CardTextFormatter.FormatDeckLabel(card, index + 1);
                entries[index] = DeckEntryTextFormatter.Format(card, deckLabel, isUsed, isWaitingForPlayerCard, currentConfig?.traitTuning, stateText);
            }

            return entries;
        }

        private RewardOptionEntryViewData[] BuildRewardOptionEntryViewData()
        {
            bool canChooseReward = currentRun?.FlowStage == RunFlowStage.ChoosingReward
                && currentRun.PendingRewardOffer?.Options != null;

            if (!canChooseReward)
            {
                return Array.Empty<RewardOptionEntryViewData>();
            }

            RewardOptionEntryViewData[] entries = new RewardOptionEntryViewData[currentRun.PendingRewardOffer.Options.Count];
            for (int index = 0; index < currentRun.PendingRewardOffer.Options.Count; index++)
            {
                RewardOption option = currentRun.PendingRewardOffer.Options[index];
                CardInstance targetCard = FindTargetCardForRewardOption(option);
                string targetCardLabel = CardTextFormatter.FormatDeckLabel(targetCard, currentRun.PlayerDeck);
                entries[index] = RewardOptionEntryTextFormatter.Format(option, targetCard, targetCardLabel, currentConfig?.traitTuning, true);
            }

            return entries;
        }

        private LogEntryViewData[] BuildLogEntryViewData()
        {
            if (currentRun?.ActiveBattle?.Logs == null)
            {
                return Array.Empty<LogEntryViewData>();
            }

            LogEntryViewData[] rows = new LogEntryViewData[currentRun.ActiveBattle.Logs.Count];
            for (int index = 0; index < currentRun.ActiveBattle.Logs.Count; index++)
            {
                rows[index] = LogTextFormatter.Format(currentRun.ActiveBattle.Logs[index]);
            }

            return rows;
        }

        private SlotResultRowViewData[] BuildSlotResultRowViewData()
        {
            RoundResult latestRoundResult = GetLatestRoundResult();
            if (latestRoundResult?.SlotResults == null)
            {
                return Array.Empty<SlotResultRowViewData>();
            }

            SlotResultRowViewData[] rows = new SlotResultRowViewData[latestRoundResult.SlotResults.Count];
            for (int index = 0; index < latestRoundResult.SlotResults.Count; index++)
            {
                SlotCombatResult slotResult = latestRoundResult.SlotResults[index];
                rows[index] = SlotCombatResultTextFormatter.Format(slotResult, FindPlayerCard(slotResult?.PlayerCardInstanceId));
            }

            return rows;
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

            int selectedIndex = ClampSnapshotSelectionIndex(latestRoundResult.Snapshots.Count);
            return latestRoundResult.Snapshots[selectedIndex];
        }

        private static BoardSlotState GetSlot(LaneState laneState, int index)
        {
            if (laneState?.Slots == null || index < 0 || index >= laneState.Slots.Count)
            {
                return null;
            }

            return laneState.Slots[index];
        }

        private BoardSlotViewData BuildBoardSlotViewData(LaneState laneState, int index, int? currentRoundIndex)
        {
            BoardSlotState slotState = GetSlot(laneState, index);
            if (slotState != null)
            {
                return BoardSlotTextFormatter.Format(slotState, currentConfig?.traitTuning, currentRoundIndex);
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

        private CardInstance FindTargetCardForRewardOption(RewardOption rewardOption)
        {
            if (rewardOption == null)
            {
                return null;
            }

            if (rewardOption.UpgradePayload != null)
            {
                return FindPlayerCard(rewardOption.UpgradePayload.TargetCardInstanceId);
            }

            if (rewardOption.ReplacePayload != null)
            {
                return FindPlayerCard(rewardOption.ReplacePayload.TargetCardInstanceId);
            }

            return null;
        }
    }
}
