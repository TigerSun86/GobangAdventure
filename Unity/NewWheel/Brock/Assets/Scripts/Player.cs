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
    [SerializeField] ItemDb itemDb;
    [SerializeField] Dictionary<int, ShopItem> idToWeapon;
    [SerializeField] bool isShopping = false;

    private GameObject[] weaponSlots;

    public void InitializeWeapons()
    {
        idToWeapon = GetIdToWeapon();
        DestroyAllWeapons();
        weaponSuits = new GameObject[weaponSlots.Length];
        for (int id = 0; id < weaponSlots.Length; id++)
        {
            if (!idToWeapon.ContainsKey(id))
            {
                continue;
            }
            Vector3 position = weaponSlots[id].transform.position;
            weaponSuits[id] = Instantiate(weaponSuitPrefab, position, Quaternion.identity, weaponSlots[id].transform);
            weaponSuits[id].tag = "PlayerWeapon";
            ShopItem shopItem = idToWeapon[id];
            weaponSuits[id].GetComponent<WeaponSuit>().Initialize(shopItem.weaponConfig);
        }
    }

    private void Awake()
    {
        if (!isShopping)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        InitializeWeaponSlots();
        InitializeWeapons();
    }

    private void FixedUpdate()
    {
        if (isShopping)
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

    private Dictionary<int, ShopItem> GetIdToWeapon()
    {
        int id = 0;
        Dictionary<int, ShopItem> idToWeapon = new Dictionary<int, ShopItem>();
        foreach (string itemName in itemDb.playerItemNames)
        {
            ShopItem shopItem = itemDb.GetShopItem(itemName);
            idToWeapon[id] = shopItem;
            id++;
        }

        return idToWeapon;
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
}
