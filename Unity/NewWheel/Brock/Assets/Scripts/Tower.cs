using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField, Required]
    private WeaponLayout weaponLayout;

    private void Start()
    {
        this.weaponLayout.SetWeaponConfig(0, ConfigDb.Instance.weaponConfigDb.Get("Tower Rock 1"));
        this.weaponLayout.SetWeaponConfig(1, ConfigDb.Instance.weaponConfigDb.Get("Tower Paper 1"));
        this.weaponLayout.SetWeaponConfig(2, ConfigDb.Instance.weaponConfigDb.Get("Tower Scissor 1"));
        this.weaponLayout.RefreshWeapons();
    }
}
