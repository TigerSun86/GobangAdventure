using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BR3.Presentation.DebugUi
{
    public sealed class ActionBarView : MonoBehaviour
    {
        [SerializeField] private Button loadConfigButton;
        [SerializeField] private Button newRunButton;
        [SerializeField] private Button startBattleButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TMP_Text statusMessageText;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void Bind(
            Action onLoadConfig,
            Action onNewRun,
            Action onStartBattle,
            Action onContinue,
            Action onQuit)
        {
            BindButton(loadConfigButton, onLoadConfig);
            BindButton(newRunButton, onNewRun);
            BindButton(startBattleButton, onStartBattle);
            BindButton(continueButton, onContinue);
            BindButton(quitButton, onQuit);
        }

        public void SetButtonsInteractable(
            bool canLoadConfig,
            bool canCreateRun,
            bool canStartBattle,
            bool canContinue,
            bool canQuit)
        {
            SetInteractable(loadConfigButton, canLoadConfig);
            SetInteractable(newRunButton, canCreateRun);
            SetInteractable(startBattleButton, canStartBattle);
            SetInteractable(continueButton, canContinue);
            SetInteractable(quitButton, canQuit);
        }

        public void SetStatusMessage(string message)
        {
            if (statusMessageText != null)
            {
                statusMessageText.text = message;
            }
        }

        private static void BindButton(Button button, Action handler)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            if (handler != null)
            {
                button.onClick.AddListener(handler.Invoke);
            }
        }

        private static void SetInteractable(Selectable selectable, bool isInteractable)
        {
            if (selectable != null)
            {
                selectable.interactable = isInteractable;
            }
        }
    }
}
