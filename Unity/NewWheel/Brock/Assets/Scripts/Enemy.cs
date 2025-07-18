using UnityEngine;

[RequireComponent(typeof(DieWithDependency))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField, Required]
    private GameObject weaponSuitPrefab;

    [SerializeField, AssignedInCode]
    private EnemyConfig enemyConfig;

    private Transform target;

    private WeaponSuit weaponSuit;

    public void Initialize(EnemyConfig enemyConfig)
    {
        this.enemyConfig = enemyConfig;
        GameObject weaponSuitObject = Instantiate(weaponSuitPrefab, transform.position, Quaternion.identity, transform);
        weaponSuitObject.tag = "EnemyWeapon";
        this.weaponSuit = weaponSuitObject.GetComponent<WeaponSuit>();
        this.weaponSuit.Initialize(this.enemyConfig.weaponConfig);
        DieWithDependency death = GetComponent<DieWithDependency>();
        death.dependency = weaponSuitObject;
    }

    private void FixedUpdate()
    {
        WeaponSuit[] targets = this.weaponSuit.skillActor.GetSkillAttack()?.GetTargets(range: 999);
        this.target = (targets != null && targets.Length > 0) ? targets[0].transform : null;
        if (this.target != null)
        {
            if (IsHealing())
            {
                // Stay.
                return;
            }

            if (this.enemyConfig.aiStrategy.HasFlag(AiStrategy.RunAwayWhenLowHealth)
                && this.weaponSuit.GetHealth().health < (this.weaponSuit.GetHealth().maxHealth / 2f))
            {
                MoveAway();
                return;
            }

            if (IsTargetFarAway())
            {
                MoveToTarget();
            }
        }
        else
        {
            this.target = GameObject.Find("AllyTower")?.transform;
            MoveToTarget();
        }
    }

    private bool IsHealing()
    {
        return this.weaponSuit.skillActor.IsHealing();
    }

    private void MoveAway()
    {
        if (!this.weaponSuit.capabilityController.Can(CapabilityType.Move))
        {
            return;
        }

        Vector3 direction = transform.position - target.position;
        direction.Normalize();
        transform.position += direction * speed * Time.deltaTime;
    }

    private void MoveToTarget()
    {
        if (!this.weaponSuit.capabilityController.Can(CapabilityType.Move))
        {
            return;
        }

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
