using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BR3.Presentation.DebugUi
{
    public sealed class InspectorPanelView : MonoBehaviour
    {
        private const string LogTemplateName = "Log Entry Template";
        private const string LogTextName = "Log Text";
        private const string SlotResultTemplateName = "Slot Result Entry Template";
        private const string SlotNameTextName = "Slot Name Text";
        private const string SlotResultSummaryTextName = "Slot Result Summary Text";

        [SerializeField] private TMP_Text latestRoundResultText;
        [SerializeField] private Transform slotResultsListRoot;
        [SerializeField] private Transform logsContentRoot;
        [SerializeField] private TMP_Dropdown snapshotPhaseDropdown;
        [SerializeField] private TMP_Text snapshotText;
        [SerializeField] private TMP_Text rewardDetailsText;

        public Transform SlotResultsListRoot => slotResultsListRoot;
        public Transform LogsContentRoot => logsContentRoot;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetSnapshotPhaseOptions(IReadOnlyList<string> phaseLabels, int selectedIndex)
        {
            if (snapshotPhaseDropdown == null)
            {
                return;
            }

            snapshotPhaseDropdown.ClearOptions();

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>(phaseLabels.Count);
            for (int index = 0; index < phaseLabels.Count; index++)
            {
                options.Add(new TMP_Dropdown.OptionData(phaseLabels[index]));
            }

            snapshotPhaseDropdown.AddOptions(options);
            snapshotPhaseDropdown.SetValueWithoutNotify(Mathf.Clamp(selectedIndex, 0, Math.Max(0, options.Count - 1)));
        }

        public void BindSnapshotPhaseChanged(Action<int> onValueChanged)
        {
            if (snapshotPhaseDropdown == null)
            {
                return;
            }

            snapshotPhaseDropdown.onValueChanged.RemoveAllListeners();
            if (onValueChanged != null)
            {
                snapshotPhaseDropdown.onValueChanged.AddListener(onValueChanged.Invoke);
            }
        }

        public void Render(InspectorPanelViewData viewData)
        {
            SetText(latestRoundResultText, viewData?.LatestRoundResult?.SummaryText ?? "-");
            SetText(snapshotText, viewData?.SnapshotText ?? "-");
            SetText(rewardDetailsText, viewData?.RewardDetailsText ?? "-");
        }

        public void RenderLogRows(IReadOnlyList<LogEntryViewData> logRows)
        {
            RenderEntries(
                logsContentRoot,
                LogTemplateName,
                (instance, row) => ApplyText(instance, LogTextName, row?.LogText ?? "-"),
                logRows);
        }

        public void RenderSlotResultRows(IReadOnlyList<SlotResultRowViewData> slotResultRows)
        {
            RenderEntries(
                slotResultsListRoot,
                SlotResultTemplateName,
                (instance, row) =>
                {
                    ApplyText(instance, SlotNameTextName, row?.SlotNameText ?? "-");
                    ApplyText(instance, SlotResultSummaryTextName, row?.SummaryText ?? "-");
                },
                slotResultRows);
        }

        public void SetSnapshotPhaseInteractable(bool isInteractable)
        {
            if (snapshotPhaseDropdown != null)
            {
                snapshotPhaseDropdown.interactable = isInteractable;
            }
        }

        private static void SetText(TMP_Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static void RenderEntries<TViewData>(
            Transform root,
            string templateName,
            Action<Transform, TViewData> bindAction,
            IReadOnlyList<TViewData> rows)
        {
            if (root == null)
            {
                return;
            }

            Transform templateTransform = root.Find(templateName);
            if (templateTransform == null)
            {
                return;
            }

            templateTransform.gameObject.SetActive(false);

            for (int childIndex = root.childCount - 1; childIndex >= 0; childIndex--)
            {
                Transform child = root.GetChild(childIndex);
                if (child != templateTransform)
                {
                    Destroy(child.gameObject);
                }
            }

            if (rows == null)
            {
                return;
            }

            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                GameObject instance = Instantiate(templateTransform.gameObject, root);
                instance.name = $"{templateTransform.name} (Generated {rowIndex})";
                bindAction?.Invoke(instance.transform, rows[rowIndex]);
                instance.SetActive(true);
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
