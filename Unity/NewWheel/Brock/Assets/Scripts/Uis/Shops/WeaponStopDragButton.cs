using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStopDragButton : MonoBehaviour
{
    [SerializeField, AssignedInCode]
    private int slotId;

    [SerializeField, Required]
    private Button button;

    public void SetSlotId(int slotId)
    {
        if (slotId < 0)
        {
            Debug.LogError("Slot ID cannot be negative.");
            return;
        }

        this.slotId = slotId;
    }

    public void DragHere()
    {
        if (!WeaponUiManager.Instance.IsDragging())
        {
            Debug.LogError("Should be dragging a weapon.");
            return;
        }

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.SwapWeapon(WeaponUiManager.Instance.GetDraggingSlotId(), slotId);
        WeaponUiManager.Instance.ClearDragging();
    }

    void Start()
    {
        string buttonText;
        if (HasWeaponSuit())
        {
            buttonText = "Swap";
        }
        else
        {
            buttonText = "Move Here";
        }

        button.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
    }

    private bool HasWeaponSuit()
    {
        return !ConfigDb.Instance.weaponInventory.IsSlotEmpty(slotId);
    }
}
