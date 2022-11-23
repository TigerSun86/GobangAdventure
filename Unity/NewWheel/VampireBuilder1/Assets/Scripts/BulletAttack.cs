using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletAttack : MonoBehaviour
{
    [SerializeField] FloatVariable attackFactor;

    [SerializeField] UnityEvent<Collider2D> hitEnemyEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log(other.ClosestPoint(position: transform.position));

            hitEnemyEvent.Invoke(other);
            AttackObject(other.gameObject);
        }
    }

    private void AttackObject(GameObject gameObject)
    {
        Damagable damagable = gameObject.GetComponent<Damagable>();
        damagable.TakeDamage((int)attackFactor.value);

    }
}
