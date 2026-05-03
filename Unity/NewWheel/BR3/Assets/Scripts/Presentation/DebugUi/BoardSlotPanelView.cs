using TMPro;
using UnityEngine;

namespace BR3.Presentation.DebugUi
{
    public sealed class BoardSlotPanelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text slotTitleText;
        [SerializeField] private TMP_Text occupantNameText;
        [SerializeField] private TMP_Text traitsText;
        [SerializeField] private TMP_Text powerText;
        [SerializeField] private TMP_Text extraText;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void Render(BoardSlotViewData viewData)
        {
            SetText(slotTitleText, viewData?.SlotTitleText ?? "-");
            SetText(occupantNameText, viewData?.OccupantNameText ?? "-");
            SetText(traitsText, viewData?.TraitsText ?? "-");
            SetText(powerText, viewData?.PowerText ?? "-");
            SetText(extraText, viewData?.ExtraText ?? "-");
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
