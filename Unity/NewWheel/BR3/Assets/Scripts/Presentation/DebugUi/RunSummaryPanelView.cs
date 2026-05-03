using TMPro;
using UnityEngine;

namespace BR3.Presentation.DebugUi
{
    public sealed class RunSummaryPanelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerHpText;
        [SerializeField] private TMP_Text enemyIndexText;
        [SerializeField] private TMP_Text enemyHpText;
        [SerializeField] private TMP_Text battlesPlayedText;
        [SerializeField] private TMP_Text rewardsClaimedText;
        [SerializeField] private TMP_Text runStageText;
        [SerializeField] private TMP_Text battleStageText;
        [SerializeField] private TMP_Text roundText;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetPlaceholder(string placeholder)
        {
            SetText(playerHpText, placeholder);
            SetText(enemyIndexText, placeholder);
            SetText(enemyHpText, placeholder);
            SetText(battlesPlayedText, placeholder);
            SetText(rewardsClaimedText, placeholder);
            SetText(runStageText, placeholder);
            SetText(battleStageText, placeholder);
            SetText(roundText, placeholder);
        }

        private static void SetText(TMP_Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
