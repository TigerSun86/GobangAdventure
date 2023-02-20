using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitTarget : MonoBehaviour
{
    public int maxHitCount = 1;

    public AttackData attackBase;

    public string targetTag;

    [SerializeField] UnityEvent<GameObject, AttackData> attackTargetSelectEvent;

    [SerializeField] UnityEvent<Collider2D, GameObject> enemyHitEvent;

    [SerializeField] UnityEvent afterLastHitEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == targetTag && maxHitCount > 0)
        {
            maxHitCount--;
            attackTargetSelectEvent.Invoke(other.gameObject, attackBase);
            enemyHitEvent.Invoke(other, gameObject);
            if (maxHitCount == 0)
            {
                afterLastHitEvent.Invoke();
            }
        }
    }
}
