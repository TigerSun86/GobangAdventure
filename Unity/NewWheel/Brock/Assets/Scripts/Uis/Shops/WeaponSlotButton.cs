using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlotButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField, Required]
    private GameObject dragSourceMenuPrefab;

    [SerializeField, Required]
    private GameObject dragTargetMenuPrefab;

    private WeaponOperationMenu dragSourceMenuInstance;

    private WeaponOperationMenu dragTargetMenuInstance;

    public void OnSelect(BaseEventData eventData)
    {
        if (!WeaponUiManager.Instance.IsDragging())
        {
            CreateDragSourceMenu();
        }
        else
        {
            // A weapon is dragged currently.
            CreateDragTargetMenu();
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // Destroy the menu if the menu hasn't been enabled yet.
        if (dragSourceMenuInstance != null && !dragSourceMenuInstance.IsEnabled)
        {
            dragSourceMenuInstance.Destroy();
        }
        if (dragTargetMenuInstance != null && !dragTargetMenuInstance.IsEnabled)
        {
            dragTargetMenuInstance.Destroy();
        }
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { EnableMenu(); });
    }

    private void CreateDragSourceMenu()
    {
        GameObject menuObject = Instantiate(dragSourceMenuPrefab, this.transform.position, Quaternion.identity, this.transform.parent);
        menuObject.transform.position = GetSafeSpawnPosition(menuObject.transform.GetComponent<RectTransform>(), this.transform.position + new Vector3(0.8f, 0, 0));

        this.dragSourceMenuInstance = menuObject.GetComponent<WeaponOperationMenu>();
        this.dragSourceMenuInstance.GetComponentInChildren<WeaponStartDragButton>().SetSlotId(GetWeaponSlot().GetSlotId());
        this.dragSourceMenuInstance.GetComponentInChildren<WeaponStatsText>().SetSlotId(GetWeaponSlot().GetSlotId());
    }

    private void CreateDragTargetMenu()
    {
        GameObject menuObject = Instantiate(dragTargetMenuPrefab, this.transform.position, Quaternion.identity, this.transform.parent);
        menuObject.transform.position = GetSafeSpawnPosition(menuObject.transform.GetComponent<RectTransform>(), this.transform.position + new Vector3(0.8f, 0, 0));

        this.dragTargetMenuInstance = menuObject.GetComponent<WeaponOperationMenu>();
        this.dragTargetMenuInstance.GetComponentInChildren<WeaponStopDragButton>().SetSlotId(GetWeaponSlot().GetSlotId());
        this.dragTargetMenuInstance.GetComponentInChildren<WeaponStopDragUpgradeButton>().SetSlotId(GetWeaponSlot().GetSlotId());
        this.dragTargetMenuInstance.GetComponentInChildren<WeaponStatsText>().SetSlotId(GetWeaponSlot().GetSlotId());
    }

    private WeaponSlot GetWeaponSlot()
    {
        // WeaponSlot -> WeaponSlotUi ->WeaponSlotButton
        return transform.parent.GetComponentInParent<WeaponSlot>();
    }

    private void EnableMenu()
    {
        if (dragSourceMenuInstance != null)
        {
            dragSourceMenuInstance.Enable();
        }
        else if (dragTargetMenuInstance != null)
        {
            dragTargetMenuInstance.Enable();
        }
    }

    private Vector3 GetSafeSpawnPosition(RectTransform rectTransform, Vector3 desiredPosition)
    {
        Vector2 halfSize = new Vector2(rectTransform.rect.width / 2 + 0.5f, rectTransform.rect.height / 2 + 0.5f);

        // Get the world coordinates for the screen edges
        Vector3 screenMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 screenMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // Clamp the given position to fit within the screen bounds
        float clampedX = Mathf.Clamp(desiredPosition.x, screenMin.x + halfSize.x, screenMax.x - halfSize.x);
        float clampedY = Mathf.Clamp(desiredPosition.y, screenMin.y + halfSize.y, screenMax.y - halfSize.y);

        // Preserve the desired Z position
        return new Vector3(clampedX, clampedY, 0);
    }
}
