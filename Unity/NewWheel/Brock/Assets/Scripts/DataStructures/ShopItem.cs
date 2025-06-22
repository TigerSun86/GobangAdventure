using UnityEngine;

[CreateAssetMenu(menuName = "ShopItem")]
public class ShopItem : ScriptableObject
{
    public Sprite image;

    public string displayName;

    public int level;

    public int price;

    public WeaponConfig weaponConfig;
}