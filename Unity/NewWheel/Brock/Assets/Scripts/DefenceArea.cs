using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damagable))]
public class DefenceArea : MonoBehaviour
{
    [SerializeField] public WeaponBaseType weaponBaseType;

    [SerializeField] public Vector2 offset;

    [SerializeField] GameObject weaponPrefab;

    public Weapon weapon;

    public void SetWeapon(WeaponConfig weaponConfig)
    {
        this.weaponBaseType = weaponConfig.weaponBaseType;
        GameObject weaponObject = Instantiate(this.weaponPrefab, this.transform.position, Quaternion.identity, this.transform);
        this.weapon = weaponObject.GetComponent<Weapon>();
        this.weapon.weaponBaseType = weaponBaseType;
        this.weapon.SetSkill(weaponConfig.skills);
        GetComponent<Health>().SetMaxHealth(weaponConfig.health);
    }
}
