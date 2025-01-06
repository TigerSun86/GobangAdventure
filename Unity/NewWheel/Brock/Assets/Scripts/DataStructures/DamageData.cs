using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData
{
    public GameObject gameObject;

    public float rawAmount;

    public float actualAmount;

    public DamageType damageType;

    public DamageData(GameObject gameObject, float rawAmount, float actualAmount, DamageType damageType)
    {
        this.gameObject = gameObject;
        this.rawAmount = rawAmount;
        this.actualAmount = actualAmount;
        this.damageType = damageType;
    }

    public override string ToString()
    {
        return $"{gameObject.transform.position},{rawAmount},{actualAmount},{damageType}";
    }
}