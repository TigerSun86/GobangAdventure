using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float speed;

    public AiStrategy aiStrategy;

    Transform target;

    DefenceArea defenceArea;
    Health health;
    SkillActor skillActor;

    private void Awake()
    {
        target = GameObject.Find("Player").transform;
        defenceArea = GetComponent<DefenceArea>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        this.skillActor = GetComponentInChildren<SkillActor>();
        if (aiStrategy.HasFlag(AiStrategy.Heal))
        {
            this.skillActor.SetSkillPriority(SkillType.Heal, SkillActor.PriorityHigh);
        }
    }

    public void SetWeapon(WeaponBaseType weaponBaseType, SkillConfig[] skills)
    {
        this.defenceArea.SetWeapon(weaponBaseType, skills);
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            if (IsHealing())
            {
                // Stay.
                return;
            }

            if (aiStrategy.HasFlag(AiStrategy.RunAwayWhenLowHealth)
                && health.health < (health.maxHealth / 2f))
            {
                MoveAway();
                return;
            }

            if (IsTargetFarAway())
            {
                MoveToTarget();
            }
        }
    }

    private bool IsHealing()
    {
        return this.skillActor.IsHealing();
    }

    private void MoveAway()
    {
        Vector3 direction = transform.position - target.position;
        direction.Normalize();
        transform.position += direction * speed * Time.deltaTime;
    }

    private void MoveToTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.Normalize();
        transform.position += direction * speed * Time.deltaTime;
    }

    private bool IsTargetFarAway()
    {
        return Vector3.Distance(transform.position, target.position)
            > GetShortestWeaponRange() - 0.2;
    }

    private float GetShortestWeaponRange()
    {
        if (this.skillActor.GetSkillAttack() != null)
        {
            return this.skillActor.GetSkillAttack().skillConfig.range;
        }
        return 0;
    }
}
