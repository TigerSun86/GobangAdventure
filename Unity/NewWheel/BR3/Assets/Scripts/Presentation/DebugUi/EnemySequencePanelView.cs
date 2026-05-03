using TMPro;
using UnityEngine;

namespace BR3.Presentation.DebugUi
{
    public sealed class EnemySequencePanelView : MonoBehaviour
    {
        [SerializeField] private Transform sequenceListRoot;
        [SerializeField] private TMP_Text emptyStateText;

        public Transform SequenceListRoot => sequenceListRoot;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetEmptyStateText(string message)
        {
            if (emptyStateText != null)
            {
                emptyStateText.text = message;
                emptyStateText.gameObject.SetActive(!string.IsNullOrWhiteSpace(message));
            }
        }
    }
}
