using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Attack : MonoBehaviour
{
    [SerializeField] FloatVariable attackFactor;

    [SerializeField] string targetTag;

    [SerializeField] UnityEvent<Collider2D, GameObject> attackEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(targetTag))
        {
            AttackObject(other.gameObject);
            attackEvent.Invoke(other, gameObject);
        }
    }

    public void AttackObject(GameObject gameObject, float attackBase = 1f)
    {
        float damage = attackBase * attackFactor.value;
        DamageType damageType = DamageType.NORMAL_ATTACK;
        CriticalHit criticalHit = GetComponent<CriticalHit>();
        if (criticalHit != null)
        {
            (damage, damageType) = criticalHit.CalculateDamage(damage);
        }

        Damagable damagable = gameObject.GetComponent<Damagable>();
        damagable.TakeDamage((int)damage, damageType);
    }
}
