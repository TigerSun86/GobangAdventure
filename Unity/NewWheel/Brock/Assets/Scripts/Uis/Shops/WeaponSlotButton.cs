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
        dragSourceMenuInstance = Instantiate(dragSourceMenuPrefab, transform.position + new Vector3(0.4f, 0, 0), Quaternion.identity, transform.parent).GetComponent<WeaponOperationMenu>();
        dragSourceMenuInstance.GetComponentInChildren<WeaponStartDragButton>().SetSlotId(GetWeaponSlot().GetSlotId());
    }

    private void CreateDragTargetMenu()
    {
        dragTargetMenuInstance = Instantiate(dragTargetMenuPrefab, transform.position + new Vector3(0.4f, 0, 0), Quaternion.identity, transform.parent).GetComponent<WeaponOperationMenu>();
        dragTargetMenuInstance.GetComponentInChildren<WeaponStopDragButton>().SetSlotId(GetWeaponSlot().GetSlotId());
        dragTargetMenuInstance.GetComponentInChildren<WeaponStopDragUpgradeButton>().SetSlotId(GetWeaponSlot().GetSlotId());
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
}
