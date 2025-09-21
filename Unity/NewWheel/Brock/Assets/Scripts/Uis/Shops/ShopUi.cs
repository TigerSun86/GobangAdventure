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
        if (this.lootManager.GetWeaponCount() > 0)
        {
            GenerateWeaponPurchaseUI();
        }
        else if (this.lootManager.GetItemCount() > 0)
        {
            GenerateItemPurchaseUI();
        }
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

    private void GenerateWeaponPurchaseUI()
    {
        for (int i = 0; i < 4; i++)
        {
            ShopItem item = this.shopItemDb.GetRandomWeapon();
            ItemUi itemUi = Instantiate(itemUiPrefab, transform).GetComponent<ItemUi>();
            SetItemUi(itemUi, item);
        }
    }

    private void GenerateItemPurchaseUI()
    {
        for (int i = 0; i < 4; i++)
        {
            ShopItem item = this.shopItemDb.GetRandomItem();
            ItemUi itemUi = Instantiate(itemUiPrefab, transform).GetComponent<ItemUi>();
            SetItemUi(itemUi, item);
        }
    }

    private void OnItemPurchased(string id)
    {
        ShopItem item = this.shopItemDb.GetShopItem(id);
        if (item.IsWeapon())
        {
            if (!this.weaponInventory.TryAdd(item))
            {
                // Weapon slots are full. Keep the current shop UI so that user can sell some weapons first. 
                return;
            }

            this.lootManager.DecreaseWeaponCount();
        }
        else if (item.IsItem())
        {
            this.itemInventory.TryAdd(item);
            this.lootManager.DecreaseItemCount();
        }
        else
        {
            Debug.LogError($"Unknown item type: {id}");
            return;
        }

        this.playerUi.RefreshWeapons();

        if (this.lootManager.GetWeaponCount() > 0 || this.lootManager.GetItemCount() > 0)
        {
            // Can still buy more, destroy current items to make room to generate UI for the next purchase.
            foreach (ItemUi itemUi in GetComponentsInChildren<ItemUi>())
            {
                Destroy(itemUi.gameObject);
            }
        }
        else
        {
            // Cannot buy anymore, disable all purchase buttons.
            foreach (ItemUi itemUi in GetComponentsInChildren<ItemUi>())
            {
                itemUi.DisablePurchaseButton();
            }

            return;
        }

        if (this.lootManager.GetWeaponCount() > 0)
        {
            GenerateWeaponPurchaseUI();
        }
        else if (this.lootManager.GetItemCount() > 0)
        {
            GenerateItemPurchaseUI();
        }
        else
        {
            Debug.LogError($"Unknown item type");
        }
    }
}