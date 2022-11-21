using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttack : MonoBehaviour
{
    [SerializeField] FloatVariable attackFactor;

    private void OnTriggerEnter2D(Collider2D other)
    {
        AttackObject(gameObject: other.gameObject);
    }

    private void AttackObject(GameObject gameObject)
    {
        if (gameObject.CompareTag("Enemy"))
        {
            Damagable damagable = gameObject.GetComponent<Damagable>();
            damagable.TakeDamage((int)attackFactor.value);
        }
    }
}
