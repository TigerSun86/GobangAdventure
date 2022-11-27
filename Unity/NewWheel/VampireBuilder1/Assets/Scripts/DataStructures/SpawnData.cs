using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnData
{
    public EnemyType enemyType;

    public int minCount;

    public int maxCount;

    public bool isValid()
    {
        return minCount >= 0 && maxCount >= 0 && minCount <= maxCount;
    }

    public override string ToString()
    {
        return $"{enemyType},{minCount},{maxCount}";
    }
}