using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalHit : MonoBehaviour
{
    [SerializeField] FloatVariable criticalHitChance;

    [SerializeField] FloatVariable criticalHitDamage;

    public (float damage, DamageType damageType) CalculateDamage(float attack)
    {
        float damage = attack;
        DamageType damageType = DamageType.NORMAL_ATTACK;
        if (criticalHitChance.value > 0)
        {
            bool isCriticalHit = Random.value < criticalHitChance.value;
            if (isCriticalHit)
            {
                damage *= criticalHitDamage.value;
                damageType = DamageType.CRITICAL_HIT;
            }
        }

        return (damage, damageType);
    }
}
