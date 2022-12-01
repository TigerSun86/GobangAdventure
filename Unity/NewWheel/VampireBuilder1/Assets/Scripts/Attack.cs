using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Attack : MonoBehaviour
{
    [SerializeField] FloatVariable attackFactor;

    [SerializeField] CriticalHit criticalHit;

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

    private void AttackObject(GameObject gameObject)
    {
        float damage = attackFactor.value;
        DamageType damageType = DamageType.NORMAL_ATTACK;
        if (criticalHit != null)
        {
            (damage, damageType) = criticalHit.CalculateDamage(damage);
        }

        Damagable damagable = gameObject.GetComponent<Damagable>();
        damagable.TakeDamage((int)damage, damageType);
    }
}
