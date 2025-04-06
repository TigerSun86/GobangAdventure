using Unity.VisualScripting;
using UnityEngine;

public class SkillHeal : SkillBase
{
    public SkillHeal(WeaponSuit weaponSuit, SkillConfig skillConfig) : base(weaponSuit, skillConfig)
    {
    }

    // Returns true if finished.
    protected override bool Act()
    {
        if (!AreTargetsValid())
        {
            return true;
        }

        ChangeOwnerSpriteColorGradually(Color.green);
        float remainingTime = skillConfig.actionTime - timeInCurrentState;
        if (remainingTime <= 0)
        {
            foreach (WeaponSuit target in targets)
            {
                Heal(target);
            }
            return true;
        }
        return false;
    }

    // Returns true if finished.
    protected override bool Recover()
    {
        ChangeOwnerSpriteColorGradually(Color.blue);
        float remainingTime = skillConfig.recoveryTime - timeInCurrentState;
        return remainingTime <= 0;
    }

    protected override bool ForceExcludeTarget(WeaponSuit target)
    {
        Healable healable = target.weaponStand.GetComponent<Healable>();
        return healable == null || healable.IsFullHealth();
    }

    private void Heal(WeaponSuit target)
    {
        if (target.IsDestroyed())
        {
            return;
        }

        Healable healable = target.weaponStand.GetComponent<Healable>();
        healable.TakeHealing((int)this.skillConfig.value);
    }

    private void ChangeOwnerSpriteColorGradually(Color color)
    {
        float progress = Mathf.Clamp01(timeInCurrentState / skillConfig.actionTime);
        Color targetColor = Color.Lerp(weaponSuit.weaponItem.GetComponent<SpriteRenderer>().color, color, progress);
        weaponSuit.weaponItem.GetComponent<SpriteRenderer>().color = targetColor;
    }
}