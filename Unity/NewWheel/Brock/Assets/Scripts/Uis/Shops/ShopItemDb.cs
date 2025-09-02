using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ShopItemDb
{
    private List<ShopItem> shopItems;

    public ShopItemDb(WeaponConfigDb weaponConfigDb, ItemConfigDb itemConfigDb)
    {
        this.shopItems = new List<ShopItem>();
        HashSet<string> names = new HashSet<string>();
        foreach (WeaponConfig weaponConfig in weaponConfigDb.GetAll())
        {
            if (names.Contains(weaponConfig.GetId()))
            {
                Debug.LogError($"Duplicate weapon name: {weaponConfig.GetId()}");
                continue;
            }

            ShopItem shopItem = WeaponConfigToShopItem(weaponConfig);
            this.shopItems.Add(shopItem);
        }

        foreach (ItemConfig itemConfig in itemConfigDb.GetAll())
        {
            if (names.Contains(itemConfig.GetId()))
            {
                Debug.LogError($"Duplicate item name: {itemConfig.GetId()}");
                continue;
            }

            ShopItem shopItem = ItemConfigToShopItem(itemConfig);
            this.shopItems.Add(shopItem);
        }
    }

    public ShopItem GetShopItem(string id)
    {
        foreach (ShopItem shopItem in shopItems)
        {
            if (shopItem.displayName == id)
            {
                return shopItem;
            }
        }

        Debug.LogError($"Item not found: {id}");
        return null;
    }

    public ShopItem GetRandomShopItem()
    {
        if (shopItems.Count == 0)
        {
            Debug.LogError("No shop items available.");
            return null;
        }

        int randomIndex = Random.Range(0, shopItems.Count);
        return shopItems[randomIndex];
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
