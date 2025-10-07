using System.Linq;

public class SkillBuffBlockingCd : SkillBase
{
    // Returns true if finished.
    protected override bool Act()
    {
        if (!AreTargetsValid())
        {
            return true;
        }

        foreach (WeaponSuit target in this.targets)
        {
            BuffTracker buffTracker = target.GetComponent<BuffTracker>();
            if (buffTracker == null)
            {
                continue;
            }

            Buff buffClone = this.skillConfig.buff1.Clone();
            buffTracker.Add(buffClone);
        }

        return true;
    }

    // Returns true if finished.
    protected override bool Recover()
    {
        return true;
    }

    protected override float GetCalculatedCdTime()
    {
        if (IsBuffOnTargets())
        {
            return float.PositiveInfinity;
        }

        return base.GetCalculatedCdTime();
    }

    private bool IsBuffOnTargets()
    {
        if (AreTargetsValid())
        {
            foreach (WeaponSuit target in this.targets)
            {
                BuffTracker buffTracker = target.GetComponent<BuffTracker>();
                if (buffTracker == null)
                {
                    continue;
                }

                // TODO: Check if the buff came from this skill.
                Buff buff = buffTracker.Get(this.skillConfig.buff1.buffType)
                    .FirstOrDefault();
                if (buff != null)
                {
                    return true;
                }
            }
        }

        return false;
    }
}