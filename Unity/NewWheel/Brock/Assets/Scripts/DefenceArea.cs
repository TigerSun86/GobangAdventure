using UnityEngine;

[RequireComponent(typeof(Damagable))]
public class DefenceArea : MonoBehaviour
{
    [SerializeField] public WeaponBaseType weaponBaseType;

    GameObject character;

    [SerializeField]
    public Vector2 offset;

    [SerializeField] GameObject weaponPrefab;

    public Weapon weapon;

    public void SetWeapon(WeaponBaseType weaponBaseType, SkillConfig[] skills)
    {
        this.weaponBaseType = weaponBaseType;
        GameObject weaponObject = Instantiate(this.weaponPrefab, this.transform.position, Quaternion.identity, this.transform);
        this.weapon = weaponObject.GetComponent<Weapon>();
        this.weapon.weaponBaseType = weaponBaseType;
        this.weapon.SetSkill(skills);
    }

    public void SetCharacter(GameObject character)
    {
        this.character = character;
    }
}
