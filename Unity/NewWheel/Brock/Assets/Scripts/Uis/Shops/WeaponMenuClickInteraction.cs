using UnityEngine;
using UnityEngine.UI;

public class WeaponMenuClickInteraction : MonoBehaviour
{
    public GameObject menuPrefab;
    private GameObject menuInstance;

    void OnMouseEnter()
    {
        if (menuInstance == null)
        {
            menuInstance = Instantiate(menuPrefab, gameObject.transform.position, Quaternion.identity);
            // Hook up button dynamically
            Button moveButton = menuInstance.transform.Find("Panel").Find("MoveButton").GetComponent<Button>();
            Draggable draggable = GetComponent<Draggable>();
            if (moveButton != null && draggable != null)
            {
                moveButton.onClick.AddListener(draggable.StartDragging);
            }
        }
    }


    void OnMouseExit()
    {
        if (menuInstance != null)
        {
            Destroy(menuInstance);
        }
    }
}
