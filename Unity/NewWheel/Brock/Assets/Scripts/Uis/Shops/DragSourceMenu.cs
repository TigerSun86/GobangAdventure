using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSourceMenu : MonoBehaviour
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

        moveButton.onClick.AddListener(() =>
        {
            WeaponUiManager.Instance.UnhideSelectables();
            StartDragging();
            Destroy(gameObject);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void StartDragging()
    {
        if (WeaponUiManager.Instance.currentlyDragging != null)
        {
            throw new Exception("Already dragging a weapon.");
        }

        WeaponUiManager.Instance.currentlyDragging = weaponSlot.GetWeaponSuit();
    }
}
