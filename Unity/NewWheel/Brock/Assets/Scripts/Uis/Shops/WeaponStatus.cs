using System;
using UnityEngine;

[Serializable]
public class WeaponStatus
{
    private ShopItemDb shopItemDb;

    [SerializeField, AssignedInCode]
    private ShopItem shopItem;

    [SerializeField, AssignedInCode]
    private int slotId;

    [SerializeField, AssignedInCode]
    private int currentExperience;

    public static bool AreUpgradable(WeaponStatus weapon, WeaponStatus expendable)
    {
        if (weapon == null || expendable == null)
        {
            return false;
        }

        if (!ShopItem.AreSameName(weapon.shopItem, expendable.shopItem))
        {
            return false;
        }

        if (!weapon.HasNextLevel() || !expendable.HasNextLevel())
        {
            return false;
        }

        return true;
    }

    public WeaponStatus(ShopItemDb shopItemDb, ShopItem shopItem, int slotId = -1)
    {
        this.shopItemDb = shopItemDb;
        this.shopItem = shopItem;
        this.slotId = slotId;
        this.currentExperience = this.shopItem.weaponConfig.experienceWorth;
    }

    public ShopItem GetShopItem()
    {
        return this.shopItem;
    }

    public void SetSlotId(int slotId)
    {
        this.slotId = slotId;
    }

    public int GetSlotId()
    {
        return this.slotId;
    }

    public int GetCurrentExperience()
    {
        return this.currentExperience;
    }

    public bool HasNextLevel()
    {
        return this.shopItem.weaponConfig.experienceToNextLevel > 0;
    }

    public void Upgrade(WeaponStatus expendable)
    {
        if (!AreUpgradable(this, expendable))
        {
            Debug.LogError($"Cannot upgrade weapon: {this.shopItem.displayName} using {expendable?.shopItem.displayName}");
            return;
        }

        this.currentExperience += expendable.currentExperience;
        while (HasNextLevel()
            && this.currentExperience >= this.shopItem.weaponConfig.experienceToNextLevel)
        {
            ShopItem nextLevel = this.shopItemDb.GetNextLevel(this.shopItem);
            if (nextLevel == null)
            {
                Debug.LogError($"There is no next level for : {this.shopItem.displayName}");
                return;
            }

            this.shopItem = nextLevel;
        }
    }
}