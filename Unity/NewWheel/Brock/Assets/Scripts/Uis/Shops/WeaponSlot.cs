using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField, Required] public GameObject weaponSlotUiPrefab;
    [SerializeField, AssignedInCode] public GameObject weaponSlotUiInstance;

    public WeaponSuit GetWeaponSuit()
    {
        return GetComponentInChildren<WeaponSuit>();
    }

    void Start()
    {
        weaponSlotUiInstance = Instantiate(weaponSlotUiPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        weaponSlotUiInstance.SetActive(false);
    }

    void Update()
    {
        if (GetWeaponSuit() != null || WeaponUiManager.Instance.currentlyDragging != null)
        {
            weaponSlotUiInstance.SetActive(true);
        }
        else
        {
            weaponSlotUiInstance.SetActive(false);
        }
    }
}
