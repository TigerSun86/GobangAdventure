using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BR3.Presentation.DebugUi
{
    public sealed class EnemySequencePanelView : MonoBehaviour
    {
        private const string SequenceTemplateName = "Enemy Sequence Entry Template";
        private const string SequenceIndexTextName = "Sequence Index Text";
        private const string SequenceCardTextName = "Sequence Card Text";

        [SerializeField] private Transform sequenceListRoot;

        public Transform SequenceListRoot => sequenceListRoot;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void RenderSequenceRows(IReadOnlyList<EnemySequenceRowViewData> rows)
        {
            if (sequenceListRoot == null)
            {
                return;
            }

            GameObject template = FindTemplate(sequenceListRoot, SequenceTemplateName);
            if (template == null)
            {
                return;
            }

            template.SetActive(false);
            ClearGeneratedEntries(sequenceListRoot, template.transform);

            if (rows == null)
            {
                return;
            }

            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                GameObject instance = Instantiate(template, sequenceListRoot);
                instance.name = $"{template.name} (Generated {rowIndex})";
                ApplyText(instance.transform, SequenceIndexTextName, rows[rowIndex]?.SequenceIndexText ?? "-");
                ApplyText(instance.transform, SequenceCardTextName, rows[rowIndex]?.SequenceCardText ?? "-");
                instance.SetActive(true);
            }
        }

        private static GameObject FindTemplate(Transform root, string templateName)
        {
            Transform templateTransform = root.Find(templateName);
            return templateTransform == null ? null : templateTransform.gameObject;
        }

        private static void ClearGeneratedEntries(Transform root, Transform templateTransform)
        {
            for (int childIndex = root.childCount - 1; childIndex >= 0; childIndex--)
            {
                Transform child = root.GetChild(childIndex);
                if (child != templateTransform)
                {
                    Destroy(child.gameObject);
                }
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
