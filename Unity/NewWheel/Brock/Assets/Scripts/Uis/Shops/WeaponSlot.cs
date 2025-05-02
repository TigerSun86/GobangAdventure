using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField, Required] public GameObject weaponSlotUiPrefab;
    [SerializeField, AssignedInCode] public GameObject weaponSlotUiInstance;

    void Start()
    {
        weaponSlotUiInstance = Instantiate(weaponSlotUiPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        weaponSlotUiInstance.SetActive(false);
    }

    void Update()
    {
        if (GetComponentInChildren<WeaponSuit>() != null)
        {
            weaponSlotUiInstance.SetActive(true);
        }
        else
        {
            weaponSlotUiInstance.SetActive(false);
        }
    }
}
