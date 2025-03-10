using System;
using UnityEngine;

[Serializable]
public class EnemyConfig
{
    public WeaponBaseType weaponBaseType;

    public int health;

    public int attack;

    public Vector2 positionInFleet;

    public AiStrategy aiStrategy;

    public override string ToString()
    {
        return $"{weaponBaseType},{health},{attack},{positionInFleet}";
    }
}