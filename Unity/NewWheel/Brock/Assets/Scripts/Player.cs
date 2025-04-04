using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] GameObject teamMatePrefab;
    [SerializeField] GameObject[] defenceAreas;
    [SerializeField] float speed = 5f;
    [SerializeField] ItemDb itemDb;
    [SerializeField] Vector2[] defenceAreaOffsets;
    [SerializeField] Dictionary<int, ShopItem> idToWeapon;
    [SerializeField] bool isShopping = false;

    public void InitializeWeapons()
    {
        idToWeapon = GetIdToWeapon();
        float radius = GetPlayerRadius();
        defenceAreaOffsets = GetDefenceAreaOffsets(radius, 8);
        defenceAreas = new GameObject[defenceAreaOffsets.Length];
        for (int i = 0; i < defenceAreaOffsets.Length; i++)
        {
            if (!idToWeapon.ContainsKey(i))
            {
                continue;
            }
            Vector3 position = (Vector2)transform.position + defenceAreaOffsets[i];
            defenceAreas[i] = Instantiate(teamMatePrefab, position, Quaternion.identity, this.transform);
            ShopItem shopItem = idToWeapon[i];
            defenceAreas[i].GetComponent<DefenceArea>().SetWeapon(shopItem.weaponConfig);
        }
    }

    private void Awake()
    {
        if (!isShopping)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        InitializeWeapons();
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

    private void FixedUpdate()
    {
        if (isShopping)
        {
            return;
        }

        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        rb.linearVelocity = speed * move;

        for (int i = 0; i < defenceAreas.Length; i++)
        {
            if (defenceAreas[i] != null)
            {
                defenceAreas[i].transform.position = (Vector2)transform.position + defenceAreaOffsets[i];
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
