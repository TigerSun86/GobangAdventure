using System.Collections;
using System.Collections.Generic;
using BR3.Application;
using BR3.Domain.Results;
using BR3.Domain.Runtime;
using BR3.Presentation.DebugUi;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace BR3.Tests.PlayMode
{
    public sealed class DebugWorkbenchSmokeTests
    {
        private const string ScenePath = "Assets/Scenes/DebugWorkbenchScene.unity";

        [UnityTest]
        public IEnumerator DebugWorkbenchScene_BootsAndFindsController()
        {
            DebugSceneController controller = null;

            yield return LoadDebugWorkbenchScene();
            yield return WaitForController(controllerResult => controller = controllerResult);

            Assert.That(controller, Is.Not.Null);
            Assert.That(controller.CurrentConfig, Is.Null);
            Assert.That(controller.CurrentRun, Is.Null);
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator LoadConfig_FromDebugWorkbenchScene_LoadsSupportedConfig()
        {
            DebugSceneController controller = null;

            yield return LoadDebugWorkbenchScene();
            yield return WaitForController(controllerResult => controller = controllerResult);

            controller.OnLoadConfigButtonPressed();
            yield return null;

            Assert.That(controller.CurrentConfig, Is.Not.Null);
            Assert.That(controller.CurrentConfig.playerStart, Is.Not.Null);
            Assert.That(controller.CurrentConfig.playerStart.startingDeck, Is.Not.Null.And.Count.GreaterThan(0));
            Assert.That(controller.CurrentConfig.enemies, Is.Not.Null.And.Count.GreaterThan(0));
            Assert.That(controller.CurrentRun, Is.Null);
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator NewRun_AfterConfigLoad_CreatesReadyRunWithoutActiveBattle()
        {
            DebugSceneController controller = null;

            yield return LoadDebugWorkbenchScene();
            yield return WaitForController(controllerResult => controller = controllerResult);

            controller.OnLoadConfigButtonPressed();
            yield return null;

            controller.OnNewRunButtonPressed();
            yield return null;

            Assert.That(controller.CurrentRun, Is.Not.Null);
            Assert.That(controller.CurrentRun.FlowStage, Is.EqualTo(RunFlowStage.ReadyForNextBattle));
            Assert.That(controller.CurrentRun.CurrentEnemy, Is.Not.Null);
            Assert.That(controller.CurrentRun.ActiveBattle, Is.Null);
            Assert.That(controller.CurrentRun.PendingRewardOffer, Is.Null);
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator StartBattle_AfterNewRun_CreatesInitialBattleState()
        {
            DebugSceneController controller = null;

            yield return CreateStartedBattle(controllerResult => controller = controllerResult);

            Assert.That(controller.CurrentRun, Is.Not.Null);
            Assert.That(controller.CurrentRun.FlowStage, Is.EqualTo(RunFlowStage.InBattle));
            Assert.That(controller.CurrentRun.ActiveBattle, Is.Not.Null);
            Assert.That(controller.CurrentRun.ActiveBattle.BattleFlowStage, Is.EqualTo(BattleFlowStage.WaitingForPlayerCard));
            Assert.That(controller.CurrentRun.ActiveBattle.RoundIndex, Is.EqualTo(1));
            Assert.That(controller.CurrentRun.ActiveBattle.EnemySequence, Is.Not.Null.And.Count.GreaterThan(0));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator SubmitPlayerCard_AfterBattleStart_ProducesRoundResultAndPresentationState()
        {
            DebugSceneController controller = null;

            yield return CreateStartedBattle(controllerResult => controller = controllerResult);

            string selectedCardId = controller.CurrentRun.PlayerDeck[0].InstanceId;

            controller.OnPlayerCardSelected(selectedCardId);
            yield return null;

            Assert.That(controller.CurrentRun.ActiveBattle, Is.Not.Null);
            Assert.That(controller.CurrentRun.ActiveBattle.BattleFlowStage, Is.EqualTo(BattleFlowStage.PresentingRoundResult));
            Assert.That(controller.CurrentRun.ActiveBattle.RoundResults, Has.Count.EqualTo(1));
            Assert.That(controller.CurrentRun.ActiveBattle.UsedPlayerCardIds.Contains(selectedCardId), Is.True);
            Assert.That(controller.CurrentRun.ActiveBattle.RoundResults[0], Is.Not.Null);
            Assert.That(controller.CurrentRun.ActiveBattle.RoundResults[0].PlayerCardInstanceId, Is.EqualTo(selectedCardId));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator Continue_AfterRoundResult_AdvancesBattleOutOfPresentationState()
        {
            DebugSceneController controller = null;

            yield return CreateStartedBattle(controllerResult => controller = controllerResult);
            yield return SubmitFirstPlayerCard(controller);

            controller.OnContinueButtonPressed();
            yield return null;

            Assert.That(controller.CurrentRun.ActiveBattle, Is.Not.Null);
            Assert.That(controller.CurrentRun.ActiveBattle.BattleFlowStage, Is.EqualTo(BattleFlowStage.WaitingForPlayerCard));
            Assert.That(controller.CurrentRun.ActiveBattle.BattleFlowStage, Is.Not.EqualTo(BattleFlowStage.PresentingRoundResult));
            Assert.That(controller.CurrentRun.ActiveBattle.RoundIndex, Is.EqualTo(2));
            Assert.That(controller.CurrentRun.FlowStage, Is.EqualTo(RunFlowStage.InBattle));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator ChooseReward_WhenPendingRewardExists_AdvancesRunStateReasonably()
        {
            DebugSceneController controller = null;

            yield return CreateLoadedRun(controllerResult => controller = controllerResult);

            SetupPendingRewardOffer(controller);
            RewardOffer originalRewardOffer = controller.CurrentRun.PendingRewardOffer;
            string selectedOptionId = FindRewardOptionId(originalRewardOffer, RewardOptionType.Skip);

            controller.OnRewardOptionSelected(selectedOptionId);
            yield return null;

            Assert.That(originalRewardOffer, Is.Not.Null);
            Assert.That(controller.CurrentRun.ActiveBattle, Is.Null);
            Assert.That(controller.CurrentRun.PendingRewardOffer, Is.Null);
            Assert.That(controller.CurrentRun.CurrentEnemy.RewardsClaimed, Is.EqualTo(1));
            Assert.That(controller.CurrentRun.FlowStage, Is.EqualTo(RunFlowStage.ReadyForNextBattle));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator Continue_WhenThirdBattleCompletesWithEnemyAlive_EntersDefeat()
        {
            DebugSceneController controller = null;

            yield return CreateLoadedRun(controllerResult => controller = controllerResult);

            SetupPendingDefeatBattle(controller);

            controller.OnContinueButtonPressed();
            yield return null;

            Assert.That(controller.CurrentRun.FlowStage, Is.EqualTo(RunFlowStage.Defeat));
            Assert.That(controller.CurrentRun.ActiveBattle, Is.Null);
            Assert.That(controller.CurrentRun.PendingRewardOffer, Is.Null);
            Assert.That(controller.CurrentRun.CurrentEnemy.BattlesPlayed, Is.EqualTo(3));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator ChooseReward_WhenFinalEnemyIsDefeated_EntersVictory()
        {
            DebugSceneController controller = null;

            yield return CreateLoadedRun(controllerResult => controller = controllerResult);

            SetupPendingVictoryReward(controller);
            RewardOffer originalRewardOffer = controller.CurrentRun.PendingRewardOffer;
            string selectedOptionId = FindRewardOptionId(originalRewardOffer, RewardOptionType.Skip);

            controller.OnRewardOptionSelected(selectedOptionId);
            yield return null;

            Assert.That(controller.CurrentRun.FlowStage, Is.EqualTo(RunFlowStage.Victory));
            Assert.That(controller.CurrentRun.ActiveBattle, Is.Null);
            Assert.That(controller.CurrentRun.PendingRewardOffer, Is.Null);
            Assert.That(controller.CurrentRun.CurrentEnemy.RewardsClaimed, Is.EqualTo(1));
            LogAssert.NoUnexpectedReceived();
        }

        private static IEnumerator LoadDebugWorkbenchScene()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(
                ScenePath,
                new LoadSceneParameters(LoadSceneMode.Single));

            yield return null;
        }

        private static IEnumerator WaitForController(System.Action<DebugSceneController> setController)
        {
            for (int frame = 0; frame < 30; frame++)
            {
                DebugSceneController controller = Object.FindAnyObjectByType<DebugSceneController>();
                if (controller != null)
                {
                    setController(controller);
                    yield break;
                }

                yield return null;
            }

            setController(null);
        }

        private static IEnumerator CreateLoadedRun(System.Action<DebugSceneController> setController)
        {
            DebugSceneController controller = null;

            yield return LoadDebugWorkbenchScene();
            yield return WaitForController(controllerResult => controller = controllerResult);

            controller.OnLoadConfigButtonPressed();
            yield return null;

            controller.OnNewRunButtonPressed();
            yield return null;

            setController(controller);
        }

        private static IEnumerator CreateStartedBattle(System.Action<DebugSceneController> setController)
        {
            DebugSceneController controller = null;

            yield return CreateLoadedRun(controllerResult => controller = controllerResult);

            controller.OnStartBattleButtonPressed();
            yield return null;

            setController(controller);
        }

        private static IEnumerator SubmitFirstPlayerCard(DebugSceneController controller)
        {
            string selectedCardId = controller.CurrentRun.PlayerDeck[0].InstanceId;
            controller.OnPlayerCardSelected(selectedCardId);
            yield return null;
        }

        private static void SetupPendingRewardOffer(DebugSceneController controller)
        {
            RuntimeStateFactory runtimeStateFactory = new RuntimeStateFactory();
            RewardService rewardService = new RewardService(runtimeStateFactory);

            controller.CurrentRun.ActiveBattle = null;
            controller.CurrentRun.CurrentEnemy.CurrentHp = 10;
            controller.CurrentRun.CurrentEnemy.RewardsClaimed = 0;
            controller.CurrentRun.PendingRewardOffer = rewardService.CreateRewardOffer(
                controller.CurrentRun.PlayerDeck,
                controller.CurrentConfig.rewardGeneration,
                rewardIndexForCurrentEnemy: 1);
            controller.CurrentRun.FlowStage = RunFlowStage.ChoosingReward;
            controller.RefreshAll();
        }

        private static void SetupPendingDefeatBattle(DebugSceneController controller)
        {
            controller.CurrentRun.CurrentEnemy.BattlesPlayed = 2;
            controller.CurrentRun.CurrentEnemy.RewardsClaimed = 0;
            controller.CurrentRun.CurrentEnemy.CurrentHp = 5;
            controller.CurrentRun.PendingRewardOffer = null;
            controller.CurrentRun.ActiveBattle = CreatePresentingBattleState(battleIndexForEnemy: 3, roundIndex: 3);
            controller.CurrentRun.FlowStage = RunFlowStage.InBattle;
            controller.RefreshAll();
        }

        private static void SetupPendingVictoryReward(DebugSceneController controller)
        {
            RuntimeStateFactory runtimeStateFactory = new RuntimeStateFactory();
            RewardService rewardService = new RewardService(runtimeStateFactory);
            int finalEnemyIndex = controller.CurrentConfig.enemies.Count - 1;

            controller.CurrentRun.CurrentEnemyIndex = finalEnemyIndex;
            controller.CurrentRun.CurrentEnemy = runtimeStateFactory.CreateEnemyProgressState(controller.CurrentConfig.enemies[finalEnemyIndex]);
            controller.CurrentRun.CurrentEnemy.CurrentHp = 0;
            controller.CurrentRun.CurrentEnemy.RewardsClaimed = 0;
            controller.CurrentRun.ActiveBattle = null;
            controller.CurrentRun.PendingRewardOffer = rewardService.CreateRewardOffer(
                controller.CurrentRun.PlayerDeck,
                controller.CurrentConfig.rewardGeneration,
                rewardIndexForCurrentEnemy: 1);
            controller.CurrentRun.FlowStage = RunFlowStage.ChoosingReward;
            controller.RefreshAll();
        }

        private static BattleState CreatePresentingBattleState(int battleIndexForEnemy, int roundIndex)
        {
            return new BattleState
            {
                BattleIndexForEnemy = battleIndexForEnemy,
                RoundIndex = roundIndex,
                PlayerLane = null,
                EnemyLane = null,
                UsedPlayerCardIds = new HashSet<string>(),
                EnemySequence = new List<BR3.Config.CardSpec>(),
                RoundResults = new List<RoundResult>(),
                Logs = new List<string>(),
                Snapshots = new List<PhaseSnapshot>(),
                BattleFlowStage = BattleFlowStage.PresentingRoundResult,
            };
        }

        private static string FindRewardOptionId(RewardOffer rewardOffer, RewardOptionType rewardOptionType)
        {
            for (int optionIndex = 0; optionIndex < rewardOffer.Options.Count; optionIndex++)
            {
                RewardOption option = rewardOffer.Options[optionIndex];
                if (option.Type == rewardOptionType)
                {
                    return option.OptionId;
                }
            }

            Assert.Fail($"Could not find reward option of type {rewardOptionType}.");
            return null;
        }
    }
}
