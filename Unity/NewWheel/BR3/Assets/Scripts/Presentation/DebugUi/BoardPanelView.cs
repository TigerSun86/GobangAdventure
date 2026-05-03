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

        public void Render(BoardViewData viewData)
        {
            enemySlot1?.Render(viewData?.EnemySlot1);
            enemySlot2?.Render(viewData?.EnemySlot2);
            enemySlot3?.Render(viewData?.EnemySlot3);
            playerSlot1?.Render(viewData?.PlayerSlot1);
            playerSlot2?.Render(viewData?.PlayerSlot2);
            playerSlot3?.Render(viewData?.PlayerSlot3);
        }
    }
}
