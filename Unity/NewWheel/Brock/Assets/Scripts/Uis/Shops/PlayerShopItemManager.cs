using System.Collections.Generic;
using UnityEngine;

public class PlayerShopItemManager : MonoBehaviour
{
    public static PlayerShopItemManager Instance { get; private set; }

    private ShopItemDb shopItemDb;

    private Dictionary<int, ShopItem> slotIdToShopItem;

    private bool isInitialized = false;

    private int maxStorage = 1;

    public void InitializeIfNeeded()
    {
        if (isInitialized)
        {
            return;
        }

        isInitialized = true;
        this.shopItemDb = new ShopItemDb();
        this.slotIdToShopItem = new Dictionary<int, ShopItem>();
    }

    public ShopItemDb GetShopItemDb()
    {
        return shopItemDb;
    }

    public void SetMaxStorage(int maxStorage)
    {
        this.maxStorage = maxStorage;
    }

    public bool TryAdd(string itemName)
    {
        if (slotIdToShopItem.Count >= maxStorage)
        {
            return false;
        }

        for (int slotId = 0; slotId < this.maxStorage; slotId++)
        {
            if (!slotIdToShopItem.ContainsKey(slotId))
            {
                slotIdToShopItem[slotId] = shopItemDb.GetShopItem(itemName);
                return true;
            }
        }

        Debug.LogError("Should not reach here.");
        return false;
    }

    public ShopItem Get(int slotId)
    {
        if (!slotIdToShopItem.ContainsKey(slotId))
        {
            return null;
        }

        return slotIdToShopItem[slotId];
    }

    public void Swap(int sourceSlotId, int targetSlotId)
    {
        if (slotIdToShopItem.ContainsKey(sourceSlotId) && slotIdToShopItem.ContainsKey(targetSlotId))
        {
            ShopItem temp = slotIdToShopItem[sourceSlotId];
            slotIdToShopItem[sourceSlotId] = slotIdToShopItem[targetSlotId];
            slotIdToShopItem[targetSlotId] = temp;
        }
        else if (slotIdToShopItem.ContainsKey(sourceSlotId))
        {
            slotIdToShopItem[targetSlotId] = slotIdToShopItem[sourceSlotId];
            slotIdToShopItem.Remove(sourceSlotId);
        }
        else if (slotIdToShopItem.ContainsKey(targetSlotId))
        {
            slotIdToShopItem[sourceSlotId] = slotIdToShopItem[targetSlotId];
            slotIdToShopItem.Remove(targetSlotId);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}
