using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] GameObject weaponSlotPrefab;
    [SerializeField] GameObject teamMatePrefab;
    [SerializeField] GameObject[] defenceAreas;
    [SerializeField] float speed = 5f;
    [SerializeField] ItemDb itemDb;
    [SerializeField] Dictionary<int, ShopItem> idToWeapon;
    [SerializeField] bool isShopping = false;

    private GameObject[] weaponSlots;

    public void InitializeWeapons()
    {
        idToWeapon = GetIdToWeapon();
        DestroyAllDefenceAreas();
        defenceAreas = new GameObject[weaponSlots.Length];
        for (int id = 0; id < weaponSlots.Length; id++)
        {
            if (!idToWeapon.ContainsKey(id))
            {
                continue;
            }
            Vector3 position = weaponSlots[id].transform.position;
            defenceAreas[id] = Instantiate(teamMatePrefab, position, Quaternion.identity, weaponSlots[id].transform);
            ShopItem shopItem = idToWeapon[id];
            defenceAreas[id].GetComponent<WeaponStand>().SetWeapon(shopItem.weaponConfig);
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

        for (int id = 0; id < defenceAreas.Length; id++)
        {
            if (defenceAreas[id] != null)
            {
                defenceAreas[id].transform.position = weaponSlots[id].transform.position;
            }
        }
    }

    private void InitializeWeaponSlots()
    {
        float radius = GetPlayerRadius();
        Vector2[] defenceAreaOffsets = GetDefenceAreaOffsets(radius, 8);
        weaponSlots = new GameObject[defenceAreaOffsets.Length];
        for (int i = 0; i < defenceAreaOffsets.Length; i++)
        {
            Vector3 position = (Vector2)transform.position + defenceAreaOffsets[i];
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

    private void DestroyAllDefenceAreas()
    {
        if (defenceAreas == null)
        {
            return;
        }

        foreach (GameObject defenceArea in defenceAreas)
        {
            if (defenceArea != null)
            {
                Destroy(defenceArea);
            }
        }
    }

    private float GetPlayerRadius()
    {
        float radius = transform.localScale.x / 2f;
        return radius;
    }

    private Vector2[] GetDefenceAreaOffsets(float radius, int count)
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
