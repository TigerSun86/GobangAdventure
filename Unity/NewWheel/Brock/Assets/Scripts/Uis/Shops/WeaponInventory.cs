using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponInventory
{
    [SerializeField]
    private ShopItemDb shopItemDb;

    private Dictionary<int, ShopItem> slotIdToShopItem;

    private int maxStorage = 1;

    public WeaponInventory(ShopItemDb shopItemDb)
    {
        this.shopItemDb = shopItemDb;
        this.slotIdToShopItem = new Dictionary<int, ShopItem>();
    }

    public void SetMaxStorage(int maxStorage)
    {
        this.maxStorage = maxStorage;
    }

    public bool TryAdd(string id)
    {
        ShopItem shopItem = this.shopItemDb.GetShopItem(id);
        return TryAdd(shopItem);
    }

    public bool TryAdd(ShopItem shopItem)
    {
        if (this.slotIdToShopItem.Count >= maxStorage)
        {
            return false;
        }

        if (shopItem == null || !shopItem.IsWeapon())
        {
            Debug.LogError($"Cannot find weapon: {shopItem?.displayName}");
            return false;
        }

        for (int slotId = 0; slotId < this.maxStorage; slotId++)
        {
            if (!this.slotIdToShopItem.ContainsKey(slotId))
            {
                this.slotIdToShopItem[slotId] = shopItem;
                return true;
            }
        }

        Debug.LogError("Should not reach here.");
        return false;
    }

    public ShopItem Get(int slotId)
    {
        if (!this.slotIdToShopItem.ContainsKey(slotId))
        {
            return null;
        }

        return this.slotIdToShopItem[slotId];
    }

    public void Swap(int sourceSlotId, int targetSlotId)
    {
        if (this.slotIdToShopItem.ContainsKey(sourceSlotId) && this.slotIdToShopItem.ContainsKey(targetSlotId))
        {
            ShopItem temp = this.slotIdToShopItem[sourceSlotId];
            this.slotIdToShopItem[sourceSlotId] = this.slotIdToShopItem[targetSlotId];
            this.slotIdToShopItem[targetSlotId] = temp;
        }
        else if (this.slotIdToShopItem.ContainsKey(sourceSlotId))
        {
            this.slotIdToShopItem[targetSlotId] = this.slotIdToShopItem[sourceSlotId];
            this.slotIdToShopItem.Remove(sourceSlotId);
        }
        else if (this.slotIdToShopItem.ContainsKey(targetSlotId))
        {
            this.slotIdToShopItem[sourceSlotId] = this.slotIdToShopItem[targetSlotId];
            this.slotIdToShopItem.Remove(targetSlotId);
        }
    }
}
