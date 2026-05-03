using TMPro;
using UnityEngine;

namespace BR3.Presentation.DebugUi
{
    public sealed class EnemySequencePanelView : MonoBehaviour
    {
        [SerializeField] private Transform sequenceListRoot;

        public Transform SequenceListRoot => sequenceListRoot;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}
