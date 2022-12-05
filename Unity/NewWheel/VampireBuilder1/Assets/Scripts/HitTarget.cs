using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitTarget : MonoBehaviour
{
    [SerializeField] AttackData attackBase = new AttackData(1f);

    [SerializeField] string targetTag;

    [SerializeField] UnityEvent<GameObject, AttackData> attackTargetSelectEvent;

    [SerializeField] UnityEvent<Collider2D, GameObject> enemyHitEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (DamagableUtilities.IsDamagable(other.gameObject, targetTag))
        {
            attackTargetSelectEvent.Invoke(other.gameObject, attackBase);
            enemyHitEvent.Invoke(other, gameObject);
        }
    }
}
