using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Attack : MonoBehaviour
{
    [SerializeField] FloatVariable attackFactor;

    public void AttackObject(GameObject gameObject, AttackData attackBase)
    {
        float damage = attackBase.attackBase;
        if (!attackBase.attackOption.HasFlag(AttackOption.NoFactor))
        {
            damage *= attackFactor.value;
        }

        DamageType damageType = DamageType.NORMAL_ATTACK;
        if (!attackBase.attackOption.HasFlag(AttackOption.NoCritical))
        {
            CriticalHit criticalHit = GetComponent<CriticalHit>();
            if (criticalHit != null)
            {
                (damage, damageType) = criticalHit.CalculateDamage(damage);
            }
        }

        Damagable damagable = gameObject.GetComponent<Damagable>();
        damagable.TakeDamage((int)damage, damageType);
    }
}
