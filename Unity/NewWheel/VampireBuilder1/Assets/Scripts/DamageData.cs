using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData
{
    public Vector3 position;

    public float amount;

    public DamageType damageType;

    public DamageData(Vector3 position, float amount, DamageType damageType)
    {
        this.position = position;
        this.amount = amount;
        this.damageType = damageType;
    }

    public override string ToString()
    {
        return $"{position},{amount},{damageType}";
    }
}