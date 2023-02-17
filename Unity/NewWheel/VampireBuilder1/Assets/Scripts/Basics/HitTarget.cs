using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitTarget : MonoBehaviour
{
    public int maxHitCount = 1;

    public AttackData attackBase;

    [SerializeField] string targetTag;

    [SerializeField] UnityEvent<GameObject, AttackData> attackTargetSelectEvent;

    [SerializeField] UnityEvent<Collider2D, GameObject> enemyHitEvent;

    private int hitCount;

    private void Start()
    {
        hitCount = maxHitCount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == targetTag && hitCount > 0)
        {
            hitCount--;
            attackTargetSelectEvent.Invoke(other.gameObject, attackBase);
            enemyHitEvent.Invoke(other, gameObject);
        }
    }
}
