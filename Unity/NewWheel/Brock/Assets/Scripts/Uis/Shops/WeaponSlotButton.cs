using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlotButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField, Required] public GameObject dragSourceMenuPrefab;
    [SerializeField, Required] public GameObject dragTargetMenuPrefab;
    private WeaponOperationMenu dragSourceMenuInstance;
    private WeaponOperationMenu dragTargetMenuInstance;

    public void OnSelect(BaseEventData eventData)
    {
        if (WeaponUiManager.Instance.currentlyDragging == null)
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
            Destroy(dragSourceMenuInstance);
        }
        if (dragTargetMenuInstance != null && !dragTargetMenuInstance.IsEnabled)
        {
            Destroy(dragTargetMenuInstance);
        }
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { EnableMenu(); });
    }

    private void CreateDragSourceMenu()
    {
        dragSourceMenuInstance = Instantiate(dragSourceMenuPrefab, transform.position + new Vector3(0.4f, 0, 0), Quaternion.identity, transform.parent).GetComponent<WeaponOperationMenu>();
        dragSourceMenuInstance.GetComponentInChildren<WeaponStartDragButton>().weaponSlot = GetWeaponSlot();
    }

    private void CreateDragTargetMenu()
    {
        dragTargetMenuInstance = Instantiate(dragTargetMenuPrefab, transform.position + new Vector3(0.4f, 0, 0), Quaternion.identity, transform.parent).GetComponent<WeaponOperationMenu>();
        dragTargetMenuInstance.GetComponentInChildren<WeaponStopDragButton>().weaponSlot = GetWeaponSlot();
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
