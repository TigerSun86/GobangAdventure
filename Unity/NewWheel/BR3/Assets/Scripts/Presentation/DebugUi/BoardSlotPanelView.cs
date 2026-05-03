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

        public void SetPlaceholder(string placeholder)
        {
            SetText(slotTitleText, placeholder);
            SetText(occupantNameText, placeholder);
            SetText(traitsText, placeholder);
            SetText(powerText, placeholder);
            SetText(extraText, placeholder);
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
