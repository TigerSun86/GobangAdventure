using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "OnlyOneOfEach/ItemDb")]
public class ItemDb : ScriptableObject
{
    [SerializeField] public ShopItem[] shopItems;

    [SerializeField] UnityEvent countToBuyChangeEvent;

    public List<string> playerItemNames = new List<string>();

    [SerializeField] private float countToBuy;

    private Dictionary<int, ShopItem> slotIdToShopItem = new Dictionary<int, ShopItem>();

    public int CountToBuy
    {
        get { return (int)countToBuy; }
    }

    public ShopItem GetShopItem(string itemName)
    {
        foreach (ShopItem shopItem in shopItems)
        {
            if (shopItem.name == itemName)
            {
                return shopItem;
            }
        }

        throw new System.Exception("Item not found");
    }

    public ShopItem GetShopItemBySlotId(int id)
    {
        if (!slotIdToShopItem.ContainsKey(id))
        {
            return null;
        }

        return slotIdToShopItem[id];
    }

    public void UpdateSlotIdToShopItem()
    {
        int id = 0;
        foreach (string itemName in playerItemNames)
        {
            ShopItem shopItem = GetShopItem(itemName);
            if (slotIdToShopItem.ContainsValue(shopItem))
            {
                continue;
            }

            while (slotIdToShopItem.ContainsKey(id))
            {
                id++;
            }

            slotIdToShopItem[id] = shopItem;
            id++;
        }
    }

    public void SwapSlotIdToShopItem(int sourceId, int targetId)
    {
        if (slotIdToShopItem.ContainsKey(sourceId) && slotIdToShopItem.ContainsKey(targetId))
        {
            ShopItem temp = slotIdToShopItem[sourceId];
            slotIdToShopItem[sourceId] = slotIdToShopItem[targetId];
            slotIdToShopItem[targetId] = temp;
        }
        else if (slotIdToShopItem.ContainsKey(sourceId))
        {
            slotIdToShopItem[targetId] = slotIdToShopItem[sourceId];
            slotIdToShopItem.Remove(sourceId);
        }
        else if (slotIdToShopItem.ContainsKey(targetId))
        {
            slotIdToShopItem[sourceId] = slotIdToShopItem[targetId];
            slotIdToShopItem.Remove(targetId);
        }
    }

    public void IncreaseCountToBuy()
    {
        this.countToBuy += 0.5f;
        this.countToBuyChangeEvent.Invoke();
    }

    public void DecreaseCountToBuy()
    {
        this.countToBuy -= 1;
        this.countToBuyChangeEvent.Invoke();
    }

    public void OnEnable()
    {
        this.playerItemNames.Clear();
        this.slotIdToShopItem.Clear();
        this.countToBuy = 2;
    }

    public void OnDisable()
    {
        this.playerItemNames.Clear();
        this.slotIdToShopItem.Clear();
        this.countToBuy = 2;
    }
}
