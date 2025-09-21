using System.Collections.Generic;
using UnityEngine;

public class ShopItemDb
{
    private List<ShopItem> weapons;

    private List<ShopItem> items;

    public ShopItemDb(WeaponConfigDb weaponConfigDb, ItemConfigDb itemConfigDb)
    {
        this.weapons = new List<ShopItem>();
        HashSet<string> names = new HashSet<string>();
        foreach (WeaponConfig weaponConfig in weaponConfigDb.GetAll())
        {
            if (names.Contains(weaponConfig.GetId()))
            {
                Debug.LogError($"Duplicate weapon name: {weaponConfig.GetId()}");
                continue;
            }

            ShopItem shopItem = WeaponConfigToShopItem(weaponConfig);
            this.weapons.Add(shopItem);
        }

        this.items = new List<ShopItem>();
        foreach (ItemConfig itemConfig in itemConfigDb.GetAll())
        {
            if (names.Contains(itemConfig.GetId()))
            {
                Debug.LogError($"Duplicate item name: {itemConfig.GetId()}");
                continue;
            }

            ShopItem shopItem = ItemConfigToShopItem(itemConfig);
            this.items.Add(shopItem);
        }
    }

    public ShopItem GetShopItem(string id)
    {
        foreach (ShopItem shopItem in this.weapons)
        {
            if (shopItem.displayName == id)
            {
                return shopItem;
            }
        }

        foreach (ShopItem shopItem in this.items)
        {
            if (shopItem.displayName == id)
            {
                return shopItem;
            }
        }

        Debug.LogError($"Item not found: {id}");
        return null;
    }

    public ShopItem GetRandomWeapon()
    {
        if (weapons.Count == 0)
        {
            Debug.LogError("No weapon available.");
            return null;
        }

        int randomIndex = Random.Range(0, weapons.Count);
        return weapons[randomIndex];
    }

    public ShopItem GetRandomItem()
    {
        if (this.items.Count == 0)
        {
            Debug.LogError("No item available.");
            return null;
        }

        int randomIndex = Random.Range(0, this.items.Count);
        return this.items[randomIndex];
    }

    private ShopItem WeaponConfigToShopItem(WeaponConfig weaponConfig)
    {
        ShopItem shopItem = new ShopItem();
        shopItem.image = weaponConfig.sprite;
        shopItem.displayName = weaponConfig.GetId();
        shopItem.level = weaponConfig.level;
        shopItem.price = weaponConfig.price;
        shopItem.weaponConfig = weaponConfig;
        return shopItem;
    }

    private ShopItem ItemConfigToShopItem(ItemConfig itemConfig)
    {
        ShopItem shopItem = new ShopItem();
        shopItem.image = itemConfig.sprite;
        shopItem.displayName = itemConfig.GetId();
        shopItem.level = itemConfig.level;
        shopItem.price = itemConfig.price;
        shopItem.itemConfig = itemConfig;
        return shopItem;
    }
}
