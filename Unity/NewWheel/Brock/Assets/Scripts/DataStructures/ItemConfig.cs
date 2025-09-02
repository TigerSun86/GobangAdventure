using System;
using UnityEngine;

[Serializable]
public class ItemConfig
{
    public string itemName;

    public int level;

    public int price;

    public Sprite sprite;

    public float maxHealth;

    public float attack;

    public string GetId()
    {
        return $"{itemName} {level}";
    }
}