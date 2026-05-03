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
        }

        private void InitializeInspectorShell()
        {
            string[] phaseNames = Enum.GetNames(typeof(RoundPhase));
            inspectorPanelView?.SetSnapshotPhaseOptions(phaseNames, debugUiState.SelectedSnapshotPhaseIndex);
        }

        private void RefreshShell()
        {
            const string placeholder = "-";

            runSummaryPanelView?.SetVisible(true);
            runSummaryPanelView?.SetPlaceholder(placeholder);

            boardPanelView?.SetVisible(true);
            boardPanelView?.SetPlaceholders(placeholder);

            enemySequencePanelView?.SetVisible(true);
            enemySequencePanelView?.SetEmptyStateText("Enemy sequence entries will be generated in Task 7C3.");

            playerDeckPanelView?.SetVisible(true);
            playerDeckPanelView?.SetEmptyStateText("Deck entries will be generated in Task 7C3.");

            rewardPanelView?.SetVisible(true);
            rewardPanelView?.SetPlaceholderText("No reward pending.");

            inspectorPanelView?.SetVisible(true);
            inspectorPanelView?.SetPlaceholderText(placeholder);

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
    }
}
