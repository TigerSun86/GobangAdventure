using UnityEngine;

[RequireComponent(typeof(DieWithDependency))]
public class Enemy : MonoBehaviour
{
    [SerializeField] float speed;

    [SerializeField] GameObject weaponSuitPrefab;

    public AiStrategy aiStrategy;

    Transform target;

    WeaponSuit weaponSuit;

    public void SetWeapon(WeaponConfig2 weaponConfig)
    {
        GameObject weaponSuitObject = Instantiate(weaponSuitPrefab, transform.position, Quaternion.identity, transform);
        weaponSuitObject.tag = "EnemyWeapon";
        this.weaponSuit = weaponSuitObject.GetComponent<WeaponSuit>();
        this.weaponSuit.Initialize(weaponConfig);
        DieWithDependency death = GetComponent<DieWithDependency>();
        death.dependency = weaponSuitObject;
    }

    private void FixedUpdate()
    {
        WeaponSuit[] targets = this.weaponSuit.skillActor.GetSkillAttack()?.GetTargets(range: 999);
        this.target = (targets != null && targets.Length > 0) ? targets[0].transform : null;
        if (target != null)
        {
            if (IsHealing())
            {
                // Stay.
                return;
            }

            if (aiStrategy.HasFlag(AiStrategy.RunAwayWhenLowHealth)
                && weaponSuit.GetHealth().health < (weaponSuit.GetHealth().maxHealth / 2f))
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
        return this.weaponSuit.skillActor.IsHealing();
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
        if (this.weaponSuit.skillActor.GetSkillAttack() != null)
        {
            return this.weaponSuit.skillActor.GetSkillAttack().skillConfig.range;
        }
        return 0;
    }
}
