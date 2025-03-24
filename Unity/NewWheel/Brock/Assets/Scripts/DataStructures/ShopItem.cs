using UnityEngine;

[CreateAssetMenu(menuName = "ShopItem")]
public class ShopItem : ScriptableObject
{
    public Sprite image;

    public string displayName;

    public WeaponBaseType weaponBaseType;

    public int level;

    public int price;

    public SkillConfig[] skills;
}