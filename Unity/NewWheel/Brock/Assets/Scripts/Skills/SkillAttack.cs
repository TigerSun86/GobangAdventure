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
        owner.GetComponent<Weapon>().MoveToTarget(targets[0].transform, remainingTime);
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
        owner.GetComponent<Weapon>().ReturnToDefenceArea(remainingTime);

        return remainingTime <= 0;
    }

    private void DealDamage(GameObject target)
    {
        if (target.IsDestroyed() || target.GetComponent<DefenceArea>() == null)
        {
            return;
        }

        Damagable damagable = target.GetComponent<Damagable>();

        double damage = this.skillConfig.value;

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