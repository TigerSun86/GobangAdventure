using UnityEngine;

namespace BR3.Presentation.DebugUi
{
    public sealed class BoardPanelView : MonoBehaviour
    {
        [SerializeField] private BoardSlotPanelView enemySlot1;
        [SerializeField] private BoardSlotPanelView enemySlot2;
        [SerializeField] private BoardSlotPanelView enemySlot3;
        [SerializeField] private BoardSlotPanelView playerSlot1;
        [SerializeField] private BoardSlotPanelView playerSlot2;
        [SerializeField] private BoardSlotPanelView playerSlot3;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetPlaceholders(string placeholder)
        {
            enemySlot1?.SetPlaceholder(placeholder);
            enemySlot2?.SetPlaceholder(placeholder);
            enemySlot3?.SetPlaceholder(placeholder);
            playerSlot1?.SetPlaceholder(placeholder);
            playerSlot2?.SetPlaceholder(placeholder);
            playerSlot3?.SetPlaceholder(placeholder);
        }
    }
}
