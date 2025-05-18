using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragTargetMenu : MonoBehaviour
{
    [AssignedInCode] public WeaponSlot weaponSlot;

    [SerializeField, Required] Button moveButton;

    public bool IsEnabled { get; private set; }

    public void Enable()
    {
        IsEnabled = true;

        moveButton.interactable = true;

        WeaponUiManager.Instance.HideAllOtherSelectables(GetComponentsInChildren<Button>());
        // Select the first button.
        Button firstButton = GetComponentsInChildren<Button>().FirstOrDefault();
        if (firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
    }

    public void Disable()
    {
        IsEnabled = false;

        moveButton.interactable = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Disable();

        string buttonText;
        if (HasWeaponSuit())
        {
            buttonText = "Swap";
        }
        else
        {
            buttonText = "Move Here";
        }

        moveButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        moveButton.onClick.AddListener(() =>
        {
            WeaponUiManager.Instance.UnhideSelectables();
            SwapHere();
            Destroy(gameObject);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool HasWeaponSuit()
    {
        return weaponSlot.GetWeaponSuit() != null;
    }

    private void SwapHere()
    {
        if (WeaponUiManager.Instance.currentlyDragging == null)
        {
            throw new Exception("Should be dragging a weapon.");
        }

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.SwapWeapon(WeaponUiManager.Instance.currentlyDragging, weaponSlot.gameObject);
        WeaponUiManager.Instance.currentlyDragging = null;
    }
}
