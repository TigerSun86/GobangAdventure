using System.Linq;

public class LifestealOnTakenDamageHandler : IOnTakenDamageHandler
{
    public void Handle(DamageData damageData)
    {
        if (damageData.damageType == DamageType.NONE
            || (damageData.damageType & DamageType.HEALING) != 0)
        {
            // Ignore healing events.
            return;
        }

        if (damageData.actualAmount <= 0)
        {
            return;
        }

        BuffTracker buffTracker = damageData.source.GetComponent<BuffTracker>();
        Buff buff = buffTracker.Get(BuffType.LifestealPercentage)
            .OrderByDescending(b => b.value1) // Sort by value1 (percentage).
            .FirstOrDefault();
        if (buff != null)
        {
            float lifestealPercent = buff.value1;
            if (lifestealPercent > 0)
            {
                int healAmount = (int)(damageData.actualAmount * lifestealPercent);
                if (healAmount < 1)
                {
                    // Ensure at least 1 HP is healed.
                    healAmount = 1;
                }

                // Note: healing source and target are the same.
                Healable healable = damageData.source.GetComponentInChildren<Healable>();
                // TODO: Change the skill type to the skill triggering the buff.
                healable.TakeHealing(damageData.source, damageData.skillType, healAmount);
            }
        }
    }
}