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

    private void AttackObject(GameObject gameObject)
    {
        Damagable damagable = gameObject.GetComponent<Damagable>();
        damagable.TakeDamage((int)attackFactor.value);
    }
}
