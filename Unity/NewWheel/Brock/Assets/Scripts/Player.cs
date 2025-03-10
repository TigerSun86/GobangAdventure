using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] GameObject teamMatePrefab;

    [SerializeField] GameObject[] defenceAreas;
    [SerializeField] float speed = 5f;
    [SerializeField] ItemDb itemDb;
    [SerializeField] SkillIdToGameObjectDictionary skillIdToPrefab;

    private Vector2[] defenceAreaOffsets;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        float radius = GetPlayerRadius();
        defenceAreaOffsets = GetDefenceAreaOffsets(radius, 4);
        defenceAreas = new GameObject[defenceAreaOffsets.Length];
        for (int i = 0; i < defenceAreaOffsets.Length; i++)
        {
            Vector3 position = (Vector2)transform.position + defenceAreaOffsets[i];
            defenceAreas[i] = Instantiate(teamMatePrefab, position, Quaternion.identity, this.transform);
        }
        for (int i = 0; i < Math.Min(defenceAreas.Length, itemDb.playerItemNames.Count); i++)
        {
            GameObject defenceArea = defenceAreas[i];
            ShopItem shopItem = itemDb.GetShopItem(itemDb.playerItemNames[i]);
            GameObject weaponPrefab = skillIdToPrefab[shopItem.weaponBaseType];
            defenceArea.GetComponent<DefenceArea>().SetWeapon(weaponPrefab, shopItem);
        }
    }

    private void FixedUpdate()
    {
        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        rb.linearVelocity = speed * move;

        for (int i = 0; i < defenceAreas.Length; i++)
        {
            defenceAreas[i].transform.position = (Vector2)transform.position + defenceAreaOffsets[i];
        }
    }

    private float GetPlayerRadius()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Get the size of the sprite (assuming the sprite is a perfect circle)
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;  // Diameter
        // Take the scale into account
        float radius = (spriteWidth * transform.localScale.x) / 2f;
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
