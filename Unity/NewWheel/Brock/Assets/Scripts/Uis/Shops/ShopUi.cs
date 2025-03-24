using UnityEngine;

public class ShopUi : MonoBehaviour
{
    [SerializeField] ItemDb itemDb;
    [SerializeField] GameObject itemUiPrefab;


    void Awake()
    {
        //Fill the shop's UI list with items
        GenerateShopItemsUI();
    }

    private void SetItemUi(ItemUi itemUi, ShopItem item)
    {
        itemUi.gameObject.name = "Item" + "-" + item.name;

        itemUi.SetImage(item.image);
        itemUi.SetName(item.displayName);
        itemUi.SetCategory(item.level, item.weaponBaseType.ToString());
        itemUi.SetSkills(item.skills);
        itemUi.SetPrice(item.price);

        itemUi.OnItemPurchase(item.name, OnItemPurchased);
    }

    void GenerateShopItemsUI()
    {
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = Random.Range(0, itemDb.shopItems.Length);
            ShopItem item = itemDb.shopItems[randomIndex];
            ItemUi itemUi = Instantiate(itemUiPrefab, transform).GetComponent<ItemUi>();
            SetItemUi(itemUi, item);
        }
    }

    void OnItemPurchased(string itemName)
    {
        itemDb.playerItemNames.Add(itemName);
    }
}