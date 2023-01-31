using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(HitTarget))]
public class ShockWave : SkillPrefab
{
    [SerializeField] float moveSpeed;

    float maxDistance;

    Rigidbody2D rb;

    Vector2 initialPosition;

    HitTarget hitTarget;

    public override SkillId GetSkillId()
    {
        return SkillId.SHOCK_WAVE;
    }

    private void Start()
    {
        maxDistance = GetSkillAttribute(AttributeType.AREA);
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.right * moveSpeed;
        initialPosition = rb.position;
        hitTarget = GetComponent<HitTarget>();
        hitTarget.attackBase = new AttackData(GetSkillId(), skillAttributeManager);
    }

    private void Update()
    {
        float distance = Math.Abs(initialPosition.x - rb.position.x);
        if (distance >= maxDistance)
        {
            Destroy(this.gameObject);
        }
    }
}