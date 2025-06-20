using UnityEngine;

public class ShopUi : MonoBehaviour
{
    [SerializeField] ItemDb itemDb;
    [SerializeField] GameObject itemUiPrefab;
    [SerializeField] Player playerUi;

    private PlayerShopItemManager playerShopItemManager;

    void Start()
    {
        this.playerShopItemManager = PlayerShopItemManager.Instance;
        this.playerShopItemManager.InitializeIfNeeded();

        //Fill the shop's UI list with items
        GenerateShopItemsUI();
    }

    private void SetItemUi(ItemUi itemUi, ShopItem item)
    {
        itemUi.gameObject.name = "Item" + "-" + item.name;

        itemUi.SetImage(item.image);
        itemUi.SetName(item.displayName);
        itemUi.SetCategory(item.level, item.weaponConfig2.weaponBaseType.ToString());
        itemUi.SetSkills(item.weaponConfig2.GetSkills());
        itemUi.SetPrice(item.price);

        itemUi.OnItemPurchase(item.displayName, OnItemPurchased);
    }

    void GenerateShopItemsUI()
    {
        for (int i = 0; i < 4; i++)
        {
            ShopItem item = this.playerShopItemManager.GetShopItemDb().GetRandomShopItem();
            ItemUi itemUi = Instantiate(itemUiPrefab, transform).GetComponent<ItemUi>();
            SetItemUi(itemUi, item);
        }
    }

    void OnItemPurchased(string itemName)
    {
        if (!this.playerShopItemManager.TryAdd(itemName))
        {
            // Weapon slots are full.
            return;
        }

        playerUi.InitializeWeapons();
        itemDb.DecreaseCountToBuy();
        if (itemDb.CountToBuy > 0)
        {
            foreach (ItemUi itemUi in GetComponentsInChildren<ItemUi>())
            {
                Destroy(itemUi.gameObject);
            }
            GenerateShopItemsUI();
        }
        else
        {
            foreach (ItemUi itemUi in GetComponentsInChildren<ItemUi>())
            {
                itemUi.DisablePurchaseButton();
            }
        }
    }
}