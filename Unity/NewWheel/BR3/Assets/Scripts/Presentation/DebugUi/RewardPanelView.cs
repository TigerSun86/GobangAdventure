using TMPro;
using UnityEngine;

namespace BR3.Presentation.DebugUi
{
    public sealed class RewardPanelView : MonoBehaviour
    {
        [SerializeField] private Transform rewardListRoot;
        [SerializeField] private TMP_Text rewardPlaceholderText;

        public Transform RewardListRoot => rewardListRoot;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetPlaceholderText(string message)
        {
            if (rewardPlaceholderText != null)
            {
                rewardPlaceholderText.text = message;
                rewardPlaceholderText.gameObject.SetActive(!string.IsNullOrWhiteSpace(message));
            }
        }
    }
}
