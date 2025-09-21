using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Manages the player's weapon statuses.
[Serializable]
public class WeaponInventory
{
    [SerializeField, AssignedInCode]
    private List<WeaponStatus> weaponStatuses;

    [SerializeField, AssignedInCode]
    private int maxStorage;

    private ShopItemDb shopItemDb;

    public WeaponInventory(ShopItemDb shopItemDb)
    {
        this.weaponStatuses = new List<WeaponStatus>();
        SetMaxStorage(0);
        this.shopItemDb = shopItemDb;
    }

    public void SetMaxStorage(int maxStorage)
    {
        this.maxStorage = maxStorage;
    }

    public bool TryAdd(string id)
    {
        ShopItem shopItem = this.shopItemDb.GetShopItem(id);
        return TryAdd(shopItem);
    }

    public bool TryAdd(ShopItem shopItem)
    {
        if (this.weaponStatuses.Count >= maxStorage)
        {
            return false;
        }

        if (shopItem == null || !shopItem.IsWeapon())
        {
            Debug.LogError($"Cannot find weapon: {shopItem?.displayName}");
            return false;
        }

        for (int slotId = 0; slotId < this.maxStorage; slotId++)
        {
            if (IsSlotEmpty(slotId))
            {
                this.weaponStatuses.Add(new WeaponStatus(this.shopItemDb, shopItem, slotId));
                return true;
            }
        }

        Debug.LogError("Should not reach here.");
        return false;
    }

    public void Upgrade(int slotId, int expendableSlotId)
    {
        if (IsSlotEmpty(expendableSlotId))
        {
            Debug.LogError("Should not expend empty slot.");
            return;
        }

        WeaponStatus expendable = GetBySlotId(expendableSlotId);
        Upgrade(slotId, expendable);
        RemoveBySlotId(expendableSlotId);
    }

    public void Upgrade(int slotId, WeaponStatus expendable)
    {
        if (IsSlotEmpty(slotId))
        {
            Debug.LogError("Should not upgrade empty slot.");
            return;
        }

        WeaponStatus currentWeapon = GetBySlotId(slotId);
        currentWeapon.Upgrade(expendable);
    }

    public bool IsSlotEmpty(int slotId)
    {
        return GetBySlotId(slotId) == null;
    }

    public WeaponStatus GetBySlotId(int slotId)
    {
        return this.weaponStatuses.FirstOrDefault(ws => ws.GetSlotId() == slotId);
    }

    public void Swap(int sourceSlotId, int targetSlotId)
    {
        WeaponStatus sourceWeapon = GetBySlotId(sourceSlotId);
        WeaponStatus targetWeapon = GetBySlotId(targetSlotId);
        if (sourceWeapon != null)
        {
            sourceWeapon.SetSlotId(targetSlotId);
        }

        if (targetWeapon != null)
        {
            targetWeapon.SetSlotId(sourceSlotId);
        }
    }

    private void RemoveBySlotId(int expendableSlotId)
    {
        WeaponStatus expendable = GetBySlotId(expendableSlotId);
        if (expendable == null)
        {
            Debug.LogError("Should not remove empty slot.");
            return;
        }

        this.weaponStatuses.Remove(expendable);
    }
}
