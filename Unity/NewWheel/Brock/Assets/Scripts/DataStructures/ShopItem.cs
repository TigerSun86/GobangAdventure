using System;
using UnityEngine;

[Serializable]
public class ShopItem
{
    public Sprite image;

    public string displayName;

    public int level;

    public int price;

    public WeaponConfig weaponConfig;

    public ItemConfig itemConfig;

    public static bool AreSameName(ShopItem a, ShopItem b)
    {
        if (a == null || b == null)
        {
            return false;
        }

        return a.weaponConfig.weaponName == b.weaponConfig.weaponName;
    }

    public bool IsWeapon()
    {
        return weaponConfig != null;
    }

    public bool IsItem()
    {
        return itemConfig != null;
    }
}