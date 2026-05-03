using TMPro;
using UnityEngine;

namespace BR3.Presentation.DebugUi
{
    public sealed class PlayerDeckPanelView : MonoBehaviour
    {
        [SerializeField] private Transform deckListRoot;
        [SerializeField] private TMP_Text emptyStateText;

        public Transform DeckListRoot => deckListRoot;

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
