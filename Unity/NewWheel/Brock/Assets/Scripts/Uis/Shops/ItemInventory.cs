using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemInventory
{
    [SerializeField]
    private ShopItemDb shopItemDb;

    [SerializeField]
    private List<ItemConfig> ownedItems;

    public ItemInventory(ShopItemDb shopItemDb)
    {
        this.shopItemDb = shopItemDb;
        this.ownedItems = new List<ItemConfig>();
    }

    public bool TryAdd(string id)
    {
        ShopItem shopItem = this.shopItemDb.GetShopItem(id);
        return TryAdd(shopItem);
    }

    public bool TryAdd(ShopItem shopItem)
    {
        if (shopItem == null || !shopItem.IsItem())
        {
            Debug.LogError($"Cannot find item: {shopItem?.displayName}");
            return false;
        }

        this.ownedItems.Add(shopItem.itemConfig);
        return true;
    }
}
