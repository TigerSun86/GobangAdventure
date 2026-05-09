using System.Collections;
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

            yield return LoadDebugWorkbenchScene();
            yield return WaitForController(controllerResult => controller = controllerResult);

            controller.OnLoadConfigButtonPressed();
            yield return null;

            controller.OnNewRunButtonPressed();
            yield return null;

            controller.OnStartBattleButtonPressed();
            yield return null;

            Assert.That(controller.CurrentRun, Is.Not.Null);
            Assert.That(controller.CurrentRun.FlowStage, Is.EqualTo(RunFlowStage.InBattle));
            Assert.That(controller.CurrentRun.ActiveBattle, Is.Not.Null);
            Assert.That(controller.CurrentRun.ActiveBattle.BattleFlowStage, Is.EqualTo(BattleFlowStage.WaitingForPlayerCard));
            Assert.That(controller.CurrentRun.ActiveBattle.RoundIndex, Is.EqualTo(1));
            Assert.That(controller.CurrentRun.ActiveBattle.EnemySequence, Is.Not.Null.And.Count.GreaterThan(0));
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
    }
}
