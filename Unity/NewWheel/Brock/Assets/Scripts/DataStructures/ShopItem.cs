using UnityEngine;

[CreateAssetMenu(menuName = "ShopItem")]
public class ShopItem : ScriptableObject
{
    public Sprite image;

    public string displayName;

    public WeaponBaseType weaponBaseType;

    public int level;

    [Range(0, 100)] public int attack;

    public int price;

    public SkillConfig[] skills;

    public override string ToString()
    {
        return $"{displayName},{weaponBaseType},{level},{attack},{price}";
    }
}