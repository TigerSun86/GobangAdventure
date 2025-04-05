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
        if (!AreTargetsValid())
        {
            return true;
        }

        float remainingTime = skillConfig.actionTime - timeInCurrentState;
        owner.GetComponent<WeaponItem>().MoveToTarget(targets[0].transform, remainingTime);
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
        owner.GetComponent<WeaponItem>().ReturnToDefenceArea(remainingTime);

        return remainingTime <= 0;
    }

    private void DealDamage(GameObject target)
    {
        if (target.IsDestroyed() || target.GetComponent<WeaponStand>() == null)
        {
            return;
        }

        Damagable damagable = target.GetComponent<Damagable>();

        double damage = this.skillConfig.value;

        DamageType damageType = DamageType.NORMAL_ATTACK;
        WeaponBaseTypeMatchResult matchResult = WeaponBaseTypeUtility.GetMatchResult(owner.GetComponent<WeaponItem>().weaponBaseType, target.GetComponent<WeaponStand>().weaponBaseType);
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