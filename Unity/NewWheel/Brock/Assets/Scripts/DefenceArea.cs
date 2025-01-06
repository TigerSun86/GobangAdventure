using UnityEngine;

[RequireComponent(typeof(Damagable))]
public class DefenceArea : MonoBehaviour
{
    [SerializeField] public WeaponBaseType weaponBaseType;

    GameObject character;

    [SerializeField]
    public Vector2 offset;

    public Weapon weapon;

    public void SetWeapon(GameObject weaponPrefab, ShopItem shopItem = null)
    {
        GameObject weaponObject = Instantiate(weaponPrefab, this.transform.position, Quaternion.identity, this.transform);
        this.weapon = weaponObject.GetComponent<Weapon>();
        this.weaponBaseType = this.weapon.weaponBaseType;
        this.weapon.shopItem = shopItem;
    }

    public void SetCharacter(GameObject character)
    {
        this.character = character;
    }
}
