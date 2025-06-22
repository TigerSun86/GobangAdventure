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
}