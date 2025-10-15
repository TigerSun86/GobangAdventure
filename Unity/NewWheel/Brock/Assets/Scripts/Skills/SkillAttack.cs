using UnityEngine;

public class SkillAttack : SkillBase
{
    private BuffTracker buffTracker;

    public override void Initialize(WeaponSuit weaponSuit, SkillConfig skillConfig)
    {
        base.Initialize(weaponSuit, skillConfig);
        this.buffTracker = this.weaponSuit.GetComponent<BuffTracker>();
        if (this.buffTracker == null)
        {
            Debug.LogError("WeaponSuit does not have a BuffTracker component.");
        }
    }

    // Returns true if finished.
    protected override bool Act()
    {
        if (!AreTargetsValid())
        {
            return true;
        }

        float remainingTime = skillConfig.actionTime - timeInCurrentState;
        weaponSuit.weaponItem.MoveToTarget(targets[0].transform, remainingTime);
        if (remainingTime <= 0)
        {
            // Damage is handled by DamageAction.
            return true;
        }
        return false;
    }

    // Returns true if finished.
    protected override bool Recover()
    {
        float remainingTime = skillConfig.recoveryTime - timeInCurrentState;
        weaponSuit.weaponItem.ReturnToStand(remainingTime);

        return remainingTime <= 0;
    }
}