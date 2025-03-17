using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SkillAttack : SkillBase
{
    public SkillAttack(GameObject owner, SkillConfig skillConfig) : base(owner, skillConfig)
    {
    }

    // Returns true if finished.
    protected override bool Act()
    {
        if (targets == null || targets.Length == 0)
        {
            Debug.LogError("No target to attack");
            return true;
        }

        float remainingTime = skillConfig.actionTime - timeInCurrentState;
        Move(owner.transform, targets[0].transform, remainingTime);
        if (remainingTime <= 0)
        {
            foreach (GameObject target in targets)
            {
                DealDamage(target);
            }
            return true;
        }
        return false;
    }

    // Returns true if finished.
    protected override bool Recover()
    {
        float remainingTime = skillConfig.recoveryTime - timeInCurrentState;
        Move(owner.transform, owner.transform.parent, remainingTime);
        return remainingTime <= 0;
    }


    private void Move(Transform source, Transform target, float remainingTime)
    {
        if (remainingTime > 0.01f && !source.IsDestroyed() && !target.IsDestroyed())
        {
            Vector3 direction = target.position - source.position;
            float dynamicSpeed = direction.magnitude / remainingTime;
            direction.Normalize();
            owner.GetComponent<Rigidbody2D>().linearVelocity = dynamicSpeed * direction;
        }
        else
        {
            owner.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }

    private void DealDamage(GameObject target)
    {
        if (target.IsDestroyed() || target.GetComponent<DefenceArea>() == null)
        {
            return;
        }

        Damagable damagable = target.GetComponent<Damagable>();

        double damage = 2;

        DamageType damageType = DamageType.NORMAL_ATTACK;
        WeaponBaseTypeMatchResult matchResult = WeaponBaseTypeUtility.GetMatchResult(owner.GetComponent<Weapon>().weaponBaseType, target.GetComponent<DefenceArea>().weaponBaseType);
        if (matchResult == WeaponBaseTypeMatchResult.STRONG)
        {
            damageType = DamageType.CRITICAL_HIT;
            damage *= 2;
        }
        else if (matchResult == WeaponBaseTypeMatchResult.WEAK)
        {
            damageType = DamageType.WEAK_ATTACK;
            damage /= 2;
        }
        damagable.TakeDamage((int)damage, damageType);
    }
}