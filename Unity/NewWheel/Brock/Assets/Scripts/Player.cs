using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] DefenceArea[] defenceAreas;
    [SerializeField] float speed = 5f;
    [SerializeField] ItemDb itemDb;
    [SerializeField] SkillIdToGameObjectDictionary skillIdToPrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        for (int i = 0; i < Math.Min(defenceAreas.Length, itemDb.playerItemNames.Count); i++)
        {
            DefenceArea defenceArea = defenceAreas[i];
            defenceArea.SetCharacter(gameObject);
            defenceArea.transform.position = (Vector2)transform.position + defenceArea.offset;
            ShopItem shopItem = itemDb.GetShopItem(itemDb.playerItemNames[i]);
            GameObject weaponPrefab = skillIdToPrefab[shopItem.weaponBaseType];
            defenceArea.SetWeapon(weaponPrefab, shopItem);
        }
    }

    private void FixedUpdate()
    {
        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        rb.linearVelocity = speed * move;

        foreach (DefenceArea defenceArea in defenceAreas)
        {
            defenceArea.transform.position = (Vector2)transform.position + defenceArea.offset;
        }
    }
}
