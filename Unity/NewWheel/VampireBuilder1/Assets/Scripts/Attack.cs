using System.Collections;
using System.Collections.Generic;
using Timers;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] float interval = 0.5f;
    [SerializeField] float attackValue = 1f;

    [SerializeField] string targetTag;

    private bool canAttack = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        AttackObject(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        AttackObject(other.gameObject);
    }

    private void AttackObject(GameObject gameObject)
    {
        if (canAttack && gameObject.CompareTag(targetTag))
        {
            var damagable = gameObject.GetComponent<Damagable>();
            damagable.TakeDamage((int)attackValue);
            canAttack = false;
            TimersManager.SetTimer(this, interval, () => { canAttack = true; });
        }
    }
}
