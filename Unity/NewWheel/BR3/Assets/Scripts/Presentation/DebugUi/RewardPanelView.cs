using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BR3.Presentation.DebugUi
{
    public sealed class RewardPanelView : MonoBehaviour
    {
        private const string RewardOptionTemplateName = "Reward Option Entry Template";
        private const string OptionTitleTextName = "Option Title Text";
        private const string OptionDetailsTextName = "Option Details Text";

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

        public void RenderRewardOptions(IReadOnlyList<RewardOptionEntryViewData> rewardOptions, Action<string> onRewardSelected)
        {
            if (rewardListRoot == null)
            {
                return;
            }

            Transform templateTransform = rewardListRoot.Find(RewardOptionTemplateName);
            if (templateTransform == null)
            {
                return;
            }

            templateTransform.gameObject.SetActive(false);
            ClearGeneratedEntries(rewardListRoot, templateTransform);

            if (rewardOptions == null)
            {
                return;
            }

            for (int optionIndex = 0; optionIndex < rewardOptions.Count; optionIndex++)
            {
                RewardOptionEntryViewData option = rewardOptions[optionIndex];
                GameObject instance = Instantiate(templateTransform.gameObject, rewardListRoot);
                instance.name = $"{templateTransform.name} (Generated {optionIndex})";

                ApplyText(instance.transform, OptionTitleTextName, option?.TitleText ?? "-");
                ApplyText(instance.transform, OptionDetailsTextName, option?.DetailsText ?? "-");
                BindButton(instance, option, onRewardSelected);
                instance.SetActive(true);
            }
        }

        private static void BindButton(GameObject instance, RewardOptionEntryViewData option, Action<string> onRewardSelected)
        {
            Button button = instance.GetComponent<Button>();
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.interactable = option != null && option.IsInteractable;

            if (option != null && option.IsInteractable && !string.IsNullOrWhiteSpace(option.OptionId) && onRewardSelected != null)
            {
                string optionId = option.OptionId;
                button.onClick.AddListener(() => onRewardSelected(optionId));
            }
        }

        private static void ClearGeneratedEntries(Transform root, Transform templateTransform)
        {
            for (int childIndex = root.childCount - 1; childIndex >= 0; childIndex--)
            {
                Transform child = root.GetChild(childIndex);
                if (child != templateTransform)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private static void ApplyText(Transform root, string childName, string value)
        {
            TMP_Text[] texts = root.GetComponentsInChildren<TMP_Text>(true);
            for (int index = 0; index < texts.Length; index++)
            {
                if (texts[index].name == childName)
                {
                    texts[index].text = value;
                    return;
                }
            }
        }
    }
}
