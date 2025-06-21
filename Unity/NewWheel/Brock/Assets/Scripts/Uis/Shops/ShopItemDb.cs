using System.Collections.Generic;
using UnityEngine;

public class ShopItemDb : MonoBehaviour
{
    [SerializeField, AssignedInCode] public List<ShopItem> shopItems;

    public ShopItem GetShopItem(string itemName)
    {
        foreach (ShopItem shopItem in shopItems)
        {
            if (shopItem.displayName == itemName)
            {
                return shopItem;
            }
        }

        Debug.LogError($"Item not found: {itemName}");
        return null;
    }

    private void Start()
    {
        this.shopItems = new List<ShopItem>();
        foreach (WeaponConfig2 weaponConfig in ConfigDb.Instance.weaponConfigDb.GetAll())
        {
            ShopItem shopItem = WeaponConfigToShopItem(weaponConfig);
            this.shopItems.Add(shopItem);
        }
    }

    private ShopItem WeaponConfigToShopItem(WeaponConfig2 weaponConfig)
    {
        ShopItem shopItem = ScriptableObject.CreateInstance<ShopItem>();
        shopItem.image = weaponConfig.sprite;
        shopItem.displayName = $"{weaponConfig.weaponName} {weaponConfig.level}";
        shopItem.level = weaponConfig.level;
        shopItem.price = weaponConfig.price;
        shopItem.weaponConfig2 = weaponConfig;
        return shopItem;
    }
}
