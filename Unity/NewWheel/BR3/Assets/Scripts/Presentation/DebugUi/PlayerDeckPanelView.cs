using TMPro;
using UnityEngine;

namespace BR3.Presentation.DebugUi
{
    public sealed class PlayerDeckPanelView : MonoBehaviour
    {
        [SerializeField] private Transform deckListRoot;

        public Transform DeckListRoot => deckListRoot;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}
