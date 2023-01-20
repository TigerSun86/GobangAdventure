using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChainLightning : SkillPrefab
{
    private static readonly float ATTACK_INTERVAL = 0.2f;

    private static readonly float ATTACK_AREA = 10f;

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
        remainingCount = (int)Math.Round(skillAttributes[AttributeType.AREA]);
        currentAttack = skillAttributes[AttributeType.ATTACK];
        radius = ATTACK_AREA;
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

        if (remainingCount <= 0 || target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        remainingCount--;

        attackTargetSelectEvent.Invoke(target, new AttackData(commonAttributes, skillAttributes)
        {
            attack = currentAttack
        });

        currentAttack *= (1 - skillAttributes[AttributeType.ATTACK_DECREASE]);

        debugPosition = this.transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, radius);
        Collider2D nextTarget = PositionUtilities.FindRandom(
            colliders,
            (o) => DamagableUtilities.IsDamagableEnemy(o.gameObject),
            (o) => o.gameObject.transform.position);
        if (nextTarget == null)
        {
            Destroy(this.gameObject);
            return;
        }

        chainLightningRunEvent.Invoke(new List<Vector3>() { this.transform.position, nextTarget.gameObject.transform.position });

        this.transform.position = nextTarget.gameObject.transform.position;
        this.target = nextTarget.gameObject;
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
