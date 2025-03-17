using Unity.VisualScripting;
using UnityEngine;

public class SkillHeal : SkillBase
{
    public SkillHeal(GameObject owner, SkillConfig skillConfig) : base(owner, skillConfig)
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

        ChangeOwnerSpriteColorGradually(Color.green);
        float remainingTime = skillConfig.actionTime - timeInCurrentState;
        if (remainingTime <= 0)
        {
            foreach (GameObject target in targets)
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

    protected override bool ForceExcludeTarget(GameObject target)
    {
        Healable healable = target.GetComponent<Healable>();
        return healable == null || healable.IsFullHealth();
    }

    private void Heal(GameObject target)
    {
        if (target.IsDestroyed() || target.GetComponent<DefenceArea>() == null)
        {
            return;
        }

        Healable healable = target.GetComponent<Healable>();
        healable.TakeHealing((int)this.skillConfig.value);
    }

    private void ChangeOwnerSpriteColorGradually(Color color)
    {
        float progress = Mathf.Clamp01(timeInCurrentState / skillConfig.actionTime);
        Color targetColor = Color.Lerp(owner.GetComponent<SpriteRenderer>().color, color, progress);
        owner.GetComponent<SpriteRenderer>().color = targetColor;
    }
}