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
        defenceArea = GetComponent<DefenceArea>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        this.skillActor = GetComponentInChildren<SkillActor>();
    }

    public void SetWeapon(WeaponConfig weaponConfig)
    {
        this.defenceArea.SetWeapon(weaponConfig);
    }

    private void FixedUpdate()
    {
        GameObject[] targets = this.skillActor.GetSkillAttack()?.GetTargets(range: 999);
        this.target = (targets != null && targets.Length > 0) ? targets[0].transform : null;
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
            > GetShortestWeaponRange();
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
