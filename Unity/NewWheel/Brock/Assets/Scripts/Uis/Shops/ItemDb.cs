using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOneOfEach/ItemDb")]
public class ItemDb : ScriptableObject
{
    [SerializeField] public ShopItem[] shopItems;

    public List<string> playerItemNames = new List<string>();

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


    public void OnEnable()
    {
        this.playerItemNames.Clear();
    }

    public void OnDisable()
    {
        this.playerItemNames.Clear();
    }
}
