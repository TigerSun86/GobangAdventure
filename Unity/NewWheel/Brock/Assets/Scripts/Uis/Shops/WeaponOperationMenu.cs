using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponOperationMenu : MonoBehaviour
{
    public bool IsEnabled { get; private set; }

    public void Enable()
    {
        IsEnabled = true;

        Enable(true);

        HideOtherMenus();
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

        Enable(false);
    }

    public void HideOtherMenus()
    {
        WeaponUiManager.Instance.HideAllOtherSelectables(GetComponentsInChildren<Button>());
    }

    public void UndoHideOtherMenus()
    {
        WeaponUiManager.Instance.UnhideSelectables();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Disable();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Enable(bool enable)
    {
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            button.interactable = enable;
        }
    }
}
