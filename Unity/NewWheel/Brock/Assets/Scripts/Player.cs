using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] GameObject weaponSlotPrefab;
    [SerializeField] GameObject weaponSuitPrefab;
    [SerializeField] GameObject[] weaponSuits;
    [SerializeField] float speed = 5f;

    private GameObject[] weaponSlots;

    private PlayerShopItemManager playerShopItemManager;

    public void InitializeWeapons()
    {
        DestroyAllWeapons();
        weaponSuits = new GameObject[weaponSlots.Length];
        for (int id = 0; id < weaponSlots.Length; id++)
        {
            ShopItem shopItem = this.playerShopItemManager.Get(id);
            if (shopItem == null)
            {
                continue;
            }

            Vector3 position = weaponSlots[id].transform.position;
            weaponSuits[id] = Instantiate(weaponSuitPrefab, position, Quaternion.identity, weaponSlots[id].transform);
            weaponSuits[id].tag = "PlayerWeapon";
            weaponSuits[id].GetComponent<WeaponSuit>().Initialize(shopItem.weaponConfig);
        }
    }

    public void SwapWeapon(WeaponSuit weapon, GameObject targetSlot)
    {
        if (weapon == null || targetSlot == null)
        {
            Debug.LogError("Weapon or target slot is null");
            return;
        }

        GameObject sourceSlot = weapon.transform.parent.gameObject;
        int sourceId = GetWeaponSlotId(weapon.transform.parent.gameObject);
        if (sourceId < 0)
        {
            Debug.LogError("Invalid source slot ID: " + sourceId);
            return;
        }
        int targetId = GetWeaponSlotId(targetSlot);
        if (targetId < 0)
        {
            Debug.LogError("Invalid source slot ID: " + targetId);
            return;
        }
        this.playerShopItemManager.Swap(sourceId, targetId);
        SwapWeaponSuit(sourceSlot, targetSlot);
    }

    private void Start()
    {
        if (WaveManager.Instance.IsWaveRunning)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        InitializeWeaponSlots();

        this.playerShopItemManager = PlayerShopItemManager.Instance;
        this.playerShopItemManager.InitializeIfNeeded();
        this.playerShopItemManager.SetMaxStorage(this.weaponSlots.Length);

        InitializeWeapons();
    }

    private void FixedUpdate()
    {
        if (!WaveManager.Instance.IsWaveRunning)
        {
            return;
        }

        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        rb.linearVelocity = speed * move;

        for (int id = 0; id < weaponSuits.Length; id++)
        {
            if (weaponSuits[id] != null)
            {
                weaponSuits[id].transform.position = weaponSlots[id].transform.position;
            }
        }
    }

    private void InitializeWeaponSlots()
    {
        float radius = GetPlayerRadius();
        Vector2[] offsets = GetWeaponSlotOffsets(radius, 8);
        weaponSlots = new GameObject[offsets.Length];
        for (int i = 0; i < offsets.Length; i++)
        {
            Vector3 position = (Vector2)transform.position + offsets[i];
            weaponSlots[i] = Instantiate(weaponSlotPrefab, position, Quaternion.identity, this.transform);
        }
    }

    private void DestroyAllWeapons()
    {
        if (weaponSuits == null)
        {
            return;
        }

        foreach (GameObject weapon in weaponSuits)
        {
            if (weapon != null)
            {
                // Only destroy is not enough because it happens in the end of the frame.
                // So we need to disable it first to avoid FindGameObjectsWithTag returning it.
                weapon.SetActive(false);
                Destroy(weapon);
            }
        }
    }

    private float GetPlayerRadius()
    {
        float radius = transform.localScale.x / 2f;
        return radius;
    }

    private Vector2[] GetWeaponSlotOffsets(float radius, int count)
    {
        Vector2[] offsets = new Vector2[count];
        for (int i = 0; i < count; i++)
        {
            float angle = i * 2 * Mathf.PI / count;
            offsets[i] = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
        }
        return offsets;
    }

    private int GetWeaponSlotId(GameObject slot)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == slot)
            {
                return i;
            }
        }
        return -1; // Return -1 if the slot is not found
    }

    private void SwapWeaponSuit(GameObject sourceSlot, GameObject targetSlot)
    {
        int sourceId = GetWeaponSlotId(sourceSlot);
        int targetId = GetWeaponSlotId(targetSlot);

        GameObject sourceWeapon = weaponSuits[sourceId];
        GameObject targetWeapon = weaponSuits[targetId];

        if (sourceWeapon != null)
        {
            sourceWeapon.transform.SetParent(targetSlot.transform);
            sourceWeapon.transform.position = targetSlot.transform.position;
        }

        if (targetWeapon != null)
        {
            targetWeapon.transform.SetParent(sourceSlot.transform);
            targetWeapon.transform.position = sourceSlot.transform.position;
        }

        weaponSuits[sourceId] = targetWeapon;
        weaponSuits[targetId] = sourceWeapon;
    }
}
