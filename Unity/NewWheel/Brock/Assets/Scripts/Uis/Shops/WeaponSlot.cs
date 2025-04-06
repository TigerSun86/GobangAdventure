using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField, Required] public GameObject weaponSlotUiPrefab;

    void Start()
    {
        Instantiate(weaponSlotUiPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
    }
}
