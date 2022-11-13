using System.Collections;
using System.Collections.Generic;
using Timers;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] float interval = 0.5f;
    [SerializeField] float attackValue = 1f;

    public string TargetTag { get; set; }

    private bool canAttack = true;

    private void OnCollisionStay2D(Collision2D collision)
    {
        AttackObject(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AttackObject(collision.gameObject);
    }

    private void AttackObject(GameObject gameObject)
    {
        if (canAttack && gameObject.CompareTag(TargetTag))
        {
            var damagable = gameObject.GetComponent<Damagable>();
            damagable.TakeDamage((int)attackValue);
            canAttack = false;
            TimersManager.SetTimer(this, interval, () => { canAttack = true; });
        }
    }
}
