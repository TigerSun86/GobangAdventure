using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BR3.Presentation.DebugUi
{
    public sealed class InspectorPanelView : MonoBehaviour
    {
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

        private static void SetText(TMP_Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
