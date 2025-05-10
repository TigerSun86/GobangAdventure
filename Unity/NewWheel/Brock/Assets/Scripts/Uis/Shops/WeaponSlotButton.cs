using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlotButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField, Required] public GameObject dragSourceMenuPrefab;
    [SerializeField, Required] public GameObject dragTargetMenuPrefab;
    private GameObject dragSourceMenuInstance;
    private GameObject dragTargetMenuInstance;

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
        if (dragSourceMenuInstance != null
            && dragSourceMenuInstance.GetComponentsInChildren<Button>().All(b => !b.IsInteractable()))
        {
            Destroy(dragSourceMenuInstance);
        }
        if (dragTargetMenuInstance != null
            && dragTargetMenuInstance.GetComponentsInChildren<Button>().All(b => !b.IsInteractable()))
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
        dragSourceMenuInstance = Instantiate(dragSourceMenuPrefab, transform.position + new Vector3(0.4f, 0, 0), Quaternion.identity, transform.parent);
        DisableMenu();
        Button moveButton = dragSourceMenuInstance.transform.Find("Panel").Find("MoveButton").GetComponent<Button>();
        if (moveButton != null)
        {
            moveButton.onClick.AddListener(() =>
            {
                WeaponUiManager.Instance.UnhideSelectables();
                StartDragging();
                Destroy(dragSourceMenuInstance);
            });
        }
    }

    private void CreateDragTargetMenu()
    {
        dragTargetMenuInstance = Instantiate(dragTargetMenuPrefab, transform.position + new Vector3(0.4f, 0, 0), Quaternion.identity, transform.parent);
        DisableMenu();
        Button moveHereButton = dragTargetMenuInstance.transform.Find("Panel").Find("MoveHereButton").GetComponent<Button>();
        if (moveHereButton != null)
        {
            moveHereButton.onClick.AddListener(() =>
            {
                WeaponUiManager.Instance.UnhideSelectables();
                MoveHere();
                Destroy(dragTargetMenuInstance);
            });
        }
    }

    private WeaponSlot GetWeaponSlot()
    {
        // WeaponSlot -> WeaponSlotUi ->WeaponSlotButton
        return transform.parent.GetComponentInParent<WeaponSlot>();
    }

    private void StartDragging()
    {
        if (WeaponUiManager.Instance.currentlyDragging != null)
        {
            throw new Exception("Already dragging a weapon.");
        }

        WeaponUiManager.Instance.currentlyDragging = GetWeaponSlot().GetWeaponSuit();
    }

    private void MoveHere()
    {
        if (WeaponUiManager.Instance.currentlyDragging == null)
        {
            throw new Exception("Should be dragging a weapon.");
        }

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.MoveWeapon(WeaponUiManager.Instance.currentlyDragging, GetWeaponSlot().gameObject);
        WeaponUiManager.Instance.currentlyDragging = null;
    }

    private void EnableMenu()
    {
        EnableMenu(true);
    }

    private void DisableMenu()
    {
        EnableMenu(false);
    }

    private void EnableMenu(bool enable)
    {
        if (dragSourceMenuInstance != null)
        {
            foreach (Button button in dragSourceMenuInstance.GetComponentsInChildren<Button>())
            {
                button.interactable = enable;
            }

            if (enable)
            {
                WeaponUiManager.Instance.HideAllOtherSelectables(dragSourceMenuInstance.GetComponentsInChildren<Button>());
            }
        }
        else if (dragTargetMenuInstance != null)
        {
            foreach (Button button in dragTargetMenuInstance.GetComponentsInChildren<Button>())
            {
                button.interactable = enable;
            }

            if (enable)
            {
                WeaponUiManager.Instance.HideAllOtherSelectables(dragTargetMenuInstance.GetComponentsInChildren<Button>());
            }
        }
    }
}
