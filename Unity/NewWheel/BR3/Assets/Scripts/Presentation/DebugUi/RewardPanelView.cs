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

        public void Render(RewardPanelViewData viewData)
        {
            if (rewardPlaceholderText != null)
            {
                string placeholderText = viewData?.PlaceholderText ?? string.Empty;
                rewardPlaceholderText.text = placeholderText;
                rewardPlaceholderText.gameObject.SetActive(!string.IsNullOrWhiteSpace(placeholderText));
            }
        }
    }
}
