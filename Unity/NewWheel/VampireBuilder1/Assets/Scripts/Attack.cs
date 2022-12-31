using UnityEngine;

public class Attack : MonoBehaviour
{
    public void AttackObject(GameObject gameObject, AttackData attackBase)
    {
        float damage = attackBase.attack;
        DamageType damageType = DamageType.NORMAL_ATTACK;
        if (attackBase.criticalRate > 0)
        {
            bool isCriticalHit = Random.value < attackBase.criticalRate;
            if (isCriticalHit)
            {
                damage *= attackBase.criticalAmount;
                damageType = DamageType.CRITICAL_HIT;
            }
        }

        Damagable damagable = gameObject.GetComponent<Damagable>();
        damagable.TakeDamage((int)damage, damageType);
    }
}
