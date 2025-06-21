using System.Collections.Generic;
using UnityEngine;

public class ShopItemDb
{
    private List<ShopItem> shopItems;

    public ShopItemDb()
    {
        this.shopItems = new List<ShopItem>();
        foreach (WeaponConfig2 weaponConfig in ConfigDb.Instance.weaponConfigDb.GetAll())
        {
            ShopItem shopItem = WeaponConfigToShopItem(weaponConfig);
            this.shopItems.Add(shopItem);
        }
    }

    public ShopItem GetShopItem(string itemName)
    {
        foreach (ShopItem shopItem in shopItems)
        {
            if (shopItem.displayName == itemName)
            {
                return shopItem;
            }
        }

        Debug.LogError($"Item not found: {itemName}");
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

    private ShopItem WeaponConfigToShopItem(WeaponConfig2 weaponConfig)
    {
        ShopItem shopItem = ScriptableObject.CreateInstance<ShopItem>();
        shopItem.image = weaponConfig.sprite;
        shopItem.displayName = weaponConfig.GetId();
        shopItem.level = weaponConfig.level;
        shopItem.price = weaponConfig.price;
        shopItem.weaponConfig2 = weaponConfig;
        return shopItem;
    }
}
