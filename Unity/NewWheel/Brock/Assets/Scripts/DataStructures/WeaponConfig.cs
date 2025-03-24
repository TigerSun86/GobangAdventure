using UnityEngine;

[CreateAssetMenu(menuName = "WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    public WeaponBaseType weaponBaseType;

    public int health;

    public SkillConfig[] skills;
}