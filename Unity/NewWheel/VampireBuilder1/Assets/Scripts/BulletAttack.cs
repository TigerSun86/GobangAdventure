using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletAttack : MonoBehaviour
{
    [SerializeField] FloatVariable attackFactor;

    [SerializeField] UnityEvent<Collider2D, GameObject> hitEnemyEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            hitEnemyEvent.Invoke(other, gameObject);
            AttackObject(other.gameObject);
        }
    }

    private void AttackObject(GameObject gameObject)
    {
        Damagable damagable = gameObject.GetComponent<Damagable>();
        damagable.TakeDamage((int)attackFactor.value);

    }
}
