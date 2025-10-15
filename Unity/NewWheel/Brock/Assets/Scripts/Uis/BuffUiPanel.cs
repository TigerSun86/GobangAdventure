using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BuffTracker))]
public class BuffUiPanel : MonoBehaviour
{
    [SerializeField, Required]
    private GameObject uiPrefab;

    private ModifierContainer modifierContainer;

    private List<(Modifier buff, GameObject instance)> buffUiInstances;

    private bool isDirty;

    public void NotifyDirty()
    {
        this.isDirty = true;
    }

    private void Start()
    {
        this.modifierContainer = GetComponent<ModifierContainer>();
        this.buffUiInstances = new List<(Modifier, GameObject)>();
        this.isDirty = true;
    }

    private void FixedUpdate()
    {
        if (!this.isDirty)
        {
            return;
        }

        this.isDirty = false;

        List<Modifier> buffs = this.modifierContainer.GetAllModifiers()
            .Where(m => m.config.buffType != BuffType.None)
            .ToList();

        // Remove instances for buffs that no longer exist
        for (int i = this.buffUiInstances.Count - 1; i >= 0; i--)
        {
            if (!buffs.Contains(this.buffUiInstances[i].buff))
            {
                Destroy(this.buffUiInstances[i].instance);
                this.buffUiInstances.RemoveAt(i);
            }
        }

        // Add instances for new buffs
        foreach (Modifier buff in buffs)
        {
            if (!this.buffUiInstances.Any(bi => bi.buff == buff))
            {
                GameObject uiInstance = Instantiate(uiPrefab, this.transform);
                uiInstance.name = $"Buff_{buff.config.buffType}";
                uiInstance.GetComponent<SpriteRenderer>().sprite = buff.config.buffIconSprite;
                this.buffUiInstances.Add((buff, uiInstance));
            }
        }

        ArrangePositionOfUiInstances();
    }

    private void ArrangePositionOfUiInstances()
    {
        if (this.buffUiInstances.Count == 0)
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
        float rowY = startY - (this.buffUiInstances[0].instance.GetComponent<Renderer>()?.bounds.size.y ?? 1f) / 2;
        float currentX = leftBound;
        float maxHeightInRow = 0f;

        foreach ((Modifier buff, GameObject uiInstance) in this.buffUiInstances)
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
