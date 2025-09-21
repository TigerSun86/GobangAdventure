using UnityEngine;

public class ShopUi : MonoBehaviour
{
    [SerializeField]
    private GameObject itemUiPrefab;

    [SerializeField]
    private Player playerUi;

    [SerializeField, AssignedInCode]
    private LootManager lootManager;

    [SerializeField, AssignedInCode]
    private ShopItemDb shopItemDb;

    [SerializeField, AssignedInCode]
    private ItemInventory itemInventory;

    [SerializeField, AssignedInCode]
    private WeaponInventory weaponInventory;

    void Start()
    {
        this.lootManager = LootManager.Instance;
        this.shopItemDb = ConfigDb.Instance.shopItemDb;
        this.itemInventory = ConfigDb.Instance.itemInventory;
        this.weaponInventory = ConfigDb.Instance.weaponInventory;

        //Fill the shop's UI list with items
        GenerateShopItemsUI();
    }

    private void SetItemUi(ItemUi itemUi, ShopItem item)
    {
        itemUi.gameObject.name = "Item" + "-" + item.displayName;

        itemUi.SetImage(item.image);
        itemUi.SetName(item.displayName);
        string catagory = item.IsWeapon() ? item.weaponConfig.weaponBaseType.ToString() : string.Empty;
        itemUi.SetCategory(item.level, catagory);
        if (item.IsWeapon())
        {
            itemUi.SetSkills(item.weaponConfig.GetSkills());
        }

        itemUi.SetPrice(item.price);

        itemUi.OnItemPurchase(item.displayName, OnItemPurchased);
    }

    void GenerateShopItemsUI()
    {
        for (int i = 0; i < 4; i++)
        {
            ShopItem item = this.shopItemDb.GetRandomShopItem();
            ItemUi itemUi = Instantiate(itemUiPrefab, transform).GetComponent<ItemUi>();
            SetItemUi(itemUi, item);
        }
    }

    void OnItemPurchased(string id)
    {
        ShopItem item = this.shopItemDb.GetShopItem(id);
        if (item.IsWeapon())
        {
            if (!this.weaponInventory.TryAdd(item))
            {
                // Weapon slots are full.
                return;
            }
        }
        else if (item.IsItem())
        {
            this.itemInventory.TryAdd(item);
        }
        else
        {
            Debug.LogError($"Unknown item type: {id}");
            return;
        }

        this.playerUi.RefreshWeapons();

        this.lootManager.DecreaseWeaponCount();
        if (this.lootManager.GetWeaponCount() > 0)
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