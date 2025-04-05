using UnityEngine;

public class WeaponMenuClickInteraction : MonoBehaviour
{
    public GameObject menu;
    private GameObject currentMenuInstance;

    void OnMouseEnter()
    {
        if (currentMenuInstance == null)
        {
            currentMenuInstance = Instantiate(menu, gameObject.transform.position, Quaternion.identity);
        }
    }


    void OnMouseExit()
    {
        if (currentMenuInstance != null)
        {
            Destroy(currentMenuInstance);
        }
    }
}
