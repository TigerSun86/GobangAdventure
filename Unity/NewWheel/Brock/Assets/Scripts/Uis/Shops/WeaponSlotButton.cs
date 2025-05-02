using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlotButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField, Required] public GameObject menuPrefab;
    private GameObject menuInstance;

    public void OnSelect(BaseEventData eventData)
    {
        if (menuInstance == null && WeaponUiManager.Instance.currentlyDragging == null)
        {
            menuInstance = Instantiate(menuPrefab, transform.position, Quaternion.identity, transform.parent);
            DisableMenu();
            // Hook up button dynamically
            Button moveButton = menuInstance.transform.Find("Panel").Find("MoveButton").GetComponent<Button>();
            Draggable draggable = GetComponent<Draggable>();
            if (moveButton != null && draggable != null)
            {
                moveButton.onClick.AddListener(() => { draggable.StartDragging(); Destroy(menuInstance); });
            }
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (menuInstance != null)
        {
            Destroy(menuInstance);
        }
    }

    public void EnableMenu()
    {
        EnableMenu(true);
    }

    public void DisableMenu()
    {
        EnableMenu(false);
    }

    private void EnableMenu(bool enable)
    {
        if (menuInstance != null)
        {
            foreach (Button button in menuInstance.GetComponentsInChildren<Button>())
            {
                button.interactable = enable;
            }
        }
    }
}
