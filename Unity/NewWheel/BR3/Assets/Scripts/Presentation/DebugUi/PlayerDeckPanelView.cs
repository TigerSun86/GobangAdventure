using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BR3.Presentation.DebugUi
{
    public sealed class PlayerDeckPanelView : MonoBehaviour
    {
        private const string DeckTemplateName = "Deck Entry Template";
        private const string CardTitleTextName = "Card Title Text";
        private const string CardTraitsTextName = "Card Traits Text";
        private const string CardStatsTextName = "Card Stats Text";
        private const string CardStateTextName = "Card State Text";

        [SerializeField] private Transform deckListRoot;

        public Transform DeckListRoot => deckListRoot;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void RenderDeckEntries(IReadOnlyList<DeckEntryViewData> deckEntries, Action<string> onCardSelected)
        {
            if (deckListRoot == null)
            {
                return;
            }

            Transform templateTransform = deckListRoot.Find(DeckTemplateName);
            if (templateTransform == null)
            {
                return;
            }

            templateTransform.gameObject.SetActive(false);
            ClearGeneratedEntries(deckListRoot, templateTransform);

            if (deckEntries == null)
            {
                return;
            }

            for (int entryIndex = 0; entryIndex < deckEntries.Count; entryIndex++)
            {
                DeckEntryViewData entry = deckEntries[entryIndex];
                GameObject instance = Instantiate(templateTransform.gameObject, deckListRoot);
                instance.name = $"{templateTransform.name} (Generated {entryIndex})";

                ApplyText(instance.transform, CardTitleTextName, entry?.TitleText ?? "-");
                ApplyText(instance.transform, CardTraitsTextName, entry?.TraitsText ?? "-");
                ApplyText(instance.transform, CardStatsTextName, entry?.StatsText ?? "-");
                ApplyText(instance.transform, CardStateTextName, entry?.StateText ?? "-");
                BindButton(instance, entry, onCardSelected);
                instance.SetActive(true);
            }
        }

        private static void BindButton(GameObject instance, DeckEntryViewData entry, Action<string> onCardSelected)
        {
            Button button = instance.GetComponent<Button>();
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.interactable = entry != null && entry.IsInteractable;

            if (entry != null && entry.IsInteractable && !string.IsNullOrWhiteSpace(entry.CardInstanceId) && onCardSelected != null)
            {
                string cardInstanceId = entry.CardInstanceId;
                button.onClick.AddListener(() => onCardSelected(cardInstanceId));
            }
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
