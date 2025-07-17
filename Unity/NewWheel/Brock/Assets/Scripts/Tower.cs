using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField, Required]
    private GameObject weaponSuitPrefab;

    private WeaponSuit weaponSuit;

    private void Start()
    {
        WeaponConfig weaponConfig = ConfigDb.Instance.weaponConfigDb.Get("Basic Rock 1");

        GameObject weaponSuitObject = Instantiate(this.weaponSuitPrefab, transform.position, Quaternion.identity, transform);
        weaponSuitObject.tag = "PlayerWeapon";
        this.weaponSuit = weaponSuitObject.GetComponent<WeaponSuit>();
        this.weaponSuit.Initialize(weaponConfig);
    }
}
