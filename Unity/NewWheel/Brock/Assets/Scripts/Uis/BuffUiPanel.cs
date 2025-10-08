using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BuffTracker))]
public class BuffUiPanel : MonoBehaviour
{
    [SerializeField, Required]
    private BuffTypeToSpriteDictionary images;

    [SerializeField, Required]
    private GameObject uiPrefab;

    private BuffTracker buffTracker;

    private List<(Buff buff, GameObject instance)> buffInstances;

    private void Start()
    {
        this.buffTracker = GetComponent<BuffTracker>();
        this.buffInstances = new List<(Buff, GameObject)>();
    }

    private void FixedUpdate()
    {
        List<Buff> buffs = this.buffTracker.GetAll().ToList();
        bool changed = false;

        // Remove instances for buffs that no longer exist
        for (int i = this.buffInstances.Count - 1; i >= 0; i--)
        {
            if (!buffs.Contains(this.buffInstances[i].buff))
            {
                Destroy(this.buffInstances[i].instance);
                this.buffInstances.RemoveAt(i);
                changed = true;
            }
        }

        // Add instances for new buffs
        foreach (Buff buff in buffs)
        {
            if (buff.invisible)
            {
                continue;
            }

            if (!this.buffInstances.Any(bi => bi.buff == buff))
            {
                if (!this.images.TryGetValue(buff.buffType, out Sprite sprite))
                {
                    Debug.LogWarning($"No sprite found for buff type: {buff.buffType}");
                    continue;
                }

                GameObject uiInstance = Instantiate(uiPrefab, this.transform);
                uiInstance.name = $"Buff_{buff.buffType}";
                uiInstance.GetComponent<SpriteRenderer>().sprite = sprite;
                this.buffInstances.Add((buff, uiInstance));
                changed = true;
            }
        }

        if (!changed)
        {
            return;
        }

        ArrangePositionOfUiInstances();
    }

    private void ArrangePositionOfUiInstances()
    {
        if (this.buffInstances.Count == 0)
        {
            return;
        }

        Renderer parentRenderer = GetComponentInChildren<WeaponStand>().GetComponent<Renderer>();
        if (parentRenderer == null)
        {
            return;
        }

        Bounds parentBounds = parentRenderer.bounds;
        float leftBound = parentBounds.min.x;
        float rightBound = parentBounds.max.x;
        float startY = parentBounds.min.y;

        float gap = 0f;
        float rowY = startY - (this.buffInstances[0].instance.GetComponent<Renderer>()?.bounds.size.y ?? 1f) / 2;
        float currentX = leftBound;
        float maxHeightInRow = 0f;

        foreach ((Buff buff, GameObject uiInstance) in this.buffInstances)
        {
            Renderer uiRenderer = uiInstance.GetComponent<Renderer>();
            if (uiRenderer == null)
            {
                continue;
            }

            float width = uiRenderer.bounds.size.x;
            float height = uiRenderer.bounds.size.y;
            maxHeightInRow = Mathf.Max(maxHeightInRow, height);

            if (currentX + width > rightBound)
            {
                currentX = leftBound;
                rowY -= maxHeightInRow + gap;
                maxHeightInRow = height;
            }

            Vector3 newPosition = new Vector3(currentX + width / 2, rowY, uiInstance.transform.position.z);
            uiInstance.transform.position = newPosition;

            currentX += width + gap;
        }
    }
}
