using UnityEngine;
using UnityEngine.UI;

public class WeaponStopDragUpgradeButton : MonoBehaviour
{
    [SerializeField, AssignedInCode]
    private int slotId;

    [SerializeField, Required]
    private Button button;

    private WeaponInventory weaponInventory;

    public void SetSlotId(int slotId)
    {
        if (slotId < 0)
        {
            Debug.LogError("Slot ID cannot be negative.");
            return;
        }

        this.slotId = slotId;
    }

    public void Upgrade()
    {
        if (!WeaponUiManager.Instance.IsDragging())
        {
            Debug.LogError("Should be dragging a weapon.");
            return;
        }

        this.weaponInventory.Upgrade(this.slotId, WeaponUiManager.Instance.GetDraggingSlotId());

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.RefreshWeapons();
        WeaponUiManager.Instance.ClearDragging();
    }

    void Start()
    {
        if (!WeaponUiManager.Instance.IsDragging())
        {
            Debug.LogError("Should be dragging a weapon.");
            return;
        }

        this.weaponInventory = ConfigDb.Instance.weaponInventory;

        WeaponStatus upgradingWeapon = this.weaponInventory.GetBySlotId(slotId);
        if (upgradingWeapon != null)
        {
            WeaponStatus expendable = this.weaponInventory.GetBySlotId(WeaponUiManager.Instance.GetDraggingSlotId());
            if (WeaponStatus.AreUpgradable(expendable, upgradingWeapon))
            {
                return;
            }
        }

        // Disable the button if not upgradable
        this.button.gameObject.SetActive(false);
    }
}
