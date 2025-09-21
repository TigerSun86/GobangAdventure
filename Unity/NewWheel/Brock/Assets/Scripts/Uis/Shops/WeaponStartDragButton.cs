using UnityEngine;
using UnityEngine.UI;

public class WeaponStartDragButton : MonoBehaviour
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

    public void StartDrag()
    {
        if (WeaponUiManager.Instance.IsDragging())
        {
            Debug.LogError("Already dragging a weapon.");
            return;
        }

        WeaponUiManager.Instance.SetDraggingSlotId(this.slotId);
    }
}
