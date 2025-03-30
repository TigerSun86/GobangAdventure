using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "OnlyOneOfEach/ItemDb")]
public class ItemDb : ScriptableObject
{
    [SerializeField] public ShopItem[] shopItems;

    [SerializeField] UnityEvent countToBuyChangeEvent;

    public List<string> playerItemNames = new List<string>();

    private float countToBuy;

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
        this.countToBuy = 2;
    }

    public void OnDisable()
    {
        this.playerItemNames.Clear();
        this.countToBuy = 2;
    }
}
