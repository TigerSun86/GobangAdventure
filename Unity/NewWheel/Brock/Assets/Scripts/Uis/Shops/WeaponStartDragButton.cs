using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStartDragButton : MonoBehaviour
{
    [AssignedInCode] public WeaponSlot weaponSlot;

    [SerializeField, Required] Button button;

    public void StartDrag()
    {
        if (WeaponUiManager.Instance.currentlyDragging != null)
        {
            throw new Exception("Already dragging a weapon.");
        }

        WeaponUiManager.Instance.currentlyDragging = weaponSlot.GetWeaponSuit();
    }
}
