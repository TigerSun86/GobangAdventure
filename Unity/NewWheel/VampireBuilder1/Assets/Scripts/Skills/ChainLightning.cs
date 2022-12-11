using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ChainLightning : MonoBehaviour
{
    private static readonly float ATTACK_INTERVAL = 0.2f;

    [SerializeField] float initialAttackBase;

    [SerializeField] IntVariable maxCount;

    [SerializeField] FloatVariable attackDecreaseRate;

    [SerializeField] FloatVariable attackAreaFactor;

    [SerializeField] UnityEvent<List<Vector3>> chainLightningRunEvent;

    [SerializeField] UnityEvent<GameObject, AttackData> attackTargetSelectEvent;

    int remainingCount;

    float currentAttack;

    float radius;

    private Vector2? debugPosition;

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        remainingCount = maxCount.value;
        currentAttack = initialAttackBase;
        radius = attackAreaFactor.value + 2;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.fixedDeltaTime;
        if (timer > 0f)
        {
            return;
        }

        timer = ATTACK_INTERVAL;

        if (remainingCount <= 0)
        {
            Destroy(this.gameObject);
            return;
        }

        remainingCount--;

        debugPosition = this.transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, radius);
        Collider2D target = PositionUtilities.FindRandom(
            colliders,
            (o) => DamagableUtilities.IsDamagableEnemy(o.gameObject),
            (o) => o.gameObject.transform.position);
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        float attack = currentAttack;
        currentAttack *= (1 - attackDecreaseRate.value);
        attackTargetSelectEvent.Invoke(target.gameObject, new AttackData(attack));
        chainLightningRunEvent.Invoke(new List<Vector3>() { this.transform.position, target.gameObject.transform.position });

        this.transform.position = target.gameObject.transform.position;
    }

    void OnDrawGizmos()
    {
        if (debugPosition.HasValue)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(debugPosition.Value, radius);
        }
    }
}
