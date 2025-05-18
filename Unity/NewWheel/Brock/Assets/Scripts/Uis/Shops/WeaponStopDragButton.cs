using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStopDragButton : MonoBehaviour
{
    [AssignedInCode] public WeaponSlot weaponSlot;

    [SerializeField, Required] Button button;

    public void DragHere()
    {
        if (WeaponUiManager.Instance.currentlyDragging == null)
        {
            throw new Exception("Should be dragging a weapon.");
        }

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.SwapWeapon(WeaponUiManager.Instance.currentlyDragging, weaponSlot.gameObject);
        WeaponUiManager.Instance.currentlyDragging = null;
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
        return weaponSlot.GetWeaponSuit() != null;
    }
}
